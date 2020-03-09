using System;

namespace Minsk.CodeAnalysis.Symbols
{
	[Serializable]
	//TODO(Debate): ArrayTypeSymbol should know how big it is!
	//Maybe. Maybe not. I am not to sure actually. But we
	//shal see.
	public class ArrayTypeSymbol : TypeSymbol
	{
		public TypeSymbol Child { get; private set; }
		public ArrayTypeSymbol(TypeSymbol child) : base($"{child.Name}[]")
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

		//How long is our default value? 
		//Probably 0, but then we don't 
		//depend on the child...
		//so idk
		public override bool HasDefaultValue => true; // Child.HasDefaultValue;
		public override object DefaultValue
		{
			get
			{
				var result = new object[0];
				return result;
			}
		}


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