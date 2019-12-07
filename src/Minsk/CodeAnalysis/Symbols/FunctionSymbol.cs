using Minsk.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis
{
	[Serializable]
	public sealed class FunctionSymbol
	{
		private static uint _index = 0;
		public FunctionSymbol(string name, TypeSymbol returnType)
		{
			_index++;
			Index = _index;
			Name = name;
			Type = returnType;
			Parameter = new VariableSymbolCollection();
			Parameter.Seal();
		}

		public FunctionSymbol(string name, VariableSymbol parameter, TypeSymbol returnType)
		{
			_index++;
			Index = _index;
			Name = name;
			Type = returnType;
			Parameter = new VariableSymbolCollection();
			Parameter.Add(parameter);
			Parameter.Seal();
		}

		public FunctionSymbol(string name, VariableSymbolCollection parameter, TypeSymbol returnType)
		{
			_index++;
			Index = _index;
			Name = name;
			Parameter = parameter;
			Type = returnType;
		}
		public FunctionSymbol(uint index, string name, VariableSymbolCollection parameter, TypeSymbol returnType)
		{
			Index = index;
			Name = name;
			Parameter = parameter;
			Type = returnType;
		}

		public string Name { get; }
		public VariableSymbolCollection Parameter { get; }
		public TypeSymbol Type { get; internal set; }
		//public bool IsReadOnly => true;
		public uint Index { get; }

		public override bool Equals(object obj)
		{
			return obj is FunctionSymbol symbol &&
					 Index == symbol.Index;
			//Name == symbol.Name &&
			//EqualityComparer<VariableSymbolCollection>.Default.Equals(Parameter, symbol.Parameter) &&
			//EqualityComparer<TypeSymbol>.Default.Equals(Type, symbol.Type);
		}

		public override int GetHashCode()
		{
			return -2134847229 + Index.GetHashCode();
		}

		public override string ToString()
		{
			if (Name == "constructor")
				return $"new {Type}({string.Join<VariableSymbol>(", ", Parameter.ToArray())})";
			return $"{Name}({string.Join<VariableSymbol>(", ", Parameter.ToArray())}) : {Type}";
		}

		public static bool operator ==(FunctionSymbol left, FunctionSymbol right)
		{
			return EqualityComparer<FunctionSymbol>.Default.Equals(left, right);
		}

		public static bool operator !=(FunctionSymbol left, FunctionSymbol right)
		{
			return !(left == right);
		}
	}
}