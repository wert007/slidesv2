using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public class Stack : Element
	{
		private List<Element> _children = new List<Element>();

		public Element[] children => _children.ToArray();
		public FlowAxis StackFlow { get; }
		public override ElementKind kind => ElementKind.Stack;


		public Stack(FlowAxis orientation)
		{
			StackFlow = orientation;
		}


		public void add(Element child)
		{
			_children.Add(child);
		}

		protected override Unit get_InitialWidth()
		{
			switch (StackFlow)
			{
				case FlowAxis.Horizontal:
					var result = children[0].width;
					for (int i = 1; i < children.Length; i++)
					{
						result += children[i].width;
					}
					return result;
				case FlowAxis.Vertical:

					var max = children[0].width;
					for (int i = 1; i < children.Length; i++)
					{
						var current = children[i].width;
						if (current.Value > max.Value) //TODO(Essential): Add a comparison for units
						{
							max = current;
						}
					}
					return max;
				default:
					throw new Exception();
			}
		}

		protected override Unit get_InitialHeight()
		{
			switch (StackFlow)
			{
				case FlowAxis.Vertical:
					var result = children[0].height;
					for (int i = 1; i < children.Length; i++)
					{
						result += children[i].height;
					}
					return result;
				case FlowAxis.Horizontal:
					var max = children[0].height;
					for (int i = 1; i < children.Length; i++)
					{
						var current = children[i].height;
						if (current.Value > max.Value) //TODO(Essential): Add a comparison for units
						{
							max = current;
						}
					}
					return max;
				default:
					throw new Exception();
			}
		}

	}
}
