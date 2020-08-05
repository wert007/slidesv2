using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Elements
{
	public enum CaptionPlacement
	{
		TopInwards,
		RightInwards,
		BottomInwards,
		LeftInwards,
		TopOutwards,
		RightOutwards,
		BottomOutwards,
		LeftOutwards
	}
	public class Captioned : ParentElement
	{
		public Captioned(Element child, string caption)
			: this(child, caption, CaptionPlacement.BottomOutwards) { }
		public Captioned(Element child, string caption, CaptionPlacement captionPlacement)
		{
			this.child = child;
			this.child.h_parent = this;
			this.caption = new Label(caption);
			this.caption.h_parent = this;
			this.captionPlacement = captionPlacement;
			addApplyStyleHandler("captionPlacement", v => { captionPlacement = (CaptionPlacement)v; });
			_namedChildren["caption"] = this.caption;
			_namedChildren["child"] = this.child;
		}
		public override ElementKind kind => ElementKind.Captioned;
		public override bool h_AllowsHorizontalStretching => child.h_AllowsHorizontalStretching;
		public override bool h_AllowsVerticalStretching => child.h_AllowsVerticalStretching;
		protected override bool NeedsInitialSizeCalculated => true;

		private CaptionPlacement _captionPlacement;

		public Element child { get; }
		public Label caption { get; }
		public CaptionPlacement captionPlacement
		{
			get => _captionPlacement; set
			{
				_captionPlacement = value;
				UpdateLayout();
			}
		}

		protected override void UpdateLayout()
		{
			switch (captionPlacement)
			{
				case CaptionPlacement.TopInwards:
				case CaptionPlacement.TopOutwards:
					caption.orientation = Orientation.StretchTop;
					child.orientation = Orientation.StretchBottom;
					break;
				case CaptionPlacement.BottomInwards:
				case CaptionPlacement.BottomOutwards:
					caption.orientation = Orientation.StretchBottom;
					child.orientation = Orientation.StretchTop;
					break;
				case CaptionPlacement.RightInwards:
				case CaptionPlacement.RightOutwards:
					caption.orientation = Orientation.RightStretch;
					child.orientation = Orientation.LeftStretch;
					break;
				case CaptionPlacement.LeftInwards:
				case CaptionPlacement.LeftOutwards:
					caption.orientation = Orientation.LeftStretch;
					child.orientation = Orientation.RightStretch;
					break;
			}
			if (CaptionPlacementIsVertical(captionPlacement))
				_height = null;
			else
				_width = null;

			//if(CaptionPlacementIsVertical(captionPlacement))
			//{
			//	if (_width != null)
			//		child.width = _width;
			//	//TODO: Find something more generic then image??
			//	if (child is Image)
			//		child.height = new Unit(100, Unit.UnitKind.Percent) - caption.height;
			//}
			//else
			//{
			//	if (_height != null)
			//		child.height = _height;
			//	//TODO: Find something more generic then image??
			//	if (child is Image)
			//		child.width = new Unit(100, Unit.UnitKind.Percent) - caption.width;
			//}
		}

		protected override IEnumerable<Element> get_Children()
		{
			yield return child;
			yield return caption;
		}

		internal override Unit get_InitialHeight()
		{
			//TODO: This is wrong! get_InitialHeight() should never(!) return null
			// instead fiddle with get_StyleHeight() or NeedsInitialSizeCalculated!
			if (CaptionPlacementIsVertical(captionPlacement)) return null;
			var childHeight = child.height;
			if (captionPlacement == CaptionPlacement.TopOutwards || captionPlacement == CaptionPlacement.BottomOutwards)
				return childHeight + caption.height;
			else
				return childHeight;
		}

		internal override Unit get_InitialWidth()
		{
			if (!CaptionPlacementIsVertical(captionPlacement)) return null;
			var childWidth = child.width;
			if (captionPlacement == CaptionPlacement.LeftOutwards || captionPlacement == CaptionPlacement.RightOutwards)
				return childWidth + caption.width;
			else
				return childWidth;
		}


		private bool CaptionPlacementIsVertical(CaptionPlacement captionPlacement)
		{
			switch (captionPlacement)
			{
				case CaptionPlacement.TopInwards:
				case CaptionPlacement.BottomInwards:
				case CaptionPlacement.TopOutwards:
				case CaptionPlacement.BottomOutwards:
					return true;
				case CaptionPlacement.RightInwards:
				case CaptionPlacement.LeftInwards:
				case CaptionPlacement.RightOutwards:
				case CaptionPlacement.LeftOutwards:
					return false;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
