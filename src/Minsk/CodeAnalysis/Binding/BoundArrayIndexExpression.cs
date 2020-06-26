using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundArrayAccessExpression : BoundExpression
	{
		public BoundArrayAccessExpression(BoundExpression child, BoundExpression index)
		{
			Child = child;
			Index = index;
			var arrayType = child.Type as ArrayTypeSymbol;
			Type = arrayType?.Child ?? PrimitiveTypeSymbol.Error;
		}


		public override BoundNodeKind Kind => BoundNodeKind.ArrayAccessExpression;

		public BoundExpression Child { get; }
		public BoundExpression Index { get; }

		public override TypeSymbol Type { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var other = (BoundArrayAccessExpression)expression;
			if (!Index.Equals(other.Index))
				return false;
			return Child.Equals(other.Child);
		}
	}
}