namespace Minsk.CodeAnalysis.Syntax
{
	internal class CastExpressionSyntax : ExpressionSyntax
	{
		public CastExpressionSyntax(SyntaxToken castKeyword, ExpressionSyntax expression, TypeDeclarationSyntax type)
		{
			CastKeyword = castKeyword;
			Expression = expression;
			Type = type;
		}

		public SyntaxToken CastKeyword { get; }
		public ExpressionSyntax Expression { get; }
		public TypeDeclarationSyntax Type { get; }

		public override SyntaxKind Kind => SyntaxKind.CastExpression;
	}
}