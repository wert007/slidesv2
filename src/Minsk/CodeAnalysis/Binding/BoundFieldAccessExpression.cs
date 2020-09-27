using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundFieldAccessExpression : BoundExpression
	{
		public BoundFieldAccessExpression(BoundExpression parent, BoundVariableExpression field)
		{
			Parent = parent;
			Field = field;
		}

		public BoundExpression Parent { get; }
		public BoundVariableExpression Field { get; }
		public bool IsReadOnly => Field.Variable.IsReadOnly;

		public override TypeSymbol Type => Field.Type;

		public override BoundNodeKind Kind => BoundNodeKind.FieldAccessExpression;

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundFieldAccessExpression)expression;
			if (!Parent.Equals(e.Parent))
				return false;
			return Field.EqualsBoundExpression(e.Field);
		}

	}
}
