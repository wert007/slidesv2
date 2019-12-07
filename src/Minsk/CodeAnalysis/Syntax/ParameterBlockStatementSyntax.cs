namespace Minsk.CodeAnalysis.Syntax
{
	internal class ParameterBlockStatementSyntax : StatementSyntax
	{
		public ParameterBlockStatementSyntax(SyntaxToken openParenthesis, ParameterStatementSyntax[] statements, SyntaxToken closeParenthesis)
		{
			OpenParenthesis = openParenthesis;
			Statements = statements;
			CloseParenthesis = closeParenthesis;
		}

		public SyntaxToken OpenParenthesis { get; }
		public ParameterStatementSyntax[] Statements { get; }
		public SyntaxToken CloseParenthesis { get; }

		public override SyntaxKind Kind => SyntaxKind.ParameterBlockStatement;
	}
}