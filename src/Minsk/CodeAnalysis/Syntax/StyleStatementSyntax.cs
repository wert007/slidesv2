namespace Minsk.CodeAnalysis.Syntax
{
	internal class StyleStatementSyntax : StatementSyntax
	{
		public StyleStatementSyntax(SyntaxToken styleKeyword, SyntaxToken identifier, SingleParameterStatement parameter, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endStyleKeyword)
		{
			StyleKeyword = styleKeyword;
			Identifier = identifier;
			Parameter = parameter;
			ColonToken = colonToken;
			Body = body;
			EndStyleKeyword = endStyleKeyword;
		}

		public override SyntaxKind Kind => SyntaxKind.StyleStatement;

		public SyntaxToken StyleKeyword { get; }
		public SyntaxToken Identifier { get; }
		public SingleParameterStatement Parameter { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndStyleKeyword { get; }
	}
}