namespace SVGLib.Filters.Lights
{
	public class PointLight : Light
	{
		public override string Name => "fePointLight";
		public PointLight(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public float X { get; }
		public float Y { get; }
		public float Z { get; }
	}
}
