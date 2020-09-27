using Slides.Data;
using Slides.Styling;
using System.Linq;

namespace Slides.Elements
{
	public class TextElementStyling : ElementStyling
	{
		private static readonly Font stdFont = new Font("Arial");
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public float lineHeight { get => h_LineHeight ?? 1.4f; set => h_LineHeight = value; }
		public float? h_LineHeight { get; set; } = null;

		protected readonly new TextElement _element;
		protected override Element h_Element => _element;


		protected TextElementStyling(TextElement element) : base(element)
		{
			_element = element;
		}

		public TextElementStyling()
		{
		}

		public static TextElementStyling h_CreateElementStyling(TextElement e)
		{
			return new TextElementStyling(e);
		}


		// TODO: Use the same mechanisms as in Element!
		//Local like in "Not inheritated".
		private Font GetLocalFont()
		{
			if (font != null)
				return font;
			foreach (var style in get_AppliedStyles().Reverse())
				if (style.Substyles.GetRootCustomStyle().Properties.ContainsKey("font"))
					return (Font)style.Substyles.GetRootCustomStyle().Properties["font"];
			return null;
		}

		private Font GetParentFont() =>
			((TextElementStyling)h_parent?
			.h_Styling)?
			.GetFont() ??
			((TextElement)h_Element)?
			.get_SlideStyle()?
			.font;
		private Font GetFont()
		{
			Font stdStyleFont = null;
			if (Element.StdStyle != null && Element.StdStyle.GetMainStyle().Properties.TryGetValue("font", out var stdStyleFontObj))
				stdStyleFont = (Font)stdStyleFontObj;
			return GetLocalFont() ?? GetParentFont() ?? stdStyleFont;
		}

		private Unit GetLocalFontsize()
		{
			if (fontsize != null) return fontsize;
			foreach (var style in get_AppliedStyles().Reverse())
				if (style.GetMainStyle().Properties.ContainsKey("fontsize"))
					return (Unit)style.GetMainStyle().Properties["fontsize"];
			return null;
		}

		private Unit GetParentFontsize() =>
			((TextElementStyling)h_parent?
			.h_Styling)?
			.GetFontsize() ??
			((TextElement)h_Element)?
			.get_SlideStyle()?
			.fontsize;

		protected Unit GetFontsize()
		{
			return GetLocalFontsize() ?? GetParentFontsize();
		}

		public bool InheritsFont() => GetParentFont() != null && GetLocalFont() == null;
		public bool InheritsFontsize() => GetParentFontsize() != null && GetLocalFontsize() == null;

		public Vector2 h_MeasureText(string text, Font defaultFont = null)
		{
			var font = GetFont();
			if (font == null || !font.exists)
				font = defaultFont;
			if (font == null || !font.exists)
				font = stdFont;

			Unit stdStyleFontsize = null;
			if (Element.StdStyle != null && Element.StdStyle.GetMainStyle().Properties.TryGetValue("fontsize", out var stdStyleFontsizeObj))
				stdStyleFontsize = (Unit)stdStyleFontsizeObj;
			var fontsize = GetFontsize() ?? stdStyleFontsize ?? new Unit(14, Unit.UnitKind.Point);
			return font.Measure(text, fontsize * lineHeight);
		}

	}
	public abstract class TextElement : Element
	{
		public new TextElementStyling styling { get; }
		public override ElementStyling h_Styling => styling;


		public Font font { get => ((TextElementStyling)h_Styling).font; set => ((TextElementStyling)h_Styling).font = value; }
		public Unit fontsize { get => ((TextElementStyling)h_Styling).fontsize; set => ((TextElementStyling)h_Styling).fontsize = value; }
		public float lineHeight { get => ((TextElementStyling)h_Styling).lineHeight; set => ((TextElementStyling)h_Styling).lineHeight = value; }


		protected TextElement(object noInit) : base(noInit)
		{

		}
		public TextElement() : base(null)
		{
			styling = TextElementStyling.h_CreateElementStyling(this);
			Init();
		}

		protected Vector2 MeasureText(string text, Font defaultFont = null) => ((TextElementStyling)h_Styling).h_MeasureText(text, defaultFont);
	}
}
