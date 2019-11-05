using System.Collections.Generic;
using System.Linq;

namespace Slides
{
	public class TupleType
	{
		public Dictionary<string, object> Fields { get; }

		public object this[int index] => Fields.Values.ElementAt(index);

		public TupleType(Dictionary<string, object> fields)
		{
			Fields = fields;
		}
	}
}