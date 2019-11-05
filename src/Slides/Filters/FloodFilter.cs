namespace Slides.Filters
{
	public class FloodFilter : SVGFilter
	{
		public override string Name => "feFlood";
		public FloodFilter(Color floodColor, float floodOpacity)
		{
			FloodColor = floodColor;
			FloodOpacity = floodOpacity;
		}

		public Color FloodColor { get; }
		public float FloodOpacity { get; }
	}
}
