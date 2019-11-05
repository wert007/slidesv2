namespace Minsk.CodeAnalysis.Syntax
{
	internal class AnimationParameterStatementSyntax : StatementSyntax
	{
		public AnimationParameterStatementSyntax(SyntaxToken openParenthesis, ParameterStatementSyntax elementParameter, SyntaxToken commaToken, ParameterStatementSyntax timeParameter, SyntaxToken closeParenthesis)
		{
			OpenParenthesis = openParenthesis;
			ElementParameter = elementParameter;
			CommaToken = commaToken;
			TimeParameter = timeParameter;
			CloseParenthesis = closeParenthesis;
		}

		public override SyntaxKind Kind => SyntaxKind.AnimationParameterStatement;

		public SyntaxToken OpenParenthesis { get; }
		public ParameterStatementSyntax ElementParameter { get; }
		public SyntaxToken CommaToken { get; }
		public ParameterStatementSyntax TimeParameter { get; }
		public SyntaxToken CloseParenthesis { get; }
	}
}