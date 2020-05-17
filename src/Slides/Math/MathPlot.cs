using Slides.Elements;

namespace Slides.MathExpressions
{
	public class MathPlot : Chart
	{
		public MathPlot(MathFormula graph, Range range)
			: this(graph, range, 0.5f)
		{
		}
		public MathPlot(MathFormula graph, Range range, float step)
		{
			Graph = graph;
			InputVariable = "x";
			for (int i = 0; i < graph.Variables.Length; i++)
			{
				if (graph.IsUnknown(graph.Variables[i]))
				{
					InputVariable = graph.Variables[i];
					break;
				}
			}
			Range = range;
			Step = step;
			color = new Color(255, 0, 0, 255);
			showGrid = true;
			showTooltip = true;
			showXAxis = true;
			showYAxis = true;
		}

		public MathFormula Graph { get; }
		public Range Range { get; }
		public float Step { get; }
		public string InputVariable { get; }

		//TODO: Should the kind still be chart, but the chartType
		// be MathPlot? I mean probably
		public override ElementKind kind => ElementKind.MathPlot;

		public override ChartType chartType => ChartType.LineChart;

		protected override Unit get_InitialHeight()
		{
			return new Unit(480, Unit.UnitKind.Pixel);
		}

		protected override Unit get_InitialWidth()
		{
			return new Unit(640, Unit.UnitKind.Pixel);
		}
	}
}
