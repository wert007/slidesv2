namespace Minsk.CodeAnalysis.Syntax
{
	internal class EmptyArrayConstructorExpressionSyntax : ExpressionSyntax
	{
		public EmptyArrayConstructorExpressionSyntax(SyntaxToken openBracketToken, TypeDeclarationSyntax typeDeclaration, SyntaxToken semicolonToken, ExpressionSyntax lengthExpression, SyntaxToken closeBracketToken)
		{
			OpenBracketToken = openBracketToken;
			TypeDeclaration = typeDeclaration;
			SemicolonToken = semicolonToken;
			LengthExpression = lengthExpression;
			CloseBracketToken = closeBracketToken;
		}

		public override SyntaxKind Kind => SyntaxKind.EmptyArrayConstructorExpression;

		public SyntaxToken OpenBracketToken { get; }
		public TypeDeclarationSyntax TypeDeclaration { get; }
		public SyntaxToken SemicolonToken { get; }
		public ExpressionSyntax LengthExpression { get; }
		public SyntaxToken CloseBracketToken { get; }

		public override bool IsLValue => false;
	}
}