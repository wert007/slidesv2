using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundArrayIndex : BoundExpression
	{
		public BoundArrayIndex(BoundExpression boundIndex, BoundArrayIndex boundChild)
		{
			BoundIndex = boundIndex;
			BoundChild = boundChild;
		}

		public BoundExpression BoundIndex { get; }
		public BoundArrayIndex BoundChild { get; }

		public override BoundNodeKind Kind => BoundNodeKind.ArrayIndex;

		public override TypeSymbol Type => PrimitiveTypeSymbol.Void;

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var other = (BoundArrayIndex)expression;
			if (!BoundIndex.Equals(other.BoundIndex))
				return false;
			if (BoundChild == null)
			{
				if (other.BoundChild == null)
					return true;
				else
					return false;
			}
			else
				return BoundChild.Equals(other.BoundChild);
					}
	}
}