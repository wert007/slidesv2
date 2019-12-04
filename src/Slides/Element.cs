using Slides.Filters;
using System;
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
		List
	}

	[Serializable]
	public class Style
	{
		public string Name { get; }
		public Dictionary<string, object> ModifiedFields { get; }

		public Style(string name, Dictionary<string, object> modifiedFields)
		{
			Name = name;
			ModifiedFields = modifiedFields;
		}
	}

	public abstract class Element : IFilterInput
	{
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

		//public void setMargin(float value)
		//{
		//	setMargin(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void marginTop(float value)
		//{
		//	marginTop(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void marginLeft(float value)
		//{
		//	marginLeft(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void marginBottom(float value)
		//{
		//	marginBottom(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void marginRight(float value)
		//{
		//	marginRight(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void setMargin(Unit value)
		//{
		//	margin = new Thickness(value, value, value, value);
		//}

		//public void marginTop(Unit value)
		//{
		//	if (margin == null)
		//		margin = new Thickness();
		//	margin.Top = value;
		//}

		//public void marginLeft(Unit value)
		//{
		//	if (margin == null)
		//		margin = new Thickness();
		//	margin.Left = value;
		//}

		//public void marginBottom(Unit value)
		//{
		//	if (margin == null)
		//		margin = new Thickness();
		//	margin.Bottom = value;
		//}

		//public void marginRight(Unit value)
		//{
		//	if (margin == null)
		//		margin = new Thickness();
		//	margin.Right = value;
		//}

		//public void setPadding(float value)
		//{
		//	setPadding(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void paddingTop(float value)
		//{
		//	paddingTop(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void paddingLeft(float value)
		//{
		//	paddingLeft(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void paddingBottom(float value)
		//{
		//	paddingBottom(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void paddingRight(float value)
		//{
		//	paddingRight(new Unit(value, Unit.UnitKind.Percent));
		//}

		//public void setPadding(Unit value)
		//{
		//	padding = new Thickness(value, value, value, value);
		//}

		//public void paddingTop(Unit value)
		//{
		//	if (padding == null)
		//		padding = new Thickness();
		//	padding.Top = value;
		//}

		//public void paddingLeft(Unit value)
		//{
		//	if (padding == null)
		//		padding = new Thickness();
		//	padding.Left = value;
		//}

		//public void paddingBottom(Unit value)
		//{
		//	if (padding == null)
		//		padding = new Thickness();
		//	padding.Bottom = value;
		//}

		//public void paddingRight(Unit value)
		//{
		//	if (padding == null)
		//		padding = new Thickness();
		//	padding.Right = value;
		//}
	}
}
