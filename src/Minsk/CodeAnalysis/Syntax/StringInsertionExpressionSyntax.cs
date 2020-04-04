namespace Minsk.CodeAnalysis.Syntax
{
	internal class StringInsertionExpressionSyntax : ExpressionSyntax
	{
		public StringInsertionExpressionSyntax(SyntaxToken openBraceToken, ExpressionSyntax expression, SyntaxToken closeBraceToken)
		{
			OpenBraceToken = openBraceToken;
			Expression = expression;
			CloseBraceToken = closeBraceToken;
		}

		public override SyntaxKind Kind => SyntaxKind.StringInsertionExpression;

		public SyntaxToken OpenBraceToken { get; }
		public ExpressionSyntax Expression { get; }
		public SyntaxToken CloseBraceToken { get; }

		public override bool IsLValue => false;
	}
}