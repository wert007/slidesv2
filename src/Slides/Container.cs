using System;

namespace Slides
{
	public class SplittedContainer : Element
	{
		public Container childA { get; private set; }
		public Container childB { get; private set; }
		public FlowAxis flow { get; }
		public override ElementType type => ElementType.SplittedContainer;

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

		protected override Unit get_InitialHeight()
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

		protected override Unit get_InitialWidth()
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
	}

	public class Container : Element
	{
		public Element child { get; private set; }

		public override ElementType type => ElementType.Container;

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