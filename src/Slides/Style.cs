using System;
using System.Collections.Generic;

namespace Slides
{
	[Serializable]
	public class Style
	{
		public string Name { get; }
		public Dictionary<string, object> ModifiedFields { get; }

		public Style(string name, Dictionary<string, object> modifiedFields)
		{
			Name = name;
			ModifiedFields = modifiedFields;
		}
	}
}
