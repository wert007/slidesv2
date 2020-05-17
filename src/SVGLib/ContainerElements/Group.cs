using SVGLib.Datatypes;
using SVGLib.GraphicsElements;

namespace SVGLib.ContainerElements
{
	public class SVGTag : SVGElement
	{
		public SVGTag(ViewBox viewBox, SVGElement[] children)
		{
			ViewBox = viewBox;
			Children = children;
		}

		public override SVGElementKind Kind => SVGElementKind.SVGTag;
		public ViewBox ViewBox { get; }
		public SVGElement[] Children { get; }
	}
	public class Group : SVGElement
	{
		//TODO: <g> doesn't need any width nor height! so why do we???
		public Group(SVGElement[] children)
		{
			Children = children;
		}

		public override SVGElementKind Kind => SVGElementKind.Group;
		public SVGElement[] Children { get; }
	}
}
