namespace Minsk.CodeAnalysis.Syntax
{
	internal class TemplateStatementSyntax : StatementSyntax
	{
		public TemplateStatementSyntax(SyntaxToken templateKeyword, SyntaxToken identifier, SingleParameterStatement parameterStatement, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endTemplateKeyword)
		{
			TemplateKeyword = templateKeyword;
			Identifier = identifier;
			ParameterStatement = parameterStatement;
			ColonToken = colonToken;
			Body = body;
			EndTemplateKeyword = endTemplateKeyword;
		}

		public override SyntaxKind Kind => SyntaxKind.TemplateStatement;

		public SyntaxToken TemplateKeyword { get; }
		public SyntaxToken Identifier { get; }
		public SingleParameterStatement ParameterStatement { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndTemplateKeyword { get; }
	}
}