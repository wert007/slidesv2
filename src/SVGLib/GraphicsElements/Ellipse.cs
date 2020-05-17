namespace SVGLib.GraphicsElements
{
	public class Ellipse : BasicShape
	{
		public Ellipse(int cx, int cy, int rx, int ry)
		{
			this.cx = cx;
			this.cy = cy;
			this.rx = rx;
			this.ry = ry;
			width = rx * 2;
			height = ry * 2;
		}

		public override SVGElementKind Kind => SVGElementKind.Ellipse;
		public int rx { get; set; }
		public int ry { get; set; }
		public int cx { get; set; }
		public int cy { get; set; }

		public override Path toPath()
		{
			return new Path(width, height);
		}
	}
}
