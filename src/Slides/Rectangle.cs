﻿namespace Slides
{
	public class Rectangle : Element
	{

		public Rectangle(int width, int height)
		{
			this.width = new Unit(width, Unit.UnitKind.Pixel);
			this.height = new Unit(height, Unit.UnitKind.Pixel);
		}
		public Rectangle(Unit width, Unit height)
		{
			this.width = width;
			this.height = height;
		}

		public Color fill { get { return background?.Color; } set { background = new Brush(value); } }

		public override ElementType type => ElementType.Rectangle;

		protected override Unit get_InitialHeight() => height;

		protected override Unit get_InitialWidth() => width;
	}
}
