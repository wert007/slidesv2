using System.Collections.Immutable;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundStringExpression : BoundExpression
	{
		public BoundStringExpression(ImmutableArray<BoundExpression> expressions)
		{
			Expressions = expressions;
		}

		public ImmutableArray<BoundExpression> Expressions { get; }

		public override TypeSymbol Type => PrimitiveTypeSymbol.String;

		public override BoundNodeKind Kind => BoundNodeKind.StringExpression;
	}
}