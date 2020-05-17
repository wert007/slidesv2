using Slides.Elements;
using SVGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.SVG
{
	public class SVGContainer : Element
	{
		public SVGContainer(SVGElement element)
		{
			Element = element;
		}

		public SVGElement Element { get; }

		public override ElementKind kind => ElementKind.SVGContainer;

		protected override Unit get_InitialHeight() => new Unit(300, Unit.UnitKind.Pixel);

		protected override Unit get_InitialWidth() => new Unit(300, Unit.UnitKind.Pixel);
	}
}
