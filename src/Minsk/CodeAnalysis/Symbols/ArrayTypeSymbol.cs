namespace Minsk.CodeAnalysis.Symbols
{
	public class ArrayTypeSymbol : TypeSymbol
	{
		public TypeSymbol Child { get; private set; }
		public ArrayTypeSymbol(TypeSymbol child) : base(child.Name + "[]")
		{
			Child = child;
		}

		public override bool TryLookUpFunction(string name, out FunctionSymbol[] function)
		{
			switch (name)
			{
				case "len":
					function = new FunctionSymbol[]
						{
							new FunctionSymbol("len", PrimitiveTypeSymbol.Integer)
						};
					return true;
				default:
					return base.TryLookUpFunction(name, out function);
			}
		}

		public override TypeType Type => TypeType.Array;

		public override bool IsData => Child.IsData;

		public override bool AllowsNone => false;

		public override bool InnerCanBeConvertedTo(TypeSymbol to)
		{
			if(to is ArrayTypeSymbol a)
			{
				return Child.CanBeConvertedTo(a.Child);
			}
			return false;
		}
	}
}