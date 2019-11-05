using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundFieldAssignmentExpression : BoundExpression
	{
		public BoundFieldAssignmentExpression(BoundFieldAccesExpression field, BoundExpression initializer)
		{
			Field = field;
			Initializer = initializer;
		}

		public BoundFieldAccesExpression Field { get; }
		public BoundExpression Initializer { get; }

		public override TypeSymbol Type => Initializer.Type;

		public override BoundNodeKind Kind => BoundNodeKind.FieldAssignmentExpression;

	}
}
