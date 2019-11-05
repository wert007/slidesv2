using System.Collections.Generic;

namespace Slides
{
	public class CaseCall
	{
		public CaseCall(float condition, Attribute[] changedFields)
		{
			Condition = condition;
			ChangedFields = new Dictionary<string, object>();
			var unchangedFields = new List<string>();
			foreach (var field in changedFields)
			{
				if (field.Value == null)
					unchangedFields.Add(field.Name);
				else
					ChangedFields.Add(field.Name, field.Value);
			}
			UnchangedFields = unchangedFields.ToArray();
		}

		public float Condition { get; }
		public Dictionary<string, object> ChangedFields { get; }
		public string[] UnchangedFields { get; }
	}
}
