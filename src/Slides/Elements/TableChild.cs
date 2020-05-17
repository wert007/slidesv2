namespace Slides.Elements
{
	public class TableChild : Element
	{
		public string content { get; set; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Alignment? align { get; set; }

		public override ElementKind kind => ElementKind.TableChild;

		private static Font stdFont = new Font("Arial");

		public TableChild(string content)
		{
			this.content = content;
			font = null;
			fontsize = null;
			align = null;
			position = "relative";
		}

		public override string ToString() => content;

		public Unit get_Height(Font font, Unit fontsize)
		{
			var f = this.font;
			if (f == null)
				f = font;
			if (f == null)
				f = stdFont;
			var fsize = this.fontsize;
			if (fsize == null)
				fsize = fontsize;
			var v = f.Measure(content, fsize).Y;
			return new Unit(v, Unit.UnitKind.Pixel);
		}
		public Unit get_Width(Font font, Unit fontsize)
		{
			var f = this.font;
			if (f == null)
				f = font;
			if (f == null)
				f = stdFont;
			var fsize = this.fontsize;
			if (fsize == null)
				fsize = fontsize;
			var v = f.Measure(content, fsize).X;
			return new Unit(v, Unit.UnitKind.Pixel);
		}

		protected override Unit get_InitialWidth()
		{
			return get_Width(font, fontsize ?? new Unit(14, Unit.UnitKind.Point));
		}

		protected override Unit get_InitialHeight()
		{
			return get_Height(font, fontsize ?? new Unit(14, Unit.UnitKind.Point));
		}
	}
}
