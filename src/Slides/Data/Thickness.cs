using System;

namespace Slides
{
	[Serializable]
	public class Thickness
	{
		public Unit Left { get; set; }
		public Unit Top { get; set; }
		public Unit Right { get; set; }
		public Unit Bottom { get; set; }
		public Unit Vertical => Top + Bottom;
		public Unit Horizontal => Left + Right;

		public Thickness(Unit left, Unit top, Unit right, Unit bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public Thickness() :this(new Unit(), new Unit(), new Unit(), new Unit())
		{

		}

		public override string ToString()
		{
			return $"{Top} {Right} {Bottom} {Left}";
		}

		public static Thickness operator + (Thickness a, Thickness b)
		{
			return new Thickness(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);
		}
	}
}
