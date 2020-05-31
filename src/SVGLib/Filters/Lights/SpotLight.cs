namespace SVGLib.Filters.Lights
{
	public class SpotLight : Light
	{
		public override string Name => "feSpotLight";
		public SpotLight(float x, float y, float z, float pointAtX, float pointAtY, float pointAtZ, float specularExponent, float limitingConeAngle)
		{
			X = x;
			Y = y;
			Z = z;
			PointAtX = pointAtX;
			PointAtY = pointAtY;
			PointAtZ = pointAtZ;
			SpecularExponent = specularExponent;
			LimitingConeAngle = limitingConeAngle;
		}

		public float X { get; }
		public float Y { get; }
		public float Z { get; }
		public float PointAtX { get; }
		public float PointAtY { get; }
		public float PointAtZ { get; }
		public float SpecularExponent { get; }
		public float LimitingConeAngle { get; }
	}
}
