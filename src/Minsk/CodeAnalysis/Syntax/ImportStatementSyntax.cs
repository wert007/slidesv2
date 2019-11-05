namespace Minsk.CodeAnalysis.Syntax
{
	internal class ImportStatementSyntax : StatementSyntax
	{
		public ImportStatementSyntax(SyntaxToken importKeyword, FunctionExpressionSyntax functionCall, SyntaxToken asKeyword, SyntaxToken identifier, SyntaxToken semicolonToken)
		{
			ImportKeyword = importKeyword;
			Initializer = functionCall;
			AsKeyword = asKeyword;
			Identifier = identifier;
			SemicolonToken = semicolonToken;
		}

		public override SyntaxKind Kind => SyntaxKind.ImportStatement;

		public SyntaxToken ImportKeyword { get; }
		public FunctionExpressionSyntax Initializer { get; }
		public SyntaxToken AsKeyword { get; }
		public SyntaxToken Identifier { get; }
		public SyntaxToken SemicolonToken { get; }
	}
}