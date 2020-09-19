using System;
using System.Collections.Generic;
using System.Globalization;

namespace Slides.Data
{
	[Serializable]
	public class Unit
	{
		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");
		public static readonly Unit HundredPercent = new Unit(100, UnitKind.Percent);
		internal static readonly Unit Thin   = new Unit(1, UnitKind.Pixel);
		internal static readonly Unit Medium = new Unit(3, UnitKind.Pixel);
		internal static readonly Unit Thick  = new Unit(5, UnitKind.Pixel);

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
			HorizontalPercent,
			VerticalPercent,
		}

		public Unit(float value, UnitKind kind)
		{
			Value = value;
			if (float.IsInfinity(value)) 
				throw new Exception();
			Kind = kind;
		}

		public Unit()
		{
			Value = 0;
			Kind = UnitKind.Pixel;
		}

		public Unit(Unit unit)
		{
			Value = unit.Value;
			Kind = unit.Kind;
		}

		public override string ToString()
		{
			if (Kind == UnitKind.Auto)
				return "auto";
			return $"{Value.ToString(_usCulture)}{ToString(Kind)}";
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
				case UnitKind.HorizontalPercent:
					return "vw";
				case UnitKind.VerticalPercent:
					return "vh";
				default:
					return kind.ToString();
			}
		}

		public bool IsRelative()
		{
			switch (Kind)
			{
				case UnitKind.Point:
				case UnitKind.Pixel:

				case UnitKind.Addition:
				case UnitKind.Subtraction:
					return false;
				case UnitKind.Percent:
				case UnitKind.Auto:
				case UnitKind.CharacterWidth:
				case UnitKind.HorizontalPercent:
				case UnitKind.VerticalPercent:
					return true;
				default:
					throw new NotImplementedException();
			}
		}

		internal virtual Unit GetMaxComponent() => this;

		public static Unit operator +(Unit a, Unit b)
		{
			if (a is UnitAddition add) 
				return add.Add(b);
			if (a is UnitSubtraction sub) 
				return sub.Add(b);
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
			if (a is UnitAddition add)
				return add.Subtract(b);
			if (a is UnitSubtraction sub)
				return sub.Subtract(b);
			if (a.Kind == b.Kind && a.Kind != UnitKind.Subtraction && a.Kind != UnitKind.Addition)
				return new Unit(a.Value - b.Value, a.Kind);
			if (a.Value == 0 && a.Kind != UnitKind.Auto)
				return b;
			if (b.Value == 0 && b.Kind != UnitKind.Auto)
				return a;
			return new UnitSubtraction(a, b);
		}

		public static Unit Max(Unit a, Unit b)
		{
			if (a.Kind == b.Kind && a.Kind != UnitKind.Addition && a.Kind != UnitKind.Subtraction) return a.Value > b.Value ? a : b;
			var maxA = a.GetMaxComponent();
			while (maxA.Kind == UnitKind.Addition || maxA.Kind == UnitKind.Subtraction)
				maxA = maxA.GetMaxComponent();
			var maxB = b.GetMaxComponent();
			while (maxB.Kind == UnitKind.Addition || maxB.Kind == UnitKind.Subtraction)
				maxB = maxB.GetMaxComponent();
			if (maxA.IsRelative() && !maxB.IsRelative()) return a;
			if (!maxA.IsRelative() && maxB.IsRelative()) return b;
			if (maxA.Kind == maxB.Kind) return maxA.Value > maxB.Value ? a : b;
			throw new Exception();
		}

		public virtual int h_ToPixel(int referenceSize)
		{
			switch (Kind)
			{
				case UnitKind.VerticalPercent:
				case UnitKind.Percent:
				case UnitKind.HorizontalPercent:
					return (int)Math.Round(referenceSize * Value * 0.01f);
				case UnitKind.Pixel:
					return (int)Math.Round(Value);
				case UnitKind.Subtraction:
				case UnitKind.Addition:
				case UnitKind.CharacterWidth:
				case UnitKind.Point:
				case UnitKind.Auto:
				default:
					throw new Exception();
			}
		}

		public static Unit operator *(float a, Unit b)
		{
			if (b.Kind == UnitKind.Subtraction)
				return ((UnitSubtraction)b) * a;
			if (b.Kind == UnitKind.Addition)
				return ((UnitAddition)b) * a;
			return new Unit(b.Value * a, b.Kind);
		}

		public static Unit operator *(Unit a, float b) => b * a;

		public static Unit operator /(Unit a, float b)
		{
			if (a.Kind == UnitKind.Subtraction)
				return ((UnitSubtraction)a) / b;
			if (a.Kind == UnitKind.Addition)
				return ((UnitAddition)a) / b;
			return new Unit(a.Value / b, a.Kind);
		}

		public static bool operator ==(Unit left, Unit right)
		{
			return EqualityComparer<Unit>.Default.Equals(left, right);
		}

		public static bool operator !=(Unit left, Unit right)
		{
			return !(left == right);
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
				case "vw":
					return UnitKind.HorizontalPercent;
				case "vh":
					return UnitKind.VerticalPercent;
				default:
					return null;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Unit unit &&
					 Value == unit.Value &&
					                     //Not always true. Like auto
					 (Kind == unit.Kind || Value == 0);
		}
	}
}
