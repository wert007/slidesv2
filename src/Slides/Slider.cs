using System;
using System.Collections.Generic;

namespace Slides
{
	public class Function
	{
		public Function(string formula)
		{
			Formula = formula;
		}

		public string Formula { get; }

		public string Insert(string variable)
		{
			return Formula.Replace("x", variable);
		}
	}

	public class Dependency
	{
		public Dependency(Element element, string field, Function value)
		{
			Element = element;
			Field = field;
			Value = value;
		}

		public Element Element { get; }
		public string Field { get; }
		public Function Value { get; }
	}
	public class Slider : Element
	{
		public Range range { get; }
		public int value { get; }

		private List<Dependency> _dependencies = new List<Dependency>();
		public List<Dependency> get_Dependencies() => _dependencies;

		public void add_Dependency(Dependency d)
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
