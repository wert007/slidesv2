using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundForStatement : BoundStatement
	{
		public BoundForStatement(VariableSymbol variable, BoundExpression collection, BoundBlockStatement body)
		{
			Variable = variable;
			Collection = collection;
			Body = body;
		}

		public override BoundNodeKind Kind => BoundNodeKind.ForStatement;
		public VariableSymbol Variable { get; }
		public BoundExpression Collection { get; }
		public BoundBlockStatement Body { get; }

	}
}
