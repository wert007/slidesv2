using System.Collections.Generic;

namespace Slides.Data
{
	public class BorderLine
	{
		public BorderLine()
		{
			h_Width = null;
			_style = BorderStyle.Unset;
			_color = null;
		}

		public BorderLine(BorderLine line)
		{
			h_Width = line.h_Width;
			_style = line._style;
			_color = line._color;
		}

		public BorderLine(Unit width, BorderStyle style, Color color)
		{
			h_Width = width;
			_style = style;
			_color = color;
		}

		public virtual Unit width
		{
			get => h_Width ?? Unit.Medium; set
			{
				h_Width = value;
				if (_style == BorderStyle.Unset) _style = BorderStyle.Solid;
			}
		}
		private BorderStyle _style;
		public virtual BorderStyle style { get => _style; set => _style = value; }
		private Color _color;
		public virtual Color n_color
		{
			get => _color; set
			{
				_color = value;
				if (_style == BorderStyle.Unset) _style = BorderStyle.Solid;
			}
		}
		public Unit h_Width { get; set; }

		public override bool Equals(object obj)
		{
			return obj is BorderLine line &&
					 style == line.style &&
					 EqualityComparer<Color>.Default.Equals(n_color, line.n_color) &&
					 EqualityComparer<Unit>.Default.Equals(h_Width, line.h_Width);
		}

		public override int GetHashCode()
		{
			int hashCode = -1067737745;
			hashCode = hashCode * -1521134295 + style.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(n_color);
			hashCode = hashCode * -1521134295 + EqualityComparer<Unit>.Default.GetHashCode(h_Width);
			return hashCode;
		}

		public static bool operator ==(BorderLine left, BorderLine right)
		{
			return EqualityComparer<BorderLine>.Default.Equals(left, right);
		}

		public static bool operator !=(BorderLine left, BorderLine right)
		{
			return !(left == right);
		}
	}
}
