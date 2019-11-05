namespace Minsk.CodeAnalysis.Syntax
{
	internal class CaseBlockStatementSyntax : StatementSyntax
	{
		public CaseBlockStatementSyntax(SyntaxToken caseKeyword, ExpressionSyntax condition, SyntaxToken colonToken, BlockStatementSyntax body)
		{
			CaseKeyword = caseKeyword;
			Condition = condition;
			ColonToken = colonToken;
			Body = body;
		}

		public SyntaxToken CaseKeyword { get; }
		public ExpressionSyntax Condition { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }

		public override SyntaxKind Kind => SyntaxKind.CaseBlockStatement;
	}
}