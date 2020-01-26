using System;

namespace Slides
{
	[Serializable]
	public class Thickness
	{
		public Unit left { get; set; }
		public Unit top { get; set; }
		public Unit right { get; set; }
		public Unit bottom { get; set; }
		public Unit Vertical => top + bottom;
		public Unit Horizontal => left + right;

		public Thickness(Unit left, Unit top, Unit right, Unit bottom)
		{
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
		}

		public Thickness() :this(new Unit(), new Unit(), new Unit(), new Unit())
		{

		}

		public override string ToString()
		{
			return $"{top} {right} {bottom} {left}";
		}

		public static Thickness operator + (Thickness a, Thickness b)
		{
			return new Thickness(a.left + b.left, a.top + b.top, a.right + b.right, a.bottom + b.bottom);
		}
	}
}
