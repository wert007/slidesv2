using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundLiteralExpression : BoundExpression
	{
		public BoundLiteralExpression(object value)
		{
			Value = value;
			_type = TypeSymbolTypeConverter.Instance.LookSymbolUp(Value.GetType());
		}

		public BoundLiteralExpression(object value, TypeSymbol type)
		{
			Value = value;
			_type = type;
		}

		private TypeSymbol _type;
		public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
		public override TypeSymbol Type => _type;
		public object Value { get; }
	}
}
