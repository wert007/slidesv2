using SVGLib.Datatypes;
using SVGLib.GraphicsElements;

namespace SVGLib.ContainerElements
{
	public enum AspectRatioMeetOrSlice 
	{
		Meet, 
		Slice,
	}
	public enum AspectRatioAlign 
	{
		None,
		XMinYMin,
		XMidYMin,
		XMaxYMin,
		XMinYMid,
		XMidYMid,
		XMaxYMid,
		XMinYMax,
		XMidYMax,
		XMaxYMax,
	}
	public class SVGTag : SVGElement
	{
		public SVGTag(ViewBox viewBox, SVGElement[] children)
		{
			ViewBox = viewBox;
			Children = children;
		}

		public AspectRatioAlign PreserveAspectRatioAlign { get; set; }
		public AspectRatioMeetOrSlice PreserveAspectRatioMeetOrSlice { get; set; }

		public override SVGElementKind Kind => SVGElementKind.SVGTag;
		public ViewBox ViewBox { get; set; }
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
