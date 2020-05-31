using Slides;
using Slides.Code;
using Slides.Debug;
using Slides.Elements;
using Slides.SVG;
using Slides.Transforms;
using System;
using System.Linq;
using System.Text;

namespace HTMLWriter
{
	public static class StyleWriter
	{
		private static string TypeSymbolToCSSClass(string type)
		{
			switch (type)
			{
				case "std": return "*";
				case "Slide": return "slide";
				case "Label": return "label";
				case "Image": return "image";
				case "Table": return "table, .tablecell";
				default:
					Logger.LogUnmatchedCSSField(type);
					return type.ToLower();
			}
		}
		private static void WriteTypedModification(CSSWriter writer, TypedModifications typedModifications)
		{
			writer.StartClass(TypeSymbolToCSSClass(typedModifications.Type));

			foreach (var modifiedField in typedModifications.ModifiedFields)
			{
				writer.WriteAttribute(CSSWriter.ToCssAttribute(modifiedField.Key), modifiedField.Value);
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
						writer.WriteAttribute(CSSWriter.ToCssAttribute(modifiedField.Key), modifiedField.Value);
						break;
				}
			}
			writer.EndSelector();

			foreach (var substyle in style.Substyles)
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
					case "orientation":
						//This is no css attribute. we set it in the element.
					case "margin":
						//We ignore margin, because we write it per element
						//and mostly just use left, top, right, bottom and
						//not the actual margin.
						break;
					default:
						writer.WriteAttribute(CSSWriter.ToCssAttribute(modifiedField.Key), modifiedField.Value);
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
				writer.WriteAttribute("height", 100 - parent.Attributes.padding.top.Value - parent.Attributes.padding.bottom.Value + "vh");
			}
			writer.EndSelector();
		}

		public static void WriteElement(CSSWriter writer, string parentName, Element element, Element parent = null)
		{
			if (element.name == null)
				return;
			var id = $"{parentName}-{element.name}";
			//TODO: New way to get properties from elements?
			//A way that includes the applied styles??
			//something like
			//var listOfPropertys = {"borderColor"};
			//foreach prop in listOfPropertys:
			//	writer.WriteAttributeIfValue(CSSWriter.ToCssAttribute(prop), element.get_Property(prop));
			writer.StartId(id);
			WriteBrush(writer, element.background);
			var properties = new string[] { "borderColor", "borderStyle", "borderThickness", "color", "filter", "padding" };
			foreach (var prop in properties)
			{
				writer.WriteAttributeIfValue(CSSWriter.ToCssAttribute(prop), element.get_Property(prop));
			}
			WriteOrientation(writer, element, parent);


			WriteTransform(writer, element.get_Transforms());

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
					writer.WriteAttribute("object-fit", i.stretching);
					break;
				case Table t:
					writer.WriteAttribute("border-collapse", "collapse");
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
				writer.WriteAttribute(CSSWriter.ToCssAttribute(modifiedField.Key), modifiedField.Value);
			}
			writer.EndSelector();
		}




		//TODO(Next Thing): Think about differences between margin and padding.
		//In CSS and in Slides. 
		private static void WriteOrientation(CSSWriter writer, Element element, Element parent = null)
		{
			var appliedStyles = element.get_AppliedStyles();
			if (element.position == null)
			{
				if (parent != null && parent is Stack)
					writer.WriteAttribute("position", "relative");
				else
					writer.WriteAttribute("position", "absolute");
			}
			else
				writer.WriteAttribute("position", element.position);


			var m = (Thickness)appliedStyles.FirstOrDefault(s => s.ModifiedFields.ContainsKey("margin"))?.ModifiedFields["margin"] ?? new Thickness();
			if (element.margin != new Thickness())
				m = element.margin;

			var margin = m + element.padding;
			if (parent != null && parent.padding != null)
			{
				margin += parent.padding;
			}

			//writer.WriteAttributeIfValue("margin", element.margin);
			//writer.WriteAttributeIfValue("padding", element.padding);


			var unit100Percent = new Unit(100, Unit.UnitKind.Percent);
			var orientation = element.orientation;
			if(element.get_Property("orientation") == null)
			{
				foreach (var style in appliedStyles)
				{
					if (style.ModifiedFields.ContainsKey("orientation"))
						orientation = (Orientation)style.ModifiedFields["orientation"];
				}
			}
			var hasHorizontalStretch = orientation == Orientation.StretchTop ||
												orientation == Orientation.Stretch ||
												orientation == Orientation.StretchCenter ||
												orientation == Orientation.StretchBottom;
			var hasVerticalStretch = orientation == Orientation.LeftStretch ||
											 orientation == Orientation.Stretch ||
											 orientation == Orientation.CenterStretch ||
											 orientation == Orientation.RightStretch;


			{
				if (hasVerticalStretch)
				{
					writer.WriteAttribute("height", unit100Percent - margin.Vertical);
				}
				else if (element.get_StyleHeight() == null)
				{
					writer.WriteAttribute("height", "fit-content");
				}
				else
					writer.WriteAttributeIfValue("height", element.get_StyleHeight());


				if (hasHorizontalStretch)
					writer.WriteAttribute("width", unit100Percent - margin.Horizontal);
				else if (element.get_StyleWidth() == null)
				{
					writer.WriteAttribute("width", "fit-content");
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
