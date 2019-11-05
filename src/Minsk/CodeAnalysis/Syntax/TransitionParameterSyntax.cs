namespace Minsk.CodeAnalysis.Syntax
{
	internal class TransitionParameterSyntax : SyntaxNode
	{
		public TransitionParameterSyntax(SyntaxToken openParenthesis, ParameterStatementSyntax fromParameter, SyntaxToken commaToken, ParameterStatementSyntax toParameter, SyntaxToken closeParenthesis)
		{
			OpenParenthesis = openParenthesis;
			FromParameter = fromParameter;
			CommaToken = commaToken;
			ToParameter = toParameter;
			CloseParenthesis = closeParenthesis;
		}

		public override SyntaxKind Kind => SyntaxKind.TransitionParameter;

		public SyntaxToken OpenParenthesis { get; }
		public ParameterStatementSyntax FromParameter { get; }
		public SyntaxToken CommaToken { get; }
		public ParameterStatementSyntax ToParameter { get; }
		public SyntaxToken CloseParenthesis { get; }
	}
}