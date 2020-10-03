using System;

namespace Minsk.CodeAnalysis.Symbols
{
	[Serializable]
	public class PrimitiveTypeSymbol : TypeSymbol
	{
		public PrimitiveType PrimitiveType { get; private set; }


		public override TypeType Type => TypeType.Primitive;

		public PrimitiveTypeSymbol(PrimitiveType type) : base(type.ToString())
		{
			PrimitiveType = type;
		}
		public PrimitiveTypeSymbol(PrimitiveType type, string name) : base(name)
		{
			PrimitiveType = type;
		}

		public readonly static TypeSymbol Error = new PrimitiveTypeSymbol(PrimitiveType.Error);
		public readonly static TypeSymbol AnonymFor = new PrimitiveTypeSymbol(PrimitiveType.AnonymFor);

		public static readonly TypeSymbol Void = new PrimitiveTypeSymbol(PrimitiveType.Void);

		public readonly static TypeSymbol Integer = new PrimitiveTypeSymbol(PrimitiveType.Integer, "int");
		public readonly static TypeSymbol Unit = new PrimitiveTypeSymbol(PrimitiveType.Unit);
		public readonly static TypeSymbol Float = new PrimitiveTypeSymbol(PrimitiveType.Float, "float");
		public readonly static TypeSymbol Bool = new PrimitiveTypeSymbol(PrimitiveType.Bool, "bool");
		public readonly static TypeSymbol String = new PrimitiveTypeSymbol(PrimitiveType.String, "string");

		public readonly static TypeSymbol Object = new PrimitiveTypeSymbol(PrimitiveType.Object, "object");

		public override bool AllowsNone => PrimitiveType == PrimitiveType.Void;
		public override bool HasDefaultValue => PrimitiveType != PrimitiveType.Object;
		public override object DefaultValue
		{
			get
			{
				switch (PrimitiveType)
				{
					case PrimitiveType.Bool:
						return false;
					case PrimitiveType.Integer:
						return 0;
					case PrimitiveType.String:
						return "";
					case PrimitiveType.Float:
						return 0f;
					case PrimitiveType.Unit:
						return new Slides.Data.Unit();
					case PrimitiveType.Void:
					case PrimitiveType.Error:
					case PrimitiveType.Undefined:
					case PrimitiveType.Object:
						return null;
					default:
						throw new NotImplementedException();
				}
			}
		}

		public override bool TryLookUpFunction(string name, out FunctionSymbol[] function)
		{

			switch (PrimitiveType)
			{	
				case PrimitiveType.String:
					switch (name)
					{
						case "split":
							function = new[]
							{
								new FunctionSymbol(name, new VariableSymbol("splitBy", true, String), new ArrayTypeSymbol(String))
							};
							return true;
						default:
							function = new FunctionSymbol[0];
							return false;
					}
				case PrimitiveType.Bool:
				case PrimitiveType.Integer:
				case PrimitiveType.Float:
				case PrimitiveType.Void:
				case PrimitiveType.Error:
				case PrimitiveType.Undefined:
				case PrimitiveType.AnonymFor:
				case PrimitiveType.Unit:
				case PrimitiveType.Object:
				default:
					function = new FunctionSymbol[0];
					return false;
			}
		}

		public override bool InnerCanBeConvertedTo(TypeSymbol to)
		{
			if(to is PrimitiveTypeSymbol p)
			{
				if (PrimitiveType == PrimitiveType.Integer && p.PrimitiveType == PrimitiveType.Float)
					return true;
				if ((PrimitiveType == PrimitiveType.Integer || PrimitiveType == PrimitiveType.Float)
					&& p.PrimitiveType == PrimitiveType.Unit)
					return true;
				return false;
			}
			if (PrimitiveType == PrimitiveType.Object)
				return true;
			return false;
		}
	}
}