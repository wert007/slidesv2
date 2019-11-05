namespace Minsk.CodeAnalysis.Syntax
{
	internal class TransitionStatementSyntax : StatementSyntax
	{
		public TransitionStatementSyntax(SyntaxToken transitionKeyword, SyntaxToken identifier, TransitionParameterSyntax parameters, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endTransitionKeyword)
		{
			TransitionKeyword = transitionKeyword;
			Identifier = identifier;
			Parameters = parameters;
			ColonToken = colonToken;
			Body = body;
			EndTransitionKeyword = endTransitionKeyword;
		}

		public SyntaxToken TransitionKeyword { get; }
		public SyntaxToken Identifier { get; }
		public TransitionParameterSyntax Parameters { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndTransitionKeyword { get; }

		public override SyntaxKind Kind => SyntaxKind.TransitionStatement;
	}
}