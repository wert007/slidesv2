using Slides.Filters;
using Slides.Transforms;
using System.Collections.Generic;
using System.Text;

namespace Slides.Elements
{
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
		public Thickness marginAndPadding => margin + padding;
		public Element parent { get; set; }
		public Unit top
		{
			get { return margin.top; }
		}
		public Unit left
		{
			get { return margin.left; }
		}
		public Unit bottom
		{
			get { return margin.bottom; }
		}
		public Unit right
		{
			get { return margin.right; }
		}
		public Unit rightSide
		{
			get { return left + get_ActualWidth(); }
		}
		public Unit bottomSide
		{
			get { return top + get_ActualHeight(); }
		}

		public Unit centerX => left + get_ActualWidth() * 0.5f;
		public Unit centerY => top + get_ActualHeight() * 0.5f;

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
				if (_height != null)
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
		public string name { get; set; }
		public bool isVisible { get; set; }
		public CustomStyle hover { get; set; }
		public abstract ElementKind kind { get; }
		private Stack<CustomStyle> appliedStyles;


		private Stack<Transform> _transforms = new Stack<Transform>();

		private static int index = 0;
		private Step step { get; set; }
		public void set_Step(Step step) => this.step = step;
		public string get_Id()
		{
			if (parent != null) return $"{parent.get_Id()}-{name}";
			return $"{step.get_Id()}-{name}";
		}
		public Element()
		{
			borderColor = null;
			borderThickness = new Thickness();
			borderStyle = BorderStyle.Unset;
			background = null;
			color = new Color(0, 0, 0, 0);
			orientation = Orientation.LeftTop;
			margin = new Thickness();
			padding = new Thickness();
			isVisible = true;
			appliedStyles = new Stack<CustomStyle>();

			width = null;
			height = null;
			name = index.ToString();
			index++;
		}

		public void applyStyle(CustomStyle style)
		{
			appliedStyles.Push(style);
		}

		public void translate(Unit x, Unit y)
		{
			_transforms.Push(new SingleValueTransform(TransformKind.TranslateX, x));
			_transforms.Push(new SingleValueTransform(TransformKind.TranslateY, y));
		}

		public void rotate(float degree)
		{
			_transforms.Push(new RotationTransform(TransformKind.RotateZ, degree));
		}

		public CustomStyle[] get_AppliedStyles()
		{
			return appliedStyles.ToArray();
		}

		public Transform[] get_Transforms()
		{
			return _transforms.ToArray();
		}

		/// <summary>
		/// Gets the elements property.
		/// Return null if the name couldn't be matched to any property.
		/// </summary>
		/// <param name="name">Must be the element property name.</param>
		/// <returns></returns>
		public object get_Property(string name)
		{
			object styleValue = null;
			foreach (var style in appliedStyles)
				foreach (var field in style.ModifiedFields)
					if (field.Key == name)
						styleValue = field.Value;
			switch (name)
			{
				case "borderColor":
					return borderColor;
				case "borderThickness":
					if (borderThickness == new Thickness()) return null;
					return borderThickness;
				case "borderStyle":
					if (borderStyle == BorderStyle.Unset) return null;
					return borderStyle;
				case "background":
					return background;
				case "color":
					if (color.Equals(Color.Transparent))
						return null;
					return color;
				case "orientation":
					if (orientation == Orientation.LeftTop && styleValue != null) return null;
					return orientation;
				case "margin":
					if (margin == new Thickness()) return null;
					return margin;
				case "padding":
					if (padding == new Thickness()) return null;
					return padding;
				case "filter": return filter;
				default:
					return null;
			}
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
			return null;
		}

		public Unit get_StyleHeight()
		{
			if (_height != null)
				return _height;
			if (initHeight != null)
				return initHeight;
			return null;
		}
	}
}