namespace SVGLib.Datatypes
{
	public struct ViewBox
	{
		public ViewBox(double width, double height) : this(0, 0, width, height) { }
		public ViewBox(double x, double y, double width, double height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public double X { get; }
		public double Y { get; }
		public double Width { get; }
		public double Height { get; }

		public override string ToString() 
			=> $"{X.ToString("G", TextHelper.UsCulture)} {Y.ToString("G", TextHelper.UsCulture)} {Width.ToString("G", TextHelper.UsCulture)} {Height.ToString("G", TextHelper.UsCulture)}";
	}
}
