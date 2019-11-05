namespace Slides.Filters
{
	public class BlendFilter : SVGFilter
	{
		public override string Name => "feBlend";

		public IFilterInput Input1 { get; }
		public IFilterInput Input2 { get; }
		public BlendMode Mode { get; }

		public BlendFilter(IFilterInput input1, IFilterInput input2, BlendMode mode)
		{
			Input1 = input1;
			Input2 = input2;
			Mode = mode;
		}
	}
}
