﻿using ImageMagick;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Debug;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Minsk.CodeAnalysis.SlidesTypes
{

	public static class GlobalFunctions
	{
		public static CSVFile csv(string path)
		{
			string fileContents = null;
			using (FileStream fs = new FileStream(path, FileMode.Open))
			using (StreamReader reader = new StreamReader(fs))
			{
				fileContents = reader.ReadToEnd();
			}
			return new CSVFile(fileContents);
		}

		public static ImageSource image(string path)
		{
			var result = new ImageSource(path);
			using (var image = new MagickImage(path))
			{
				result.width = image.BaseWidth;
				result.height = image.BaseHeight;
			}
			return result;
		}
		public static void print(string message)
		{
			Console.WriteLine(message);
		}

		public static Color rgb(int r, int g, int b)
		{
			return new Color((byte)r, (byte)g, (byte)b, (byte)255);
		}

		public static Color argb(int a, int r, int g, int b)
		{
			return new Color((byte)r, (byte)g, (byte)b, (byte)a);
		}

		public static Color alpha(Color color, float alpha)
		{
			return new Color(color.R, color.G, color.B, (byte)(255 * alpha));
		}

		public static Matrix identityMatrix(int width, int height)
		{
			var result = new Matrix(width, height);
			for (int i = 0; i < Math.Min(width, height); i++)
			{
				result[i, i] = 1;
			}
			return result;
		}

		public static Matrix matrix(float[] values, int width, int height)
		{
			var result = new Matrix(width, height);
			var i = 0;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					result[x, y] = values[i];
					i++;
				}
			}
			return result;
		}

		public static ImportExpression<LibrarySymbol> lib(string path)
		{
			throw new NotSupportedException();
			//var lib = Loader.LoadFromFile(path, false, false);
			//if (lib == null)
			//	return null;
			//return new ImportExpression<LibrarySymbol>((LibrarySymbol)lib.Value);
		}

		public static ImportExpression<Font> gfont(string name)
		{
			var font = new Font(name);

			var tmp = Path.GetTempFileName();
			var href = $"https://fonts.googleapis.com/css?family={name}";
			var filename = @"c:\temp\montserrat.css";
			//var userAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";

			using (var client = new WebClient())
			{
				client.DownloadFile(href, tmp);
			}
			string contnts;
			using (FileStream fs = new FileStream(tmp, FileMode.Open))
			{
				using (StreamReader r = new StreamReader(fs))
				{
					contnts = r.ReadToEnd();
				}
			}
			var src = GtSrc(contnts);
			font.LoadFontFamily(src);
			return new ImportExpression<Font>(font, href);
		}

		private static string GtSrc(string url)
		{
			url=url.Substring(url.IndexOf("url("));
			url = url.Substring(url.IndexOf('('));
			url = url.Remove(url.IndexOf(')'));
			url = url.Trim('(', ')');
			var tmp = Path.Combine(Path.GetTempPath(), url.Split('/').Last());

			using (var client = new WebClient())
			{
				try
				{
					client.DownloadFile(url, tmp);
				}
				catch(Exception e)
				{
					Logger.LogConnectionProblems(url);
				}
				}
			return tmp;
		}

		public static void useStyle()
		{

		}

		public static void useGroup()
		{

		}

		public static void useData()
		{

		}
	}
}
