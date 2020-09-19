using System.Collections.Generic;

namespace Slides.Data
{
	public class BorderLine
	{
		public BorderLine()
		{
			h_Width = null;
			style = BorderStyle.Unset;
			n_color = null;
		}

		public BorderLine(BorderLine line)
		{
			width = line.h_Width;
			style = line.style;
			n_color = line.n_color;
		}

		public BorderLine(Unit width, BorderStyle style, Color color)
		{
			this.width = width;
			this.style = style;
			n_color = color;
		}

		public virtual Unit width { get => h_Width ?? Unit.Medium; set => h_Width = value; }
		public virtual BorderStyle style { get; set; }
		public virtual Color n_color { get; set; }
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
