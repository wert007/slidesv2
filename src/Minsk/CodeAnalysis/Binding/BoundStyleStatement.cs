using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundStyleStatement : BoundStatement
	{
		public BoundStyleStatement(VariableSymbol variable, BoundParameterStatement boundParameter, BoundBlockStatement boundBody)
		{
			Variable = variable;
			BoundParameter = boundParameter;
			BoundBody = boundBody;
		}

		public override BoundNodeKind Kind => BoundNodeKind.StyleStatement;

		public VariableSymbol Variable { get; }
		public BoundParameterStatement BoundParameter { get; }
		public BoundBlockStatement BoundBody { get; }
	}
}