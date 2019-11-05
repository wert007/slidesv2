namespace Minsk.CodeAnalysis.Syntax
{
	public class StepStatementSyntax : StatementSyntax
	{
		public StepStatementSyntax(SyntaxToken stepKeyword, SyntaxToken optionalIdentifier, SyntaxToken colonToken, BlockStatementSyntax body)
		{
			StepKeyword = stepKeyword;
			OptionalIdentifier = optionalIdentifier;
			ColonToken = colonToken;
			Body = body;
		}

		public SyntaxToken StepKeyword { get; }
		public SyntaxToken OptionalIdentifier { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }

		public override SyntaxKind Kind => SyntaxKind.StepStatement;
	}
}