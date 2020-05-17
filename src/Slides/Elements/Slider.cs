using Slides.MathExpressions;
using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public class Slider : Element
	{
		public Range range { get; }
		public int value { get; }

		private List<JSInsertionBlock> _jsInsertions = new List<JSInsertionBlock>();
		public List<JSInsertionBlock> get_JSInsertions() => _jsInsertions;

		public void add_JSInsertion(JSInsertionBlock insertion)
		{
			_jsInsertions.Add(insertion);
		}

		public Slider(Range range)
		{
			this.range = range;
			value = (range.To + range.From) / 2;
		}

		public override ElementKind kind => ElementKind.Slider;

		protected override Unit get_InitialHeight() => new Unit(15, Unit.UnitKind.Pixel);

		protected override Unit get_InitialWidth() => new Unit(100, Unit.UnitKind.Pixel);
	}
}
