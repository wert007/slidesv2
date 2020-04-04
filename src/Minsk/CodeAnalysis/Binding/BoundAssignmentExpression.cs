using Minsk.CodeAnalysis.Symbols;
using System;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundAssignmentExpression : BoundExpression
	{
		public BoundAssignmentExpression(BoundExpression lvalue, BoundExpression expression)
		{
			LValue = lvalue;
			Expression = expression;
		}

		public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
		public override TypeSymbol Type => Expression.Type;

		public BoundExpression LValue { get; }
		public BoundExpression Expression { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundAssignmentExpression)expression;
			if (!Expression.EqualsBoundExpression(e.Expression))
				return false;
			return LValue.EqualsBoundExpression(e.LValue);
		}

	}
}
