using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundEmptyArrayConstructorExpression : BoundExpression
	{
		public BoundEmptyArrayConstructorExpression(BoundExpression length, TypeSymbol type)
		{
			Length = length;
			Type = new ArrayTypeSymbol(type);
		}

		public override TypeSymbol Type { get; }

		public override BoundNodeKind Kind => BoundNodeKind.EmptyArrayConstructorExpression;

		public BoundExpression Length { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundEmptyArrayConstructorExpression)expression;
			return Length.EqualsBoundExpression(e.Length);
		}
	}
}