namespace SVGLib.Filters.Lights
{
	public class DistantLight : Light
	{
		public override string Name => "feDistantLight";
		public DistantLight(float azimuth, float elevation)
		{
			Azimuth = azimuth;
			Elevation = elevation;
		}

		public float Azimuth { get; }
		public float Elevation { get; }
	}
}
