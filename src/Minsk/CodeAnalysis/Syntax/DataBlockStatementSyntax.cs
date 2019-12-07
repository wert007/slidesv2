namespace Minsk.CodeAnalysis.Syntax
{
	internal class DataBlockStatementSyntax : StatementSyntax
	{
		public DataBlockStatementSyntax(VariableTypeDeclarationStatement[] statements)
		{
			Statements = statements;
		}

		public VariableTypeDeclarationStatement[] Statements { get; }

		public override SyntaxKind Kind => SyntaxKind.DataBlockStatement;
	}
}