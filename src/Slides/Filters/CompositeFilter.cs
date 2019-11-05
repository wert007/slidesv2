namespace Slides.Filters
{
	public class CompositeFilter : SVGFilter
	{
		public override string Name => "feComposite";

		public IFilterInput Input1 { get; }
		public IFilterInput Input2 { get; }
		public float K1 { get; }
		public float K2 { get; }
		public float K3 { get; }
		public float K4 { get; }
		public CompositeOperator Operator { get; }

		public CompositeFilter(IFilterInput input1, IFilterInput input2, CompositeOperator compositeOperator)
		{
			Input1 = input1;
			Input2 = input2;
			Operator = compositeOperator;
		}

		public CompositeFilter(IFilterInput input1, IFilterInput input2, float k1, float k2, float k3, float k4)
		{
			Input1 = input1;
			Input2 = input2;
			K1 = k1;
			K2 = k2;
			K3 = k3;
			K4 = k4;
			Operator = CompositeOperator.Arithmetic;
		}
	}
}
