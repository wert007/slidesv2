namespace Minsk.CodeAnalysis.Syntax
{
	internal class GroupStatementSyntax : StatementSyntax
	{
		public GroupStatementSyntax(SyntaxToken groupKeyword, SyntaxToken identifier, ParameterBlockStatementSyntax parameterStatement, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endGroupKeyword)
		{
			GroupKeyword = groupKeyword;
			Identifier = identifier;
			ParameterStatement = parameterStatement;
			ColonToken = colonToken;
			Body = body;
			EndGroupKeyword = endGroupKeyword;
		}

		public override SyntaxKind Kind => SyntaxKind.GroupStatement;

		public SyntaxToken GroupKeyword { get; }
		public SyntaxToken Identifier { get; }
		public ParameterBlockStatementSyntax ParameterStatement { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndGroupKeyword { get; }
	}
}