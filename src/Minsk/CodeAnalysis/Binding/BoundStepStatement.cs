using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundStepStatement : BoundStatement
	{

		public BoundStepStatement(VariableSymbol variable, BoundStatement body)
		{
			Variable = variable;
			Body = body;
		}

		public VariableSymbol Variable { get; }
		public BoundStatement Body { get; }

		public override BoundNodeKind Kind => BoundNodeKind.StepStatement;
	}
}