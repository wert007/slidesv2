using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundErrorExpression : BoundExpression
	{
		public override TypeSymbol Type => PrimitiveTypeSymbol.Error;

		public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
	}
}
