using Slides.Data;
using Slides.Helpers;
using System;

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

		public override Unit get_UninitializedStyleHeight()
		{
			if (h_Parent != null) return null;
			return get_InitialHeight();
		}
		public override Unit get_UninitializedStyleWidth()
		{
			if (h_Parent != null) return null;
			return get_InitialWidth();
		}
		internal override Unit get_InitialHeight()
		{
			Unit statedWidth = styling.get_UserDefinedWidth();
			var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
			if (SlidesHelper.GetHorizontal(orientation) == Horizontal.Stretch && h_AllowsHorizontalStretching)
				statedWidth = new Unit(100, Unit.UnitKind.Percent) - m.Horizontal;
			if (h_Parent != null)
				statedWidth = h_Parent.styling.get_StatedWidth();
			if (statedWidth != null)
				return statedWidth * source.aspectRatio;
			return new Unit(source.height, Unit.UnitKind.Pixel);
		}

		internal override Unit get_InitialWidth()
		{
			var statedHeight = styling.get_StatedHeight();
			if (statedHeight != null)
				return statedHeight / source.aspectRatio;
			return new Unit(source.width, Unit.UnitKind.Pixel);
		}
	}
}
