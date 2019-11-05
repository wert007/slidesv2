namespace Slides.Filters
{
	public class GaussianBlurFilter : SVGFilter
	{
		public override string Name => "feGaussianBlur";

		public GaussianBlurFilter(IFilterInput input, float stdDeviation, EdgeMode edgeMode)
		{
			Input = input;
			StdDeviationHorizontal = stdDeviation;
			StdDeviationVertical = stdDeviation;
			EdgeMode = edgeMode;
		}

		public GaussianBlurFilter(IFilterInput input, float stdDeviationHorizontal, float stdDeviationVertical, EdgeMode edgeMode) 
		{
			Input = input;
			StdDeviationHorizontal = stdDeviationHorizontal;
			StdDeviationVertical = stdDeviationVertical;
			EdgeMode = edgeMode;
		}

		public IFilterInput Input { get; }
		public float StdDeviationHorizontal { get; }
		public float StdDeviationVertical { get; }
		public EdgeMode EdgeMode { get; }
	}
}
