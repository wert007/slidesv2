using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis
{
	[Serializable]
	public sealed class VariableValueCollection : Dictionary<VariableSymbol, object>
	{
		public new object this[VariableSymbol symbol]
		{
			get
			{
				foreach (var key in base.Keys)
				{
					if (key == symbol)
						return base[key];
				}
				if (Parent != null)
					return Parent[symbol];
				else
					return null;
			}
			set
			{
				base[symbol] = value;
			}
		}

		public VariableValueCollection Parent { get; private set; }

		public bool HasKey(VariableSymbol variable)
		{
			if (ContainsKey(variable)) return true;
			if (Parent != null) return Parent.HasKey(variable);
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent">can be null</param>
		public VariableValueCollection(VariableValueCollection parent)
		{
			Parent = parent;
		}

		public VariableValueCollection(IDictionary<VariableSymbol, object> dictionary) : base(dictionary)
		{
			Parent = null;
		}

		public VariableValueCollection Push()
		{
			var result = new VariableValueCollection(this);
			return result;
		}

		public VariableValueCollection Pop(out VariableValueCollection old)
		{
			old = this;
			var result = Parent;
			Parent = null;
			return result;
		}
		//}
		

		//public object LookUp(VariableSymbol variable)
		//{
		//	if (!ContainsKey(variable))
		//	{
		//		if (Parent != null)
		//			return Parent.LookUp(variable);
		//		else
		//			return null;
		//	}
		//	return this[variable];
		//}
	}
}