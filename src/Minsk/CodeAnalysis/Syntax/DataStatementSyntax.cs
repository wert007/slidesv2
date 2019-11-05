namespace Minsk.CodeAnalysis.Syntax
{
	internal class DataStatementSyntax : StatementSyntax
	{
		public DataStatementSyntax(SyntaxToken dataKeyword, SyntaxToken identifier, SyntaxToken colonToken, DataBlockStatementSyntax body, SyntaxToken endDataKeyword)
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
		public DataBlockStatementSyntax Body { get; }
		public SyntaxToken EndDataKeyword { get; }

		public override SyntaxKind Kind => SyntaxKind.DataStatement;
	}
}