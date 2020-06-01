using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Elements
{
	public class UnitRect : Element
	{

		public UnitRect(Unit width, Unit height)
		{
			this.width = width;
			this.height = height;
		}

		public override ElementKind kind => ElementKind.Rect;

		protected override Unit get_InitialHeight() => height;

		protected override Unit get_InitialWidth() => width;
	}
}
