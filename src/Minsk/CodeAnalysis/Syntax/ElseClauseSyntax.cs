namespace Minsk.CodeAnalysis.Syntax
{
	internal class ElseClauseSyntax : SyntaxNode
	{
		public ElseClauseSyntax(SyntaxToken elseKeyword, SyntaxToken colonToken, BlockStatementSyntax body)
		{
			ElseKeyword = elseKeyword;
			ColonToken = colonToken;
			Body = body;
		}

		public override SyntaxKind Kind => SyntaxKind.ElseClause;

		public SyntaxToken ElseKeyword { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
	}
}