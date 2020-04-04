namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
	{
		public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
		{
			OpenParenthesisToken = openParenthesisToken;
			Expression = expression;
			CloseParenthesisToken = closeParenthesisToken;
		}

		public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
		public SyntaxToken OpenParenthesisToken { get; }
		public ExpressionSyntax Expression { get; }
		public SyntaxToken CloseParenthesisToken { get; }
		//TODO (Debate):
		//can you set (i) = 5?
		//I would say no.
		public override bool IsLValue => false;
	}
}