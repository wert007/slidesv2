namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class AssignmentExpressionSyntax : ExpressionSyntax
	{
		public AssignmentExpressionSyntax(LExpressionSyntax lvalue, SyntaxToken equalsToken, ExpressionSyntax expression)
		{
			LValue = lvalue;
			OperatorToken = equalsToken;
			Expression = expression;
		}

		public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
		public LExpressionSyntax LValue { get; }
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Expression { get; }

		public override bool IsLValue => false;
	}
}