﻿namespace Slides.Elements
{
	public class Container : Element
	{
		public Element child { get; private set; }

		public override ElementKind kind => ElementKind.Container;

		public Container()
		{

		}

		public void fill(Element element)
		{
			child = element;
		}

		protected override Unit get_InitialHeight()
		{
			return child.height;
		}

		protected override Unit get_InitialWidth()
		{
			return child.width;
		}
	}
}