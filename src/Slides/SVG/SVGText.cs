namespace Slides.SVG
{
	public class SVGText : SVGElement
	{
		public SVGText(string content)
		{
			Content = content;
		}

		public override SVGElementKind kind => SVGElementKind.Text;

		public string Content { get; }
	}
}
