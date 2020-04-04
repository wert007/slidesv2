﻿using Slides.MathExpressions;
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
		public FieldDependency(object target, string field, Formula value)
		{
			Target = target;
			Field = field;
			Value = value;
		}

		public object Target { get; }

		public string Field { get; }
		public Formula Value { get; }
	}
	public class Slider : Element
	{
		public Range range { get; }
		public int value { get; }

		private List<FieldDependency> _dependencies = new List<FieldDependency>();
		public List<FieldDependency> get_Dependencies() => _dependencies;

		public void add_Dependency(FieldDependency d)
		{
			_dependencies.Add(d);
		}

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
