namespace Minsk.CodeAnalysis.Syntax
{
	internal class StructStatementSyntax : StatementSyntax
	{
		public StructStatementSyntax(SyntaxToken dataKeyword, SyntaxToken identifier, SyntaxToken colonToken, StructBlockStatementSyntax body, SyntaxToken endDataKeyword)
		{
			DataKeyword = dataKeyword;
			Identifier = identifier;
			ColonToken = colonToken;
			Body = body;
			EndDataKeyword = endDataKeyword;
		}

		public SyntaxToken DataKeyword { get; }
		public SyntaxToken Identifier { get; }
		public SyntaxToken ColonToken { get; }
		public StructBlockStatementSyntax Body { get; }
		public SyntaxToken EndDataKeyword { get; }

		public override SyntaxKind Kind => SyntaxKind.StructStatement;
	}
}