namespace Slides.SVG
{
	public enum SVGElementKind
	{
		Group,
		Rect,
		Circle,
		Ellipse,
		Line,
		Path,
		Polygon,
		Polyline,
		Text
	}

	public abstract class SVGElement
	{
		public abstract SVGElementKind kind { get; }
		public int width { get; set; }
		public int height { get; set; }
		public int x { get; set; }
		public int y { get; set; }
		public Color fill { get; set; }
		public Color stroke { get; set; }
		public int strokeWidth { get; set; }
		public bool isVisible { get; set; }

		public SVGElement()
		{
			fill = Color.Transparent;
			stroke = Color.Transparent;
			strokeWidth = 0;
			x = 0;
			y = 0;
			width = 0;
			height = 0;
			isVisible = true;
		}

	}
}
