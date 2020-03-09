namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class AssignmentExpressionSyntax : ExpressionSyntax
	{
		public AssignmentExpressionSyntax(VariableExpressionSyntax[] variables, SyntaxToken[] commas, SyntaxToken equalsToken, ExpressionSyntax expression)
		{
			Variables = variables;
			Commas = commas;
			OperatorToken = equalsToken;
			Expression = expression;
		}

		public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
		public VariableExpressionSyntax[] Variables { get; }
		public SyntaxToken[] Commas { get; }
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Expression { get; }
	}
}