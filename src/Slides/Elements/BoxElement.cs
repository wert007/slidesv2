using System.Collections.Generic;

namespace Slides.Elements
{
	public class BoxElement : Element
	{
		public BoxElement(string typeName, Element[] children)
		{
			TypeName = typeName;
			Children = children;
		}

		public Element[] Children { get; }
		public string TypeName { get; }

		public Unit fontsize { get; set; }

		public override ElementKind kind => ElementKind.BoxElement;

		protected override Unit get_InitialHeight()
		{
			return initHeight;
		}

		protected override Unit get_InitialWidth()
		{
			return initWidth;
		}

		public override string ToString() => $"{TypeName} ({Children.Length})";
	}
}
