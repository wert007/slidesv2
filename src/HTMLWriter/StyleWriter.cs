using Slides;
using Slides.Code;
using Slides.Debug;
using System;

namespace HTMLWriter
{
	public static class StyleWriter
	{
		private static string _currentTransform = null;
		private static string TypeSymbolToCSSClass(string type)
		{
			switch (type)
			{
				case "std": return "*";
				case "_Slide": return "slide";
				case "Label": return "label";
				case "Image": return "image";
				default:
					Logger.LogUnmatchedCSSField(type);
					return type.ToLower();
			}
		}
		private static void WriteTypedModification(CSSWriter writer, TypedModifications typedModifications)
		{
			//TODO: TypeSymbol to CSS-Class Converter
			writer.StartClass(TypeSymbolToCSSClass(typedModifications.Type));

			foreach (var modifiedField in typedModifications.ModifiedFields)
			{
				writer.WriteAttribute(ToCssAttribute(modifiedField.Key), modifiedField.Value);
			}
			writer.EndSelector();
		}
		private static void WriteStdStyle(CSSWriter writer, StdStyle style, out string stdTransition)
		{
			stdTransition = null;
			writer.StartSelector("*");
			Transition toWrite = null;
			foreach (var modifiedField in style.GetStyle("*").ModifiedFields)
			{
				switch (modifiedField.Key)
				{
					case "transition":
						toWrite = (Transition)modifiedField.Value;
						break;
					default:
						writer.WriteAttribute(ToCssAttribute(modifiedField.Key), modifiedField.Value);
						break;
				}
			}
			writer.EndSelector();

			foreach(var substyle in style.Substyles)
			{
				if (substyle.Type == "*")
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
			writer.StartClass(style.Name);
			Transition toWrite = null;

			foreach (var modifiedField in style.ModifiedFields)
			{
				switch (modifiedField.Key)
				{
					case "transition":
						toWrite = (Transition)modifiedField.Value;
						break;
					default:
						writer.WriteAttribute(ToCssAttribute(modifiedField.Key), modifiedField.Value);
						break;
				}
			}
			writer.EndSelector();

			if (toWrite != null)
			{
				WriteStdTransition(writer, toWrite.name, toWrite.from, toWrite.to);
			}
		}
		public static void Write(CSSWriter writer, Style style, out string stdTransition)
		{
			stdTransition = null;
			//TODO: Where do we want to find empty style? Probably during binding!!!
			//if (style.ModifiedFields.Count == 0)
			//{
			//	Logger.LogEmptyStyle(style.Name);
			//	return;
			//}
			if (style.Name == "std")
				WriteStdStyle(writer, (StdStyle)style, out stdTransition);
			else
				WriteCustomStyle(writer, (CustomStyle)style);

			
		}

		private static void WriteStdTransition(CSSWriter writer, string transitionName, TransitionCall from, TransitionCall to)
		{
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

		public static string ToCssAttribute(string field)
		{
			switch (field)
			{
				case "font":
					return "font-family";
				case "fontsize":
					return "font-size";
				case "text":
					return "innerHTML"; //TODO: Hacky
				case "padding":
				case "color":
				case "background":
				case "filter":
				case "margin":
					return field;
				default:
					Logger.LogUnmatchedCSSField(field);
					return field;
			}
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
			writer.WriteAttributeIfValue("filter", slide.Attributes.filter);
			writer.EndId();
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
			if (parent.Attributes.padding != null)
			{
				writer.WriteAlternateThickness("margin", parent.Attributes.padding);
				writer.WriteAttribute("height", 100 - parent.Attributes.padding.Top.Value - parent.Attributes.padding.Bottom.Value + "vh");
			}
			writer.EndSelector();
		}

		public static void WriteElement(CSSWriter writer, string id, Element element, Element parent = null)
		{
			_currentTransform = "";
			if (element.name == null)
				return;
			writer.StartId(id);
			WriteBrush(writer, element.background);
			writer.WriteAttributeIfValue("border-color", element.borderColor);
			writer.WriteAttribute("border-style", element.borderStyle);
			writer.WriteAttributeIfValue("border-thickness", element.borderThickness);
			writer.WriteAttributeIfValue("color", element.color);
			writer.WriteAttributeIfValue("filter", element.filter);

			WriteOrientation(writer, element, parent);

			writer.WriteAttributeIfValue("padding", element.padding);
			if (element.parent != null && element.parent.name != null)
				writer.WriteAttribute("parent", element.parent.name);

			if (element.rotation % 360 != 0)
				_currentTransform += $"rotate({CSSWriter.GetValue(element.rotation)}deg) ";

			if(!string.IsNullOrEmpty(_currentTransform))
			{
				writer.WriteAttribute("transform", _currentTransform);
				_currentTransform = "";
			}

			switch (element)
			{
				case Label l:
					writer.WriteAttributeIfValue("font-size", l.fontsize);
					if (l.fontsize != null)
						writer.WriteAttribute("line-height", l.fontsize + new Unit(6, Unit.UnitKind.Point));
					writer.WriteAttributeIfValue("font-family", l.font);
					writer.WriteAttribute("text-align", l.align);
					break;
				case List list:
					writer.WriteAttributeIfValue("font-size", list.fontsize);
					if (list.fontsize != null)
						writer.WriteAttribute("line-height", list.fontsize + new Unit(6, Unit.UnitKind.Point));
					writer.WriteAttributeIfValue("font-family", list.font);
					//writer.WriteAttribute("text-align", list.align);
					break;
				case BoxElement b:
					writer.WriteAttributeIfValue("font-size", b.fontsize);
					if (b.fontsize != null)
						writer.WriteAttribute("line-height", b.fontsize + new Unit(6, Unit.UnitKind.Point));
					break;
				case CodeBlock cb:
					writer.WriteAttribute("font-family", cb.font);
					writer.WriteAttributeIfValue("font-size", cb.fontsize);
					break;
				case Image i:
					//BIG TODO: USE OBJECT FIT AND NO DIVS ANYMORE!!!!!!!!!!!!!!!
					switch (i.GetImageMode())
					{
						case ImageMode.WidthAndHeightSet:
						case ImageMode.StretchOrientationWidthAndHeightSet:
						case ImageMode.NoSet:
							writer.WriteAttribute("object-fit", i.stretching);
							//writer.WriteAttribute("background-image", i.source);
							//writer.WriteAttribute("background-size", i.stretching);
							//writer.WriteAttribute("background-repeat", "no-repeat");
							//writer.WriteAttribute("background-position", "center center");
							break;
						case ImageMode.WidthSet:
						case ImageMode.HeightSet:
						case ImageMode.StretchOrientationWidthSet:
						case ImageMode.StretchOrientationHeightSet:
							writer.WriteAttribute("object-fit", i.stretching);
							break;
						default:
							throw new Exception();
					}
					break;
				default:
					break;
			}
			writer.EndId();

			if (element.hover == null)
				return;

			writer.StartId(id, pseudoClass: "hover");
			foreach (var modifiedField in element.hover.ModifiedFields)
			{
				writer.WriteAttribute(ToCssAttribute(modifiedField.Key), modifiedField.Value);
			}
			writer.EndSelector();
		}

		private static void WriteOrientation(CSSWriter writer, Element element, Element parent = null)
		{
			if (element.position == null)
			{
				if (parent != null && parent is Stack)
					writer.WriteAttribute("position", "relative");
				else
					writer.WriteAttribute("position", "absolute");
			}
			else
				writer.WriteAttribute("position", element.position);
			var margin = element.margin ?? new Thickness();
			if (element.padding != null)
				margin += element.padding;
			if (parent != null && parent.padding != null)
			{
				//writer.WriteAttribute("margin", padding);
				margin += parent.padding;
			}

			var unit50Percent = new Unit(50, Unit.UnitKind.Percent);
			var unit100Percent = new Unit(100, Unit.UnitKind.Percent);

			var hasHorizontalStretch = element.orientation == Orientation.StretchTop ||
												element.orientation == Orientation.Stretch ||
												element.orientation == Orientation.StretchCenter ||
												element.orientation == Orientation.StretchBottom;
			var hasVerticalStretch = element.orientation == Orientation.LeftStretch ||
												element.orientation == Orientation.Stretch ||
												element.orientation == Orientation.CenterStretch ||
												element.orientation == Orientation.RightStretch;


			//if (element is Image i)
			//{
			//	switch (i.GetImageMode())
			//	{
			//		case ImageMode.WidthAndHeightSet:
			//			writer.WriteAttribute("width", i.width);
			//			writer.WriteAttribute("height", i.height);
			//			break;
			//		case ImageMode.WidthSet:
			//			writer.WriteAttribute("width", i.width);
			//			writer.WriteAttribute("height", new Unit(0, Unit.UnitKind.Auto));
			//			break;
			//		case ImageMode.HeightSet:
			//			writer.WriteAttribute("width", new Unit(0, Unit.UnitKind.Auto));
			//			writer.WriteAttribute("height", i.height);
			//			break;
			//		case ImageMode.StretchOrientationWidthSet:
			//			writer.WriteAttribute("width", unit100Percent - margin.Horizontal);
			//			writer.WriteAttribute("height", new Unit(0, Unit.UnitKind.Auto));
			//			break;
			//		case ImageMode.StretchOrientationHeightSet:
			//			writer.WriteAttribute("width", new Unit(0, Unit.UnitKind.Auto));
			//			writer.WriteAttribute("height", unit100Percent - margin.Vertical);
			//			break;
			//		case ImageMode.StretchOrientationWidthAndHeightSet:
			//			writer.WriteAttribute("width", unit100Percent - margin.Horizontal);
			//			writer.WriteAttribute("height", unit100Percent - margin.Vertical);
			//			break;
			//		case ImageMode.NoSet:
			//			writer.WriteAttribute("width", new Unit(i.source.width, Unit.UnitKind.Pixel));
			//			writer.WriteAttribute("height", new Unit(i.source.height, Unit.UnitKind.Pixel));
			//			//TODO: If the image is bigger than the slide (width and/or height) it will be stretched..
			//			//Whats the solution to that???? Should we use a div in this case???
			//			writer.WriteAttribute("max-width", unit100Percent - margin.Horizontal);
			//			writer.WriteAttribute("max-height", unit100Percent - margin.Vertical);
			//			break;
			//		default:
			//			throw new Exception();
			//	}
			//}
			//else
			{
				if (hasVerticalStretch)
				{
					writer.WriteAttribute("height", unit100Percent - margin.Vertical);
				}
				else if (element.get_StyleHeight() == null)
				{
					/*if(!(element is CodeBlock codeBlock) && hasVerticalCenter)
						writer.WriteAttribute("height", unit100Percent - padding.Vertical);
					else*/
					//if (element is Image)
					//	writer.WriteAttribute("height", "auto");
					//else
						writer.WriteAttribute("height", "fit-content");
				}
				else
					writer.WriteAttributeIfValue("height", element.get_StyleHeight() - margin.Vertical);


				if (hasHorizontalStretch)
					writer.WriteAttribute("width", unit100Percent - margin.Horizontal);
				else if (element.get_StyleWidth() == null)
				{
					//if (element is Image)
					//	writer.WriteAttribute("width", "auto");
					//else
						writer.WriteAttribute("width", "fit-content");
				}
				else
					writer.WriteAttributeIfValue("width", element.get_StyleWidth() - margin.Horizontal);
			}
			var marginHorizontalOffset = margin.Left - margin.Right;
			var marginVerticalOffset = margin.Top - margin.Bottom;


			switch (element.orientation)
			{
				case Orientation.LeftTop:
					writer.WriteAttribute("left", margin.Left);
					writer.WriteAttribute("top", margin.Top);
					break;
				case Orientation.StretchTop:
					writer.WriteAttribute("left", margin.Left);
					writer.WriteAttribute("right", margin.Right);
					writer.WriteAttribute("top", margin.Top);
					break;
				case Orientation.CenterTop:
					writer.WriteAttribute("left", unit50Percent + marginHorizontalOffset);
					writer.WriteAttribute("top", margin.Top);
					_currentTransform += "translate(-50%, 0) ";
					break;
				case Orientation.RightTop:
					writer.WriteAttribute("right", margin.Right);
					writer.WriteAttribute("top", margin.Top);
					break;
				case Orientation.LeftCenter:
					writer.WriteAttribute("left", margin.Left);
					writer.WriteAttribute("top", unit50Percent + marginVerticalOffset);
					_currentTransform += "translate(0, -50%) ";
					break;
				case Orientation.StretchCenter:
					writer.WriteAttribute("left", margin.Left);
					writer.WriteAttribute("right", margin.Right);
					writer.WriteAttribute("top", unit50Percent + marginVerticalOffset);
					_currentTransform += "translate(0, -50%) ";
					break;
				case Orientation.Center:
					writer.WriteAttribute("left", unit50Percent + marginHorizontalOffset);
					writer.WriteAttribute("top", unit50Percent + marginVerticalOffset);
					_currentTransform += "translate(-50%, -50%) ";
					break;
				case Orientation.RightCenter:
					writer.WriteAttribute("right", margin.Right);
					writer.WriteAttribute("top", unit50Percent + marginVerticalOffset);
					_currentTransform += "translate(0, -50%) ";
					break;
				case Orientation.LeftStretch:
					writer.WriteAttribute("left", margin.Left);
					writer.WriteAttribute("top", margin.Top);
					writer.WriteAttribute("bottom", margin.Bottom);
					break;
				case Orientation.Stretch:
					writer.WriteAttribute("left", margin.Left);
					writer.WriteAttribute("right", margin.Right);
					writer.WriteAttribute("top", margin.Top);
					writer.WriteAttribute("bottom", margin.Bottom);
					break;
				case Orientation.CenterStretch:
					writer.WriteAttribute("left", unit50Percent + marginHorizontalOffset);
					writer.WriteAttribute("top", margin.Top);
					writer.WriteAttribute("bottom", margin.Bottom);
					_currentTransform += "translate(-50%, 0) ";
					break;
				case Orientation.RightStretch:
					writer.WriteAttribute("right", margin.Right);
					writer.WriteAttribute("top", margin.Top);
					writer.WriteAttribute("bottom", margin.Bottom);
					break;
				case Orientation.LeftBottom:
					writer.WriteAttribute("left", margin.Left);
					writer.WriteAttribute("bottom", margin.Bottom);
					break;
				case Orientation.StretchBottom:
					writer.WriteAttribute("left", margin.Left);
					writer.WriteAttribute("right", margin.Right);
					writer.WriteAttribute("bottom", margin.Bottom);
					break;
				case Orientation.CenterBottom:
					writer.WriteAttribute("left", unit50Percent + marginHorizontalOffset);
					writer.WriteAttribute("bottom", margin.Bottom);
					_currentTransform += "translate(-50%, 0) ";
					break;
				case Orientation.RightBottom:
					writer.WriteAttribute("right", margin.Right);
					writer.WriteAttribute("bottom", margin.Bottom);
					break;
				default:
					throw new Exception();
			}
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
