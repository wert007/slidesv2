using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundLiteralExpression : BoundExpression
	{
		public BoundLiteralExpression(object value)
		{
			ConstantValue = value;
			_type = BuiltInTypes.Instance.LookSymbolUp(ConstantValue.GetType());
		}

		public BoundLiteralExpression(object value, TypeSymbol type)
		{
			ConstantValue = value;
			_type = type;
		}

		private TypeSymbol _type;
		public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
		public override TypeSymbol Type => _type;
		public override object ConstantValue { get; }


		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundLiteralExpression)expression;
			return ConstantValue.Equals(e.ConstantValue);
		}
	}
}
