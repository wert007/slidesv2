using System;

namespace Slides
{
	[Serializable]
	public class Brush
	{
		public enum BrushMode
		{
			SolidColor,
			ImageSource
		}

		public BrushMode Mode { get; private set; }
		public ImageSource Image { get; }
		public Color Color { get; }

		public Brush(Color color)
		{
			Color = color;
			Mode = BrushMode.SolidColor;
		}

		public Brush(ImageSource image)
		{
			Image = image;
			Mode = BrushMode.ImageSource;
		}
	}
}
