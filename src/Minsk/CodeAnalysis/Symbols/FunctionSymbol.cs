using Minsk.CodeAnalysis.Symbols;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Minsk.CodeAnalysis
{
	[Serializable]
	public sealed class FunctionSymbol
	{
		public FunctionSymbol(string name, TypeSymbol returnType)
		{
			Name = name;
			Type = returnType;
			Parameter = new VariableSymbolCollection();
			Parameter.Seal();
		}

		public FunctionSymbol(string name, VariableSymbol parameter, TypeSymbol returnType)
		{
			Name = name;
			Type = returnType;
			Parameter = new VariableSymbolCollection();
			Parameter.Add(parameter);
			Parameter.Seal();
		}

		public FunctionSymbol(string name, VariableSymbolCollection parameter, TypeSymbol returnType)
		{
			Name = name;
			Parameter = parameter;
			Type = returnType;
		}

		public string Name { get; }
		public VariableSymbolCollection Parameter { get; }
		public TypeSymbol Type { get; internal set; }
		public bool IsReadOnly => true;

		public override string ToString()
		{
			if (Name == "constructor")
				return $"new {Type}({string.Join<VariableSymbol>(", ", Parameter.ToArray())})";
			return $"{Name}({string.Join<VariableSymbol>(", ", Parameter.ToArray())}) : {Type}";
		}
	}
}