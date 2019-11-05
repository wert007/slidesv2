namespace Slides.Filters
{
	public class SpecularLightingFilter : SVGFilter
	{
		public override string Name => "feSpecularLighting";
		public SpecularLightingFilter(IFilterInput input, float surfaceScale, float specularConstant, float specularExponent, Light child)
		{
			Input = input;
			SurfaceScale = surfaceScale;
			SpecularConstant = specularConstant;
			SpecularExponent = specularExponent;
			Child = child;
		}

		public IFilterInput Input { get; }
		public float SurfaceScale { get; }
		public float SpecularConstant { get; }
		public float SpecularExponent { get; }
		public Light Child { get; }
	}
}
