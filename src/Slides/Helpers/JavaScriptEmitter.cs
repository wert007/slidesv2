using Slides.Data;
using Slides.Elements;
using Slides.MathExpressions;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Slides.Helpers
{
	public static class JavaScriptEmitter
	{
		public class JavaScriptObject
		{
			public JavaScriptObject(string representation)
			{
				Representation = representation;
			}

			public override string ToString() => Representation;

			public string Representation { get; }
		}

		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");

		public static string ObjectToString(object value)
		{
			var writer = new StringWriter();
			writer.EmitObject(value);
			return writer.ToString();
		}

		public static void EmitObject(this TextWriter writer, object value)
		{
			switch (value)
			{
				//case null:
				//	writer.Write("undefined");
				//	break;
				case string s:
					writer.EmitString(s);
					break;
				case int i:
					writer.EmitInteger(i);
					break;
				case bool b:
					writer.EmitBoolean(b);
					break;
				case float f:
					writer.EmitFloat(f);
					break;
				case Color c:
					writer.EmitColor(c);
					break;
				case Range r:
					writer.EmitRange(r);
					break;
				case Element e:
					writer.EmitElement(e);
					break;
				case SlideAttributes s:
					writer.EmitSlideAttributes(s);
					break;
				case MathFormula m:
					writer.EmitMathFormula(m);
					break;
				case JavaScriptObject jsObj:
					writer.Write(jsObj);
					break;
				case Filter filter:
					writer.EmitSVGFilter(filter);
					break;
				case YouTubePlayerParameters parameters:
					writer.EmitYouTubePlayerParameters(parameters);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private static void EmitString(this TextWriter writer, string value)
		{
			writer.Write("'");
			writer.Write(value);
			writer.Write("'");
		}

		private static void EmitInteger(this TextWriter writer, int value)
		{
			writer.Write(value);
		}

		private static void EmitBoolean(this TextWriter writer, bool value)
		{
			writer.Write(value.ToString().ToLower());
		}

		private static void EmitFloat(this TextWriter writer, float value)
		{
			writer.Write(value.ToString(_usCulture));
		}

		private static void EmitColor(this TextWriter writer, Color value)
		{
			writer.Write("'");
			if (value.IsRGBA)
			{
				writer.Write("rgba(");
				writer.Write(value.R);
				writer.Write(", ");
				writer.Write(value.G);
				writer.Write(", ");
				writer.Write(value.B);
				writer.Write(", ");
				writer.EmitFloat(value.A / 255f);
			}
			else
			{
				writer.Write("hsla(");
				writer.Write(value.Hue);
				writer.Write(", ");
				writer.Write((int)Math.Round(value.Saturation / 2.55f));
				writer.Write("%, ");
				writer.Write((int)Math.Round(value.Lightness / 2.55f));
				writer.Write("%, ");
				writer.EmitFloat(value.A / 255f);
			}
			writer.Write("'");
		}

		private static void EmitRange(this TextWriter writer, Range value)
		{
			writer.Write("new NumberRange(");
			writer.EmitObject(value.From);
			writer.Write(", ");
			writer.EmitObject(value.To);
			writer.Write(", ");
			writer.EmitObject(value.Step);
			writer.Write(")");
		}

		private static void EmitElement(this TextWriter writer, Element value)
		{
			writer.Write("document.getElementById('");
			writer.Write(value.get_Id());
			writer.Write("')");
		}

		private static void EmitSlideAttributes(this TextWriter writer, SlideAttributes value)
		{
			writer.Write("document.getElementById('");
			writer.Write(value.name);
			writer.Write("')");
		}
		private static void EmitMathFormula(this TextWriter writer, MathFormula value)
		{
			writer.Write(value.Name);
			writer.Write("_scope");
		}

		//TODO: Incomplete!
		private static void EmitSVGFilter(this TextWriter writer, Filter filter)
		{
			writer.Write("'");
			if (!(filter is CustomFilter))
				writer.Write(filter.Name);
			else { } //Do something else
			writer.Write("(");
			switch (filter)
			{
				case PercentalFilter p:
					writer.EmitFloat(p.Value);
					break;
				default:
					break;
			}
			writer.Write(")'");
		}

		private static void EmitYouTubePlayerParameters(this TextWriter writer, YouTubePlayerParameters parameters)
		{
			int BoolToInt(bool b) { return b ? 1 : 0; }
			string ListTypeToString(YouTubePlayerParameters.ListType list)
			{
				switch (list)
				{
					case YouTubePlayerParameters.ListType.Playlist:
					case YouTubePlayerParameters.ListType.Search:
						return list.ToString();
					case YouTubePlayerParameters.ListType.UserUploads:
						return "user-uploads";
					default:
						throw new NotImplementedException();
				}
			}
			writer.WriteLine("{");
			var defaultValues = new YouTubePlayerParameters();
			var w = new StringWriter();
			if (parameters.autoplay != defaultValues.autoplay)
				w.WriteLine($"autoplay: {BoolToInt(parameters.autoplay)},");
			if (parameters.color != defaultValues.color)
				w.WriteLine($"color: '{parameters.color}',");
			if (parameters.controls != defaultValues.controls)
				w.WriteLine($"controls: '{parameters.controls.ToString().ToLower()}',");
			if (parameters.disablekb != defaultValues.disablekb)
				w.WriteLine($"disablekb: {BoolToInt(parameters.disablekb)},");
			if (parameters.enablejsapi != defaultValues.enablejsapi)
				w.WriteLine($"enablejsapi: {BoolToInt(parameters.enablejsapi)},");
			if (parameters.end != defaultValues.end)
				w.WriteLine($"end: {parameters.end},");
			if (parameters.fs != defaultValues.fs)
				w.WriteLine($"fs: {BoolToInt(parameters.fs)},");
			if (parameters.hl != defaultValues.hl)
				w.WriteLine($"hl: '{parameters.hl}',");
			if (parameters.iv_load_policy != defaultValues.iv_load_policy)
				w.WriteLine($"iv_load_policy: {(parameters.iv_load_policy ? 1 : 3)},");
			if (parameters.list != defaultValues.list)
				w.WriteLine($"list: '{parameters.list}',");
			if (parameters.listType != defaultValues.listType)
				w.WriteLine($"listType: {ListTypeToString(parameters.listType.Value)},");
			if (parameters.loop != defaultValues.loop)
				w.WriteLine($"loop: {BoolToInt(parameters.loop)},");
			if (parameters.modestbranding != defaultValues.modestbranding)
				w.WriteLine($"modestbranding: {BoolToInt(parameters.modestbranding)},");
			if (parameters.origin != defaultValues.origin)
				w.WriteLine($"origin: '{parameters.origin}',");
			if (parameters.playlist != defaultValues.playlist)
				w.WriteLine($"playlist: '{string.Join(",", parameters.playlist)}',");
			if (parameters.playsinline != defaultValues.playsinline)
				w.WriteLine($"playsinline: {BoolToInt(parameters.playsinline)},");
			if (parameters.rel != defaultValues.rel)
				w.WriteLine($"rel: {BoolToInt(parameters.rel)},");
			if (parameters.showinfo != defaultValues.showinfo)
				w.WriteLine($"showinfo: {BoolToInt(parameters.showinfo)},");
			if (parameters.start != defaultValues.start)
				w.WriteLine($"start: {parameters.start},");
			var sb = w.GetStringBuilder();
			writer.WriteLine(sb.ToString().Trim().TrimEnd(','));
			writer.WriteLine("}");
		}
	}
}
