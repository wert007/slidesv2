using Slides.MathTypes;
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
		public bool showLegend { get; set; } = false;
		public bool showDownload { get; set; } = false;
		public bool showGrid { get; set; } = true;
		public bool showTooltip { get; set; } = false;

		//TODO: Better name.
		// bare()?
		// essential()?
		// plotOnly()
		public void bare()
		{
			showXAxis = false;
			showYAxis = false;
			showLegend = false;
			showDownload = false;
			showGrid = false;
			showTooltip = false;
		}
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

		public LineChart(LambdaFunction f, Range range)
		{
			minValueX = range.From;
			maxValueX = range.To;
			maxValueY = float.MinValue;
			minValueY = float.MaxValue;
			int l = range.To - range.From;
			values = new float[2][];
			values[0] = new float[l];
			values[1] = new float[l];
			int i = 0;
			while(i < l)
			{
				float x = minValueX + (maxValueX - minValueX) * i / l;
				values[0][i] = x;
				values[1][i] = f.Compute(x);
				maxValueY = Math.Max(maxValueY, f.Compute(x));
				minValueY = Math.Min(minValueY, f.Compute(x));
				i += range.Step;
			}
			xName = "x";
			yName = "y";
			Data = null;
		}

		public CSVFile Data { get; }
		public float maxValueX { get; set; }
		public float maxValueY { get; set; }
		public float minValueX { get; set; }
		public float minValueY { get; set; }

		public float[][] values { get; }

		public string xName { get; set; }
		public string yName { get; set; }

		public override ElementType type => ElementType.LineChart;
		public override ChartType chartType => ChartType.LineChart;
		protected override Unit get_InitialHeight() => new Unit(100, Unit.UnitKind.Pixel);

		protected override Unit get_InitialWidth() => new Unit(100, Unit.UnitKind.Pixel);
	}
}
