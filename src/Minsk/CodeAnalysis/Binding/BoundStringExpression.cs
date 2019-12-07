using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundStringExpression : BoundExpression
	{
		public BoundStringExpression(BoundExpression[] expressions)
		{
			Expressions = expressions;
		}

		public BoundExpression[] Expressions { get; }

		public override TypeSymbol Type => PrimitiveTypeSymbol.String;

		public override BoundNodeKind Kind => BoundNodeKind.StringExpression;
	}
}