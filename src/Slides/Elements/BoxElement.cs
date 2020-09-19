using Slides.Data;
using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public class BoxElement : ParentElement
	{
		public BoxElement(string typeName, Element[] children)
		{
			TypeName = typeName;
			BoxChildren = children;
		}

		public Element[] BoxChildren { get; }
		public string TypeName { get; }


		public override ElementKind kind => ElementKind.BoxElement;

		internal override Unit get_InitialHeight() => throw new Exception();

		internal override Unit get_InitialWidth() => throw new Exception();

		public override string ToString() => $"{TypeName} ({BoxChildren.Length})";

		protected override IEnumerable<Element> get_Children() => BoxChildren;
	}
}
