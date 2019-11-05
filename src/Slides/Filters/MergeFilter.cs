namespace Slides.Filters
{
	public class MergeFilter : SVGFilter
	{
		public override string Name => "feMerge";

		public IFilterInput[] Inputs { get; }

		public MergeFilter(IFilterInput[] inputs)
		{
			Inputs = inputs;
		}
	}
}
