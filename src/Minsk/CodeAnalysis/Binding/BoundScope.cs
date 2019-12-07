using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	public sealed class BoundScope
	{
		private SymbolCollection<VariableSymbol> _variables = new SymbolCollection<VariableSymbol>();
		private Dictionary<string, TypeSymbol> _customTypes = new Dictionary<string, TypeSymbol>();
		private Dictionary<string, FunctionSymbol> _functions = new Dictionary<string, FunctionSymbol>();
		private TypeSymbol _type;


		private static TypeSymbolTypeConverter _builtInTypes = TypeSymbolTypeConverter.Instance;
		private static GlobalFunctionsConverter _builtInFunctions = GlobalFunctionsConverter.Instance;

		public BoundScope(BoundScope parent)
		{
			Parent = parent;
		}

		public BoundScope(BoundScope parent, AdvancedTypeSymbol type)
		{
			Parent = parent;

		}

		public BoundScope Parent { get; }

		public bool TryDeclare(VariableSymbol variable, TextSpan? span = null)
		{
			if (_variables.ContainsKey(variable.Name))
				return false;

			_variables.Add(variable.Name, variable, span, true);
			return true;
		}

		public bool TryLookup(string name, out VariableSymbol variable)
		{
			if (_variables.TryGetValue(name, out variable))
				return true;

			if (Parent == null)
				return false;

			return Parent.TryLookup(name, out variable);
		}

		public bool TryDeclare(TypeSymbol type)
		{
			if (_customTypes.ContainsKey(type.Name))
				return false;

			if (_builtInTypes.ContainsSymbol(type))
				return false;
			
			_customTypes.Add(type.Name, type);
			return true;
		}

		public bool TryLookup(string name, out TypeSymbol type)
		{
			if (_customTypes.TryGetValue(name, out type))
				return true;

			if (_builtInTypes.TryGetSymbol(name, out type))
				return true;

			if (Parent == null)
				return false;

			return Parent.TryLookup(name, out type);
		}

		public bool TryDeclare(FunctionSymbol function)
		{
			if (_functions.ContainsKey(function.Name))
				return false;

			if (_builtInFunctions.ContainsSymbol(function))
				return false;

			_functions.Add(function.Name, function);
			return true;
		}

		public bool TryLookup(string name, out FunctionSymbol[] function)
		{
			if (_functions.TryGetValue(name, out var f))
			{
				function = new FunctionSymbol[] { f };
				return true;
			}
			if (_builtInFunctions.TryGetSymbol(name, out function))
				return true;
			if (Parent == null)
				return false;

			return Parent.TryLookup(name, out function);
		}

		public VariableSymbol[] GetDeclaredVariables()
		{
			return _variables.Values.ToArray();
		}

		public TypeSymbol[] GetDeclaredCustomTypes()
		{
			return _customTypes.Values.ToArray();
		}

		public FunctionSymbol[] GetDeclaredFunctions()
		{
			return _functions.Values.ToArray();
		}

		public VariableSymbol[] GetUnusedVariables()
		{
			return _variables.GetSymbolsWithReferences(0).ToArray();
		}

		public void ClearReferencedVariables()
		{
			_variables.Clear();
		}

		internal TextSpan? GetDeclarationSpan(VariableSymbol variable)
		{
			return _variables.GetDeclarationSpan(variable);
		}

		internal void Declare(VariableSymbol variable, bool noWarnings = false)
		{
			_variables.Add(variable.Name, variable, null, noWarnings);
		}

		internal IEnumerable<VariableSymbol> GetVariablesReferenced(int referenceCount)
		{
			return _variables.GetSymbolsWithReferences(referenceCount);
		}
	}
}
