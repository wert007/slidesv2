using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis
{
	public sealed class FunctionCallableCollection : Dictionary<FunctionSymbol, Callable>
	{
		public FunctionCallableCollection() { }
		public FunctionCallableCollection(IDictionary<FunctionSymbol, Callable> dictionary) : base(dictionary)
		{

		}

		public FunctionCallableCollection(FunctionSymbol[] symbols, Callable callable)
		{
			foreach (var symbol in symbols)
			{
				Add(symbol, callable);
			}
		}


		internal bool TryGetSymbol(string name, out FunctionSymbol[] function)
		{
			function = Keys.Where(f => f.Name == name).ToArray();
			return Keys.Any(f => f.Name == name);
		}

		public bool HasKey(FunctionSymbol function)
		{
			if (ContainsKey(function)) return true;
			return false;
		}
	}
}