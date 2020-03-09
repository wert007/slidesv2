using Minsk.CodeAnalysis.Symbols;
using System;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundAssignmentExpression : BoundExpression
	{
		public BoundAssignmentExpression(BoundVariableExpression[] variables, BoundExpression expression)
		{
			Variables = variables;
			Expression = expression;
		}

		public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
		public override TypeSymbol Type => Expression.Type;
		public BoundExpression Expression { get; }
		public BoundVariableExpression[] Variables { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundAssignmentExpression)expression;
			if (Variables.Length != e.Variables.Length)
				return false;
			if (!Expression.EqualsBoundExpression(e.Expression))
				return false;
			for (int i = 0; i < Variables.Length; i++)
			{
				if (!Variables[i].EqualsBoundExpression(e.Variables[i]))
					return false;
			}
			return true;
		}

	}
}
