using Slides.Elements;
using Slides.Helpers;

namespace Slides.Code
{
	public class CodeBlock : TextElement
	{
		public override ElementKind kind => ElementKind.CodeBlock;
		public override bool h_AllowsVerticalStretching => false;

		public bool showLineNumbers { get; set; } = true;
		public int lineStart { get; set; } = 1;
		public string code { get; set; }
		public string language { get; set; }

		public CodeBlock(string code, string language)
		{
			this.code = code;
			this.language = language;
		}

		internal override Unit get_InitialWidth() => new Unit(MeasureText(code, new Font("Consolas")).X, Unit.UnitKind.Pixel);
		internal override Unit get_InitialHeight()
		{
			var defaultFont = new Font("Consolas");
			var height = MeasureText(code, defaultFont, 1.25f).Y;
			height += MeasureText("\n\n\n", defaultFont, 0.75f).Y;
			return new Unit(height, Unit.UnitKind.Pixel);
		}
	}
}
