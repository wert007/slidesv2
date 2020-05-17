using System;
using System.Collections.Generic;
using System.Text;

namespace SVGLib.Datatypes
{
	public struct Color
	{
		private Color(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public byte R { get; }
		public byte G { get; }
		public byte B { get; }
		public byte A { get; }
		public static Color Transparent => FromRGBA(0, 0, 0, 0);
		public static Color Black => FromRGB(0, 0, 0);

		public static Color FromGrayValue(double value) => FromGrayValue(ToByte(value));
		public static Color FromGrayValue(byte value) => FromRGB(value, value, value);
		public static Color FromRGB(double r, double g, double b) => FromRGB(ToByte(r), ToByte(g), ToByte(b));
		public static Color FromRGB(byte r, byte g, byte b) => FromRGBA(r, g, b, 255);
		public static Color FromRGBA(byte r, byte g, byte b, byte a) => new Color(r, g, b, a);
		public static Color FromAlphaColor(Color c, double alpha) => new Color(c.R, c.G, c.B, ClampByte(c.A * alpha));

		private static byte ClampByte(double value)
		{
			if (value > 255) return 255;
			if (value < 0) return 0;
			return (byte)Math.Round(value);
		}

		private static byte ToByte(double value)
		{
			if (value > 1) return 255;
			if (value < 0) return 0;
			return (byte)Math.Round(255 * value);
		}

		public override bool Equals(object obj)
		{
			return obj is Color color &&
					 R == color.R &&
					 G == color.G &&
					 B == color.B &&
					 A == color.A;
		}

		public static bool operator ==(Color left, Color right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Color left, Color right)
		{
			return !(left == right);
		}
	}
}
