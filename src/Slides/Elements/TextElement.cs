using Slides.Styling;
using System.Linq;

namespace Slides.Elements
{
	public abstract class TextElement : Element
	{
		private static readonly Font stdFont = new Font("Arial");

		public Font font { get; set; }
		public Unit fontsize { get; set; }


		//Local like in "Not inheritated".
		//Better name!
		private Font GetLocalFont()
		{
			if (font != null)
				return font;
			foreach (var style in get_AppliedStyles().Reverse())
				if (style.Substyles.GetRootCustomStyle().Properties.ContainsKey("font"))
					return (Font)style.Substyles.GetRootCustomStyle().Properties["font"];
			return null;
		}

		private Font GetParentFont() => h_parent?.GetFont() ?? get_SlideStyle()?.font;

		private Font GetFont()
		{
			Font stdStyleFont = null;
			if (StdStyle != null && StdStyle.GetMainStyle().Properties.TryGetValue("font", out var stdStyleFontObj))
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

		private Unit GetParentFontsize() => h_parent?.GetFontsize() ?? get_SlideStyle()?.fontsize;

		protected Unit GetFontsize()
		{
			Unit stdStyleFontsize = null;
			if (StdStyle != null && StdStyle.GetMainStyle().Properties.TryGetValue("fontsize", out var stdStyleFontsizeObj))
				stdStyleFontsize = (Unit)stdStyleFontsizeObj;
			return GetLocalFontsize() ?? GetParentFontsize() ?? stdStyleFontsize;
		}

		public bool InheritsFont() => GetParentFont() != null && GetLocalFont() == null;
		public bool InheritsFontsize() => GetParentFontsize() != null && GetLocalFontsize() == null;

		protected Vector2 MeasureText(string text, Font defaultFont = null, float? lineHeightFactor = null)
		{
			var font = GetFont();
			if (font == null || !font.exists)
				font = defaultFont;
			if (font == null || !font.exists)
				font = stdFont;
			var fontsize = GetFontsize() ?? new Unit(14, Unit.UnitKind.Point);
			return font.Measure(text, fontsize * (lineHeightFactor ?? 1f));
		}
	}
}
