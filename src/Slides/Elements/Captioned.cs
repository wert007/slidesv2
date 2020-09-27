using Slides.Data;
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
			this.child.h_Parent = this;
			this.caption = new Label(caption);
			this.caption.h_Parent = this;
			this.captionPlacement = captionPlacement;
			addApplyStyleHandler("captionPlacement", v => { captionPlacement = (CaptionPlacement)v; });
			_namedChildren["caption"] = this.caption;
			_namedChildren["child"] = this.child;
		}
		public override ElementKind kind => ElementKind.Captioned;
		public override bool h_AllowsHorizontalStretching => child.h_AllowsHorizontalStretching;
		public override bool h_AllowsVerticalStretching => child.h_AllowsVerticalStretching;

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

		//protected override Unit get_UninitializedStyleHeight()
		//{
		//	// Didn't really think about always returning null, but it seems to work?
		//	//if (CaptionPlacementIsVertical(captionPlacement)) return null;
		//	//return get_InitialHeight();
		//	return null;
		//}

		
		//protected override Unit get_UninitializedStyleWidth()
		//{
		//	// Didn't really think about always returning null, but it seems to work?
		//	//if (!CaptionPlacementIsVertical(captionPlacement)) return null;
		//	//return get_InitialWidth();
		//	return null;
		//}

		public override void UpdateLayout()
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
			{
				if (child is Image && styling.get_UserDefinedHeight() != null)
					child.height = new Unit(100, Unit.UnitKind.Percent) - caption.height;
			//	_height = null;
			}
			else
			{
				if (child is Image && styling.get_UserDefinedWidth() != null)
					child.width = new Unit(100, Unit.UnitKind.Percent) - caption.width;
			//	_width = null;
			}
		}

		protected override IEnumerable<Element> get_Children()
		{
			yield return child;
			yield return caption;
		}

		internal override Unit get_InitialHeight()
		{
			var childHeight = child.height;
			if (captionPlacement == CaptionPlacement.TopOutwards || captionPlacement == CaptionPlacement.BottomOutwards)
				return childHeight + caption.height;
			else
				return childHeight;
		}

		internal override Unit get_InitialWidth()
		{
			var childWidth = child.width;
			Console.WriteLine("ChildWidth is " + childWidth);
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
