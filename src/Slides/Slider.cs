using Slides.MathExpressions;
using System;
using System.Collections.Generic;

namespace Slides
{
	public class Formula
	{
		public Formula(string formula)
		{
			F = formula;
		}

		public string F { get; }

		public string Insert(string variable)
		{
			return F.Replace("%x%", variable);
		}
	}

	public class FieldDependency
	{
		public FieldDependency(Element element, string field, Formula value)
		{
			MathFormula = null;
			Element = element;
			Field = field;
			Value = value;
		}

		public FieldDependency(MathFormula m, string field, Formula value)
		{
			MathFormula = m;
			Element = null;
			Field = field;
			Value = value;
		}

		public MathFormula MathFormula { get; }
		public Element Element { get; }
		public string Field { get; }
		public Formula Value { get; }
	}
	public class Slider : Element
	{
		public Range range { get; }
		public int value { get; }

		public Slider(Range range)
		{
			this.range = range;
			value = (range.To - range.From) / 2;
		}

		public override ElementType type => ElementType.Slider;

		protected override Unit get_InitialHeight() => new Unit(15, Unit.UnitKind.Pixel);

		protected override Unit get_InitialWidth() => new Unit(100, Unit.UnitKind.Pixel);
	}
}
