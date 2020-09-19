﻿using Slides.Data;
using Slides.Helpers;
using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public class Stack : Element
	{
		private List<Element> _children = new List<Element>();
		private Orientation _orientation;

		public Element[] children => _children.ToArray();
		public FlowAxis StackFlow { get; }
		public override ElementKind kind => ElementKind.Stack;
		public new Orientation orientation
		{
			get => _orientation;
			set
			{
				_orientation = value;
				switch (StackFlow)
				{
					case FlowAxis.Horizontal:
						foreach (var c in _children)
							c.orientation = SlidesHelper.AddOrientations(SlidesHelper.GetHorizontal(c.orientation), SlidesHelper.GetVertical(_orientation));
						break;
					case FlowAxis.Vertical:
						foreach (var c in _children)
							c.orientation = SlidesHelper.AddOrientations(SlidesHelper.GetHorizontal(_orientation), SlidesHelper.GetVertical(c.orientation));
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}

		public Stack(FlowAxis orientation)
		{
			StackFlow = orientation;
		}


		public void add(Element child)
		{
			_children.Add(child);
		}

		internal override Unit get_InitialWidth()
		{
			if (children.Length == 0) return new Unit();
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

		internal override Unit get_InitialHeight()
		{
			if (children.Length == 0) return new Unit();
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
