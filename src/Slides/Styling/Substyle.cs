using System.Collections.Generic;

namespace Slides.Styling
{
	public class Substyle : Style
	{
		public Selector Selector { get; }

		public Dictionary<string, object> Properties { get; }

		public override string Name => Selector.ToString();

		public Substyle(Selector selector)
		{
			Selector = selector;
			Properties = new Dictionary<string, object>();
		}

		public void Add(string name, object value)
		{
			Properties[name] = value;
		}

		public bool HasProperty(string name) => Properties.ContainsKey(name);

		public object GetValue(string name) => HasProperty(name) ? Properties[name] : null;

		public override Substyle GetMainStyle() => this;
	}
}
