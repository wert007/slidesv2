namespace SVGLib.GraphicsElements
{
	public class Text : SVGGraphicsElement
	{
		public Text(string content)
		{
			Content = content;
		}

		public override SVGElementKind Kind => SVGElementKind.Text;

		public string Content { get; }
	}
}
