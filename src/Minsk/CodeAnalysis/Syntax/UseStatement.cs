namespace Minsk.CodeAnalysis.Syntax
{
	internal class UseStatement : StatementSyntax
	{
		
		public UseStatement(SyntaxToken keyword, SyntaxToken token, SyntaxToken semicolon)
		{
			Keyword = keyword;
			Token = token;
			Semicolon = semicolon;
		}

		public SyntaxToken Keyword { get; }
		public SyntaxToken Token { get; }
		public SyntaxToken Semicolon { get; }

		public override SyntaxKind Kind => SyntaxKind.UseStatement;
	}
}