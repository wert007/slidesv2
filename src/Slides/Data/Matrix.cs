using System;
using System.Text;
using SVGMatrix = SVGLib.Datatypes.Matrix;


namespace Slides.Data
{
	[Serializable]
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


		public static implicit operator Matrix(SVGMatrix m)
		{
			var result = new Matrix(m.Columns, m.Rows);
			result.values = m.Values;
			return result;
		}

		public static implicit operator SVGMatrix(Matrix m)
		{
			var result = new SVGMatrix(m.columns, m.rows);
			result.Values = m.values;
			return result;
		}
	}
}
