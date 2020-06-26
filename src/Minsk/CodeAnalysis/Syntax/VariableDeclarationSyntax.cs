namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class VariableDeclarationSyntax : StatementSyntax
	{
		public VariableDeclarationSyntax(SyntaxToken keyword, VariableExpressionSyntax variable, TypeDeclarationSyntax optionalTypeDeclaration, SyntaxToken equalsToken, ExpressionSyntax initializer, SyntaxToken semicolonToken)
		{
			Keyword = keyword;
			Variable = variable;
			OptionalTypeDeclaration = optionalTypeDeclaration;
			EqualsToken = equalsToken;
			Initializer = initializer;
			SemicolonToken = semicolonToken;
		}

		public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;
		public SyntaxToken Keyword { get; }
		public VariableExpressionSyntax Variable { get; }
		public TypeDeclarationSyntax OptionalTypeDeclaration { get; }
		public SyntaxToken EqualsToken { get; }
		public ExpressionSyntax Initializer { get; }
		public SyntaxToken SemicolonToken { get; }
	}
}