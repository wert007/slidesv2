using System;

namespace Slides.Data
{
	public class Border : BorderLine
	{
		public BorderLine top { get; set; }
		public BorderLine right { get; set; }
		public BorderLine bottom { get; set; }
		public BorderLine left { get; set; }

		public override Color n_color
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

				// This happens when you create a new Border and it calls the base constructor. 
				// it will try to set style, but fails because all fields (top, right, bottom left)
				// are null.
				//
				// TODO: Maybe we should introduce a local field to BorderLine, which it will set
				// instead of these properties
				if (top == null) return;
				top.n_color = value;
				right.n_color = value;
				bottom.n_color = value;
				left.n_color = value;
			}
		}
		public override BorderStyle style
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
				// This happens when you create a new Border and it calls the base constructor. 
				// it will try to set style, but fails because all fields (top, right, bottom left)
				// are null.
				if (top == null) return;
				top.style = value;
				right.style = value;
				bottom.style = value;
				left.style = value;
			}
		}
		public override Unit width
		{
			get
			{
				if (top.h_Width != null)
					return top.h_Width;
				else if (right.h_Width != null)
					return right.h_Width;
				else if (bottom.h_Width != null)
					return bottom.h_Width;
				else if (left.h_Width != null)
					return left.h_Width;
				return Unit.Medium;
			}
			set
			{
				top.width = value;
				right.width = value;
				bottom.width = value;
				left.width = value;
			}
		}

		public Border()
		{
			top = new BorderLine();
			right = new BorderLine();
			bottom = new BorderLine();
			left = new BorderLine();
		}

		public Border(BorderLine line)
		{
			top = new BorderLine(line);
			right = new BorderLine(line);
			bottom = new BorderLine(line);
			left = new BorderLine(line);
		}

		public bool get_AllEqual()
		{
			return top == right && right == bottom && bottom == left;
		}
	}
}
