using Slides.Data;
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
			child.h_Parent = this;
			child.orientation = Orientation.Stretch;
		}

		internal override Unit get_InitialHeight()
		{
			if (child == null) return new Unit();
			return child.get_InitialHeight();
		}

		internal override Unit get_InitialWidth()
		{
			if (child == null) return new Unit();
			return child.get_InitialWidth();
		}

		protected override IEnumerable<Element> get_Children()
		{
			if(child != null)
				yield return child;
		}
	}
}