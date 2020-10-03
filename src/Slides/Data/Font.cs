using Slides.Helpers;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace Slides.Data
{
	[Serializable]
	public class Font
	{
		public Font(string name)
		{
			this.name = name;
			exists = true;
			try
			{
				_fontFamily = new FontFamily(name);
			}
			catch (ArgumentException)
			{
				exists = false;
			}
		}

		public string name { get; }
		public bool exists { get; private set; }

		[NonSerialized]
		private FontFamily _fontFamily;

		public override string ToString() => name;

		public bool LoadFontFamily(string path)
		{
			if (!File.Exists(path))
				return false;

			exists = true;
			PrivateFontCollection collection = new PrivateFontCollection();
			collection.AddFontFile(path);
			try
			{
				_fontFamily = new FontFamily(name, collection);
			}
			catch (ArgumentException)
			{
				exists = false;
			}

			return exists;
		}

		public Vector2 Measure(string text, Unit fontsize, float lineHeight)
		{

			if (!exists)
				return new Vector2();
			// This is really just a magic number which yielded through trial and error
			// the expected results. It should be somehow possible to replace. I hope.
			/* Result of that trial an error.
				stackHorizontal.height = 51.43746px;
				              expected = 51.600px;
				   stackVertical.width = 172.8371px;
				              expected = 172.837px;
			 */
			const float shrinkFactor = 0.913447f;
			var size = fontsize.Value * shrinkFactor;
			if (fontsize.Kind == Unit.UnitKind.Point)
				size *= SlidesConverter.PointUnitConversionFactor;
			var unit = GraphicsUnit.World;
			switch (fontsize.Kind)
			{
				case Unit.UnitKind.Point:
					unit = GraphicsUnit.Point;
					break;
				case Unit.UnitKind.Percent:
					unit = GraphicsUnit.World;
					break;
				case Unit.UnitKind.Pixel:
					unit = GraphicsUnit.Pixel;
					break;
				case Unit.UnitKind.Auto:
				case Unit.UnitKind.Addition:
				case Unit.UnitKind.CharacterWidth:
				case Unit.UnitKind.Subtraction:
				default:
					throw new Exception();
			}

			var font = new System.Drawing.Font(_fontFamily, size, FontStyle.Regular, unit);

			SizeF result;
			using (var image = new Bitmap(1, 1))
			using (var g = Graphics.FromImage(image))
			{
				result = g.MeasureString(text, font);
			}

			var vec = new Vector2(result);
			// Default browser lineheight is 1.2 which seems to be equivalent to 1.SOMETHING in System.Drawing.
			vec.Y *= lineHeight - (0.2f * shrinkFactor);
			return vec;
		}
	}
}
