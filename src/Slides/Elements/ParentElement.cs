using SimpleLogger;
using Slides.Styling;
using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public abstract class ParentElement : TextElement
	{
		protected abstract IEnumerable<Element> get_Children();
		protected Dictionary<string, Element> _namedChildren = new Dictionary<string, Element>();
		protected virtual bool _shouldApplyStyleToChildren { get; } = false;
		protected override void HandleApplyStyle(Style style)
		{
			foreach (var substyle in style.Substyles.GetIterator())
			{
				if (substyle.Selector.Child != null)
				{
					var name = substyle.Selector.Child.Name;
					if (!_namedChildren.ContainsKey(name))
					{
						Logger.Log($"Could not find a children named '{name}' for {GetType().Name} '{get_Id()}'.");
						continue;
					}
					var element = _namedChildren[name];
					element.applyStyle(substyle);
				}
			}
			if(_shouldApplyStyleToChildren)
				foreach (var child in get_Children())
					child.applyStyle(style);
		}
	}
}
