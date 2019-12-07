using System;
using System.Globalization;

namespace Slides
{
	[Serializable]
	public class Time
	{
		public float Value { get; }
		public TimeUnit Unit { get; }

		public enum TimeUnit
		{
			Milliseconds,
			Seconds,
			Minutes,
			Hours,
		}

		public Time(float value, TimeUnit unit)
		{
			Value = value;
			Unit = unit;
		}

		public int ToMilliseconds()
		{
			var result = Value;
			switch (Unit)
			{
				case TimeUnit.Milliseconds:
					break;
				case TimeUnit.Seconds:
					result *= 1000;
					break;
				case TimeUnit.Minutes:
					result *= 60000;
					break;
				case TimeUnit.Hours:
					result *= 360000;
					break;
				default:
					throw new Exception();
			}
			return (int)Math.Round(result);
		}

		public override string ToString()
		{
			return Value.ToString() + ToString(Unit);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return Value.ToString(formatProvider) + ToString(Unit);
		}

		public static string ToString(TimeUnit unit)
		{
			switch (unit)
			{
				case TimeUnit.Milliseconds:
					return "ms";
				case TimeUnit.Seconds:
					return "s";
				case TimeUnit.Minutes:
					return "m";
				case TimeUnit.Hours:
					return "h";
				default:
					throw new NotImplementedException();
			}
		}

		public static bool TryParse(string text, out Time value)
		{
			return TryParse(text, CultureInfo.CurrentCulture, out value);
		}
		public static bool TryParse(string text, IFormatProvider formatProvider, out Time value)
		{
			value = null;
			var split = text.Length - 1;
			while (char.IsLetter(text[split]))
				split--;

			var unit = ToTimeUnit(text.Substring(split));
			if (unit == null)
				return false;
			if (float.TryParse(text.Remove(split), NumberStyles.Float, formatProvider, out float f))
			{
				value = new Time(f, unit.Value);
				return true;
			}
			return false;
		}

		private static TimeUnit? ToTimeUnit(string text)
		{
			switch (text)
			{
				case "ms":
					return TimeUnit.Milliseconds;
				case "s":
					return TimeUnit.Seconds;
				case "m":
					return TimeUnit.Minutes;
				case "h":
					return TimeUnit.Hours;
				default:
					return null;
			}
		}
	}
}
