namespace Minsk.CodeAnalysis.Syntax
{
	internal class VariableTypeDeclarationStatement : StatementSyntax
	{
		public VariableTypeDeclarationStatement(ParameterStatementSyntax parameter, SyntaxToken semicolonToken)
		{
			Parameter = parameter;
			SemicolonToken = semicolonToken;
		}

		public override SyntaxKind Kind => SyntaxKind.ParameterStatement; //TODO: Does this make any sense?

		public ParameterStatementSyntax Parameter { get; }
		public SyntaxToken SemicolonToken { get; }
	}
}