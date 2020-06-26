using SVGLib.Datatypes;

namespace SVGLib.GraphicsElements
{
	public abstract class SVGGraphicsElement : SVGElement
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public SVGGraphicsElement()
		{
			Fill = Color.Black;
			Stroke = Color.Transparent;
			StrokeWidth = 0;
			X = 0;
			Y = 0;
			Width = 0;
			Height = 0;
			IsVisible = true;
		}

	}
}
