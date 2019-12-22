using System;

namespace Slides
{
	public enum ChartType
	{
		LineChart,
	}
	public abstract class Chart : Element
	{
		public bool showXAxis { get; set; } = true;
		public bool showYAxis { get; set; } = true;
		public bool showLegend { get; set; } = true;
		public abstract ChartType chartType { get; }	
	}
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

			values = new int[Data.Width][];
			for (int x = 0; x < Data.Width; x++)
			{
				values[x] = new int[Data.Height - 1];
				for (int y = 1; y < Data.Height; y++)
				{
					var value = int.Parse(Data.GetValue(x, y));
					values[x][y - 1] = value;
					if(x == 0)
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
		public int maxValueX { get; set; }
		public int maxValueY { get; set; }
		public int minValueX { get; set; }
		public int minValueY { get; set; }

		public int[][] values { get; }
		
		public string xName { get; set; }
		public string yName { get; set; }

		public override ElementType type => ElementType.LineChart;
		public override ChartType chartType => ChartType.LineChart;
		protected override Unit get_InitialHeight() => new Unit(100, Unit.UnitKind.Pixel);

		protected override Unit get_InitialWidth() => new Unit(100, Unit.UnitKind.Pixel);
	}
}
