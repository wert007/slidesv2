namespace Slides.Transforms
{
	public class SingleValueTransform : Transform
	{
		public SingleValueTransform(TransformKind type, Unit value)
		{
			Kind = type;
			Value = value;
		}

		public override TransformKind Kind { get; }
		public Unit Value { get; }
	}
}
