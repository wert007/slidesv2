using System;
using System.Collections.Generic;

namespace Slides
{
	[Serializable]
	public class Color
	{
		public static readonly Color Transparent = new Color(0, 0, 0, 0);
		public byte R { get; }
		public byte G { get; }
		public byte B { get; }
		public byte A { get; }

		public int Hue { private set; get; }
		public byte Saturation { private set; get; }
		public byte Lightness { private set; get; }
		public bool IsRGBA { get; private set; } = true;

		public Color(float r, float g, float b, float a)
		{
			R = (byte)(255 * r);
			G = (byte)(255 * g);
			B = (byte)(255 * b);
			A = (byte)(255 * a);
		}

		public Color(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g; 
			B = b;
			A = a;
		}

		public static Color FromHSLA(int hue, byte saturation, byte light, byte alpha)
		{
			var result = new Color(0, 0, 0, alpha);
			result.Hue = hue;
			result.Saturation = saturation;
			result.Lightness = light;
			result.IsRGBA = false;
			return result;
		}

		public override bool Equals(object obj)
		{
			if(obj is Color c)
			{
				if (A != c.A)
					return false;
				if (A == 0)
					return true;

				if(c.IsRGBA == IsRGBA)
					return c.R == R && c.G == G && c.B == B;
				return c.Hue == Hue && c.Lightness == Lightness && c.Saturation == Saturation;
			}
			return false;
		}

		public string ToHex()
		{
			return "#" + A.ToString("X2") + R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
		}

		public static IEnumerable<KeyValuePair<string, Color>> GetStaticColors()
		{
			//CSS 2.1
			yield return new KeyValuePair<string, Color>("white",   new Color(255, 255, 255, 255));
			yield return new KeyValuePair<string, Color>("black",   new Color(  0,   0,   0, 255));
			yield return new KeyValuePair<string, Color>("blue",    new Color(  0,   0, 255, 255));
			yield return new KeyValuePair<string, Color>("aqua",    new Color(  0, 255, 255, 255));
			yield return new KeyValuePair<string, Color>("lime",    new Color(  0, 255,   0, 255));
			yield return new KeyValuePair<string, Color>("green",   new Color(  0, 128,   0, 255));
			yield return new KeyValuePair<string, Color>("gray",    new Color(128, 128, 128, 255));
			yield return new KeyValuePair<string, Color>("olive",   new Color(128, 128,   0, 255));
			yield return new KeyValuePair<string, Color>("navy",    new Color(  0,   0, 128, 255));
			yield return new KeyValuePair<string, Color>("red",     new Color(255,   0,   0, 255));
			yield return new KeyValuePair<string, Color>("teal",    new Color(  0, 128, 128, 255));
			yield return new KeyValuePair<string, Color>("orange",  new Color(255, 165,   0, 255));
			yield return new KeyValuePair<string, Color>("purple",  new Color(128,   0, 128, 255));
			yield return new KeyValuePair<string, Color>("silver",  new Color(192, 192, 192, 255));
			yield return new KeyValuePair<string, Color>("yellow",  new Color(255, 255,   0, 255));
			yield return new KeyValuePair<string, Color>("maroon",  new Color(128,   0,   0, 255));
			yield return new KeyValuePair<string, Color>("fuchsia", new Color(255,   0, 255, 255));

			//CSS 3
			yield return new KeyValuePair<string, Color>("aliceblue",      new Color(240, 248, 255, 255));
			yield return new KeyValuePair<string, Color>("antiquewhite",   new Color(250, 235, 215, 255));
			yield return new KeyValuePair<string, Color>("aquamarine",     new Color(127, 255, 212, 255));
			yield return new KeyValuePair<string, Color>("azure",          new Color(240, 255, 255, 255));
			yield return new KeyValuePair<string, Color>("beige",          new Color(245, 245, 220, 255));
			yield return new KeyValuePair<string, Color>("bisque",         new Color(255, 228, 196, 255));
			yield return new KeyValuePair<string, Color>("blanchedalmond", new Color(255, 235, 205, 255));
			yield return new KeyValuePair<string, Color>("blueviolet",     new Color(138,  43, 226, 255));
			yield return new KeyValuePair<string, Color>("brown",          new Color(165,  42,  42, 255));
			yield return new KeyValuePair<string, Color>("burlywood",      new Color(222, 184, 135, 255));
			yield return new KeyValuePair<string, Color>("cadetblue",      new Color( 95, 158, 160, 255)); //TODO
			yield return new KeyValuePair<string, Color>("chartreuse",     new Color( 95, 158, 160, 255)); //TODO
			yield return new KeyValuePair<string, Color>("chocolate",      new Color(210, 105,  30, 255));
			yield return new KeyValuePair<string, Color>("coral",          new Color(255, 127,  80, 255));
			yield return new KeyValuePair<string, Color>("cornflowerblue", new Color(100, 149, 237, 255));
			yield return new KeyValuePair<string, Color>("cornsilk",       new Color(255, 248, 220, 255));
			yield return new KeyValuePair<string, Color>("crimson",        new Color(220,  20,  60, 255));
			yield return new KeyValuePair<string, Color>("cyan",           new Color(  0, 255, 255, 255));
			yield return new KeyValuePair<string, Color>("darkblue",       new Color(  0,   0, 139, 255));
			yield return new KeyValuePair<string, Color>("darkcyan",       new Color(  0, 139, 139, 255));
			yield return new KeyValuePair<string, Color>("darkgoldenrod",  new Color(184, 134,  11, 255));
			yield return new KeyValuePair<string, Color>("darkgray",       new Color(169, 169, 169, 255));
			yield return new KeyValuePair<string, Color>("darkgreen",      new Color(  0, 100,   0, 255));
			yield return new KeyValuePair<string, Color>("darkkhaki",      new Color(189, 183, 107, 255));
			yield return new KeyValuePair<string, Color>("darkmagenta",    new Color(139,   0, 139, 255));
			yield return new KeyValuePair<string, Color>("darkolivegreen", new Color( 85, 107,  47, 255));
			yield return new KeyValuePair<string, Color>("darkorange",    new Color(255, 140,   0, 255));
			yield return new KeyValuePair<string, Color>("darkorchid",    new Color(153,  50, 204, 255));
			yield return new KeyValuePair<string, Color>("darkred",       new Color(139,   0,   0, 255));
			yield return new KeyValuePair<string, Color>("darksalmon",    new Color(233, 150, 122, 255));
			yield return new KeyValuePair<string, Color>("darkseagreen",  new Color(143, 188, 143, 255));
			yield return new KeyValuePair<string, Color>("darkslateblue", new Color( 72,  61, 139, 255));
			yield return new KeyValuePair<string, Color>("darkviolet",    new Color(148,   0, 211, 255));
			yield return new KeyValuePair<string, Color>("darkturquoise", new Color(  0, 206, 209, 255));
			yield return new KeyValuePair<string, Color>("darkslategray", new Color( 47,  79,  79, 255));

			yield return new KeyValuePair<string, Color>("deeppink", new Color(255,  20, 147, 255));

			//TODO: Finish it up!
			//https://www.tutorialrepublic.com/css-reference/css-color-names.php






		}
	}
}
