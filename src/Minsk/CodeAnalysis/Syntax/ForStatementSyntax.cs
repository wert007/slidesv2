namespace Minsk.CodeAnalysis.Syntax
{
	internal class ForStatementSyntax : StatementSyntax
	{
		public ForStatementSyntax(SyntaxToken forKeyword, VariableExpressionSyntax variable, VariableExpressionSyntax optionalIndexer, SyntaxToken inKeyword, ExpressionSyntax collection, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endForKeyword)
		{
			ForKeyword = forKeyword;
			Variable = variable;
			OptionalIndexer = optionalIndexer;
			InKeyword = inKeyword;
			Collection = collection;
			ColonToken = colonToken;
			Body = body;
			EndForKeyword = endForKeyword;
		}

		public override SyntaxKind Kind => SyntaxKind.ForStatement;

		public SyntaxToken ForKeyword { get; }
		public VariableExpressionSyntax Variable { get; }
		public VariableExpressionSyntax OptionalIndexer { get; }
		public SyntaxToken InKeyword { get; }
		public ExpressionSyntax Collection { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndForKeyword { get; }
	}
}