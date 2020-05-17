namespace SVGLib.GraphicsElements
{
	public class Line : BasicShape
	{
		public Line(int x1, int y1, int x2, int y2)
		{
			x = x1;
			y = y1;
			width = x2 - x1;
			height = y2 - y1;
		}

		public override SVGElementKind Kind => SVGElementKind.Line;

		public override Path toPath()
		{
			return new Path(width, height);
		}
	}
}
