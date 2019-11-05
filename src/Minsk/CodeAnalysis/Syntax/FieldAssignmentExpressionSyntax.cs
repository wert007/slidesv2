namespace Minsk.CodeAnalysis.Syntax
{
	internal class FieldAssignmentExpressionSyntax : ExpressionSyntax
	{
		public FieldAssignmentExpressionSyntax(FieldAccessExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			Left = left;
			OperatorToken = operatorToken;
			Right = right;
		}

		public FieldAccessExpressionSyntax Left { get; }
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Right { get; }

		public override SyntaxKind Kind => SyntaxKind.FieldAssignmentExpression;
	}
}