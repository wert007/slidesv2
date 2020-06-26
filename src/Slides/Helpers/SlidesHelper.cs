using Slides.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Helpers
{
	public static class SlidesHelper
	{
		public static Horizontal GetHorizontal(Orientation o)
		{
			switch (o)
			{
				case Orientation.LeftTop:
				case Orientation.LeftCenter:
				case Orientation.LeftStretch:
				case Orientation.LeftBottom:
					return Horizontal.Left;
				case Orientation.StretchTop:
				case Orientation.StretchCenter:
				case Orientation.Stretch:
				case Orientation.StretchBottom:
					return Horizontal.Stretch;
				case Orientation.CenterTop:
				case Orientation.Center:
				case Orientation.CenterStretch:
				case Orientation.CenterBottom:
					return Horizontal.Center;
				case Orientation.RightTop:
				case Orientation.RightCenter:
				case Orientation.RightStretch:
				case Orientation.RightBottom:
					return Horizontal.Right;
				default:
					throw new NotImplementedException();
			}
		}

		public static Vertical GetVertical(Orientation o)
		{
			switch (o)
			{
				case Orientation.LeftTop:
				case Orientation.StretchTop:
				case Orientation.CenterTop:
				case Orientation.RightTop:
					return Vertical.Top;
				case Orientation.LeftCenter:
				case Orientation.StretchCenter:
				case Orientation.Center:
				case Orientation.RightCenter:
					return Vertical.Center;
				case Orientation.LeftStretch:
				case Orientation.Stretch:
				case Orientation.CenterStretch:
				case Orientation.RightStretch:
					return Vertical.Stretch;
				case Orientation.LeftBottom:
				case Orientation.StretchBottom:
				case Orientation.CenterBottom:
				case Orientation.RightBottom:
					return Vertical.Bottom;
				default:
					throw new NotImplementedException();
			}
		}
		public static Orientation AddOrientations(Horizontal h, Vertical v)
		{
			switch (h)
			{
				case Horizontal.Left:
					switch (v)
					{
						case Vertical.Top: return Orientation.LeftTop;
						case Vertical.Stretch: return Orientation.LeftStretch;
						case Vertical.Center: return Orientation.LeftCenter;
						case Vertical.Bottom: return Orientation.LeftBottom;
					}
					break;
				case Horizontal.Stretch:
					switch (v)
					{
						case Vertical.Top: return Orientation.StretchTop;
						case Vertical.Stretch: return Orientation.Stretch;
						case Vertical.Center: return Orientation.StretchCenter;
						case Vertical.Bottom: return Orientation.StretchBottom;
					}
					break;
				case Horizontal.Center:
					switch (v)
					{
						case Vertical.Top: return Orientation.CenterTop;
						case Vertical.Stretch: return Orientation.CenterStretch;
						case Vertical.Center: return Orientation.Center;
						case Vertical.Bottom: return Orientation.CenterBottom;
					}
					break;
				case Horizontal.Right:
					switch (v)
					{
						case Vertical.Top: return Orientation.RightTop;
						case Vertical.Stretch: return Orientation.RightStretch;
						case Vertical.Center: return Orientation.RightCenter;
						case Vertical.Bottom: return Orientation.RightBottom;
					}
					break;
			}
			throw new Exception();
		}

	}
}
