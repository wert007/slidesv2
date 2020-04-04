using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundVariableExpression : BoundExpression
	{
		public BoundVariableExpression(VariableSymbol variable)
		{
			Variable = variable;
			Type = variable.Type;
		}

		public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
		public override TypeSymbol Type { get; }
		public VariableSymbol Variable { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundVariableExpression)expression;
			if (!Variable.Equals(e.Variable))
				return false;
			return true;
		}
	}
}
