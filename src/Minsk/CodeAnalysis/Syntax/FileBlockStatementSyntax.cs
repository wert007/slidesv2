namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class FileBlockStatementSyntax : StatementSyntax
	{
		public FileBlockStatementSyntax(StatementSyntax[] statements)
		{
			Statements = statements;
		}

		public override SyntaxKind Kind => SyntaxKind.FileBlockStatement;
		public StatementSyntax[] Statements { get; }
	}
}