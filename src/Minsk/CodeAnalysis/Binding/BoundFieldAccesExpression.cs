using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundFieldAccesExpression : BoundExpression
	{
		public BoundFieldAccesExpression(BoundExpression parent, BoundVariableExpression field)
		{
			Parent = parent;
			Field = field;
		}

		public BoundExpression Parent { get; }
		public BoundVariableExpression Field { get; }
		public bool IsReadOnly => Field.Variable.IsReadOnly;

		public override TypeSymbol Type => Field.Type;

		public override BoundNodeKind Kind => BoundNodeKind.FieldAccessExpression;

	}
}
