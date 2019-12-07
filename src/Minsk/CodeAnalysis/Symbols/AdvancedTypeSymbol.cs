using System;

namespace Minsk.CodeAnalysis.Symbols
{
	[Serializable]
	public class AdvancedTypeSymbol : TypeSymbol
	{
		public AdvancedTypeSymbol(string name, VariableSymbolCollection fields, FunctionSymbolCollection constructor)
			: this(name, fields, constructor, FunctionSymbolCollection.Empty)
		{
		}
		public AdvancedTypeSymbol(string name, VariableSymbolCollection fields, FunctionSymbolCollection constructor, FunctionSymbolCollection functions) :
			this(name, fields, constructor, functions, null)
		{ }
		public AdvancedTypeSymbol(string name, VariableSymbolCollection fields, FunctionSymbolCollection constructor, FunctionSymbolCollection functions, TypeSymbol parent) : this(name, fields, VariableSymbolCollection.Empty, constructor, functions, parent)
		{
			Fields = fields;
			Constructor = constructor;
			Functions = functions;
			Parent = parent;
		}
		public AdvancedTypeSymbol(string name, VariableSymbolCollection fields, VariableSymbolCollection staticFields, FunctionSymbolCollection constructor, FunctionSymbolCollection functions, TypeSymbol parent) : base(name)
		{
			Fields = fields;
			Constructor = constructor;
			Functions = functions;
			Parent = parent;
			StaticFields = staticFields;
		}
		public AdvancedTypeSymbol(string name, VariableSymbolCollection fields, VariableSymbolCollection staticFields, FunctionSymbolCollection constructor, FunctionSymbolCollection functions, TypeSymbol parent, TypeSymbol[] canBeCastedTo) : base(name)
		{
			Fields = fields;
			Constructor = constructor;
			Functions = functions;
			Parent = parent;
			CanBeCastedTo = canBeCastedTo;
			StaticFields = staticFields;
		}

		private bool _isData = false;
		public override TypeType Type => TypeType.Advanced;
		public override bool IsData => _isData;
		public override bool AllowsNone => false;

		public VariableSymbolCollection StaticFields { get; }
		public VariableSymbolCollection Fields { get; }
		public FunctionSymbolCollection Constructor { get; }
		public FunctionSymbolCollection Functions { get; }
		public TypeSymbol Parent { get; }
		public TypeSymbol[] CanBeCastedTo { get; }

		//private static TypeSymbol _size = null;
		//public static TypeSymbol Size
		//{
		//	get
		//	{
		//		var fields = new VariableSymbolCollection();
		//		fields.Add(new VariableSymbol("value", false, PrimitiveTypeSymbol.Float));
		//		fields.Add(new VariableSymbol("type", false, EnumTypeSymbol.Sizes));
		//		fields.Seal();
		//		var functions = new FunctionSymbolCollection();
		//		functions.Seal();
		//		if(_size == null)
		//			_size = new AdvancedTypeSymbol("Size", fields, fields, functions);
		//		return _size;
		//	}
		//}

		public override bool TryLookUpField(string name, out VariableSymbol field)
		{
			if (Fields.TryLookUp(name, out field))
				return true;
			if (Parent != null)
				return Parent.TryLookUpField(name, out field);
			return false;
		}

		public override bool TryLookUpStaticField(string name, out VariableSymbol field)
		{
			if (StaticFields.TryLookUp(name, out field))
				return true;
			if (Parent != null)
				return Parent.TryLookUpStaticField(name, out field);
			return false;
		}

		public override bool TryLookUpFunction(string name, out FunctionSymbol[] function)
		{
			if (Functions.TryLookUp(name, out function))
				return true;
			if (Parent != null)
				return Parent.TryLookUpFunction(name, out function);
			return false;
		}

		public override bool InnerCanBeConvertedTo(TypeSymbol to)
		{
			if(to is AdvancedTypeSymbol ats && ats.CanBeCastedTo != null)
				foreach (var candidate in ats.CanBeCastedTo)
				{
					if (CanBeConvertedTo(candidate))
						return true;
				}
			if(CanBeCastedTo != null)
				foreach (var candidate in CanBeCastedTo)
				{
					if (candidate.CanBeConvertedTo(to))
						return true;
				}
			if (Parent == null)
				return false;
			return Parent.CanBeConvertedTo(to);
		}

		internal void SetData(bool isData)
		{
			_isData = isData;
		}
	}
}