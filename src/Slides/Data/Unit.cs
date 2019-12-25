using System;
using System.Globalization;

namespace Slides
{
	[Serializable]
	public class Unit
	{
		public float Value { get; }
		public UnitKind Kind { get; }

		public enum UnitKind
		{
			Point,
			Percent,
			Pixel,
			Auto,
			Addition,
			CharacterWidth,
			Subtraction,
		}

		public Unit(float value, UnitKind kind)
		{
			Value = value;
			Kind = kind;
		}

		public Unit()
		{
			Value = 0;
			Kind = UnitKind.Point;
		}

		public override string ToString()
		{
			if (Kind == UnitKind.Auto)
				return "auto";
			return $"{Value}{ToString(Kind)}";
		}

		public string ToString(IFormatProvider formatProvider)
		{
			if (Kind == UnitKind.Auto)
				return "auto";
			return $"{Value.ToString(formatProvider)}{ToString(Kind)}";
		}

		public static string ToString(UnitKind kind)
		{
			switch (kind)
			{
				case UnitKind.Point:
					return "pt";
				case UnitKind.Percent:
					return "%";
				case UnitKind.Pixel:
					return "px";
				case UnitKind.Auto:
					return "auto";
				case UnitKind.CharacterWidth:
					return "ch";
				default:
					return kind.ToString();
			}
		}

		public static Unit operator +(Unit a, Unit b)
		{
			if (a.Kind == b.Kind)
				return new Unit(a.Value + b.Value, a.Kind);
			if (a.Value == 0 && a.Kind != UnitKind.Auto)
				return b;
			if (b.Value == 0 && b.Kind != UnitKind.Auto)
				return a;
			return new UnitAddition(a, b);
		}

		public static Unit operator -(Unit a, Unit b)
		{
			if (a.Kind == b.Kind)
				return new Unit(a.Value - b.Value, a.Kind);
			if (a.Value == 0 && a.Kind != UnitKind.Auto)
				return b;
			if (b.Value == 0 && b.Kind != UnitKind.Auto)
				return a;
			return new UnitSubtraction(a, b);
		}

		public static bool TryParse(string text, IFormatProvider formatProvider, out Unit unitResult)
		{
			unitResult = null;
			if (text == "auto")
			{
				unitResult = new Unit(0, UnitKind.Auto);
				return true;
			}
			var split = text.Length;
			while (text[split - 1] == '%' ||
				char.IsLetter(text[split - 1]))
			{
				split--;
			}
			var kind = ToUnitKind(text.Substring(split));
			if (kind == null)
				return false;
			if (float.TryParse(text.Remove(split), NumberStyles.Float, formatProvider, out var value))
			{
				unitResult = new Unit(value, kind.Value);
				return true;
			}
			return false;

		}

		public static bool TryParse(string text, out Unit unitResult)
		{
			return TryParse(text, CultureInfo.CurrentCulture, out unitResult);
		}

		private static UnitKind? ToUnitKind(string v)
		{
			switch (v)
			{
				case "pt":
					return UnitKind.Point;
				case "%":
					return UnitKind.Percent;
				case "px":
					return UnitKind.Pixel;
				case "auto":
					return UnitKind.Auto;

				case "ch":
					return UnitKind.CharacterWidth;
				default:
					return null;
			}
		}

		public static Unit Convert(object value)
		{
			if (value is Unit u)
				return u;
			if (value is float f)
				return new Unit(f * 100, UnitKind.Percent);
			if (value is int i)
				return new Unit(i, UnitKind.Pixel);
			return new Unit(-1, UnitKind.Auto);
		}
	}
}
