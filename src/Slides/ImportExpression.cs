namespace Slides
{
	public class ImportExpression<T>
	{
		public ImportExpression(T value)
		{
			Value = value;
		}

		public ImportExpression(T value, string href) : this(value)
		{
			Href = href;
		}

		public T Value { get; }
		public string Href { get; }
	}
}
