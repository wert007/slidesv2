using System.Text;

namespace SVGLib.Datatypes
{
	public struct Matrix
	{
		public float[][] Values { get; set; }
		public int Rows { get; }
		public int Columns { get; }

		public float this[int x, int y]
		{
			get
			{
				return Values[x][y]; 
			}
			set
			{
				Values[x][y] = value;
			}
		}

		public Matrix(int width, int height)
		{
			Columns = width;
			Rows = height;
			Values = new float[width][];
			for (int i = 0; i < width; i++)
			{
				Values[i] = new float[height];
			}
		}

		public float Sum()
		{
			var result = 0f;
			for (int x = 0; x < Columns; x++)
			{
				for (int y = 0; y < Rows; y++)
				{
					result += Values[x][y];
				}
			}
			return result;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			for (int x = 0; x < Columns; x++)
			{
				for (int y = 0; y < Rows; y++)
				{
					builder.Append(Values[x][y] + " ");
				}
			}
			return builder.ToString();
		}
	
}
}
