using Slides.Debug;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis.Symbols
{
	public enum TypeType
	{
		Enum,
		Array,
		Primitive,
		Advanced,
		Noneable,
		Tuple,
	}

	[Serializable]
	public abstract class TypeSymbol
	{
		private static uint id = 0;

		public TypeSymbol(string name)
		{
			Name = name;
			Id = id++;
		}

		public static readonly TypeSymbol Undefined = new PrimitiveTypeSymbol(PrimitiveType.Undefined); 

		public string Name { get; }
		public uint Id { get; }
		public abstract TypeType Type { get; }
		public abstract bool IsData { get; }
		public abstract bool AllowsNone { get; }
		public abstract bool HasDefaultValue { get; }
		public abstract object DefaultValue { get; }

		public override bool Equals(object obj)
		{
			var symbol = obj as TypeSymbol;
			return symbol != null &&
					 Name == symbol.Name;
		}

		public override string ToString() => Name;

		public static bool operator ==(TypeSymbol symbol1, TypeSymbol symbol2)
		{
			return EqualityComparer<TypeSymbol>.Default.Equals(symbol1, symbol2);
		}

		public static bool operator !=(TypeSymbol symbol1, TypeSymbol symbol2)
		{
			return !(symbol1 == symbol2);
		}


		public virtual bool TryLookUpField(string name, out VariableSymbol field)
		{
			field = null;
			return false;
		}

		public virtual bool TryLookUpFunction(string name, out FunctionSymbol[] function)
		{
			function = new FunctionSymbol[0];
			return false;
		}

		public static TypeSymbol ToType(object value)
		{
			var type = value.GetType();
			if (type == typeof(int))
				return PrimitiveTypeSymbol.Integer;
			else if (type == typeof(bool))
				return PrimitiveTypeSymbol.Bool;
			else if (type == typeof(string))
				return PrimitiveTypeSymbol.String;
			else if (type == typeof(float))
				return PrimitiveTypeSymbol.Float;
			return PrimitiveTypeSymbol.Error;
		}
		
		public bool CanBeConvertedTo(TypeSymbol to)
		{
			if (this == Undefined || to == Undefined)
				return true;
			if (this == PrimitiveTypeSymbol.Object || to == PrimitiveTypeSymbol.Object)
				return true;
			if (Type != TypeType.Noneable && to.Type == TypeType.Noneable)
			{
				var baseType = ((NoneableTypeSymbol)to).BaseType;
				return this == baseType || InnerCanBeConvertedTo(baseType);
			}
			return this == to || InnerCanBeConvertedTo(to);
		}

		public virtual bool InnerCanBeConvertedTo(TypeSymbol to)
		{
			return false;
		}

		public override int GetHashCode()
		{
			var hashCode = 242898725;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			//hashCode = hashCode * -1521134295 + Id.GetHashCode();
			//hashCode = hashCode * -1521134295 + Type.GetHashCode();
			//hashCode = hashCode * -1521134295 + IsData.GetHashCode();
			//hashCode = hashCode * -1521134295 + AllowsNone.GetHashCode();
			return hashCode;
		}
	}
}