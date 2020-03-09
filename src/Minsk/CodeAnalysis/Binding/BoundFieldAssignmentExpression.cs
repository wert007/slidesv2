using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundFieldAssignmentExpression : BoundExpression
	{
		public BoundFieldAssignmentExpression(BoundFieldAccessExpression field, BoundExpression initializer)
		{
			Field = field;
			Initializer = initializer;
		}

		public BoundFieldAccessExpression Field { get; }
		public BoundExpression Initializer { get; }

		public override TypeSymbol Type => Initializer.Type;

		public override BoundNodeKind Kind => BoundNodeKind.FieldAssignmentExpression;

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundFieldAssignmentExpression)expression;
			if (!Field.EqualsBoundExpression(e.Field))
				return false;
			return Initializer.EqualsBoundExpression(e.Initializer);
		}
	}
}
