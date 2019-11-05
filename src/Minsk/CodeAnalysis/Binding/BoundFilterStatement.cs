using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundFilterStatement : BoundStatement
	{
		public BoundFilterStatement(VariableSymbol variable, BoundParameterBlockStatement parameter, BoundBlockStatement body)
		{
			Variable = variable;
			Parameter = parameter;
			Body = body;
		}

		public VariableSymbol Variable { get; }
		public BoundParameterBlockStatement Parameter { get; }
		public BoundBlockStatement Body { get; }

		public override BoundNodeKind Kind => BoundNodeKind.FilterStatement;

	}
}