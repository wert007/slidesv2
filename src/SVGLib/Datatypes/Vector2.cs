using System;

namespace SVGLib.Datatypes
{
	public struct Vector2
	{
		public Vector2(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; }
		public double Y { get; }

		public static readonly Vector2 NaN = new Vector2(double.NaN, double.NaN);

		public double Length()
		{
			return Math.Sqrt(X * X + Y * Y);
		}

		public override string ToString()
		{
			return $"{{ X: {X} Y: {Y} }}";
		}
		public override bool Equals(object obj)
		{
			return obj is Vector2 vector &&
					 X == vector.X &&
					 Y == vector.Y;
		}

		public static bool operator ==(Vector2 left, Vector2 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector2 left, Vector2 right)
		{
			return !(left == right);
		}

		public static bool IsNaN(Vector2 vector)
		{
			return double.IsNaN(vector.X) && double.IsNaN(vector.Y);
		}

		public static Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X - b.X, a.Y - b.Y);
		}
		public static Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X + b.X, a.Y + b.Y);
		}
		public static Vector2 operator *(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X * b.X, a.Y * b.Y);
		}
		public static Vector2 operator /(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X / b.X, a.Y / b.Y);
		}

		public static Vector2 operator %(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X % b.X, a.Y % b.Y);
		}
	}
}
