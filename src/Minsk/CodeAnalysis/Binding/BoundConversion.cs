using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundConversion : BoundExpression
	{
		public BoundConversion(BoundExpression expression, TypeSymbol type)
		{
			Expression = expression;
			Type = type;
		}


		public override BoundNodeKind Kind => BoundNodeKind.Conversion;

		public BoundExpression Expression { get; }
		public override TypeSymbol Type { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundConversion)expression;
			return Expression.EqualsBoundExpression(e.Expression);
		}
	}
}