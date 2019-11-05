using System;

namespace Slides
{

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

		public static Brush FromObject(object value)
		{
			switch (value)
			{
				case Color color:
					return new Brush(color);
				case ImageSource image:
					return new Brush(image);
				default:
					throw new ArgumentException();
			}
		}
	}
}
