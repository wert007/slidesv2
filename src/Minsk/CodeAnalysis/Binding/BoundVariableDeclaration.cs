using System.Collections.Immutable;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundVariableDeclaration : BoundStatement
	{

		public BoundVariableDeclaration(ImmutableArray<VariableSymbol> variables, BoundExpression initializer)
		{
			Variables = variables;
			Initializer = initializer;
		}

		public BoundVariableDeclaration(VariableSymbol variable, BoundExpression initializer)
		{
			Initializer = initializer;
			Variables = ImmutableArray.Create(variable);
		}

		public override BoundNodeKind Kind => BoundNodeKind.VariableDeclaration;
		public ImmutableArray<VariableSymbol> Variables { get; }
		public BoundExpression Initializer { get; }
	}
}
