using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundVariableDeclaration : BoundStatement
	{

		public BoundVariableDeclaration(VariableSymbol variable, BoundExpression initializer)
		{
			Initializer = initializer;
			Variable = variable;
		}

		public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;
		public VariableSymbol Variable { get; }
		public BoundExpression Initializer { get; }
	}
}
