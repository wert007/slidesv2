using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides
{
	[Serializable]

	public abstract class Style
	{
		public abstract string Name { get; }

	}

	[Serializable]
	public class CustomStyle : Style
	{

		public CustomStyle(string name, Dictionary<string, object> modifiedFields)
		{
			Name = name;
			ModifiedFields = modifiedFields;
		}

		public override string Name { get; }
		public Dictionary<string, object> ModifiedFields { get; }
	}

	[Serializable]
	public class StdStyle : Style
	{
		public override string Name => "std";
		public TypedModifications[] Substyles { get; }


		public StdStyle(TypedModifications[] substyles)
		{
			Substyles = substyles;
		}

		public object GetValue(string field, string type)
		{
			var substyle = GetStyle(type);
			if (substyle != null)
			{
				if (substyle.HasValue(field))
					return substyle.GetValue(field);
			}
			return GetStyle("*").GetValue(field);
		}

		public bool HasValue(string field, string type)
		{
			var substyle = GetStyle(type);
			if(substyle != null)
			{
				if (substyle.HasValue(field))
					return true;
			}
			return GetStyle("*").HasValue(field);
		}

		public TypedModifications GetStyle(string type)
		{
			return Substyles.FirstOrDefault(t => t.Type == type);
		}
	}


	[Serializable]

	public class TypedModifications
	{
		public string Type { get; }
		public Dictionary<string, object> ModifiedFields { get; }

		public TypedModifications(string type, Dictionary<string, object> modifiedFields)
		{
			Type = type;
			ModifiedFields = modifiedFields;
		}

		public bool HasValue(string field)
		{
			return ModifiedFields.ContainsKey(field);
		}

		public object GetValue(string field)
		{
			return ModifiedFields[field];
		}
	}
}
