using System;
using System.Text;

namespace Slides
{
	public class Matrix
	{
		public float[][] values { get; set; } 
		public int rows { get; }
		public int columns { get; }

		public float this[int x, int y]
		{
			get
			{ return values[x][y]; }
			set
			{
				values[x][y] = value;
			}
		}

		public Matrix(int width, int height)
		{
			columns = width;
			rows = height;
			values = new float[width][];
			for (int i = 0; i < width; i++)
			{
				values[i] = new float[height];
			}
		}

		public float Sum()
		{
			var result = 0f;
			for (int x = 0; x < columns; x++)
			{
				for (int y = 0; y < rows; y++)
				{
					result += values[x][y];
				}
			}
			return result;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			for (int x = 0; x < columns; x++)
			{
				for (int y = 0; y < rows; y++)
				{
					builder.Append(values[x][y] + " ");
				}
			}
			return builder.ToString();
		}
	}
}
