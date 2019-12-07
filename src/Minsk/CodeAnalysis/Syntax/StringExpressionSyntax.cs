using System.Linq;

namespace Minsk.CodeAnalysis.Syntax
{
	class StringExpressionSyntax : ExpressionSyntax
	{
		public StringExpressionSyntax(SyntaxToken dollarSignToken, LiteralExpressionSyntax[] literals, StringInsertionExpressionSyntax[] insertions)
		{
			DollarSignToken = dollarSignToken;
			Literals = literals;
			Insertions = insertions;
			FirstElementIsLiteral = (literals.FirstOrDefault()?.Span.Start ?? int.MaxValue) < (insertions.FirstOrDefault()?.Span.Start ?? int.MaxValue);
		}

		public SyntaxToken DollarSignToken { get; }
		public LiteralExpressionSyntax[] Literals { get; }
		public StringInsertionExpressionSyntax[] Insertions { get; }
		public bool FirstElementIsLiteral { get; }

		public override SyntaxKind Kind => SyntaxKind.StringExpression;
	}
}