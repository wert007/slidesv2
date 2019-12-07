using System;

namespace Minsk.CodeAnalysis.Symbols
{
	[Serializable]
	public class NullableTypeSymbol : TypeSymbol
	{
		public NullableTypeSymbol(TypeSymbol baseType) : base(baseType.Name + "?")
		{
			BaseType = baseType;
		}

		public override TypeType Type => TypeType.Nullable;

		public TypeSymbol BaseType { get; }

		public override bool IsData => BaseType.IsData;
		public override bool AllowsNone => true;
	}
}