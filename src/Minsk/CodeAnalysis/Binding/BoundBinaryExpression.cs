using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundBinaryExpression : BoundExpression
	{
		public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
		{
			Left = left;
			Op = op;
			Right = right;
		}

		public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
		public override TypeSymbol Type => Op.Type;
		public BoundExpression Left { get; }
		public BoundBinaryOperator Op { get; }
		public BoundExpression Right { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundBinaryExpression)expression;
			if (e.Op.SyntaxKind != Op.SyntaxKind)
				return false;
			if (e.Op.Kind != Op.Kind)
				return false;
			if (!Left.EqualsBoundExpression(e.Left))
				return false;
			if (!Right.EqualsBoundExpression(e.Right))
				return false;
			return true;
		}
	}
}
