using System;
using System.Collections.Generic;

namespace Slides
{
	public class Slide
	{
		public string Name { get; }
		public SlideAttributes Attributes { get; }
		public Step[] Steps { get; }
		public Template Parent { get; }

		public Slide(SlideAttributes attributes, Step[] steps, Template parent)
		{
			Name = attributes.name;
			Attributes = attributes;
			Steps = steps;
			Parent = parent;
		}
	}
}
