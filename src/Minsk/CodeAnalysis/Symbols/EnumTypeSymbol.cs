namespace Minsk.CodeAnalysis.Symbols
{
	public class EnumTypeSymbol : TypeSymbol
	{
		public EnumTypeSymbol(string name, params string[] values) : base(name)
		{
			Values = values;
		}

		public string[] Values { get; }

		public override TypeType Type => TypeType.Enum;

		public readonly static TypeSymbol Orientation = new EnumTypeSymbol("Orientation", "Horizontal", "Vertical");
		public readonly static TypeSymbol Borderstyle = new EnumTypeSymbol("Borderstyle", "none", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset");
		public readonly static TypeSymbol Sizes = new EnumTypeSymbol("SizeTypes", "percent", "pixel", "point");

		public override bool IsData => true;
		public override bool AllowsNone => false;
	}
}