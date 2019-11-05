using Minsk.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundAnimationStatement : BoundStatement
	{
		public BoundAnimationStatement(VariableSymbol variable, BoundParameterStatement elementParameter, BoundParameterStatement timeParameter, ImmutableArray<BoundCaseStatement> body)
		{
			Variable = variable;
			ElementParameter = elementParameter;
			TimeParameter = timeParameter;
			Body = body;
		}

		public override BoundNodeKind Kind => BoundNodeKind.AnimationStatement;

		public VariableSymbol Variable { get; }
		public BoundParameterStatement ElementParameter { get; }
		public BoundParameterStatement TimeParameter { get; }
		public ImmutableArray<BoundCaseStatement> Body { get; }

	}
}