namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundArrayIndex
	{
		public BoundArrayIndex(BoundExpression boundIndex, BoundArrayIndex boundChild)
		{
			BoundIndex = boundIndex;
			BoundChild = boundChild;
		}

		public BoundExpression BoundIndex { get; }
		public BoundArrayIndex BoundChild { get; }
	}
}