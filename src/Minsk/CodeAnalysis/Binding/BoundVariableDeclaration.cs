using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundVariableDeclaration : BoundStatement
	{

		public BoundVariableDeclaration(VariableSymbol[] variables, BoundExpression initializer)
		{
			Variables = variables;
			Initializer = initializer;
		}

		public BoundVariableDeclaration(VariableSymbol variable, BoundExpression initializer)
		{
			Initializer = initializer;
			Variables = new VariableSymbol[] { variable };
		}

		public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;
		public VariableSymbol[] Variables { get; }
		public BoundExpression Initializer { get; }
	}
}
