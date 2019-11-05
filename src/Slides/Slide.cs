using System.Collections.Generic;

namespace Slides
{
	public class Step
	{
		public string Name { get; }
		public AnimationCall[] AnimationCalls { get; }
		public Element[] VisualChildren { get; }
		public object[] DataChildren { get; }

		public Step(string name, SlideAttributes dummy, AnimationCall[] animationCalls, Element[] visualChildren, object[] dataChildren)
		{
			Name = name;
			AnimationCalls = animationCalls;
			VisualChildren = visualChildren;
			DataChildren = dataChildren;
		}
	}

	public class Slide
	{
		public string Name { get; }
		public SlideAttributes Attributes { get; }
		public Step[] Steps { get; }

		public Slide(SlideAttributes attributes, Step[] steps)
		{
			Name = attributes.name;
			Attributes = attributes;
			Steps = steps;
		}
	}
}
