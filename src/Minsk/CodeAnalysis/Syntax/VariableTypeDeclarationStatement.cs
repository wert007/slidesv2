namespace Minsk.CodeAnalysis.Syntax
{
	internal class VariableTypeDeclarationStatement : StatementSyntax
	{
		public VariableTypeDeclarationStatement(ParameterStatementSyntax parameter, SyntaxToken semicolonToken)
		{
			Parameter = parameter;
			SemicolonToken = semicolonToken;
		}

		public override SyntaxKind Kind => SyntaxKind.ParameterStatement; 
		//TODO(idk): Does this make any sense?
		//Good Question. Why are we doing this?

		public ParameterStatementSyntax Parameter { get; }
		public SyntaxToken SemicolonToken { get; }
	}
}