namespace Minsk.CodeAnalysis.Syntax
{
	internal class StructBlockStatementSyntax : StatementSyntax
	{
		public StructBlockStatementSyntax(VariableTypeDeclarationStatement[] statements)
		{
			Statements = statements;
		}

		public VariableTypeDeclarationStatement[] Statements { get; }

		public override SyntaxKind Kind => SyntaxKind.DataBlockStatement;
	}
}