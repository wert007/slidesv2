using System.Collections.Immutable;
using System.Linq;

namespace Minsk.CodeAnalysis.Syntax
{
	class StringExpressionSyntax : ExpressionSyntax
	{
		public StringExpressionSyntax(SyntaxToken dollarSignToken, ImmutableArray<LiteralExpressionSyntax> literals, ImmutableArray<StringInsertionExpressionSyntax> insertions)
		{
			DollarSignToken = dollarSignToken;
			Literals = literals;
			Insertions = insertions;
			FirstElementIsLiteral = (literals.FirstOrDefault()?.Span.Start ?? int.MaxValue) < (insertions.FirstOrDefault()?.Span.Start ?? int.MaxValue);
		}

		public SyntaxToken DollarSignToken { get; }
		public ImmutableArray<LiteralExpressionSyntax> Literals { get; }
		public ImmutableArray<StringInsertionExpressionSyntax> Insertions { get; }
		public bool FirstElementIsLiteral { get; }

		public override SyntaxKind Kind => SyntaxKind.StringExpression;
	}
}