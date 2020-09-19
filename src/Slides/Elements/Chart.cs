using Slides.Data;

namespace Slides.Elements
{
	public abstract class Chart : Element
	{
		public bool showXAxis { get; set; } = true;
		public bool showYAxis { get; set; } = true;
		public int xTickAmount { get; set; } = 10;
		public int yTickAmount { get; set; } = 10;
		public bool showLegend { get; set; } = false;
		public bool showDownload { get; set; } = false;
		public bool showGrid { get; set; } = true;
		public bool showTooltip { get; set; } = false;

		public override ElementKind kind => ElementKind.Chart;

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

		public Chart()
		{
			color = new Color(255, 0, 0, 255);
		}
	}
}
