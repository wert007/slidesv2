namespace Minsk.CodeAnalysis.Syntax
{
	internal class LambdaExpressionSyntax : ExpressionSyntax
	{
		public LambdaExpressionSyntax(VariableExpressionSyntax variable, SyntaxToken arrowToken, ExpressionSyntax expression)
		{
			Variable = variable;
			ArrowToken = arrowToken;
			Expression = expression;
		}

		public VariableExpressionSyntax Variable { get; }
		public SyntaxToken ArrowToken { get; }
		public ExpressionSyntax Expression { get; }

		public override SyntaxKind Kind => SyntaxKind.LambdaExpression;
	}
}