using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Elements.SVG
{
	public class Rect : Element
	{
		public Color Fill { get; set; } = new Color(0, 0, 0, 255);
		public Color Stroke { get; set; } = Color.Transparent;
		public double StrokeWidth { get; set; }

		public Rect(Unit width, Unit height)
		{
			this.width = width;
			this.height = height;
		}

		public override ElementKind kind => ElementKind.Rect;

		protected override Unit get_InitialHeight() => height;

		protected override Unit get_InitialWidth() => width;
	}
}
