using SVGLib.Datatypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Elements.SVG
{
	public class UnitLine : Element
	{
		public Color stroke { get; set; } = new Color(0, 0, 0, 255);
		public Unit strokeWidth { get; set; } = new Unit(1, Unit.UnitKind.Pixel);
		public LineCap strokeLineCap { get; set; } = LineCap.Round;
		public UnitLine(Unit x1, Unit y1, Unit x2, Unit y2)
		{
			X1 = x1;
			Y1 = y1;
			X2 = x2;
			Y2 = y2;
		}

		public Unit X1 { get; }
		public Unit Y1 { get; }
		public Unit X2 { get; }
		public Unit Y2 { get; }

		public override ElementKind kind => ElementKind.UnitSVGShape;

		internal override Unit get_InitialHeight() => Y2 - Y1;

		internal override Unit get_InitialWidth() => X2 - X1;
	}
}
