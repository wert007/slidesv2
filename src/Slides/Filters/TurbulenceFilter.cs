namespace Slides.Filters
{
	public class TurbulenceFilter : SVGFilter
	{
		public override string Name => "feTurbulence";
		public TurbulenceFilter(float baseFrequency, int numOctaves)
		{
			BaseFrequencyX = baseFrequency;
			BaseFrequencyY = baseFrequency;
			NumOctaves = numOctaves;
			Seed = 0;
			StitchTiles = StitchTiles.Stitch;
			Type = TurbulenceType.Turbulence;
		}

		public TurbulenceFilter(float baseFrequencyX, float baseFrequencyY, int numOctaves, int seed, StitchTiles stitchTiles, TurbulenceType type)
		{
			BaseFrequencyX = baseFrequencyX;
			BaseFrequencyY = baseFrequencyY;
			NumOctaves = numOctaves;
			Seed = seed;
			StitchTiles = stitchTiles;
			Type = type;
		}

		public float BaseFrequencyX { get; }
		public float BaseFrequencyY { get; }
		public int NumOctaves { get; }
		public int Seed { get; }
		public StitchTiles StitchTiles { get; }
		public TurbulenceType Type { get; }
	}
}
