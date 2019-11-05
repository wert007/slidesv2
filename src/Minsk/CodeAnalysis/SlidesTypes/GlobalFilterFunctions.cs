using Slides;
using Slides.Filters;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	public static class GlobalFilterFunctions
	{
		//TODO: Add all SVGFilters
		public static SVGFilter flood(Color color, float opacity) => new FloodFilter(color, opacity);
		public static SVGFilter turbulence(float baseFrequency, int numOctaves) => new TurbulenceFilter(baseFrequency, numOctaves);
		public static Light pointLight(float x, float y, float z) => new PointLight(x, y, z);
		public static Light distantLight(float azimuth, float elevation) => new DistantLight(azimuth, elevation);
		public static Light spotLight(float x, float y, float z, float pointAtX, float pointAtY, float pointAtZ, float specularExponent, float limitingConeAngle) => new SpotLight(x, y, z, pointAtX, pointAtY, pointAtZ, specularExponent, limitingConeAngle);
		public static SVGFilter diffuseLight(IFilterInput input, float surfaceScale, float diffuseConstant, Light child) => new DiffuseLightingFilter(input, surfaceScale, diffuseConstant, child);
		public static SVGFilter specularLight(IFilterInput input, float surfaceScale, float specularConstant, float specularExponent, Light child) =>
			new SpecularLightingFilter(input, surfaceScale, specularConstant, specularExponent, child);
		public static SVGFilter blend(IFilterInput input1, IFilterInput input2, BlendMode mode) => new BlendFilter(input1, input2, mode);

		public static SVGFilter convolve(IFilterInput input, Matrix matrix) => new ConvolveMatrixFilter(input, matrix);
		public static SVGFilter erode(IFilterInput input, float radius) => new MorphologyFilter(input, MorphologyOperator.Erode, radius);
		public static SVGFilter dilate(IFilterInput input, float radius) => new MorphologyFilter(input, MorphologyOperator.Dilate, radius);
		public static ComponentTransferFilterChild linearNode(float intercept) => new ComponentTransferFilterChild(intercept);
		public static ComponentTransferFilterChild identityNode() => new ComponentTransferFilterChild();
		public static ComponentTransferFilterChild gammaNode(float amplitude, float exponent, float offset) => new ComponentTransferFilterChild(amplitude, exponent, offset);
		public static ComponentTransferFilterChild tableNode(float[] values) => new ComponentTransferFilterChild(ComponentTransferType.Table, values);
		public static ComponentTransferFilterChild discreteNode(float[] values) => new ComponentTransferFilterChild(ComponentTransferType.Discrete, values);

		public static SVGFilter transfer(IFilterInput input, ComponentTransferFilterChild red, ComponentTransferFilterChild green, ComponentTransferFilterChild blue, ComponentTransferFilterChild alpha) => new ComponentTransferFilter(input, red, green, blue, alpha);
		public static SVGFilter blur(IFilterInput input, float stdDeviation) => new GaussianBlurFilter(input, stdDeviation, EdgeMode.Duplicate);
		public static SVGFilter saturate(IFilterInput input, float value) => new ColorMatrixFilter(input, ColorMatrixType.Saturate, value);
		public static SVGFilter colorMatrix(IFilterInput input, Matrix matrix) => new ColorMatrixFilter(input, matrix);
		
		public static Filter blur(float blurFactor) => new BlurFilter(blurFactor);
		public static Filter brightness(float value) => new ProcentalFilter("brightness", value);
		public static Filter contrast(float value) => new ProcentalFilter("contrast", value);
		public static Filter grayscale(float value) => new ProcentalFilter("grayscale", value);
		public static Filter invert(float value) => new ProcentalFilter("invert", value);
		public static Filter opacity(float value) => new ProcentalFilter("opacity", value);
		public static Filter saturate(float value) => new ProcentalFilter("saturate", value);
		public static Filter sepia(float value) => new ProcentalFilter("sepia", value);
		public static Filter dropShadow(int horizontal, int vertical, int blur, int spread, Color color) => new DropShadowFilter(horizontal, vertical, blur, spread, color);
		public static Filter hueRotate(float value) => new HueRotateFilter(value * 360f);

	}
}
