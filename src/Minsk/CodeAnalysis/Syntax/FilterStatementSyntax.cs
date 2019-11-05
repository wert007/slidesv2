namespace Minsk.CodeAnalysis.Syntax
{
	internal class FilterStatementSyntax : StatementSyntax
	{
		public FilterStatementSyntax(SyntaxToken filterKeyword, SyntaxToken identifier, ParameterBlockStatementSyntax parameter, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endFilterKeyword)
		{
			FilterKeyword = filterKeyword;
			Identifier = identifier;
			Parameter = parameter;
			ColonToken = colonToken;
			Body = body;
			EndFilterKeyword = endFilterKeyword;
		}

		public SyntaxToken FilterKeyword { get; }
		public SyntaxToken Identifier { get; }
		public ParameterBlockStatementSyntax Parameter { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndFilterKeyword { get; }

		public override SyntaxKind Kind => SyntaxKind.FilterStatement;
	}
}