namespace Minsk.CodeAnalysis.Syntax
{
	internal class FieldAccessExpressionSyntax : ExpressionSyntax
	{
		public FieldAccessExpressionSyntax(ExpressionSyntax parent, SyntaxToken colonToken, VariableExpressionSyntax variable)
		{
			Parent = parent;
			ColonToken = colonToken;
			Variable = variable;
		}

		public ExpressionSyntax Parent { get; }
		public SyntaxToken ColonToken { get; }
		public VariableExpressionSyntax Variable { get; }

		public override SyntaxKind Kind => SyntaxKind.FieldAccessExpression;
	}
}