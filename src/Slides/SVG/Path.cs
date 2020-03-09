using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Slides.SVG
{
	public enum PathOperationKind
	{
		MoveTo,
		ClosePath,
		LineTo,
		LineToHorizontal,
		LineToVertical,
		CurveTo,
		SmoothCurveTo,
		QuadraticBezierCurveTo,
		SmoothQuadraticBezierCurveTo,
		EllipticalArc,
		Error
	}

	public abstract class PathOperation
	{
		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");
		public abstract PathOperationKind Kind { get; }
		public bool IsRelative { get; protected set; }

		public override string ToString() => _ToString();
		protected abstract string _ToString();

		protected string FloatToString(float f)
		{
			return f.ToString(_usCulture);
		}

		protected static string ToLetter(bool isRelative, PathOperationKind kind)
		{
			var result = string.Empty;
			switch (kind)
			{
				case PathOperationKind.MoveTo:
					result = "M";
					break;
				case PathOperationKind.ClosePath:
					result = "Z";
					break;
				case PathOperationKind.LineTo:
					result = "L";
					break;
				case PathOperationKind.LineToHorizontal:
					result = "H";
					break;
				case PathOperationKind.LineToVertical:
					result = "V";
					break;
				case PathOperationKind.CurveTo:
					result = "C";
					break;
				case PathOperationKind.SmoothCurveTo:
					result = "S";
					break;
				case PathOperationKind.QuadraticBezierCurveTo:
					result = "Q";
					break;
				case PathOperationKind.SmoothQuadraticBezierCurveTo:
					result = "T";
					break;
				case PathOperationKind.EllipticalArc:
					result = "A";
					break;
				case PathOperationKind.Error:
					break;
				default:
					throw new Exception();
			}
			if (isRelative) return result.ToLower();
			return result.ToUpper();
		}

	}
	public class ClosePathOperation : PathOperation
	{
		public ClosePathOperation()
		{
			IsRelative = true;
		}
		public override PathOperationKind Kind => PathOperationKind.ClosePath;

		protected override string _ToString() => "z";

	}

	public class SingleCoordinateOperation : PathOperation
	{
		public SingleCoordinateOperation(bool isRelative, PathOperationKind kind, float coordinate)
		{
			IsRelative = isRelative;
			Kind = kind;
			if (kind != PathOperationKind.LineToHorizontal && kind != PathOperationKind.LineToVertical)
				Kind = PathOperationKind.Error;
			Coordinate = coordinate;
		}
		public override PathOperationKind Kind { get; }

		public float Coordinate { get; }

		protected override string _ToString()
		{
			var strCoord = FloatToString(Coordinate);
			var op = ToLetter(IsRelative, Kind);
			if (string.IsNullOrEmpty(op))
				return string.Empty;
			return $"{op}{strCoord}";
		}
	}

	public class CoordinatePairOperation : PathOperation
	{
		public CoordinatePairOperation(bool isRelativ, PathOperationKind kind, float x, float y)
		{
			IsRelative = isRelativ;
			Kind = PathOperationKind.Error;
			if (kind == PathOperationKind.MoveTo ||
				kind == PathOperationKind.LineTo ||
				kind == PathOperationKind.SmoothQuadraticBezierCurveTo)
				Kind = kind;
			X = x;
			Y = y;
		}
		public override PathOperationKind Kind { get; }
		public float X { get; }
		public float Y { get; }

		protected override string _ToString()
		{
			var xCoord = FloatToString(X);
			var yCoord = FloatToString(Y);
			var op = ToLetter(IsRelative, Kind);
			if (string.IsNullOrEmpty(op))
				return string.Empty;
			return $"{op}{xCoord},{yCoord}";
		}
	}

	public class SVGPath : SVGShape
	{
		public override SVGElementKind kind => SVGElementKind.Path;

		private List<PathOperation> _operations = new List<PathOperation>();
		public PathOperation[] Operations => _operations.ToArray();

		public SVGPath(PathOperation[] operations, int width, int height)
		{
			_operations.AddRange(operations);
			this.width = width;
			this.height = height;
		}

		public SVGPath(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public void add(PathOperation operation)
		{
			_operations.Add(operation);
		}

		public void rotate(int degree)
		{
			rotate(degree, 0.5f, 0.5f);
		}
		public void rotate(int degree, float xOrigin, float yOrigin)
		{
			_operations = PathOperationHelper.ToAbsolute(_operations.ToArray()).ToList();
			var rad = (float)(degree * Math.PI / 180f);
			_operations = PathOperationHelper.Rotate(_operations.ToArray(), rad, width * xOrigin, height * yOrigin).ToList();
		}

		public override SVGPath toPath()
		{
			return new SVGPath(Operations, width, height);
		}
	}
}
