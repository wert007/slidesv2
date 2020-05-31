using SVGLib.Datatypes;

namespace SVGLib.GraphicsElements
{
	public abstract class SVGGraphicsElement : SVGElement
	{
		public int width { get; set; }
		public int height { get; set; }
		public int x { get; set; }
		public int y { get; set; }

		public SVGGraphicsElement()
		{
			Fill = Color.Black;
			Stroke = Color.Transparent;
			StrokeWidth = 0;
			x = 0;
			y = 0;
			width = 0;
			height = 0;
			IsVisible = true;
		}

	}
}
