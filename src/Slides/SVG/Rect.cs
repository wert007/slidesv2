using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.SVG
{

	public class SVGGroup : SVGElement
	{
		//TODO: <g> doesn't need any width nor height! so why do we???
		public SVGGroup(SVGElement[] children, int width, int height)
		{
			Children = children;
			base.width = width;
			base.height = height;
		}

		public override SVGElementKind kind => SVGElementKind.Group;
		public SVGElement[] Children { get; }
	}

	public abstract class SVGShape : SVGElement
	{
		public abstract SVGPath toPath();
	}

	public class Rect : SVGShape
	{
		public float rx { get; set; }
		public float ry { get; set; }

		public override SVGElementKind kind => SVGElementKind.Rect;

		public Rect(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			rx = 0;
			ry = 0;
		}

		public override SVGPath toPath()
		{
			var result = new SVGPath(width, height);
			result.fill = fill;
			result.stroke = stroke;
			result.strokeWidth = strokeWidth;
			result.x = x;
			result.y = y;
			result.add(new CoordinatePairOperation(false, PathOperationKind.MoveTo, rx, 0));
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToHorizontal, width - rx));
			if(rx > 0 && ry > 0)
			{

			}
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToVertical, height - ry));
			if (rx > 0 && ry > 0)
			{

			}
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToHorizontal, rx));
			if (rx > 0 && ry > 0)
			{

			}
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToVertical, ry));
			if (rx > 0 && ry > 0)
			{

			}
			return result;
		}
	}

	public class Circle : SVGShape
	{
		public Circle(int cx, int cy, int r)
		{
			this.r = r;
			this.cx = cx;
			this.cy = cy;
			width = 2 * r;
			height = 2 * r;
		}

		public override SVGElementKind kind => SVGElementKind.Circle;
		public int r { get; set; }
		public int cx { get; set; }
		public int cy { get; set; }

		public override SVGPath toPath()
		{
			return new SVGPath(width, height);
		}
	}

	public class Ellipse : SVGShape
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

		public override SVGElementKind kind => SVGElementKind.Ellipse;
		public int rx { get; set; }
		public int ry { get; set; }
		public int cx { get; set; }
		public int cy { get; set; }

		public override SVGPath toPath()
		{
			return new SVGPath(width, height);
		}
	}

	public class Line : SVGShape
	{
		public Line(int x1, int y1, int x2, int y2)
		{
			x = x1;
			y = y1;
			width = x2 - x1;
			height = y2 - y1;
		}

		public override SVGElementKind kind => SVGElementKind.Line;

		public override SVGPath toPath()
		{
			return new SVGPath(width, height);
		}
	}

	public class Polyline : SVGShape
	{
		public Polyline(int[] points)
		{
			if (points.Length % 2 != 0)
				throw new Exception();
			for (int i = 0; i < points.Length; i++)
			{
				if (i % 2 == 0)
					width = Math.Max(width, points[i]);
				else
					height = Math.Max(height, points[i]);
			}
			_points = new List<int>(points);
		}

		public override SVGElementKind kind => SVGElementKind.Polyline;

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

		public override SVGPath toPath()
		{
			return new SVGPath(width, height);
		}
	}

	public class Polygon : SVGShape
	{
		public Polygon(int[] points)
		{
			if (points.Length % 2 != 0)
				throw new Exception();
			for (int i = 0; i < points.Length; i++)
			{
				if (i % 2 == 0)
					width = Math.Max(width, points[i]);
				else
					height = Math.Max(height, points[i]);
			}
			_points = new List<int>(points);
		}

		public override SVGElementKind kind => SVGElementKind.Polygon;

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

		public override SVGPath toPath()
		{
			return new SVGPath(width, height);
		}
	}
}
