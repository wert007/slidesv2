using System;

namespace SVGLib.Filters
{
	public class ComponentTransferFilterChild
	{
		public ComponentTransferType Type { get; }
		public float[] TableValues { get; }
		public float Intercept { get; }
		public float Amplitude { get; }
		public float Exponent { get; }
		public float Offset { get; }

		public ComponentTransferFilterChild()
		{
			Type = ComponentTransferType.Identity;
			TableValues = new float[0];
			Intercept = 0;
			Amplitude = 0;
			Exponent = 0;
			Offset = 0;
		}

		public ComponentTransferFilterChild(ComponentTransferType type, float[] tableValues)
		{
			Type = type;
			if (type != ComponentTransferType.Table && type != ComponentTransferType.Discrete)
				throw new Exception();
			TableValues = tableValues;
			Intercept = 0;
			Amplitude = 0;
			Exponent = 0;
			Offset = 0;
		}

		public ComponentTransferFilterChild(float intercept)
		{
			Type = ComponentTransferType.Linear;
			TableValues = new float[0];
			Intercept = intercept;
			Amplitude = 0;
			Exponent = 0;
			Offset = 0;
		}

		public ComponentTransferFilterChild(float amplitude, float exponent, float offset)
		{
			Type = ComponentTransferType.Gamma;
			TableValues = new float[0];
			Intercept = 0;
			Amplitude = amplitude;
			Exponent = exponent;
			Offset = offset;
		}
	}
}
