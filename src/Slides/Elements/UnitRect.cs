﻿using System;
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

		public override ElementKind kind => ElementKind.UnitRect;

		internal override Unit get_InitialHeight() => height;

		internal override Unit get_InitialWidth() => width;

		public Color stroke
		{
			get => borderColor.top; set
			{
				borderColor = new ColorQuadruple(value, value, value, value);
				if (borderStyle.top == BorderStyle.Unset)
					borderStyle = new BorderStyleQuadruple(BorderStyle.Solid, BorderStyle.Solid, BorderStyle.Solid, BorderStyle.Solid);
				if (strokeWidth == new Unit()) strokeWidth = new Unit(1, Unit.UnitKind.Pixel);
			}
		}
		//TODO: What should we return here??
		public Unit strokeWidth { get => borderWidth.top; set => borderWidth = new Thickness(value, value, value, value); }
		
		public static UnitRect FromBounds(Unit x1, Unit y1, Unit x2, Unit y2)
		{
			var result = new UnitRect(x2 - x1, y2 - y1);
			result.margin = new Thickness(x1, y1, new Unit(), new Unit());
			return result;
		}
	}
}
