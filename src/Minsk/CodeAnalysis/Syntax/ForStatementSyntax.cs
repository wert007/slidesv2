namespace Minsk.CodeAnalysis.Syntax
{
	internal class ForStatementSyntax : StatementSyntax
	{
		public ForStatementSyntax(SyntaxToken forKeyword, VariableExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax collection, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endForKeyword)
		{
			ForKeyword = forKeyword;
			Variable = variable;
			InKeyword = inKeyword;
			Collection = collection;
			ColonToken = colonToken;
			Body = body;
			EndForKeyword = endForKeyword;
		}

		public override SyntaxKind Kind => SyntaxKind.ForStatement;

		public SyntaxToken ForKeyword { get; }
		public VariableExpressionSyntax Variable { get; }
		public SyntaxToken InKeyword { get; }
		public ExpressionSyntax Collection { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndForKeyword { get; }
	}
}