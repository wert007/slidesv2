namespace Minsk.CodeAnalysis.Syntax
{
	internal class SingleParameterStatement : StatementSyntax
	{
		public SingleParameterStatement(SyntaxToken openParenthesis, ParameterStatementSyntax parameterStatement, SyntaxToken closeParenthesis)
		{
			OpenParenthesis = openParenthesis;
			ParameterStatement = parameterStatement;
			CloseParenthesis = closeParenthesis;
		}

		public override SyntaxKind Kind => SyntaxKind.SingleParameterStatement;

		public SyntaxToken OpenParenthesis { get; }
		public ParameterStatementSyntax ParameterStatement { get; }
		public SyntaxToken CloseParenthesis { get; }
	}
}