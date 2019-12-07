namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class VariableDeclarationSyntax : StatementSyntax
	{
		public VariableDeclarationSyntax(SyntaxToken keyword, VariableExpressionSyntax[] variables, SyntaxToken equalsToken, ExpressionSyntax initializer, SyntaxToken semicolonToken)
		{
			Keyword = keyword;
			Variables = variables;
			EqualsToken = equalsToken;
			Initializer = initializer;
			SemicolonToken = semicolonToken;
		}

		public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
		public SyntaxToken Keyword { get; }
		public VariableExpressionSyntax[] Variables { get; }
		public SyntaxToken EqualsToken { get; }
		public ExpressionSyntax Initializer { get; }
		public SyntaxToken SemicolonToken { get; }
	}
}