namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(StatementSyntax[] statements)
        {
            Statements = statements;
        }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;
        public StatementSyntax[] Statements { get; }
    }
}