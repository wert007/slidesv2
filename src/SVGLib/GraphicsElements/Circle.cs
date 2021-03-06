﻿namespace SVGLib.GraphicsElements
{
	public class Circle : BasicShape
	{
		public Circle(int cx, int cy, int r)
		{
			this.r = r;
			this.cx = cx;
			this.cy = cy;
			Width = 2 * r;
			Height = 2 * r;
		}

		public override SVGElementKind Kind => SVGElementKind.Circle;
		public int r { get; set; }
		public int cx { get; set; }
		public int cy { get; set; }

		public override Path toPath()
		{
			return new Path(Width, Height);
		}
	}
}
