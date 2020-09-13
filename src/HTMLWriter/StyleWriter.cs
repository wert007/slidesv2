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
using System.Linq;
using System.Text;

namespace HTMLWriter
{
	public static class StyleWriter
	{
		private static void WriteTypedModification(CSSWriter writer, Substyle substyle)
		{
			writer.StartSelector(SelectorToString(substyle.Selector));

			foreach (var property in substyle.Properties)
			{
				//if (property.IsNonCSS) continue;
				if (substyle.Selector.Name == "slide" && property.Key == "padding") continue;
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
			return;
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
			WriteBrush(writer, element.h_Background);
			var properties = new string[] { "borderColor", "borderStyle", "borderWidth", "color", "filter", "padding" };
			foreach (var prop in properties)
			{
				writer.WriteAttributeIfValue(CSSWriter.ToCssAttribute(prop), element.get_ActualField(prop));
			}
			WriteOrientation(writer, element, parent);


			WriteTransform(writer, element.get_Transforms());

			switch (element)
			{
				case TextElement e:
					writer.WriteAttributeIfValueOrInherit("font-size", e.fontsize, e.InheritsFontsize());

					if (e.fontsize != null && e.kind != ElementKind.Captioned)
						writer.WriteAttribute("line-height", e.fontsize + new Unit(6, Unit.UnitKind.Point));

					writer.WriteAttributeIfValueOrInherit("font-family", e.font, e.InheritsFont());
					switch (e)
					{
						case Label l:
							writer.WriteAttributeIfNotDefault("text-align", l.align, Alignment.Unset);
							break;
						case List list:
							writer.WriteAttributeIfNotDefault("list-style-type", list.markerType, List.ListMarkerType.Disk);
							if (list.get_TextMarker() != null)
								writer.WriteAttribute("padding-left", "1em");
							break;
						case Table _:
							writer.WriteAttribute("border-collapse", "collapse");
							break;
					}
					break;
				case Image i:
					writer.WriteAttribute("object-fit", i.stretching);
					break;
				default:
					break;
			}
			writer.EndId();


			if (element is List listCustomMarker)
			{
				if (!listCustomMarker.isOrdered && listCustomMarker.get_TextMarker() != null)
				{
					writer.StartId($"{id} li", "before");
					writer.WriteAttribute("content", $"\"{FormattedString.Convert(listCustomMarker.get_TextMarker()).ToHTML()}\"");
					writer.WriteAttribute("left", "0");
					writer.WriteAttribute("position", "absolute");
					writer.EndSelector();
				}
			}

			if (element.hover == null)
				return;

			writer.StartId(id, pseudoClass: "hover");
			//There should be now substyles in hover. 
			//Maybe there should. But we don't support it right now!
			foreach (var property in element.hover.Substyles.GetIterator().Single().Properties)
			{
				writer.WriteAttribute(CSSWriter.ToCssAttribute(property.Key), property.Value);
			}
			writer.EndSelector();

		}




		//TODO(Next Thing): Think about differences between margin and padding.
		//In CSS and in Slides. 
		private static void WriteOrientation(CSSWriter writer, Element element, Element parent = null)
		{
			if (string.IsNullOrEmpty(element.position))
			{
				if (parent != null && parent is Stack || parent?.kind == ElementKind.List || parent?.kind == ElementKind.Captioned)
					writer.WriteAttribute("position", "relative");
				else if(element.kind != ElementKind.Label)
					writer.WriteAttribute("position", "absolute");
			}
			else
				writer.WriteAttribute("position", element.position);


			var m = element.margin;

			var margin = m + element.padding;
			if (parent != null && parent.padding != null)
			{
				margin += parent.padding;
			}

			//writer.WriteAttributeIfValue("margin", element.margin);
			//writer.WriteAttributeIfValue("padding", element.padding);


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


			WriteOrientation_(writer, element, orientation, m);
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
					writer.WriteAttribute("background-size", ImageStretching.Cover);
					writer.WriteAttribute("background-position", "center center");
					break;
				default:
					throw new Exception();
			}
		}

	}
}
