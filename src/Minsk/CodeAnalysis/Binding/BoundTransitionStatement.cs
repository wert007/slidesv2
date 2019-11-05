using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundTransitionStatement : BoundStatement
	{
		public BoundTransitionStatement(VariableSymbol variable, BoundParameterBlockStatement boundParameters, BoundBlockStatement boundBody)
		{
			Variable = variable;
			BoundParameters = boundParameters;
			BoundBody = boundBody;
		}

		public VariableSymbol Variable { get; }
		public BoundParameterBlockStatement BoundParameters { get; }
		public BoundBlockStatement BoundBody { get; }

		public override BoundNodeKind Kind => BoundNodeKind.TransitionStatement;
	}
}