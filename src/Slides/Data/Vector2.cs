using System;
using System.Drawing;

namespace Slides.Data
{
	[Serializable]
	public struct Vector2
	{
		public static readonly Vector2 NaN = new Vector2(float.NaN, float.NaN);

		public Vector2(SizeF value) : this()
		{
			X = value.Width;
			Y = value.Height;
		}

		public Vector2(float x, float y) : this()
		{
			X = x;
			Y = y;
		}

		public float X { get; set; }
		public float Y { get; set; }

		public override string ToString()
		{
			return $"{{ X: {X} Y: {Y} }}";
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Vector2 v))
				return false;
			return v.X == X && v.Y == Y;
		}

		public static bool operator ==(Vector2 a, Vector2 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Vector2 a, Vector2 b)
		{
			return !a.Equals(b);
		}

		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X + b.X, a.Y + b.Y);
		}
		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X - b.X, a.Y - b.Y);
		}

		public static float operator *(Vector2 a, Vector2 b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		public double Length()
		{
			return Math.Sqrt(this * this);
		}

		public static bool IsNaN(Vector2 v)
		{
			return float.IsNaN(v.X) || float.IsNaN(v.Y);
		}
	}
}
