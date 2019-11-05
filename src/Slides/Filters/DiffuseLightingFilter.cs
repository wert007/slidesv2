namespace Slides.Filters
{
	public class DiffuseLightingFilter : SVGFilter
	{
		public override string Name => "feDiffuseLighting";
		public DiffuseLightingFilter(IFilterInput input, float surfaceScale, float diffuseConstant, Light child)
		{
			Input = input;
			SurfaceScale = surfaceScale;
			DiffuseConstant = diffuseConstant;
			Child = child;
		}

		public IFilterInput Input { get; }
		public float SurfaceScale { get; }
		public float DiffuseConstant { get; }
		public Light Child { get; }
	}
}
