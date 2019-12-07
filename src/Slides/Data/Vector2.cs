using System;
using System.Drawing;

namespace Slides
{
	[Serializable]
	public struct Vector2
	{
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
	}
}
