using System;

namespace Slides.Filters
{
	public class CustomFilter : Filter
	{
		public CustomFilter(string name, SVGFilter[] filters, string[] filterNames) : base("url")
		{
			Filters = filters;
			FilterNames = filterNames;
			Id = name;
		}

		public SVGFilter[] Filters { get; }
		public string[] FilterNames { get; }
		public string Id { get; }

		public string GetName(IFilterInput inputFilter)
		{
			if(inputFilter is SourceGraphicFilterInput)
			{
				return "SourceGraphic";
			}
			var i = Array.IndexOf(Filters, inputFilter);
			if (i < 0)
				return null;
			return FilterNames[i];
		}
	}
	public abstract class Filter
	{
		protected Filter(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public override string ToString()
		{
			return $"{Name}()";
		}
	}

	public class FilterAddition : Filter
	{
		public FilterAddition(Filter a, Filter b) : base("_addition")
		{
			A = a;
			B = b;
		}

		public Filter A { get; }
		public Filter B { get; }
	}

	public class ProcentalFilter : Filter
	{
		public ProcentalFilter(string name, float value) : base(name)
		{
			Value = value;
		}

		public float Value { get; }

		public override string ToString()
		{
			return $"{Name}({Value})";
		}
	}
	public class BlurFilter : Filter
	{
		public BlurFilter(float value) : base("blur")
		{
			Value = value;
		}

		public float Value { get; }

		public override string ToString()
		{
			return $"{Name}({Value}px)";
		}
	}
	public class DropShadowFilter : Filter
	{
		public DropShadowFilter(int horizontal, int vertical, int blur, int spread, Color color) : base("drop-shadow")
		{
			Horizontal = horizontal;
			Vertical = vertical;
			Blur = blur;
			Spread = spread;
			Color = color;
		}

		public int Horizontal { get; }
		public int Vertical { get; }
		public int Blur { get; }
		public int Spread { get; }
		public Color Color { get; }
	}
	public class HueRotateFilter : Filter
	{
		public HueRotateFilter(float value) : base("hue-rotate")
		{
			Value = value;
		}

		public float Value { get; }
	}
}
