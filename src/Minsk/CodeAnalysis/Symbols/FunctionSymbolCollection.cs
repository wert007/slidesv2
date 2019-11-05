using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis.Symbols
{
	public class FunctionSymbolCollection : ICollection<FunctionSymbol>
	{
		List<FunctionSymbol> symbols = new List<FunctionSymbol>();
		public bool Editable { get; private set; } = true;

		public int Count => symbols.Count;
		public FunctionSymbol this[int i] => symbols[i];
		public bool IsReadOnly => true;

		public static FunctionSymbolCollection Empty
		{
			get
			{
				var result = new FunctionSymbolCollection();
				result.Seal();
				return result;
			}
		}

		public void Seal()
		{
			Editable = false;
		}

		public void Add(FunctionSymbol item)
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

		public bool Contains(FunctionSymbol item)
		{
			return symbols.Contains(item);
		}

		public void CopyTo(FunctionSymbol[] array, int arrayIndex)
		{
			symbols.CopyTo(array, arrayIndex);
		}

		public IEnumerator<FunctionSymbol> GetEnumerator()
		{
			return symbols.GetEnumerator();
		}

		public bool Remove(FunctionSymbol item)
		{
			if (Editable)
				return symbols.Remove(item);
			else throw new Exception();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return symbols.GetEnumerator();
		}

		public bool TryLookUp(string name, out FunctionSymbol[] function)
		{
			function = symbols.Where(s => s.Name == name).ToArray();
			return function.Length > 0;
		}
	}
}