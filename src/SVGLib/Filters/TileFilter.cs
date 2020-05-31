namespace SVGLib.Filters
{
	public class TileFilter : SVGFilter
	{
		public override string Name => "feTile";

		public IFilterInput Input { get; }

		public TileFilter(IFilterInput input)
		{
			Input = input;
		}
	}
}
