namespace Slides.MathTypes
{
	public class LambdaFunction
	{
		public float[] Values { get; }

		public LambdaFunction(float[] values)
		{
			Values = values;
		}

		public float Compute(float x)
		{
			float result = 0;
			for (int i = 0; i < Values.Length; i++)
			{
				result += Values[i] * Pow(x, i);
			}
			return result;	
		}

		public static float Pow(float x, int exp)
		{
			float result = 1;
			for (int i = 0; i < exp; i++)
			{
				result *= x;
			}
			return result;
		}
	}
}
