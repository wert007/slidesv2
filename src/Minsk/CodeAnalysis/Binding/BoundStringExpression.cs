using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundStringExpression : BoundExpression
	{
		public BoundStringExpression(BoundExpression[] expressions)
		{
			Expressions = expressions;
		}

		public BoundExpression[] Expressions { get; }

		public override TypeSymbol Type => PrimitiveTypeSymbol.String;

		public override BoundNodeKind Kind => BoundNodeKind.StringExpression;

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundStringExpression)expression;
			if (Expressions.Length != e.Expressions.Length)
				return false;
			for (int i = 0; i < Expressions.Length; i++)
			{
				if (!Expressions[i].EqualsBoundExpression(e.Expressions[i]))
					return false;
			}
			return true;
		}
	}
}