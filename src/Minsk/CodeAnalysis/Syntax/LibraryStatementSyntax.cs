namespace Minsk.CodeAnalysis.Syntax
{
	internal class LibraryStatementSyntax : StatementSyntax
	{
		public LibraryStatementSyntax(SyntaxToken libraryKeyword, SyntaxToken identifier, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endLibraryKeyword)
		{
			LibraryKeyword = libraryKeyword;
			Identifier = identifier;
			ColonToken = colonToken;
			Body = body;
			EndLibraryKeyword = endLibraryKeyword;
		}

		public override SyntaxKind Kind => SyntaxKind.LibraryStatement;

		public SyntaxToken LibraryKeyword { get; }
		public SyntaxToken Identifier { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndLibraryKeyword { get; }
	}
}