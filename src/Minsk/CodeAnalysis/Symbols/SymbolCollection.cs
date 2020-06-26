using Minsk.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Symbols
{
	public sealed class SymbolCollection<T> : Dictionary<string, T>
	{
		private Dictionary<string, TextSpan> _variableDeclarations = new Dictionary<string, TextSpan>();
		private Dictionary<string, int> _referenceCounter = new Dictionary<string, int>();

		public void Add(string key, T value, TextSpan? span, bool count)
		{
			if(span.HasValue)
				_variableDeclarations.Add(key, span.Value);
			if(count && span.HasValue)
				_referenceCounter.Add(key, 0);
			base.Add(key, value);
		}

		public bool TryGetValue(string key, bool isReading, out T value)
		{
			var result = TryGetValue(key, out value);
			if (result && isReading && _referenceCounter.ContainsKey(key))
				_referenceCounter[key]++;
			return result;
		}

		public new void Clear()
		{
			_referenceCounter.Clear();
			_variableDeclarations.Clear();
			base.Clear();
		}

		public IEnumerable<T> GetSymbolsWithReferences(int references)
		{
			foreach (var symbol in _referenceCounter)
			{
				if(symbol.Value == references)
				{
					var current = base[symbol.Key];
					if(current is VariableSymbol variable)
					{
						if (variable.Type != PrimitiveTypeSymbol.Error)
							yield return current;
					}
					else
						yield return current;
				}
			}
		}

		internal TextSpan? GetDeclarationSpan(VariableSymbol variable)
		{
			if (!_variableDeclarations.ContainsKey(variable.Name))
				return null;
			return _variableDeclarations[variable.Name];
		}
		//public T Get(string name)
		//{
		//	var result = base[name];
		//	_referenceCounter[result]++;
		//	return result;
		//}

		//public void Set(string name, T value)
		//{
		//	if(ContainsKey(name))
		//		_referenceCounter.Remove(base[name]);
		//	base[name] = value;
		//}
	}
}