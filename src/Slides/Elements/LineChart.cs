using Slides.Data;
using System;

namespace Slides.Elements
{
	public class LineChart : Chart
	{
		public LineChart(CSVFile data)
		{
			Data = data;
			maxValueX = int.MinValue;
			maxValueY = int.MinValue;
			minValueX = int.MaxValue;
			minValueY = int.MaxValue;
			yName = Data.GetValue(0, 0);
			xName = Data.GetValue(1, 0);

			values = new float[Data.Width][];
			for (int x = 0; x < Data.Width; x++)
			{
				values[x] = new float[Data.Height - 1];
				for (int y = 1; y < Data.Height; y++)
				{
					var value = float.Parse(Data.GetValue(x, y));
					values[x][y - 1] = value;
					if (x == 0)
					{
						maxValueY = Math.Max(maxValueY, value);
						minValueY = Math.Min(minValueY, value);
					}
					else
					{
						maxValueX = Math.Max(maxValueX, value);
						minValueX = Math.Min(minValueX, value);
					}
				}
			}
		}

		public CSVFile Data { get; }
		public float maxValueX { get; set; }
		public float maxValueY { get; set; }
		public float minValueX { get; set; }
		public float minValueY { get; set; }

		public float[][] values { get; }

		public string xName { get; set; }
		public string yName { get; set; }

		public override ChartType chartType => ChartType.LineChart;
		internal override Unit get_InitialHeight() => new Unit(100, Unit.UnitKind.Pixel);

		internal override Unit get_InitialWidth() => new Unit(100, Unit.UnitKind.Pixel);
	}
}
