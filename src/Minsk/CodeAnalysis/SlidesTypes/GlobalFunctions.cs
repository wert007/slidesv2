using ImageMagick;
using Minsk.CodeAnalysis.Symbols;
using SimpleLogger;
using Slides;
using Slides.Data;
using Slides.Elements;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Minsk.CodeAnalysis.SlidesTypes
{

	public static class GlobalFunctions
	{
		internal static ReferenceTracker Tracker { get; private set; }

		public static string join(string seperator, object[] array)
		{
			return string.Join(seperator, array);
		}
		public static string utf32(int codepoint)
		{
			var codepointBytes = BitConverter.GetBytes(codepoint);
			var text = Encoding.UTF32.GetString(codepointBytes);
			return text.First().ToString(); //Remove trailing '\0'
		}
		public static int[] utf32(string str)
		{
			var bytes = Encoding.UTF32.GetBytes(str);
			var result = new int[bytes.Length / 4];

			//Let's just hope this is right..
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = bytes[i * 4];
				result[i] |= bytes[i * 4 + 1] << 4;
				result[i] |= bytes[i * 4 + 2] << 8;
				result[i] |= bytes[i * 4 + 3] << 12;
			}
			return result;
		}
		public static bool contains(string hay, string needle) => hay.Contains(needle);
		//public static FormattedString fmt(string str) => FormattedString.FromString(str);
		public static string toTime(int time)
		{
			var result = new StringBuilder();
			if (time % 1000 != 0)
				result.Insert(0, $" {time % 1000}ms");
			time /= 1000;
			if (time % 60 != 0)
				result.Insert(0, $" {time % 60}s");
			time /= 60;
			if (time % 60 != 0)
				result.Insert(0, $" {time % 60}m");
			time /= 60;
			if (time % 24 != 0)
				result.Insert(0, $" {time % 24}h");
			time /= 24;
			if (time != 0)
				result.Insert(0, $"{time}d");
			return result.ToString().Trim();
		}

		public static string fixedWidth(object source, int length)
		{
			var result = source.ToString().Trim();
			if (result.Length > length)
			{
				result = $"{result.Remove(length - 3).Trim()}...";
			}
			else if (result.Length < length)
			{
				if (source.GetType().IsPrimitive && source.GetType() != typeof(bool))
					result = result.PadLeft(length, '0');
				else
					result = result.PadRight(length);
			}
			return result;
		}

		//TODO: Do this smarter
		public static float @float(int i)
		{
			return i;
		}
		public static int @int(float f)
		{
			return (int)f;
		}

		public static int mod(int a, int b) => a % b;

		public static int min(int a, int b)
		{
			if (a < b) return a;
			return b;
		}

		public static float min(float a, float b)
		{
			if (a < b) return a;
			return b;
		}
		public static int max(int a, int b)
		{
			if (a > b) return a;
			return b;
		}

		public static float max(float a, float b)
		{
			if (a > b) return a;
			return b;
		}

		public static Unit max(Unit a, Unit b)
		{
			return Unit.Max(a, b);
		}
		public static CSVFile csv(string fileName)
		{
			//TODO: This isn't needed for some reason..
			//var path = Path.Combine(CompilationFlags.Directory, fileName);
			var fileContents = File.ReadAllText(fileName);
			return new CSVFile(fileContents);
		}

		public static Range stepBy(Range r, int step)
		{
			return new Range(r.From, r.To, step);
		}

		public static VideoSource video(string fileName)
		{
			var result = new VideoSource(fileName);
			var path = Path.Combine(CompilationFlags.Directory, fileName);
			Tracker.Reference = fileName;
			var media = new FFmpeg.NET.MediaFile(path);
			//TODO! another way to find ffmpeg!
			var ffmpeg = new FFmpeg.NET.Engine(@"C:\ffmpeg-4.3.1-essentials_build\bin\ffmpeg.exe");
			var meta = ffmpeg.GetMetaDataAsync(media).Result;
			var data = meta?.VideoData?.FrameSize.Split('x') ?? new[] { "0", "0" };
			result.Width = int.Parse(data[0]);
			result.Height = int.Parse(data[1]);
			return result;
		}

		public static ImageSource image(string fileName)
		{
			var result = new ImageSource(fileName);
			var path = Path.Combine(CompilationFlags.Directory, fileName);
			Tracker.Reference = fileName;
			using (var image = new MagickImage(path))
			{
				result.width = image.BaseWidth;
				result.height = image.BaseHeight;
			}
			return result;
		}

		public static ImageSource crop(ImageSource source, Unit top, Unit right, Unit bottom, Unit left)
		{
			var path = source.h_Path;
			var y = top.h_ToPixel(source.height);
			var x = left.h_ToPixel(source.width);
			var width = (Unit.HundredPercent - right).h_ToPixel(source.width);
			var height = (Unit.HundredPercent - bottom).h_ToPixel(source.height);
			using (var image = new MagickImage(Path.Combine(CompilationFlags.Directory, path)))
			{
				path = Path.Combine(Path.GetDirectoryName(path), ".generated", Path.GetFileNameWithoutExtension(path) + "_resized" + Path.GetExtension(path));
				Directory.CreateDirectory(Path.Combine(CompilationFlags.Directory, Path.GetDirectoryName(path)));
				Tracker.Reference = path;
				image.Crop(new MagickGeometry(x, y, width, height));
				using (var fs = new FileStream(Path.Combine(CompilationFlags.Directory, path), FileMode.OpenOrCreate))
					image.Write(fs);
			}
			var result = new ImageSource(path);
			result.width = width;
			result.height = height;
			return result;
		}
		/*
		public static IFrame youtube(string video)
		{
			return youtube(video, false);
		}*/
		//TODO: Use a js construct for playing YT vids.
		/*public static IFrame youtube(string video, bool autoplay)
		{
			var src = $"https://www.youtube.com/embed/{video}";
			if (autoplay)
				src = src + "?autoplay=1&mute=1";
			return new IFrame(src, "accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture", "allowfullscreen");
		}
		*/
		public static YouTubePlayer youtube(string video, YouTubeQuality quality)
		{
			Evaluator.Flags.UsesYouTube = true;
			return new YouTubePlayer(video, quality);
		}

		public static void println() => Console.WriteLine();
		public static void println(string message) => Console.WriteLine(SldStringToCSharpStr(message));
		public static void print(string message) => Console.Write(SldStringToCSharpStr(message));
		private static string SldStringToCSharpStr(string str)
		{
			if (!str.Contains("\\")) return str;
			var resultBuilder = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				var cur = str[i];
				var nex = i + 1 < str.Length ? str[i + 1] : '\0';
				if (cur != '\\')
					resultBuilder.Append(cur);
				else
				{
					i++;
					switch (nex)
					{
						case 'n':
							resultBuilder.Append('\n');
							break;
						case 't':
							resultBuilder.Append('\t');
							break;
						default:
							resultBuilder.Append(cur);
							resultBuilder.Append(nex);
							break;
					}
				}
			}
			return resultBuilder.ToString();
		}

		public static Color hsl(int hue, int sat, int light)
		{
			return Color.FromHSLA(hue, (byte)sat, (byte)light, 255);
		}
		public static Color hsla(int hue, int sat, int light, int alpha)
		{
			return Color.FromHSLA(hue, (byte)sat, (byte)light, (byte)alpha);
		}

		public static Color hex(string hex)
		{
			hex = hex.Substring(1);
			var rStr = hex.Substring(0, 2);
			var gStr = hex.Substring(2, 2);
			var bStr = hex.Substring(4, 2);
			var r = (byte)int.Parse(rStr, NumberStyles.HexNumber);
			var g = (byte)int.Parse(gStr, NumberStyles.HexNumber);
			var b = (byte)int.Parse(bStr, NumberStyles.HexNumber);
			return new Color(r, g, b, 255);
		}

		public static Color rgb(int r, int g, int b)
		{
			return new Color((byte)r, (byte)g, (byte)b, (byte)255);
		}

		public static Color rgba(int r, int g, int b, int a)
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

		public static Unit px(float value) => new Unit(value, Unit.UnitKind.Pixel);
		public static Unit pt(float value) => new Unit(value, Unit.UnitKind.Point);
		public static Unit pct(float value) => new Unit(value, Unit.UnitKind.Percent);
		public static float @float(Unit value) => value.Value;

		public static BorderLine border(Unit width, BorderStyle style, Color color)
		{
			return new BorderLine(width, style, color);
		}

		public static ImportValue lib(string path) => throw new NotSupportedException();

		//public static Font gfont(string name)
		//{
		//	return _gfont(name).Value;
		//}

		public static ImportValue font(string name)
		{
			var font = new Font(name);
			return new ImportValue(font, ImportValueType.Font);
		}

		public static ImportValue gfont(string name)
		{
			var font = new Font(name);

			var tmp = Path.GetTempFileName();
			var href = $"https://fonts.googleapis.com/css?family={name}";

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
			return new ImportValue(font, ImportValueType.Font, href);
		}

		private static string GtSrc(string url)
		{
			url = url.Substring(url.IndexOf("url("));
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
				catch (Exception e)
				{
					Logger.Log($"Cannot connect to '{url}'.");
				}
			}
			return tmp;
		}

		internal static void Set_BackDump(ReferenceTracker tracker)
		{
			Tracker = tracker;
		}
	}
}
