using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace Slides
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

		public Vector2 Measure(string text, Unit fontsize)
		{
			if (!exists)
				return new Vector2();
			var size = fontsize.Value;
			if (fontsize.Kind == Unit.UnitKind.Point)
				size *= 1.5f;
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
			{
				using (var g = Graphics.FromImage(image))
				{
						result = g.MeasureString(text, font);
				}
			}

			return new Vector2(result);
		}
	}
}
