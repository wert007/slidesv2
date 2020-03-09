using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundVariableExpression : BoundExpression
	{
		public BoundVariableExpression(VariableSymbol variable, BoundArrayIndex boundArrayIndex, TypeSymbol type)
		{
			Variable = variable;
			BoundArrayIndex = boundArrayIndex;
			Type = type;
		}

		public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
		public override TypeSymbol Type { get; }
		public VariableSymbol Variable { get; }
		public BoundArrayIndex BoundArrayIndex { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundVariableExpression)expression;
			if (!Variable.Equals(e.Variable))
				return false;
			if (BoundArrayIndex == null || e.BoundArrayIndex == null)
				return BoundArrayIndex == e.BoundArrayIndex;
			return BoundArrayIndex.Equals(e.BoundArrayIndex);
		}
	}
}
