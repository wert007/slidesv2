using Slides.Data;
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
		public Color fill
		{
			get => Element.Fill; 
			set => Element.Fill = value;
		}

		public override ElementKind kind => ElementKind.SVGContainer;

		internal override Unit get_InitialHeight() => new Unit(300, Unit.UnitKind.Pixel);

		internal override Unit get_InitialWidth() => new Unit(300, Unit.UnitKind.Pixel);
	}
}
