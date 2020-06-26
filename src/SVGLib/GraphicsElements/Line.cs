namespace SVGLib.GraphicsElements
{
	public class Line : BasicShape
	{
		public Line(int x1, int y1, int x2, int y2)
		{
			X = x1;
			Y = y1;
			Width = x2 - x1;
			Height = y2 - y1;
		}

		public override SVGElementKind Kind => SVGElementKind.Line;

		public override Path toPath()
		{
			return new Path(Width, Height);
		}
	}
}
