namespace SVGLib.PathOperations
{
	public class SingleCoordinateOperation : PathOperation
	{
		public SingleCoordinateOperation(bool isRelative, PathOperationKind kind, double coordinate)
		{
			IsRelative = isRelative;
			Kind = kind;
			if (kind != PathOperationKind.LineToHorizontal && kind != PathOperationKind.LineToVertical)
				Kind = PathOperationKind.Error;
			Coordinate = coordinate;
		}
		public override PathOperationKind Kind { get; }

		public double Coordinate { get; }

		protected override string _ToString()
		{
			var strCoord = DoubleToString(Coordinate);
			var op = ToLetter(IsRelative, Kind);
			if (string.IsNullOrEmpty(op))
				return string.Empty;
			return $"{op}{strCoord}";
		}
	}
}
