namespace Slides
{

	public class ImportValue
	{
		public ImportValue(object value, ImportValueType type)
		{
			Value = value;
			Type = type;
		}

		public ImportValue(object value, ImportValueType type, string href)
		{
			Value = value;
			Type = type;
			Href = href;
		}

		public object Value { get; }
		public ImportValueType Type { get; }
		public string Href { get; }
	}
}
