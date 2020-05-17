namespace Slides.Transforms
{
	public class RotationTransform : Transform
	{
		public RotationTransform(TransformKind type, float value)
		{
			Kind = type;
			Value = value % 360;
		}

		public override TransformKind Kind { get; }
		public float Value { get; }
	}
}
