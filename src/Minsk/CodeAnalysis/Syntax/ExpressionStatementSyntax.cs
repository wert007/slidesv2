namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class ExpressionStatementSyntax : StatementSyntax
	{
		public ExpressionStatementSyntax(ExpressionSyntax expression, SyntaxToken semicolonToken)
		{
			Expression = expression;
			SemicolonToken = semicolonToken;
		}

		public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;
		public ExpressionSyntax Expression { get; }
		public SyntaxToken SemicolonToken { get; }
	}
}