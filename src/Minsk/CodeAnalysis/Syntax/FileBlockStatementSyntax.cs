using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class FileBlockStatementSyntax : StatementSyntax
	{
		public FileBlockStatementSyntax(ImmutableArray<StatementSyntax> statements)
		{
			Statements = statements;
		}

		public override SyntaxKind Kind => SyntaxKind.FileBlockStatement;
		public ImmutableArray<StatementSyntax> Statements { get; }
	}
}