namespace Slides
{
	public class ColorQuadruple
	{
		public Color top { get; set; }
		public Color right { get; set; }
		public Color bottom { get; set; }
		public Color left { get; set; }

		public ColorQuadruple()
		{
			top = new Color(0, 0, 0, 0);
			right = new Color(0, 0, 0, 0);
			bottom = new Color(0, 0, 0, 0);
			left = new Color(0, 0, 0, 0);
		}

		public ColorQuadruple(Color top, Color right, Color bottom, Color left)
		{
			this.top = top;
			this.right = right;
			this.bottom = bottom;
			this.left = left;
		}
	}
}
