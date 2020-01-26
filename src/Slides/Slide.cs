﻿using System;
using System.Collections.Generic;

namespace Slides
{
	public class Template
	{
		public string Name { get; }
		public Element[] VisualChildren { get; }
		public object[] DataChildren { get; }
		public Template(string name, Element[] visualChildren, object[] dataChildren)
		{
			Name = name;
			VisualChildren = visualChildren;
			DataChildren = dataChildren;
		}
	}
	public class Step
	{
		public string Name { get; }
		public AnimationCall[] AnimationCalls { get; }
		public Element[] VisualChildren { get; }
		public object[] DataChildren { get; }
		public string[] DataChildrenNames { get; }

		public Step(string name, SlideAttributes dummy, AnimationCall[] animationCalls, Element[] visualChildren, object[] dataChildren, string[] dataChildrenNames)
		{
			Name = name;
			AnimationCalls = animationCalls;
			VisualChildren = visualChildren;
			DataChildren = dataChildren;
			DataChildrenNames = dataChildrenNames;
		}
	}

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
