using SimpleLogger;
using Slides;
using Slides.Code;
using Slides.Data;
using Slides.Elements;
using Slides.Helpers;
using Slides.Styling;
using Slides.SVG;
using Slides.Transforms;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HTMLWriter
{
	public static class StyleWriter
	{
		private static bool ShouldSkipTypedModification(string type, string field)
		{
			switch (field)
			{
				case "padding": return type == "slide";
				case "coding-highlighting":
				case "autoplay":
				case "orientation":
					return true;
				default:
					return false;
			}
		}

		private static void WriteTypedModification(CSSWriter writer, Substyle substyle)
		{
			writer.StartSelector(SelectorToString(substyle.Selector));

			foreach (var property in substyle.Properties)
			{
				if (ShouldSkipTypedModification(substyle.Selector.Name, property.Key)) continue;
				//if (property.IsNonCSS) continue;
				writer.WriteAttribute(CSSWriter.ToCssAttribute(property.Key), property.Value);
			}
			writer.EndSelector();
		}

		private static string SelectorToString(Selector selector)
		{
			if (selector == null) return "";
			switch (selector.Kind)
			{
				case SelectorKind.All:
					return "*";
				case SelectorKind.Custom:
				case SelectorKind.Type:
					return $".{selector.Name}{SelectorToString(selector.Child)}";
				case SelectorKind.Field:
					return $">.{selector.Name}{SelectorToString(selector.Child)}";
				default:
					throw new NotImplementedException();
			}
		}

		private static void WriteStdStyle(CSSWriter writer, StdStyle style, out string stdTransition)
		{
			stdTransition = null;
			writer.StartSelector("*");
			Transition toWrite = null;
			foreach (var property in style.Substyles.GetAllStyle().Properties)
			{
				switch (property.Key)
				{
					case "transition":
						toWrite = (Transition)property.Value;
						break;
					case "useDarkTheme":
					case "orientation":
						break;
					default:
						writer.WriteAttribute(CSSWriter.ToCssAttribute(property.Key), property.Value);
						break;
				}
			}
			writer.EndSelector();

			foreach (var substyle in style.Substyles.GetIterator())
			{
				if (substyle.Selector.Kind == SelectorKind.All)
					continue;
				WriteTypedModification(writer, substyle);
			}

			if (toWrite != null)
			{
				stdTransition = toWrite.name;
				WriteStdTransition(writer, toWrite.name, toWrite.from, toWrite.to);
			}
		}

		private static void WriteCustomStyle(CSSWriter writer, CustomStyle style)
		{
			if(style.Substyles.Count == 0)
			{
				Logger.Log($"Empty style {style.Name} found after evaluation.");
				return;
			}
			writer.StartClass($"{style.Name}");
			Transition toWrite = null;
			foreach (var property in style.Substyles.GetRootCustomStyle().Properties)
			{
				switch (property.Key)
				{
					case "transition":
						toWrite = (Transition)property.Value;
						break;

					default:
						if (property.Key.StartsWith("non-css-"))
							continue;
						writer.WriteAttribute(CSSWriter.ToCssAttribute(property.Key), property.Value);
						break;
				}
			}
			writer.EndSelector();

			foreach (var substyle in style.Substyles.GetIterator())
			{
				if (substyle.Selector.Kind == SelectorKind.Custom && substyle.Selector.Child == null)
					continue;
				WriteTypedModification(writer, substyle);
			}
			//foreach (var modifiedField in style.ModifiedFields)
			//{
			//	switch (modifiedField.Key)
			//	{
			//		case "transition":
			//			toWrite = (Transition)modifiedField.Value;
			//			break;
			//		case "orientation":
			//		//This is no css attribute. we set it in the element.
			//		case "margin":
			//		case "left":
			//		case "top":
			//		case "right":
			//		case "bottom":
			//			//We ignore margin, because we write it per element
			//			//and mostly just use left, top, right, bottom and
			//			//not the actual margin.
			//			break;
			//		default:
			//			writer.WriteAttribute(CSSWriter.ToCssAttribute(modifiedField.Key), modifiedField.Value);
			//			break;
			//	}
			//}
			//writer.EndSelector();

			//How can a custom style have a std transition?!?!
			if (toWrite != null)
			{
				WriteStdTransition(writer, toWrite.name, toWrite.from, toWrite.to);
			}
		}
		public static void Write(CSSWriter writer, Style style, out string stdTransition)
		{
			stdTransition = null;
			if (style.Name == "std")
				WriteStdStyle(writer, (StdStyle)style, out stdTransition);
			else
				WriteCustomStyle(writer, (CustomStyle)style);
		}

		private static void WriteStdTransition(CSSWriter writer, string transitionName, TransitionCall from, TransitionCall to)
		{
			//return;
			writer.StartSelector($"section.slide.{transitionName}-from");
			writer.WriteAttribute("animation-name", from.Name);
			writer.WriteAttribute("animation-duration", from.Duration);
			writer.WriteAttribute("animation-delay", from.Delay);
			writer.WriteAttribute("animation-iteration-count", 1);
			writer.WriteAttribute("animation-fill-mode", "forwards");
			writer.WriteAttribute("z-index", 5);
			writer.EndSelector();

			writer.StartSelector($"section.slide.{transitionName}-to");
			writer.WriteAttribute("animation-name", to.Name);
			writer.WriteAttribute("animation-duration", to.Duration);
			writer.WriteAttribute("animation-delay", to.Delay);
			writer.WriteAttribute("animation-iteration-count", 1);
			writer.WriteAttribute("animation-fill-mode", "forwards");
			writer.WriteAttribute("z-index", 10);
			writer.EndSelector();
		}

		public static void WriteTransition(CSSWriter writer, Transition transition)
		{
			writer.StartId(transition.name);
			WriteBrush(writer, transition.background);
			if (transition.color != null)
				writer.WriteAttribute("color", transition.color);
			if (transition.font != null)
				writer.WriteAttribute("font", transition.font);
			writer.EndId();

			//TODO(Debate): That was the old way. Do we still need it?
			//animation-name: fadeOut;
			//animation-duration: 10s;
			//animation-iteration-count: 1;
			//animation-fill-mode: forwards;
			//writer.StartSelector($"#{transition.name}.active");
			//writer.EndSelector();
		}

		public static void WriteSlide(CSSWriter writer, Slide slide)
		{
			writer.StartId(slide.Name);
			WriteBrush(writer, slide.Attributes.background);
			writer.WriteAttributeIfValue("color", slide.Attributes.color);
			writer.WriteAttributeIfValue("font", slide.Attributes.font);
			writer.WriteAttributeIfValue("filter", slide.Attributes.n_filter);
			writer.WriteAttributeIfValue("font-size", slide.Attributes.fontsize);
			writer.WriteAttributeIfValue("font-family", slide.Attributes.font);
			writer.EndId();

			writer.StartSelector($"#{slide.Name}>.slide-content");
			var padding = slide.Attributes.get_ActualPadding();
			var fullSize = new Unit(100, Unit.UnitKind.Percent);
			writer.WriteAttributeIfNotDefault("margin", padding, new Thickness());
			writer.WriteAttributeIfNotDefault("width", fullSize - padding.Horizontal, fullSize);
			writer.WriteAttributeIfNotDefault("height", fullSize - padding.Vertical, fullSize);
			writer.EndSelector();
		}

		public static void WriteStep(CSSWriter writer, Step step, Slide parent)
		{
			if (step.Name != null)
				writer.StartId(step.Name);
			else
				writer.StartSelector($"section#{parent.Name} div.step");
			writer.WriteAttributeIfValue("color", parent.Attributes.color);
			writer.WriteAttributeIfValue("font-size", parent.Attributes.fontsize);
			writer.WriteAttributeIfValue("font-family", parent.Attributes.font);
			writer.EndSelector();
		}

		public static void WriteElement(CSSWriter writer, string parentName, Element element, Element parent = null)
		{
			if (element.name == null)
				return;
			var id = $"{parentName}-{element.name}";
			writer.StartId($"{id}");
			WriteOrientation(writer, element, parent);
			
			WriteElementStyling(writer, element.h_Styling);

			switch (element)
			{
				case Label l:
					writer.WriteAttributeIfNotDefault("text-align", l.align, Alignment.Unset);
					break;
				case List list:
					writer.WriteAttributeIfNotDefault("text-align", list.align, Alignment.Unset);
					break;
				case Table _:
					writer.WriteAttribute("border-collapse", "collapse");
					break;
				case Video v:
					writer.WriteAttributeIfNotDefault("object-fit", v.stretching, Stretching.Unset);
					break;
				case Image i:
					writer.WriteAttributeIfNotDefault("object-fit", i.stretching, Stretching.Unset);
					break;
				default:
					break;
			}
			writer.EndId();

			// 'listAgain': Just because we already have a list in the switch statement above..
			if(element is List listAgain)
			{
				if (!listAgain.isOrdered && listAgain.get_Textmarker() != null)
				{
					writer.StartId($"{id} li", "before");
					writer.WriteAttribute("content", $"\"{FormattedString.Convert(listAgain.get_Textmarker()).ToHTML()}\"");
					writer.WriteAttribute("left", "0");
					writer.WriteAttribute("position", "absolute");
					writer.EndSelector();
				}

				WriteSubstylings(writer, parentName, listAgain);
			}

			if (element.hover.h_IsDefault)
				return;

			writer.StartId(id, pseudoClass: "hover");
			WriteElementStyling(writer, element.hover);
			writer.EndSelector();

		}

		private static void WriteElementStyling(CSSWriter writer, ElementStyling styling)
		{
			WriteBrush(writer, styling.h_Background);
			WriteBorder(writer, styling.border);
			var properties = new string[] { "color", "filter", "padding" };
			foreach (var prop in properties)
			{
				writer.WriteAttributeIfValue(CSSWriter.ToCssAttribute(prop), styling.get_ActualField(prop));
			}
			WriteTransform(writer, styling.get_Transforms());
			if(styling is TextElementStyling textStyling)
			{
				writer.WriteAttributeIfValueOrInherit("font-size", textStyling.fontsize, textStyling.InheritsFontsize());

				writer.WriteAttributeIfValue("line-height", textStyling.h_LineHeight);
				writer.WriteAttributeIfValueOrInherit("font-family", textStyling.font, textStyling.InheritsFont());
			}
			if(styling is ListElementStyling listStyling)
			{

				writer.WriteAttributeIfValue("list-style-type", listStyling.h_MarkerType ?? listStyling.h_Parent?.h_MarkerType);
				if (listStyling.get_Textmarker() != null)
					writer.WriteAttribute("padding-left", "1em");
			}
		}

		private static void WriteSubstylings(CSSWriter writer, string parentName, List list)
		{
			var index = 1;
			var id = $"#{parentName}-{list.name} ";
			foreach (var substyling in list.get_Stylings().Skip(1))
			{
				writer.WriteComment("\n    This is just for compatibility reasons with chrome-like browsers.\n    (Will hopefully be not needed anymore one day..)\n");
				var selectorBuilder = new StringBuilder(id);
				for (int i = 0; i < index; i++)
					selectorBuilder.Append(":-webkit-any(ul, ol) ");
				writer.StartSelector(selectorBuilder.ToString());
				WriteElementStyling(writer, substyling);
				writer.EndSelector();

				selectorBuilder.Clear();
				selectorBuilder.Append(id);
				for (int i = 0; i < index; i++)
					selectorBuilder.Append(":is(ul, ol) ");
				writer.StartSelector(selectorBuilder.ToString());
				WriteElementStyling(writer, substyling);
				writer.EndSelector();
			}
		}




		//TODO(Next Thing): Think about differences between margin and padding.
		//In CSS and in Slides. 
		private static void WriteOrientation(CSSWriter writer, Element element, Element parent = null)
		{
			var isRelative = parent != null && (parent.kind == ElementKind.Stack || parent.kind == ElementKind.List || parent.kind == ElementKind.Captioned);
			if (string.IsNullOrEmpty(element.position))
			{
				if (isRelative)
					writer.WriteAttribute("position", "relative");
				else //if(element.kind != ElementKind.Label)
					writer.WriteAttribute("position", "absolute");
			}
			else
				writer.WriteAttribute("position", element.position);

			var marginSet = element.h_Margin;
			var m = element.margin;

			var unit100Percent = new Unit(100, Unit.UnitKind.Percent);
			var orientation = element.orientation;
			var hasHorizontalStretch = SlidesHelper.GetHorizontal(orientation) == Horizontal.Stretch && element.h_AllowsHorizontalStretching;
			var hasVerticalStretch = SlidesHelper.GetVertical(orientation) == Vertical.Stretch && element.h_AllowsVerticalStretching;


			if (element.kind != ElementKind.UnitSVGShape)
			{
				if (hasVerticalStretch)
				{
					writer.WriteAttribute("height", unit100Percent - m.Vertical);
				}
				else if (element.get_StyleHeight() == null)
				{
					//writer.WriteAttribute("height", "fit-content");
				}
				else
					writer.WriteAttributeIfValue("height", element.get_StyleHeight());


				if (hasHorizontalStretch)
					writer.WriteAttribute("width", unit100Percent - m.Horizontal);
				else if (element.get_StyleWidth() == null)
				{
					//writer.WriteAttribute("width", "fit-content");
				}
				else
					writer.WriteAttributeIfValue("width", element.get_StyleWidth());
			}


			if (!isRelative)
				WriteOrientation_(writer, element, orientation, m);
			else if(parent is Stack stack)
				WriteStackAlignment(writer, stack.align, stack.StackFlow, marginSet);
			else
				writer.WriteAttributeIfValue("margin", marginSet);
		}

		private static void WriteStackAlignment(CSSWriter writer, StackAlignment align, FlowAxis flow, Thickness margin)
		{
			switch (align)
			{
				case StackAlignment.Unset:
					writer.WriteMarginOrPadding("margin", margin);
					break;
				case StackAlignment.Top:
					writer.WriteAttribute("vertical-align", "top");
					writer.WriteMarginOrPadding("margin", margin);
					break;
				case StackAlignment.Bottom:
					writer.WriteAttribute("vertical-align", "bottom");
					writer.WriteMarginOrPadding("margin", margin);
					break;
				case StackAlignment.Left:
					writer.WriteAttribute("text-align", "left");
					writer.WriteMarginOrPadding("margin", new Thickness(margin?.left, margin?.top, Unit.Auto, margin?.bottom));
					break;
				case StackAlignment.Right:
					writer.WriteAttribute("text-align", "right");
					writer.WriteMarginOrPadding("margin", new Thickness(Unit.Auto, margin?.top, margin?.right, margin?.bottom));
					break;
				case StackAlignment.Center:
					switch (flow)
					{
						case FlowAxis.Horizontal:
							writer.WriteAttribute("vertical-align", "middle");
							writer.WriteMarginOrPadding("margin", margin);
							break;
						case FlowAxis.Vertical:
							writer.WriteMarginOrPadding("margin", new Thickness(Unit.Auto, margin?.top, Unit.Auto, margin?.bottom));
							writer.WriteAttribute("text-align", "center");
							break;
						default:
							throw new NotImplementedException();
					}
					break;
				default:
							throw new NotImplementedException();
			}
		}

		private static void WriteOrientation_(CSSWriter writer, Element element, Orientation orientation, Thickness margin)
		{
			var unit50Percent = new Unit(50, Unit.UnitKind.Percent);

			var marginHorizontalOffset = margin.left - margin.right;
			var marginVerticalOffset = margin.top - margin.bottom;

			switch (orientation)
			{
				case Orientation.LeftTop:
					writer.WriteAttribute("left", margin.left);
					writer.WriteAttribute("top", margin.top);
					break;
				case Orientation.StretchTop:
					writer.WriteAttribute("left", margin.left);
					writer.WriteAttribute("right", margin.right);
					writer.WriteAttribute("top", margin.top);
					break;
				case Orientation.CenterTop:
					writer.WriteAttribute("left", unit50Percent + marginHorizontalOffset);
					writer.WriteAttribute("top", margin.top);
					element.translate(new Unit(-50, Unit.UnitKind.Percent), new Unit());
					break;
				case Orientation.RightTop:
					writer.WriteAttribute("right", margin.right);
					writer.WriteAttribute("top", margin.top);
					break;
				case Orientation.LeftCenter:
					writer.WriteAttribute("left", margin.left);
					writer.WriteAttribute("top", unit50Percent + marginVerticalOffset);
					element.translate(new Unit(), new Unit(-50, Unit.UnitKind.Percent));
					break;
				case Orientation.StretchCenter:
					writer.WriteAttribute("left", margin.left);
					writer.WriteAttribute("right", margin.right);
					writer.WriteAttribute("top", unit50Percent + marginVerticalOffset);
					element.translate(new Unit(), new Unit(-50, Unit.UnitKind.Percent));
					break;
				case Orientation.Center:
					writer.WriteAttribute("left", unit50Percent + marginHorizontalOffset);
					writer.WriteAttribute("top", unit50Percent + marginVerticalOffset);
					element.translate(new Unit(-50, Unit.UnitKind.Percent), new Unit(-50, Unit.UnitKind.Percent));
					break;
				case Orientation.RightCenter:
					writer.WriteAttribute("right", margin.right);
					writer.WriteAttribute("top", unit50Percent + marginVerticalOffset);
					element.translate(new Unit(), new Unit(-50, Unit.UnitKind.Percent));
					break;
				case Orientation.LeftStretch:
					writer.WriteAttribute("left", margin.left);
					writer.WriteAttribute("top", margin.top);
					writer.WriteAttribute("bottom", margin.bottom);
					break;
				case Orientation.Stretch:
					writer.WriteAttribute("left", margin.left);
					writer.WriteAttribute("right", margin.right);
					writer.WriteAttribute("top", margin.top);
					writer.WriteAttribute("bottom", margin.bottom);
					break;
				case Orientation.CenterStretch:
					writer.WriteAttribute("left", unit50Percent + marginHorizontalOffset);
					writer.WriteAttribute("top", margin.top);
					writer.WriteAttribute("bottom", margin.bottom);
					element.translate(new Unit(-50, Unit.UnitKind.Percent), new Unit());
					break;
				case Orientation.RightStretch:
					writer.WriteAttribute("right", margin.right);
					writer.WriteAttribute("top", margin.top);
					writer.WriteAttribute("bottom", margin.bottom);
					break;
				case Orientation.LeftBottom:
					writer.WriteAttribute("left", margin.left);
					writer.WriteAttribute("bottom", margin.bottom);
					break;
				case Orientation.StretchBottom:
					writer.WriteAttribute("left", margin.left);
					writer.WriteAttribute("right", margin.right);
					writer.WriteAttribute("bottom", margin.bottom);
					break;
				case Orientation.CenterBottom:
					writer.WriteAttribute("left", unit50Percent + marginHorizontalOffset);
					writer.WriteAttribute("bottom", margin.bottom);
					element.translate(new Unit(-50, Unit.UnitKind.Percent), new Unit());
					break;
				case Orientation.RightBottom:
					writer.WriteAttribute("right", margin.right);
					writer.WriteAttribute("bottom", margin.bottom);
					break;
				default:
					throw new Exception();
			}
		}

		private static void WriteTransform(CSSWriter writer, Transform[] transforms)
		{
			if (transforms.Length == 0)
				return;
			var builder = new StringBuilder();
			foreach (var t in transforms)
			{
				var functionName = Transform.GetFunctionName(t.Kind);
				switch (t)
				{
					case SingleValueTransform singleValue:
						builder.Append($"{functionName}({CSSWriter.GetValue(singleValue.Value)}) ");
						break;
					case RotationTransform rotation:
						builder.Append($"{functionName}({CSSWriter.GetValue(rotation.Value)}deg) ");
						break;
					default:
						break;
				}
			}
			writer.WriteAttribute("transform", builder.ToString());
		}

		private static void WriteBrush(CSSWriter writer, Brush background)
		{
			if (background == null)
				return;
			switch (background.Mode)
			{
				case Brush.BrushMode.SolidColor:
					writer.WriteAttribute("background-color", background.Color);
					break;
				case Brush.BrushMode.ImageSource:
					writer.WriteAttribute("background-image", background.Image);
					writer.WriteAttribute("background-size", Stretching.Cover);
					writer.WriteAttribute("background-position", "center center");
					break;
				default:
					throw new Exception();
			}
		}

		private static void WriteBorder(CSSWriter writer, Border border)
		{
			if(border.get_AllEqual())
			{
				writer.WriteAttributeIfNotDefault("border", border.top, new BorderLine());
			}
			else
			{
				writer.WriteAttributeIfNotDefault("border-top", border.top, new BorderLine());
				writer.WriteAttributeIfNotDefault("border-right", border.right, new BorderLine());
				writer.WriteAttributeIfNotDefault("border-bottom", border.bottom, new BorderLine());
				writer.WriteAttributeIfNotDefault("border-left", border.left, new BorderLine());
			}
		}

	}
}
