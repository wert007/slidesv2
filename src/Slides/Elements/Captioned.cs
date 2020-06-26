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
				LayoutUpdated();
			}
		}
		
		private void LayoutUpdated()
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
			//initHeight = get_InitialHeight();
			//initWidth = get_InitialWidth();
		}

		protected override IEnumerable<Element> get_Children()
		{
			yield return child;
			yield return caption;
		}

		internal override Unit get_InitialHeight()
		{
			if (captionPlacement == CaptionPlacement.TopOutwards || captionPlacement == CaptionPlacement.BottomOutwards)
				return child.height + caption.height;
			else
				return child.height;
		}

		internal override Unit get_InitialWidth()
		{
			if(captionPlacement == CaptionPlacement.LeftOutwards || captionPlacement == CaptionPlacement.RightOutwards)
				return child.width + caption.width;
			else
				return child.width;
		}
	}
}
