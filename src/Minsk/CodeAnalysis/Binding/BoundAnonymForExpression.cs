using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundAnonymForExpression : BoundExpression
	{
		public BoundAnonymForExpression()
		{

		}

		public override TypeSymbol Type => PrimitiveTypeSymbol.AnonymFor;

		public override BoundNodeKind Kind => BoundNodeKind.AnonymForExpression;

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			return true;
		}
	}
}