using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundParameterStatement : BoundStatement
	{

		public BoundParameterStatement(VariableSymbol variable, BoundExpression initializer)
		{
			Variable = variable;
			Initializer = initializer;
		}

		public VariableSymbol Variable { get; }
		public BoundExpression Initializer { get; }

		public override BoundNodeKind Kind => BoundNodeKind.ParameterStatement;
	}
}