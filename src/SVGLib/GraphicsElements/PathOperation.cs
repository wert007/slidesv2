using System;
using System.Globalization;

namespace SVGLib.PathOperations
{
	public abstract class PathOperation
	{
		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");
		public abstract PathOperationKind Kind { get; }
		public bool IsRelative { get; protected set; }

		public override string ToString() => _ToString();
		protected abstract string _ToString();

		protected string DoubleToString(double f)
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
}
