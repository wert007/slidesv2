using Slides.Filters;
using System.Collections.Generic;
using System.Text;

namespace Slides
{
	public enum ElementType
	{
		Image,
		BoxElement,
		Label,
		LineChart,
		Rectangle,
		Stack,
		Slide,
		Container,
		List,
		CodeBlock
	}

	public abstract class Element : IFilterInput
	{
		//Maybe temporary. Could be that we need to support all css attributes. 
		//If that happens we should think about something a little smarter..
		public string position { get; set; } = null;
		public Color borderColor { get; set; }
		public Thickness borderThickness { get; set; }
		public BorderStyle borderStyle { get; set; }
		public Brush background { get; set; }
		public Color color { get; set; }
		public Filter filter { get; set; }
		public Orientation orientation { get; set; }
		public Thickness margin { get; set; }
		public Thickness padding { get; set; }
		public Thickness marginAndPadding => (margin ?? new Thickness()) + (padding ?? new Thickness());
		public Element parent { get; set; }
		public Unit top => margin?.Top;
		public Unit left => margin?.Left;
		public Unit right => left == null ? get_ActualWidth() : left + get_ActualWidth();
		public Unit bottom => top == null ? get_ActualHeight() : top + get_ActualHeight();
		public Unit width
		{
			get
			{
				if (_width != null)
					return _width;
				return get_ActualWidth();
			}
			set
			{
				_width = value;
			}
		}
		public Unit height
		{
			get
			{
				if (_height!= null)
					return _height;
				return get_ActualHeight();
			}
			set
			{
				_height = value;
			}
		}
		Unit _width = null;
		Unit _height = null;
		public Unit initWidth { get; set; }
		public Unit initHeight { get; set; }
		public abstract ElementType type { get; }
		public string name { get; set; }
		public Style hover { get; set; }
		private Stack<Style> appliedStyles;

		public Element()
		{
			borderColor = null;
			borderThickness = null;
			borderStyle = BorderStyle.Initial;
			background = null;
			color = null;
			orientation = Orientation.LeftTop;
			margin = null;
			padding = null;

			appliedStyles = new Stack<Style>();

			width = null;
			height = null;
			name = null;
		}

		public void applyStyle(Style style)
		{
			appliedStyles.Push(style);
		}

		public Style[] get_AppliedStyles()
		{
			return appliedStyles.ToArray();
		}

		protected abstract Unit get_InitialWidth();
		protected abstract Unit get_InitialHeight();

		private Unit get_ActualWidth()
		{
			if (_width != null)
				return _width;
			if (initWidth != null)
				return initWidth;
			return get_InitialWidth();
		}

		private Unit get_ActualHeight()
		{
			if (_height != null)
				return _height;
			if (initHeight != null)
				return initHeight;
			return get_InitialHeight();
		}

		public Unit get_StyleWidth()
		{
			if (_width != null)
				return _width;
			if (initWidth != null)
				return initWidth;
			return new Unit(0, Unit.UnitKind.Auto);
		}

		public Unit get_StyleHeight()
		{
			if (_height != null)
				return _height;
			if (initHeight != null)
				return initHeight;
			return new Unit(100, Unit.UnitKind.Auto);
		}
	}
}
