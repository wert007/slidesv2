using System.Collections.Generic;

namespace Slides.Elements
{
	public class Container : ParentElement
	{
		public Element child { get; private set; }

		public override ElementKind kind => ElementKind.Container;

		public Container()
		{

		}

		public void fill(Element element)
		{
			child = element;
			child.orientation = Orientation.Stretch;
		}

		internal override Unit get_InitialHeight()
		{
			return child.get_InitialHeight();
		}

		internal override Unit get_InitialWidth()
		{
			return child.get_InitialWidth();
		}

		protected override IEnumerable<Element> get_Children()
		{
			yield return child;
		}
	}
}