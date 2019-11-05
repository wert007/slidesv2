using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax
{
	internal class DataBlockStatementSyntax : StatementSyntax
	{
		public DataBlockStatementSyntax(ImmutableArray<VariableTypeDeclarationStatement> statements)
		{
			Statements = statements;
		}

		public ImmutableArray<VariableTypeDeclarationStatement> Statements { get; }

		public override SyntaxKind Kind => SyntaxKind.DataBlockStatement;
	}
}