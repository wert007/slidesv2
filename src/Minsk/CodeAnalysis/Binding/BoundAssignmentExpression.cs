using Minsk.CodeAnalysis.Symbols;
using System;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundAssignmentExpression : BoundExpression
	{
		public BoundAssignmentExpression(VariableSymbol[] variables, BoundExpression expression)
		{
			Variables = variables;
			Expression = expression;
		}

		public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
		public override TypeSymbol Type => Expression.Type;
		public BoundExpression Expression { get; }
		public VariableSymbol[] Variables { get; }

	}
}
