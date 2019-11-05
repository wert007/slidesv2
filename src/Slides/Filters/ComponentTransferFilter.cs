namespace Slides.Filters
{
	public class ComponentTransferFilter : SVGFilter
	{
		public override string Name => "feComponentTransfer";
		public ComponentTransferFilter(IFilterInput input, ComponentTransferFilterChild red, ComponentTransferFilterChild green, ComponentTransferFilterChild blue, ComponentTransferFilterChild alpha)
		{
			Input = input;
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		public IFilterInput Input { get; }
		public ComponentTransferFilterChild Red { get; }
		public ComponentTransferFilterChild Green { get; }
		public ComponentTransferFilterChild Blue { get; }
		public ComponentTransferFilterChild Alpha { get; }
	}
}
