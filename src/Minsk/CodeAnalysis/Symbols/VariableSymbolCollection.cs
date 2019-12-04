using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis.Symbols
{
	[Serializable]
	public class VariableSymbolCollection : ICollection<VariableSymbol>
	{
		List<VariableSymbol> symbols = new List<VariableSymbol>();
		public VariableSymbol[] Symbols => symbols.ToArray();

		public static VariableSymbolCollection Empty
		{
			get
			{
				var result = new VariableSymbolCollection();
				result.Seal();
				return result;
			}
		}

		public VariableSymbolCollection()
		{ }

		public VariableSymbolCollection(IEnumerable<VariableSymbol> enumerable)
		{
			foreach (var item in enumerable)
			{
				Add(item);
			}
			Seal();
		}

		public bool Editable { get; private set; } = true;

		public int Count => symbols.Count;
		public VariableSymbol this[int i] => symbols[i];
		public bool IsReadOnly => true;
		public void Seal()
		{
			Editable = false;
		}

		public void Add(VariableSymbol item)
		{
			if (Editable)
				symbols.Add(item);
			else throw new Exception();
		}

		public void Clear()
		{
			if (Editable)
				symbols.Clear();
			else throw new Exception();
		}

		public bool Contains(VariableSymbol item)
		{
			return symbols.Contains(item);
		}

		public void CopyTo(VariableSymbol[] array, int arrayIndex)
		{
			symbols.CopyTo(array, arrayIndex);
		}

		public IEnumerator<VariableSymbol> GetEnumerator()
		{
			return symbols.GetEnumerator();
		}

		public bool Remove(VariableSymbol item)
		{
			if (Editable)
				return symbols.Remove(item);
			else throw new Exception();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return symbols.GetEnumerator();
		}

		public bool TryLookUp(string name, out VariableSymbol variable)
		{
			variable = symbols.FirstOrDefault(s => s.Name == name);
			return variable != null;
		}

		public void Add(VariableSymbolCollection collection)
		{
			foreach (var item in collection)
			{
				Add(item);
			}
		}
	}
}