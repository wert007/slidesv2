using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVGColor = SVGLib.Datatypes.Color;
using SVGVector2 = SVGLib.Datatypes.Vector2;

namespace Slides.Helpers
{
	public static class SlidesConverter
	{
		public const float PointUnitConversionFactor = 1.4f;
		public static object Convert(object obj, Type target)
		{
			if (target == typeof(Brush))
				return ConvertToBrush(obj);
			if (target == typeof(Color))
				return ConvertToColor(obj);
			if (target == typeof(SVGColor))
				return ConvertToSVGColor(obj);
			if (target == typeof(Unit))
				return ConvertToUnit(obj);
			return obj;
		}


		public static Brush ConvertToBrush(object value)
		{
			switch (value)
			{
				case Brush brush:
					return brush;
				case Color color:
					return new Brush(color);
				case ImageSource image:
					return new Brush(image);
				default:
					throw new ArgumentException();
			}
		}

		public static Color ConvertToColor(object value)
		{
			switch (value)
			{
				case Color color:
					return color;
				case SVGColor color:
					return new Color(color.R, color.G, color.B, color.A);
				default:
					throw new ArgumentException();
			}
		}
		private static SVGColor ConvertToSVGColor(object value)
		{
			switch (value)
			{
				case SVGColor color:
					return color;
				case Color color:
					if (color.IsRGBA)
						return SVGColor.FromRGBA(color.R, color.G, color.B, color.A);
					else throw new NotImplementedException();
				default:
					throw new ArgumentException();
			}
		}
		public static Unit ConvertToUnit(object value)
		{
			switch (value)
			{
				case Unit u:
					return u;
				case float f:
					return new Unit(f * 100, Unit.UnitKind.Percent);
				case int i:
					return new Unit(i, Unit.UnitKind.Pixel);
				default:
					throw new ArgumentException();
			}

		}
	}
}
