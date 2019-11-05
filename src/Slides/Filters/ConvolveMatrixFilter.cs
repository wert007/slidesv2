namespace Slides.Filters
{
	public class ConvolveMatrixFilter : SVGFilter
	{
		public override string Name => "feConvolveMatrix";
		public ConvolveMatrixFilter(IFilterInput input, Matrix matrix)
		{
			Input = input;
			Matrix = matrix;
			OrderX = matrix.columns;
			OrderY = matrix.rows;
			Bias = 0f;
			Divisor = matrix.Sum();
			TargetX = OrderX / 2 + 1;
			TargetY = OrderY / 2 + 1;
			EdgeMode = EdgeMode.Duplicate;
			PreserveAlpha = false;
		}

		public ConvolveMatrixFilter(IFilterInput input, Matrix matrix, float bias, float divisor, int targetX, int targetY, EdgeMode edgeMode, bool preserveAlpha) : this(input, matrix)
		{
			Bias = bias;
			Divisor = divisor;
			TargetX = targetX;
			TargetY = targetY;
			EdgeMode = edgeMode;
			PreserveAlpha = preserveAlpha;
		}

		public IFilterInput Input { get; }
		public Matrix Matrix { get; }
		public int OrderX { get; }
		public int OrderY { get; }
		public float Bias { get; }
		public float Divisor { get; }
		public int TargetX { get; }
		public int TargetY { get; }
		public EdgeMode EdgeMode { get; }
		public bool PreserveAlpha { get; }
	}
}
