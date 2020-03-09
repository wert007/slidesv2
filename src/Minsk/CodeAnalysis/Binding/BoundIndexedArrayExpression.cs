using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundIndexedArrayExpression : BoundExpression
	{
		public BoundIndexedArrayExpression(VariableSymbol variable, BoundExpression boundIndex)
		{
			Variable = variable;
			BoundIndex = boundIndex;
		}

		public override TypeSymbol Type => ((ArrayTypeSymbol)Variable.Type).Child;

		public override BoundNodeKind Kind => BoundNodeKind.IndexedArrayExpression;

		public VariableSymbol Variable { get; }
		public BoundExpression BoundIndex { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundIndexedArrayExpression)expression;
			if (!Variable.Equals(e.Variable))
				return false;
			return BoundIndex.EqualsBoundExpression(e.BoundIndex);
		}
	}
}