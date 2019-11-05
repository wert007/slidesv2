﻿using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundEnumExpression : BoundExpression
	{
		public BoundEnumExpression(EnumTypeSymbol type, string value)
		{
			Type = type;
			Value = value;
		}
		
		public override TypeSymbol Type { get; }

		public string Value { get; }

		public override BoundNodeKind Kind => BoundNodeKind.EnumExpression;

	}
}