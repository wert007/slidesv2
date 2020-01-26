namespace Minsk.CodeAnalysis.Syntax
{
	internal class MathExpressionSyntax : ExpressionSyntax
	{

		public MathExpressionSyntax(SyntaxToken mathKeyword, LiteralExpressionSyntax stringLiteral)
		{
			MathKeyword = mathKeyword;
			StringLiteral = stringLiteral;
		}

		public SyntaxToken MathKeyword { get; }
		public LiteralExpressionSyntax StringLiteral { get; }

		public override SyntaxKind Kind => SyntaxKind.MathExpression;
	}
}