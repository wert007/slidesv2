namespace Minsk.CodeAnalysis.Syntax
{
	internal class IfStatementSyntax : StatementSyntax
	{
		public IfStatementSyntax(SyntaxToken ifKeyword, ExpressionSyntax condition, SyntaxToken colonToken, BlockStatementSyntax body, ElseClauseSyntax elseClause, SyntaxToken endIfKeyword)
		{
			IfKeyword = ifKeyword;
			Condition = condition;
			ColonToken = colonToken;
			Body = body;
			ElseClause = elseClause;
			EndIfKeyword = endIfKeyword;
		}

		public SyntaxToken IfKeyword { get; }
		public ExpressionSyntax Condition { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public ElseClauseSyntax ElseClause { get; }
		public SyntaxToken EndIfKeyword { get; }

		public override SyntaxKind Kind => SyntaxKind.IfStatement;
	}
}