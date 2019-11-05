namespace Slides.Filters
{
	public class MorphologyFilter : SVGFilter
	{
		public override string Name => "feMorphology";
		public MorphologyFilter(IFilterInput input, MorphologyOperator morphologyOperator, float radius)
		{
			Input = input;
			Operator = morphologyOperator;
			Radius = radius;
		}

		public IFilterInput Input { get; }
		public MorphologyOperator Operator { get; }
		public float Radius { get; }
	}
}
