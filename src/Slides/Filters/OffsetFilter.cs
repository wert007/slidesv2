namespace Slides.Filters
{
	public class OffsetFilter : SVGFilter
	{
		public override string Name => "feOffset";

		public IFilterInput Input { get; }
		public float Dx { get; }
		public float Dy { get; }

		public OffsetFilter(IFilterInput input, float dx, float dy)
		{
			Input = input;
			Dx = dx;
			Dy = dy;
		}
	}
}
