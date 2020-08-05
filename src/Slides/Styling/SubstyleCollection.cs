using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides.Styling
{
	public class SubstyleCollection
	{
		private readonly Dictionary<string, Substyle> _substyles = new Dictionary<string, Substyle>();

		public int Count => _substyles.Count;

		public void AddSubstyle(Substyle style)
		{
			_substyles.Add(style.Selector.ToString(), style);
		}

		public void AddProperty(Selector selector, string name, object value)
		{
			var selectorName = selector.ToString();
			if (!_substyles.ContainsKey(selectorName))
				_substyles.Add(selectorName, new Substyle(selector));
			_substyles[selectorName].Add(name, value);
		}

		public Substyle GetByType(string type)
		{
			foreach (var substyle in _substyles.Values)
			{
				if (substyle.Selector.Kind != SelectorKind.Type) continue;
				if (substyle.Selector.Name == type) return substyle;
			}
			return null;
		}

		public Substyle GetAllStyle()
		{
			return _substyles.Values.FirstOrDefault(s => s.Selector.Kind == SelectorKind.All);
		}

		public IEnumerable<Substyle> GetIterator() => _substyles.Values;

		public Substyle GetRootCustomStyle()
		{
			return _substyles.Values.FirstOrDefault(s => s.Selector.Kind == SelectorKind.Custom && s.Selector.Child == null);
		}
	}
}
