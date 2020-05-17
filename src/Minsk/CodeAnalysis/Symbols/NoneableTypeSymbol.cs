using System;

namespace Minsk.CodeAnalysis.Symbols
{
	[Serializable]
	public class NoneableTypeSymbol : TypeSymbol
	{
		public NoneableTypeSymbol(TypeSymbol baseType) : base(baseType.Name + "?")
		{
			BaseType = baseType;
		}

		public override TypeType Type => TypeType.Noneable;

		public TypeSymbol BaseType { get; }

		public override bool IsData => BaseType.IsData;
		public override bool AllowsNone => true;
		public override bool HasDefaultValue => true;
		public override object DefaultValue => null;
	}
}