namespace Slides.Elements
{
	public class TableChild : Element
	{
		internal delegate void ContentUpdatedHandler();
		internal event ContentUpdatedHandler ContentUpdated;

		private string _content;
		public string content { get => _content; set
			{
				_content = value;
				ContentUpdated?.Invoke();
			}
		}
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Alignment? align { get; set; }

		public override ElementKind kind => ElementKind.TableChild;

		private static Font stdFont = new Font("Arial");

		private Unit _top;
		private Unit _left;

		public override Unit top => _top;
		public override Unit left => _left;

		public void set_Top(Unit t) => _top = t;
		public void set_Left(Unit l) => _left = l;

		public TableChild(string content)
		{
			_content = content;
			font = null;
			fontsize = null;
			align = null;
			//TODO: Remove this. There should be a solution in core.css
			position = "relative";
		}

		public override string ToString() => content;

		public Unit get_ActualTableChildHeight() => get_ActualHeight();
		public Unit get_ActualTableChildWidth() => get_ActualWidth();

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
