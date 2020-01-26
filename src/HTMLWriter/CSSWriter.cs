﻿using Slides;
using Slides.Debug;
using Slides.Filters;
using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.IO;
using System.Text;

namespace HTMLWriter
{
	public class CSSWriter : IDisposable
	{
		private IndentedTextWriter _writer;
		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");

		public CSSWriter(Stream stream)
		{
			_writer = new IndentedTextWriter(new StreamWriter(stream));
		}

		internal void WriteLine()
		{
			_writer.WriteLine();
		}

		internal void StartClass(string name, string pseudoClass = null)
		{
			if (name == null)
				throw new ArgumentNullException();
			StartSelector($".{name}", pseudoClass);
		}

		internal void StartId(string name, string pseudoClass = null)
		{
			if (name == null)
				throw new ArgumentNullException();
			StartSelector($"#{name}", pseudoClass);
		}

		internal void StartSelector(string selector, string pseudoClass = null)
		{
			if (selector == null)
				throw new ArgumentNullException();
			string postfix = "";
			if(pseudoClass != null)
			{
				postfix = $":{pseudoClass}";
			}
			_writer.WriteLine($"{selector}{postfix} {{");
			_writer.Indent++;
		}


		internal void EndClass()
		{
			EndSelector();
		}

		internal void EndSelector()
		{
			_writer.Indent--;
			_writer.WriteLine($"}}");
			_writer.WriteLine();
		}

		public void EndId()
		{
			EndSelector();
		}

		public void WriteAttributeIfValue(string name, object value)
		{
			if (value != null)
				WriteAttribute(name, value);
		}

		internal void WriteAttribute(string name, object value)
		{
			_writer.Write($"{name}: ");
			WriteValue(value);
			_writer.WriteLine(";");
		}

		public void WriteValue(object value)
		{
			switch (value)
			{
				case Color c:
					_writer.Write(ToString(c));
					break;
				case Alignment a:
					WriteAlignment(a);
					break;
				case ImageSource i:
					_writer.Write($"url(\"{i.Path.Replace('\\', '/')}\")");
					break;
				case ImageStretching stretching:
					WriteImageStretching(stretching);
					break;
				case UnitAddition unitAddition:
					_writer.Write($"calc({unitAddition})");
					break;
				case Unit unit:
					WriteUnit(unit);
					break;
				case Font font:
					_writer.Write($"'{font.name}'");
					break;
				case Time time:
					_writer.Write($"{time.toMilliseconds()}ms");
					break;
				case Filter filter:
					WriteFilter(filter);
					break;
				default:
					_writer.Write(value.ToString());
					break;
			}
		}

		private static string ToString(Color c)
		{
			if(c.IsRGBA)
				return $"rgba({c.R}, {c.G}, {c.B}, {ToString(c.A / 255f)})";
			return $"hsla({c.Hue}, {ToString(c.Saturation / 2.55f)}%, {ToString(c.Lightness / 2.55f)}%, {ToString(c.A / 255f)})";
		}

		private static string ToString(float f)
		{
			return f.ToString("0.00", _usCulture);
		}

		private void WriteFilter(Filter filter)
		{
			string parameters = "";
			switch (filter)
			{
				case BlurFilter blur:
					parameters = $"{ToString(blur.Value)}px";
					break;
				case ProcentalFilter procental:
					parameters = ToString(procental.Value);
					break;
				case HueRotateFilter hueRotate:
					parameters = $"{ToString(hueRotate.Value)}deg";
					break;
				case DropShadowFilter dropShadow:
					parameters = $"{dropShadow.Horizontal}px {dropShadow.Vertical}px {dropShadow.Blur}px {dropShadow.Spread}px {ToString(dropShadow.Color)}";
					break;
				case CustomFilter customFilter:
					parameters = $"#{customFilter.Id}";
					break;
				case FilterAddition addition:
					WriteFilter(addition.A);
					_writer.Write(" ");
					WriteFilter(addition.B);
					return;
				default:
					Logger.LogUnknownFilter(filter.GetType(), filter.Name);
					return;
			}

			_writer.Write($"{filter.Name}({parameters})");
		}

		private void WriteUnit(Unit unit)
		{
			var unitValue = unit.Value;
			if (unit.Kind == Unit.UnitKind.Point)
			{
				unitValue *= 1.5f;
			}
			if (unit.Kind == Unit.UnitKind.Auto)
			{
				_writer.Write("auto");
			}
			else
			{
				_writer.Write($"{ToString(unitValue)}{Unit.ToString(unit.Kind)}");
			}
		}

		private void WriteAlignment(Alignment a)
		{
			switch (a)
			{
				case Alignment.Left:
					_writer.Write("left");
					break;
				case Alignment.Center:
					_writer.Write("center");
					break;
				case Alignment.Right:
					_writer.Write("right");
					break;
				case Alignment.Block:
					_writer.Write("justify");
					break;
				default:
					throw new Exception();
			}
		}

		private void WriteImageStretching(ImageStretching stretching)
		{
			switch (stretching)
			{
				case ImageStretching.Stretch:
					_writer.Write("100% 100%");
					break;
				case ImageStretching.Contain:
					_writer.Write("contain");
					break;
				case ImageStretching.Cover:
					_writer.Write("cover");
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public void Dispose()
		{
			_writer.Flush();
			_writer.Dispose();
		}

		public void WriteAlternateThickness(string field, Thickness value)
		{
			WriteAttribute(field, GetAlternativeThickness(value));
		}

		public string GetAlternativeThickness(Thickness value)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append($"{GetAlternativeUnit(value.top, true)} {GetAlternativeUnit(value.right, false)} {GetAlternativeUnit(value.bottom, true)} {GetAlternativeUnit(value.left, false)}");
			return stringBuilder.ToString();
		}

		private string GetAlternativeUnit(Unit value, bool isVertical)
		{
			if (value.Kind != Unit.UnitKind.Percent)
				return value.ToString();
			var postfix = "vw";
			if (isVertical)
				postfix = "vh";
			return $"{value.Value}{postfix}";
		}

		public static string GetValue(object value)
		{
			switch (value)
			{
				case Color c:
					return ToString(c);
				case Alignment a:
					//	return ToString(a);
					throw new NotImplementedException();
				case ImageSource i:
					return $"url(\"{i.Path.Replace('\\', '/')}\")";
				case UnitAddition unitAddition:
					return $"calc({unitAddition})";
				case Unit unit:
					return unit.ToString();
				case Font font:
					return $"'{font.name}'";
				case Time time:
					return $"{time.toMilliseconds()}ms";
				case Filter filter:
					throw new NotImplementedException();
				//return ToString(filter);
				default:
					return value.ToString();
			}
		}
	}
}
