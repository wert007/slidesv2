using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundTransitionStatement : BoundStatement
	{
		public BoundTransitionStatement(VariableSymbol variable, BoundParameterStatement fromParameter, BoundParameterStatement toParameter, BoundBlockStatement boundBody)
		{
			Variable = variable;
			FromParameter = fromParameter;
			ToParameter = toParameter;
			Body = boundBody;
		}

		public VariableSymbol Variable { get; }
		public BoundParameterStatement FromParameter { get; }
		public BoundParameterStatement ToParameter { get; }
		public BoundBlockStatement Body { get; }

		public override BoundNodeKind Kind => BoundNodeKind.TransitionStatement;
	}
}