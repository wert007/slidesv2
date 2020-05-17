namespace SVGLib.PathOperations
{
	public class CoordinatePairOperation : PathOperation
	{
		public CoordinatePairOperation(bool isRelativ, PathOperationKind kind, double x, double y)
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
		public double X { get; }
		public double Y { get; }

		protected override string _ToString()
		{
			var xCoord = DoubleToString(X);
			var yCoord = DoubleToString(Y);
			var op = ToLetter(IsRelative, Kind);
			if (string.IsNullOrEmpty(op))
				return string.Empty;
			return $"{op}{xCoord},{yCoord}";
		}
	}
}
