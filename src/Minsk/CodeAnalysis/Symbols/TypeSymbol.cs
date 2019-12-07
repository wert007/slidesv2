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
		Nullable,
		Tuple
	}

	public class TupleTypeSymbol : TypeSymbol
	{
		public TupleTypeSymbol(TypeSymbol[] children) : base($"Tuple({string.Join<TypeSymbol>(", ", children)})")
		{
			Children = children;
			Length = Children.Length;
		}

		public override TypeType Type => TypeType.Tuple;

		//TODO: Huh?!
		//I mean it's kinda data type. 
		//On the other hand, if you have two container
		//in it, then those will be added to the slide
		//No questions asked.
		public override bool IsData => false;

		public override bool AllowsNone => false;

		public int Length { get; }
		public TypeSymbol[] Children { get; }


	}

	[Serializable]
	public abstract class TypeSymbol
	{
		private static Dictionary<string, TypeSymbol> registeredTypes = new Dictionary<string, TypeSymbol>();
		private static uint id = 0;

		public TypeSymbol(string name)
		{
			Name = name;
			Id = id++;
			if (Type != TypeType.Array && Type != TypeType.Nullable && Type != TypeType.Tuple)
			{
				if (registeredTypes.ContainsKey(name))
				{
					Logger.LogAlreadyRegisteredTypeSymbol(name, Type.ToString(), registeredTypes[name].Type.ToString());
					return;
				}
				registeredTypes.Add(name, this);

			}
		}

		public static void RegisterType(TypeSymbol type)
		{
			if (type.Type != TypeType.Array && type.Type != TypeType.Nullable && type.Type != TypeType.Tuple)
			{
				if (registeredTypes.ContainsKey(type.Name))
				{
					Logger.LogAlreadyRegisteredTypeSymbol(type.Name, type.Type.ToString(), registeredTypes[type.Name].Type.ToString());
					return;
				}
				registeredTypes.Add(type.Name, type);

			}
		}

		public static readonly TypeSymbol Undefined = new PrimitiveTypeSymbol(PrimitiveType.Undefined); 

		public string Name { get; }
		public uint Id { get; }
		public abstract TypeType Type { get; }
		public abstract bool IsData { get; }
		public abstract bool AllowsNone { get; }

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

		public virtual bool TryLookUpStaticField(string name, out VariableSymbol field)
		{
			field = null;
			return false;
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
			if (Type != TypeType.Nullable && to.Type == TypeType.Nullable)
				return this == ((NullableTypeSymbol)to).BaseType;
			return this == to || InnerCanBeConvertedTo(to);
		}

		public virtual bool InnerCanBeConvertedTo(TypeSymbol to)
		{
			return false;
		}

		public static TypeSymbol FromString(string name)
		{
			if (registeredTypes.ContainsKey(name))
				return registeredTypes[name];
			return null;
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