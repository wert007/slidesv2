using ImageMagick;
using Slides;
using Slides.SVG;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SVGShape = SVGLib.GraphicsElements.BasicShape;
using SVGPath = SVGLib.GraphicsElements.Path;
using SVGText = SVGLib.GraphicsElements.Text;
using SVGColor = SVGLib.Datatypes.Color;
using ViewBox = SVGLib.Datatypes.ViewBox;
using SVGGraphicsElement = SVGLib.GraphicsElements.SVGGraphicsElement;
using Circle = SVGLib.GraphicsElements.Circle;
using Line = SVGLib.GraphicsElements.Line;
using Rect = SVGLib.GraphicsElements.Rect;
using SVGLib.Parsing;
using SVGLib.PathOperations;
using SVGLib.ContainerElements;
using SVGLib;
using Slides.Elements.SVG;
using Slides.Elements;
using Slides.Data;

namespace Minsk.CodeAnalysis.SlidesTypes
{

	public static class GlobalSVGFunctions
	{
		public static UnitRect rect(Unit width, Unit height) => new UnitRect(width, height);
		public static UnitRect rect(UnitPair topLeft, UnitPair bottomRight) => UnitRect.FromBounds(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
		public static UnitLine line(UnitPair p1, UnitPair p2) => line(p1.X, p1.Y, p2.X, p2.Y);
		public static UnitLine line(Unit x1, Unit y1, Unit x2, Unit y2) => new UnitLine(x1, y1, x2, y2);

		public static SVGPath path(string m, int width, int height)
		{
			return new SVGPath(SVGParser.ParsePathInstructions(m), width, height);
		}

		public static SVGPath arrow(Direction direction, int arrowLength, int arrowWidth, float baseWidthPercent, float arrowHeadLengthPercent)
		{
			var initWidth = arrowWidth;
			var initHeight = arrowLength;
			if (direction == Direction.East || direction == Direction.West)
			{
				initWidth = arrowLength;
				initHeight = arrowWidth;
			}
			var result = new SVGPath(initWidth, initHeight);
			var width = initWidth;
			var height = initHeight;
			var baseWidth = baseWidthPercent * arrowWidth;
			var arrowHeadLength = arrowHeadLengthPercent * arrowLength;
			float ax, ay, bx, by, cx, cy, dx, dy, ex, ey, fx, fy, gx, gy;

			switch (direction)
			{
				case Direction.North:
					ax = (width - baseWidth) / 2;
					ay = height;
					bx = ax;
					by = arrowHeadLength;
					cx = 0;
					cy = by;
					dx = width / 2;
					dy = 0;
					ex = width;
					ey = by;
					fx = width - ax;
					fy = by;
					gx = fx;
					gy = height;
					break;
				case Direction.South:
					ax = width - (width - baseWidth) / 2;
					ay = 0;
					bx = ax;
					by = height - arrowHeadLength;
					cx = width;
					cy = by;
					dx = width / 2;
					dy = height;
					ex = 0;
					ey = by;
					fx = (width - baseWidth) / 2;
					fy = by;
					gx = fx;
					gy = 0;
					break;
				case Direction.West:
					ax = width;
					ay = height - (height - baseWidth) / 2;
					bx = arrowHeadLength;
					by = ay;
					cx = bx;
					cy = height;
					dx = 0;
					dy = height / 2;
					ex = bx;
					ey = 0;
					fx = ex;
					fy = (height - baseWidth) / 2;
					gx = width;
					gy = fy;
					break;
				case Direction.East:
					ax = 0;
					ay = (height - baseWidth) / 2;
					bx = width - arrowHeadLength;
					by = ay;
					cx = bx;
					cy = 0;
					dx = width;
					dy = height / 2;
					ex = bx;
					ey = height;
					fx = bx;
					fy = height - (height - baseWidth) / 2;
					gx = 0;
					gy = fy;
					break;
				default:
					throw new Exception();
			}

			result.add(new CoordinatePairOperation(false, PathOperationKind.MoveTo, ax, ay));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, bx, by));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, cx, cy));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, dx, dy));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, ex, ey));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, fx, fy));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, gx, gy));
			result.add(new ClosePathOperation());
			return result;
		}
		
		public static SVGPath intersectPaths(SVGPath a, SVGPath b)
		{
			var test = PathOperationIntersectionHandler.PathOperationShape(a, b, PathShapeOperation.Intersect);
			return new SVGPath(test, a.Width, a.Height);
		}

		public static SVGPath unitePaths(SVGPath a, SVGPath b)
		{
			var ops = PathOperationIntersectionHandler.PathOperationShape(a, b, PathShapeOperation.Unite);
			return new SVGPath(ops, a.Width, b.Width);
		}
		public static SVGPath differPaths(SVGPath a, SVGPath b)
		{
			var ops = PathOperationIntersectionHandler.PathOperationShape(a, b, PathShapeOperation.Difference);
			return new SVGPath(ops, a.Width, b.Width);
		}

		public static SVGTag loadSVG(string fileName)
		{
			var path = Path.Combine(CompilationFlags.Directory, fileName);
			var contents = File.ReadAllText(path);
			var parser = new SVGParser();
			return parser.FromSource(contents);
		}

	}
}
