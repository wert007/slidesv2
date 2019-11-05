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

		public new bool TryGetValue(string key, out T value)
		{
			var result = base.TryGetValue(key, out value);
			if (result)
			{
				if(_referenceCounter.ContainsKey(key))
					_referenceCounter[key]++;
			}
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
					yield return base[symbol.Key];
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