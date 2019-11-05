namespace Slides
{
	public class Image : Element
	{
		public Image(ImageSource source)
		{
			this.source = source;
		}

		public ImageSource source { get; set; }
		public override ElementType type => ElementType.Image;

		protected override Unit get_InitialHeight() => new Unit(source.height, Unit.UnitKind.Pixel);

		protected override Unit get_InitialWidth() => new Unit(source.width, Unit.UnitKind.Pixel);
	}
}
