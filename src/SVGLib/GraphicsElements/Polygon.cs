using System;
using System.Collections.Generic;

namespace SVGLib.GraphicsElements
{
	public class Polygon : BasicShape
	{
		public Polygon(int[] points)
		{
			if (points.Length % 2 != 0)
				throw new Exception();
			for (int i = 0; i < points.Length; i++)
				if (i % 2 == 0)
					width = Math.Max(width, points[i]);
				else
					height = Math.Max(height, points[i]);
			_points = new List<int>(points);
		}

		public override SVGElementKind Kind => SVGElementKind.Polygon;

		private List<int> _points;

		public int[] points => _points.ToArray();

		public void add(int x, int y)
		{
			_points.Add(x);
			_points.Add(y);
		}
		public void add(int[] p)
		{
			if (p.Length % 2 != 0)
				return;
			_points.AddRange(p);
		}

		public override Path toPath()
		{
			return new Path(width, height);
		}
	}
}
