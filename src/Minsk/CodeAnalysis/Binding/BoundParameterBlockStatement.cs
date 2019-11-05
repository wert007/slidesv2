using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundParameterBlockStatement : BoundStatement
	{

		public BoundParameterBlockStatement(ImmutableArray<BoundParameterStatement> statements)
		{
			Statements = statements;
		}

		public ImmutableArray<BoundParameterStatement> Statements { get; }

		public override BoundNodeKind Kind => BoundNodeKind.ParameterBlockStatement;
	}
}