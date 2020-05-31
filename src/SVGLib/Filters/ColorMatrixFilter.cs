using SVGLib.Datatypes;

namespace SVGLib.Filters
{
	public class ColorMatrixFilter : SVGFilter
	{
		public override string Name => "feColorMatrix";
		public ColorMatrixFilter(IFilterInput input, Matrix matrix)
		{
			Input = input;
			Matrix = matrix;
			Value = 0;
			Type = ColorMatrixType.Matrix;
		}

		public ColorMatrixFilter(IFilterInput input, ColorMatrixType type, float value)
		{
			Input = input;
			Type = type;
			Matrix = null;
			Value = value;
		}

		public IFilterInput Input { get; }
		public Matrix? Matrix { get; }
		public ColorMatrixType Type { get; }
		public float Value { get; }
	}
}
