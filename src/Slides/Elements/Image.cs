using Slides.Data;
using Slides.Helpers;

namespace Slides.Elements
{
	public class Image : Element
	{
		public Image(ImageSource source)
		{
			this.source = source;
			alt = string.Empty;
			stretching = ImageStretching.Contain;
		}

		public ImageSource source { get; set; }
		public ImageStretching stretching { get; set; }
		public string alt { get; set; }

		public override ElementKind kind => ElementKind.Image;

		protected override bool NeedsInitialSizeCalculated => h_parent == null;
		internal override Unit get_InitialHeight()
		{
			Unit statedWidth = null;
			if (_width != null)
				statedWidth = _width;
			var m = get_FieldAsThickness("margin") ?? new Thickness();
			if (SlidesHelper.GetHorizontal(orientation) == Horizontal.Stretch && h_AllowsHorizontalStretching)
				statedWidth = new Unit(100, Unit.UnitKind.Percent) - m.Horizontal;
			if (h_parent != null)
				statedWidth = h_parent.get_StatedWidth();
			if (statedWidth != null)
				return statedWidth * source.aspectRatio;
			return new Unit(source.height, Unit.UnitKind.Pixel);
		}

		internal override Unit get_InitialWidth()
		{
			var statedHeight = get_StatedHeight();
			if (statedHeight != null)
				return statedHeight / source.aspectRatio;
			return new Unit(source.width, Unit.UnitKind.Pixel);
		}
	}
}
