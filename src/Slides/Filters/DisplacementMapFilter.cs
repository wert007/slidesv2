namespace Slides.Filters
{
	public class DisplacementMapFilter : SVGFilter
	{
		public override string Name => "feDisplacementMap";

		public IFilterInput Input1 { get; }
		public IFilterInput Input2 { get; }
		public float Scale { get; }
		public DisplacementMapChannelSelector XChannelSelector { get; }
		public DisplacementMapChannelSelector YChannelSelector { get; }

		public DisplacementMapFilter(IFilterInput input1, IFilterInput input2, float scale, DisplacementMapChannelSelector xChannelSelector, DisplacementMapChannelSelector yChannelSelector)
		{
			Input1 = input1;
			Input2 = input2;
			Scale = scale;
			XChannelSelector = xChannelSelector;
			YChannelSelector = yChannelSelector;
		}
	}
}
