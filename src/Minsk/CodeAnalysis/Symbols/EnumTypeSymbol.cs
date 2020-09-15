using System;

namespace Minsk.CodeAnalysis.Symbols
{
	[Serializable]
	public class EnumTypeSymbol : TypeSymbol
	{
		public EnumTypeSymbol(string name, params string[] values) : base(name)
		{
			Values = values;
		}

		public string[] Values { get; }

		public override TypeType Type => TypeType.Enum;

		public override bool AllowsNone => false;

		public override bool HasDefaultValue => true;

		public override object DefaultValue => Values[0];
	}
}