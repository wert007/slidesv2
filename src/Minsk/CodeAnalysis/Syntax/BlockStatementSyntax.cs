using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(ImmutableArray<StatementSyntax> statements)
        {
            Statements = statements;
        }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;
        public ImmutableArray<StatementSyntax> Statements { get; }
    }
}