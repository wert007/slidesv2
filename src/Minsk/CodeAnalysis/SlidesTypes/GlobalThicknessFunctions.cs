﻿using Slides;
using Slides.Data;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	public static class GlobalThicknessFunctions
	{
		public static Unit toVertical(Unit value)
		{
			if (value.Kind == Unit.UnitKind.Percent)
				return new Unit(value.Value, Unit.UnitKind.VerticalPercent);
			return value;
		}
		public static Unit toHorizontal(Unit value)
		{
			if (value.Kind == Unit.UnitKind.Percent)
				return new Unit(value.Value, Unit.UnitKind.HorizontalPercent);
			return value;
		}
		public static Thickness padding(Unit value)
		{
			return new Thickness(value, value, value, value);
		}
		public static Thickness margin(Unit value)
		{
			return new Thickness(value, value, value, value);
		}
		public static Thickness padding(Unit vertical, Unit horizontal)
		{
			return new Thickness(horizontal, vertical, horizontal, vertical);
		}
		public static Thickness margin(Unit vertical, Unit horizontal)
		{
			return new Thickness(horizontal, vertical, horizontal, vertical);
		}
		public static Thickness padding(Unit top, Unit right, Unit bottom, Unit left)
		{
			return new Thickness(left, top, right, bottom);
		}
		public static Thickness margin(Unit top, Unit right, Unit bottom, Unit left)
		{
			return new Thickness(left, top, right, bottom);
		}

	}
}
