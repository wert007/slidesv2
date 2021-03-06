﻿using Slides.Data;
using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public class SplittedContainer : ParentElement
	{
		public Container childA { get; private set; }
		public Container childB { get; private set; }
		public FlowAxis flow { get; }
		public override ElementKind kind => ElementKind.SplittedContainer;

		public SplittedContainer(FlowAxis flow)
		{
			this.flow = flow;
			childA = new Container();
			childB = new Container();
		}

		public void fill(Element elementA, Element elementB)
		{
			childA.fill(elementA);
			childB.fill(elementB);
		}

		public void fillA(Element element)
		{
			childA.fill(element);
		}
		public void fillB(Element element)
		{
			childB.fill(element);
		}

		internal override Unit get_InitialHeight()
		{
			switch (flow)
			{
				case FlowAxis.Horizontal:
					return childA.height; //TODO: Still no way to compare units..
				case FlowAxis.Vertical:
					return childA.height + childB.height;
				default:
					throw new NotImplementedException();
			}
		}

		internal override Unit get_InitialWidth()
		{
			switch (flow)
			{
				case FlowAxis.Horizontal:
					return childA.width + childB.width;
				case FlowAxis.Vertical:
					return childA.width; //TODO: Still no way to compare units..
				default:
					throw new NotImplementedException();
			}
		}

		//Maybe we should return the containers themself here.
		//But a container in a container? That's a lil bit strange..
		protected override IEnumerable<Element> get_Children()
		{
			if(childA != null)
				yield return childA.child;
			if(childB != null)
				yield return childB.child;
		}
	}
}
