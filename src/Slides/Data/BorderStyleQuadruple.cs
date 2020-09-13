namespace Slides
{
	public class BorderStyleQuadruple
	{
		public BorderStyle top { get; set; }
		public BorderStyle right { get; set; }
		public BorderStyle bottom { get; set; }
		public BorderStyle left { get; set; }

		public bool h_IsUnset { get => top == BorderStyle.Unset && right == BorderStyle.Unset && bottom == BorderStyle.Unset && left == BorderStyle.Unset; }

		public BorderStyleQuadruple()
		{
			top = BorderStyle.Unset;
			right = BorderStyle.Unset;
			bottom = BorderStyle.Unset;
			left = BorderStyle.Unset;
		}

		public BorderStyleQuadruple(BorderStyle top, BorderStyle right, BorderStyle bottom, BorderStyle left)
		{
			this.top = top;
			this.right = right;
			this.bottom = bottom;
			this.left = left;
		}

	}
}
