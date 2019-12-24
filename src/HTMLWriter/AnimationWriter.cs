using Slides;
using Slides.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTMLWriter
{
	public static class AnimationWriter
	{
		private class AnimationJS
		{
			public int StepId { get; }
			public string ElementId { get; }
			public string Animation { get; }
			public int Duration { get; }

			public AnimationJS(int stepId, string elementId, string animation, int duration)
			{
				StepId = stepId;
				ElementId = elementId;
				Animation = animation;
				Duration = duration;
			}
		}

		private static HashSet<AnimationJS> _animations = new HashSet<AnimationJS>();

		private static Type GetTypeForField(string field)
		{
			switch (field)
			{
				case "margin":
					return typeof(Thickness);
				//TODO(Major): Background could be a image as well..
				//We actually would need the value of background
				//to check this.

				//On the other hand. We can't really animate images
				//There is no lerp function for images. So let's just
				//assume it is a color? And maybe make sure while Binding
				//That thats the case.
				case "background":
					return typeof(Color);
				default:
					throw new NotImplementedException();
			}
		}
		public static void Write(JavaScriptWriter writer, AnimationCall animation, int stepId, string elementId)
		{
			_animations.Add(new AnimationJS(stepId, elementId, animation.Name, animation.Time.ToMilliseconds()));

			writer.StartFunction(animation.Name, "progress", "element");
			writer.WriteVariableDeclarationInline("computedStyle", "getComputedStyle(element)", true);
			writer.WriteVariableDeclarationInline("cases", GetInitializer(writer.Indent, animation.Cases));
			writer.StartForLoop("i", "cases.length - 1");
			writer.WriteVariableDeclarationInline("prev", "cases[i]", true);
			writer.WriteVariableDeclarationInline("next", "cases[i + 1]", true);
			writer.StartIfStatement("progress > prev.condition && progress < next.condition");
			writer.WriteVariableDeclarationInline("caseprogress", "(progress - prev.condition) / (next.condition - prev.condition)");
			foreach (var field in animation.ChangedFields)
			{
				var cssField = StyleWriter.ToCssAttribute(field);

				//Check every case for the first value this field gets set to. 
				//So you can determine the actual type of the field.
				var fieldType = animation.Cases.FirstOrDefault(c => c.ChangedFields.ContainsKey(field)).ChangedFields.FirstOrDefault(cf => cf.Key == field).Value.GetType();
				if (animation.Cases.All(c => c.ChangedFields.ContainsKey(field)))
					writer.WriteAssignment($"element.style.{cssField}", GetLerp(fieldType, $"prev.{cssField}_value", $"next.{cssField}_value", "caseprogress"));
				else
				{
					//Let the horror begin.
					var conditionBuilder = new StringBuilder();
					for (int i = 0; i < animation.Cases.Length; i++)
					{
						if (animation.Cases[i].ChangedFields.ContainsKey(field)) //Value changes here
						{
							if (conditionBuilder.Length > 0)
								conditionBuilder.AppendLine(" ||");
							conditionBuilder.Append($"i == {i - 1}");
						}
					}
					writer.StartIfStatement(conditionBuilder.ToString());
					writer.WriteAssignment($"element.style.{cssField}", GetLerp(fieldType, $"prev.{cssField}_value", $"next.{cssField}_value", "caseprogress"));
					writer.EndIf();
				}
			}
			writer.WriteReturnStatement(null);
			writer.EndIf();
			writer.EndFor();
			writer.EndFunction();
		}

		private static string GetLerp(Type type, string value1, string value2, string t)
		{
			if(type == typeof(Unit))
			{
				return $"StyleUnit.lerp({value1}, {value2}, {t}).toString()";
			}
			else if(type == typeof(Thickness))
			{
				return $"Thickness.lerp({value1}, {value2}, {t}).toString()";
			}
			else if(type == typeof(Color))
			{
				return $"Color_t.lerp({value1}, {value2}, {t}).toString()";
			}
			else if (type == typeof(ProcentalFilter))
			{
				return $"ProcentalFilter.lerp({value1}, {value2}, {t}).toString()";
			}
			else if (type == typeof(BlurFilter))
			{
				return $"BlurFilter.lerp({value1}, {value2}, {t}).toString()";
			}
			else if (type == typeof(DropShadowFilter))
			{
				return $"DropShadowFilter.lerp({value1}, {value2}, {t}).toString()";
			}
			else
			{
				return $"lerp({value1}, {value2}, {t})";
			}
		}

		private static string GetInitializer(int indent, CaseCall[] cases)
		{
			var initializedFields = new Dictionary<string, object>();
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[");
			foreach (var c in cases)
			{
				stringBuilder.Append($"{ new string('\t', indent + 1) }{{ condition: {c.Condition}");
				foreach (var field in c.ChangedFields)
				{
					stringBuilder.Append($", {StyleWriter.ToCssAttribute(field.Key)}_value: {GetJSValue(field.Value, field.Key)}");
					initializedFields[field.Key] = field.Value;
				}
				foreach (var field in c.UnchangedFields)
				{
						stringBuilder.Append($", {StyleWriter.ToCssAttribute(field)}_value: ");
					if (initializedFields.ContainsKey(field))
						stringBuilder.Append(GetJSValue(initializedFields[field], field));
					else
						stringBuilder.Append(GetInitializer(field));
				}
				stringBuilder.AppendLine("},");
			}
			stringBuilder.Append($"{new string('\t', indent)}]");
			return stringBuilder.ToString();
		}

		private static string GetInitializer(string field)
		{
			var cssField = StyleWriter.ToCssAttribute(field);
			var fieldType = GetTypeForField(field);
			if(fieldType == typeof(Unit))
			{
				return $"StyleUnit.parse(computedStyle.{cssField}, {GetJSValue(GetIsVertical(field))})";
			}
			else if(fieldType == typeof(Thickness))
			{
				return $"Thickness.parse(computedStyle.{cssField})";
			}
			else if(fieldType == typeof(Color))
			{
				return $"new Color_t(computedStyle.{cssField})";
			}
			else if(fieldType == typeof(Filter))
			{
				//TODO(Major): Implement js parser for filters.
				throw new Exception();
				return $"undefined";
			}
			else
			{

				return $"computedStyle.{cssField}";
			}
		}

		private static string GetJSValue(object value, string field = null)
		{
			switch (value)
			{
				case Unit unit:
					return $"new StyleUnit({unit.Value}, \"{Unit.ToString(unit.Kind)}\", {GetJSValue(GetIsVertical(field))})";
				case Thickness thickness:
					return $"new Thickness({GetJSValue(thickness.Top)}, {GetJSValue(thickness.Right)}, {GetJSValue(thickness.Bottom)}, {GetJSValue(thickness.Left)})";
				case Color color:
					return $"new Color_t('{CSSWriter.GetValue(color)}')";
				case null:
					return "undefined";
				case bool b:
					return b.ToString().ToLower();
				case ProcentalFilter procentalFilter:
					return $"new ProcentalFilter('{procentalFilter.Name}', {procentalFilter.Value})";
				case BlurFilter blurFilter:
					return $"new BlurFilter({blurFilter.Value})";
				case DropShadowFilter dropShadowFilter:
					return $"new DropShadowFilter({dropShadowFilter.Horizontal}, {dropShadowFilter.Vertical}, {dropShadowFilter.Blur}, {dropShadowFilter.Spread}, {GetJSValue(dropShadowFilter.Color)})";
				default:
					throw new ArgumentException();
			}
		}

		private static bool? GetIsVertical(string field)
		{
			if (field == null)
				return null;
			switch (field)
			{
				case "marginTop":
					return true;
				default:
					return null;
			}
		}

		public static void EndFile(JavaScriptWriter writer)
		{
			/*
			 * function getAnimations()
{
    const animations = [
        { step_numerical_id: 5, element_id: "introduction-quoteBox", animation: quoteGoesUp, duration: 4000}
    ];
    return animations;
}
			 * */
			writer.StartFunction("getAnimations");
			writer.WriteVariableDeclarationInline("animations", GetAnimationInitializer(writer.Indent, _animations.ToArray()), true);
			writer.WriteReturnStatement("animations");
			writer.EndFunction();
			_animations.Clear();
		}

		private static string GetAnimationInitializer(int indent, AnimationJS[] animations)
		{
			var result = new StringBuilder();
			result.AppendLine("[");
			foreach (var animation in animations)
			{
				result.AppendLine($"{new string('\t', indent + 1)}{{ step_numerical_id: {animation.StepId}, element_id: \"{animation.ElementId}\", animation: {animation.Animation}, duration: {animation.Duration} }},");
			}
			result.Append($"{new string('\t', indent)}]");
			return result.ToString();
		}
	}
}
