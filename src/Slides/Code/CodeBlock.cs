using Slides.Data;
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
			//TODO: How do we replace these line-height|s rightfully?
			var height = MeasureText(code, defaultFont).Y; //line-height = 1.25f
			height += MeasureText("\n\n\n", defaultFont).Y; //line-height = 0.75f
			return new Unit(height, Unit.UnitKind.Pixel);
		}
	}
}
