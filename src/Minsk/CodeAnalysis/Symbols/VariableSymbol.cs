using System;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Symbols
{
	[Serializable]
	public sealed class VariableSymbol
	{
		public VariableSymbol(string name, bool isReadOnly, TypeSymbol type, bool needsDataFlag)
		{
			Name = name;
			IsReadOnly = isReadOnly;
			Type = type;
			IsVisible = true;
			NeedsDataFlag = needsDataFlag;
		}

		public string Name { get; }
		public bool IsReadOnly { get; }
		public TypeSymbol Type { get; internal set; }
		public bool IsVisible { get; internal set; }
		public bool NeedsDataFlag { get; internal set; }

		private bool _hasValue;
		public bool HasValue
		{
			get
			{
				if (Type.Type != TypeType.Nullable)
					return true;
				return _hasValue;
			}
			set { _hasValue = value; }
		}

		public override bool Equals(object obj)
		{
			var symbol = obj as VariableSymbol;
			return symbol != null &&
					 Name == symbol.Name &&
					 IsReadOnly == symbol.IsReadOnly &&
					 EqualityComparer<TypeSymbol>.Default.Equals(Type, symbol.Type) &&
					 IsVisible == symbol.IsVisible;
		}

		public override int GetHashCode()
		{
			var hashCode = -1162359233;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + IsReadOnly.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<TypeSymbol>.Default.GetHashCode(Type);
			hashCode = hashCode * -1521134295 + IsVisible.GetHashCode();
			hashCode = hashCode * -1521134295 + NeedsDataFlag.GetHashCode();
			return hashCode;
		}

		public override string ToString() => Name + " : " + Type.ToString();

		public static bool operator ==(VariableSymbol symbol1, VariableSymbol symbol2)
		{
			if (symbol1 is null)
				//TODO(idk): But really?
				return symbol2 is null;
			return symbol1.Equals(symbol2);
		}

		public static bool operator !=(VariableSymbol symbol1, VariableSymbol symbol2)
		{
			return !(symbol1 == symbol2);
		}
	}
}