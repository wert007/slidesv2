using Minsk.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundSlideStatement : BoundStatement
	{
		public BoundSlideStatement(VariableSymbol variable, ImmutableArray<BoundStepStatement> statements)
		{
			Variable = variable;
			Statements = statements;
		}

		public override BoundNodeKind Kind => BoundNodeKind.SlideStatement;
		public VariableSymbol Variable { get; }
		public ImmutableArray<BoundStepStatement> Statements{ get; }
	}
}