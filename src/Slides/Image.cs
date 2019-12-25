namespace Slides
{
	public class Image : Element
	{
		public Image(ImageSource source)
		{
			this.source = source;
			alt = string.Empty;
		}

		public ImageSource source { get; set; }
		public string alt { get; set; }

		public override ElementType type => ElementType.Image;

		protected override Unit get_InitialHeight() => new Unit(source.height, Unit.UnitKind.Pixel);

		protected override Unit get_InitialWidth() => new Unit(source.width, Unit.UnitKind.Pixel);
	}
}
