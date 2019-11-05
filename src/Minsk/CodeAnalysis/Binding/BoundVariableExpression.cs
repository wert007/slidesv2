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
	}
}
