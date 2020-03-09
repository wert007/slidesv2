using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundUnaryExpression : BoundExpression
	{
		public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
		{
			Op = op;
			Operand = operand;
		}

		public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
		public override TypeSymbol Type => Op.Type;
		public BoundUnaryOperator Op { get; }
		public BoundExpression Operand { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundUnaryExpression)expression;
			if (Op.Kind != e.Op.Kind)
				return false;
			if (Op.SyntaxKind != e.Op.SyntaxKind)
				return false;
			return Operand.EqualsBoundExpression(e.Operand);
		}
	}
}
