using Minsk.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
	internal class DataObject
	{
		public DataObject(AdvancedTypeSymbol baseType, object[] data)
		{
			BaseType = baseType;
			for (int i = 0; i < BaseType.Fields.Count; i++)
			{
				var field = BaseType.Fields[i];
				object value = null;
				if (data.Length > i)
					value = data[i];
				else
					throw new Exception();
				_fields.Add(field.Name, value);
			}
		}

		private Dictionary<string, object> _fields = new Dictionary<string, object>();

		public AdvancedTypeSymbol BaseType { get; }

		public bool TryLookUp(string name, out object value)
		{
			return _fields.TryGetValue(name, out value);
		}

		public bool TrySet(string name, object value)
		{
			if (!_fields.ContainsKey(name))
				return false;
			_fields[name] = value;
			return true;

		}
	}
}