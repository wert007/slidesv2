using Slides.Styling;
using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public abstract class ParentElement : TextElement
	{
		protected abstract IEnumerable<Element> get_Children();
		protected Dictionary<string, Element> _namedChildren = new Dictionary<string, Element>();

		protected override void handleApplyStyle(Style style)
		{
			foreach (var substyle in style.Substyles.GetIterator())
			{
				if (substyle.Selector.Child != null)
				{
					var name = substyle.Selector.Child.Name;
					if (!_namedChildren.ContainsKey(name))
					{
						Console.WriteLine($"WARNING: Could not find a children named '{name}' for Element '{get_Id()}'.");
						continue;
					}
					var element = _namedChildren[name];
					element.applyStyle(substyle);
				}
			}
			foreach (var child in get_Children())
			{
				child.applyStyle(style);
			}
		}
	}
}
