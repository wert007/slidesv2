using Slides.Elements;

namespace Slides
{
	public class Template
	{
		public string Name { get; }
		public Element[] VisualChildren { get; }
		public object[] DataChildren { get; }
		public Template(string name, Element[] visualChildren, object[] dataChildren)
		{
			Name = name;
			VisualChildren = visualChildren;
			DataChildren = dataChildren;
		}
	}
}
