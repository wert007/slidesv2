namespace Slides.Code
{
	public class CodeBlock : Element
	{
		public override ElementType type => ElementType.CodeBlock;

		public bool useLineNumbers { get; set; } = true;
		public int lineStart { get; set; } = 1;
		public string code { get; set; }
		public string language { get; set; }
		public string caption { get; set; }
		public Font font { get; set; } = new Font("monospace");
		public Unit fontsize { get; set; }

		public CodeBlock(string code, string language, string caption)
		{
			this.code = code;
			this.language = language;
			this.caption = caption;
		}

		protected override Unit get_InitialWidth()
		{
			return new Unit(font.Measure(code, fontsize).X, Unit.UnitKind.Pixel);
		}

		protected override Unit get_InitialHeight()
		{
			return new Unit(font.Measure(code, fontsize).Y, Unit.UnitKind.Pixel);
		}
	}
}
