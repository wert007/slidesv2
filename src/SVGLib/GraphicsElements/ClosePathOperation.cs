namespace SVGLib.PathOperations
{
	public class ClosePathOperation : PathOperation
	{
		public ClosePathOperation()
		{
			IsRelative = true;
		}
		public override PathOperationKind Kind => PathOperationKind.ClosePath;

		protected override string _ToString() => "z";

	}
}
