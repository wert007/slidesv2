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
	}
}
