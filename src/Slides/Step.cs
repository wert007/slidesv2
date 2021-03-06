﻿using Slides.Elements;

namespace Slides
{
	public class Step
	{
		public string Name { get; }
		public int ID { get; }
		public AnimationCall[] AnimationCalls { get; private set; }
		public Element[] VisualChildren { get; private set; }
		public object[] DataChildren { get; private set; }
		public string[] DataChildrenNames { get; private set; }
		public string ParentName { get; }

		private static int nextID = 0;

		public Step(string name, SlideAttributes dummy)
		{
			Name = name;
			ParentName = dummy.name;
			ID = nextID++;
		}

		public void Finalize(AnimationCall[] animationCalls, Element[] visualChildren, object[] dataChildren, string[] dataChildrenNames)
		{
			AnimationCalls = animationCalls;
			VisualChildren = visualChildren;
			DataChildren = dataChildren;
			DataChildrenNames = dataChildrenNames;
		}

		public string get_Id()
		{
			if (Name != null) return $"{ParentName}-{Name}";
			return ParentName;
		}
	}
}
