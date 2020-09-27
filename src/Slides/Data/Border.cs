using System;

namespace Slides.Data
{
	public class Border
	{
		public BorderLine top { get; set; }
		public BorderLine right { get; set; }
		public BorderLine bottom { get; set; }
		public BorderLine left { get; set; }

		public Color n_color
		{
			get
			{
				if (top.n_color != null)
					return top.n_color;
				else if (right.n_color != null)
					return right.n_color;
				else if (bottom.n_color != null)
					return bottom.n_color;
				else if (left.n_color != null)
					return left.n_color;
				return null;
			}
			set
			{
				top.n_color = value;
				right.n_color = value;
				bottom.n_color = value;
				left.n_color = value;
				if (style == BorderStyle.Unset) style = BorderStyle.Solid;
			}
		}
		public BorderStyle style
		{
			get
			{
				if (top.style != BorderStyle.Unset)
					return top.style;
				else if (right.style != BorderStyle.Unset)
					return right.style;
				else if (bottom.style != BorderStyle.Unset)
					return bottom.style;
				else if (left.style != BorderStyle.Unset)
					return left.style;
				return BorderStyle.Unset;
			}
			set
			{
				top.style = value;
				right.style = value;
				bottom.style = value;
				left.style = value;
			}
		}
		public Unit h_Width { get; set; }
		public Unit width
		{
			get
			{
				if (top.h_Width != null)
					return h_Width = top.h_Width;
				else if (right.h_Width != null)
					return h_Width = right.h_Width;
				else if (bottom.h_Width != null)
					return h_Width = bottom.h_Width;
				else if (left.h_Width != null)
					return h_Width = left.h_Width;
				h_Width = null;
				return Unit.Medium;
			}
			set
			{
				h_Width = value;
				top.width = value;
				right.width = value;
				bottom.width = value;
				left.width = value;
				if (style == BorderStyle.Unset) style = BorderStyle.Solid;
			}
		}

		public Border()
		{
			top = new BorderLine();
			right = new BorderLine();
			bottom = new BorderLine();
			left = new BorderLine();
			h_Width = null;
		}

		public Border(BorderLine line)
		{
			top = new BorderLine(line);
			right = new BorderLine(line);
			bottom = new BorderLine(line);
			left = new BorderLine(line);
			if (top.h_Width != null)
				h_Width = top.h_Width;
			else if (right.h_Width != null)
				h_Width = right.h_Width;
			else if (bottom.h_Width != null)
				h_Width = bottom.h_Width;
			else if (left.h_Width != null)
				h_Width = left.h_Width;
			else h_Width = null;
		}

		public bool get_AllEqual()
		{
			return top == right && right == bottom && bottom == left;
		}
	}
}
