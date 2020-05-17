using Slides;
using System;
using System.Linq;

namespace SVGLib.Datatypes
{
	public class Transform
	{
		public TransformKind Kind { get; }
		public double[] Values { get; }

		private Transform(TransformKind kind, params double[] values)
		{
			Kind = kind;
			Values = values;
		}

		public static Transform CreateMatrixTransform(double a, double b, double c, double d, double e, double f)
			=> new Transform(TransformKind.Matrix, a, b, c, d, e, f);

		public static Transform CreateTranslation(double x, double y = 0)
		{
			if (y == 0)
				return new Transform(TransformKind.Translate, x);
			return new Transform(TransformKind.Translate, x, y);
		}

		public static Transform CreateScale(double x, double y = double.NaN)
		{
			if (double.IsNaN(y))
				return new Transform(TransformKind.Scale, x);
			return new Transform(TransformKind.Scale, x, y);
		}
		public static Transform CreateRotation(double degree, Vector2? origin = null)
		{
			if (origin == null)
				return new Transform(TransformKind.Rotate, degree);
			return new Transform(TransformKind.Rotate, degree, origin.Value.X, origin.Value.Y);
		}

		public static Transform CreateSkewX(double a) => new Transform(TransformKind.SkewX, a);
		public static Transform CreateSkewY(double a) => new Transform(TransformKind.SkewY, a);

		public override string ToString()
		{
			var args = string.Join(",", Values.Select(v => v.ToString(TextHelper.UsCulture)));
			switch (Kind)
			{
				case TransformKind.Translate:
					return $"translate({args})";
				case TransformKind.Scale:
					return $"scale({args})";
				case TransformKind.Rotate:
					return $"rotate({args})";
				case TransformKind.SkewX:
					return $"skewX({args})";
				case TransformKind.SkewY:
					return $"skewY({args})";
				case TransformKind.Matrix:
					return $"matrix({args})";
				default:
					throw new Exception();
			}
		}

	}
}
