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

		public override bool Equals(object obj)
		{
			if (!(obj is VariableSymbolCollection collection))
				return false;
			var result = EqualityComparer<VariableSymbol[]>.Default.Equals(Symbols, collection.Symbols) &&
					 Editable == collection.Editable &&
					 Count == collection.Count;
			if (Symbols.Length != collection.Symbols.Length)
				return false;
			for(int i = 0; i < Symbols.Length; i++)
			{
				if (!Symbols[i].Equals(collection.Symbols[i]))
					return false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			var hashCode = -316702989;
			for(int i = 0; i < Symbols.Length; i++)
				hashCode = hashCode * -1521134295 + Symbols[i].GetHashCode();
			hashCode = hashCode * -1521134295 + Editable.GetHashCode();
			hashCode = hashCode * -1521134295 + Count.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(VariableSymbolCollection left, VariableSymbolCollection right)
		{
			return EqualityComparer<VariableSymbolCollection>.Default.Equals(left, right);
		}

		public static bool operator !=(VariableSymbolCollection left, VariableSymbolCollection right)
		{
			return !(left == right);
		}
	}
}