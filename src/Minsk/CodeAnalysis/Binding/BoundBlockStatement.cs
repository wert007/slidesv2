using System;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	[Serializable]
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(BoundStatement[] statements)
        {
            Statements = statements;
        }

        public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
        public BoundStatement[] Statements { get; }

	}
}
