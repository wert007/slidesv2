namespace SVGLib.Filters
{

	public abstract class SVGFilter : IFilterInput
	{
		public abstract string Name { get; }
	}

	//TODO: feImage
	//spec: https://developer.mozilla.org/en-US/docs/Web/SVG/Element/feImage
}
