namespace Slides
{
	public class SingleValueTransform : Transform
	{
		public SingleValueTransform(TransformType type, Unit value)
		{
			Type = type;
			Value = value;
		}

		public override TransformType Type { get; }
		public Unit Value { get; }
	}

	public class RotationTransform : Transform
	{
		public RotationTransform(TransformType type, float value)
		{
			Type = type;
			Value = value % 360;
		}

		public override TransformType Type { get; }
		public float Value { get; }
	}
}
