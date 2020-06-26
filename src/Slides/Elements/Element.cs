using Slides.Data;
using Slides.Helpers;
using Slides.Styling;
using Slides.Transforms;
using SVGLib.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Slides.Elements
{
	public abstract class Element : IFilterInput
	{
		//Maybe temporary. Could be that we need to support all css attributes. 
		//If that happens we should think about something a little smarter..
		//
		//Actually. That should NEVER happen!
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
		public ParentElement h_parent { get; set; }
		public virtual Unit top
		{
			get
			{
				var m = get_ActualMargin();
				switch (orientation)
				{
					case Orientation.LeftTop:
					case Orientation.StretchTop:
					case Orientation.CenterTop:
					case Orientation.RightTop:
						return m.top;
					case Orientation.LeftCenter:
					case Orientation.StretchCenter:
					case Orientation.Center:
					case Orientation.RightCenter:
						return new Unit(50, Unit.UnitKind.Percent) - get_ActualHeight() * 0.5f;
					case Orientation.LeftStretch:
					case Orientation.Stretch:
					case Orientation.CenterStretch:
					case Orientation.RightStretch:
						return m.top;
					case Orientation.LeftBottom:
					case Orientation.StretchBottom:
					case Orientation.CenterBottom:
					case Orientation.RightBottom:
						return new Unit(100, Unit.UnitKind.Percent) - get_ActualHeight();
					default:
						throw new NotImplementedException();
				}
			}
		}
		public virtual Unit left
		{
			get
			{
				var m = get_ActualMargin();
				switch (orientation)
				{
					case Orientation.LeftTop:
					case Orientation.LeftCenter:
					case Orientation.LeftStretch:
					case Orientation.LeftBottom:
						return m.left;
					case Orientation.CenterTop:
					case Orientation.Center:
					case Orientation.CenterStretch:
					case Orientation.CenterBottom:
						return new Unit(50, Unit.UnitKind.Percent) - get_ActualWidth() * 0.5f;
					case Orientation.StretchTop:
					case Orientation.StretchCenter:
					case Orientation.Stretch:
					case Orientation.StretchBottom:
						return m.left;
					case Orientation.RightTop:
					case Orientation.RightCenter:
					case Orientation.RightStretch:
					case Orientation.RightBottom:
						return new Unit(100, Unit.UnitKind.Percent) - get_ActualWidth();
					default:
						throw new NotImplementedException();
				}
			}
		}
		public virtual Unit bottom
		{
			get { return margin.bottom; }
		}
		public virtual Unit right
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
		public string name { get; set; }
		public bool isVisible { get; set; }
		public CustomStyle hover { get; set; }
		public abstract ElementKind kind { get; }
		public virtual bool h_AllowsVerticalStretching { get; } = true;
		public virtual bool h_AllowsHorizontalStretching { get; } = true;
		protected virtual bool NeedsInitialSizeCalculated { get; } = false;

		private Stack<Style> appliedStyles;

		private readonly Dictionary<string, Action<object>> applyStyleHandlers = new Dictionary<string, Action<object>>();

		private Stack<Transform> _transforms = new Stack<Transform>();

		private static int index = 0;
		private Step step { get; set; }
		private SlideAttributes slideStyle { get; set; }
		public static StdStyle StdStyle { get; private set; }

		public string get_Id() => $"{h_parent?.get_Id() ?? step.get_Id()}-{name}";
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
			appliedStyles = new Stack<Style>();

			width = null;
			height = null;
			name = index.ToString();
			index++;

			addApplyStyleHandler("orientation", v => orientation = (Orientation)v);
			addApplyStyleHandler("margin", v => margin = (Thickness)v);
			addApplyStyleHandler("left", v => margin.left = SlidesConverter.ConvertToUnit(v));
			addApplyStyleHandler("top", v => margin.top = SlidesConverter.ConvertToUnit(v));
			addApplyStyleHandler("right", v => margin.right = SlidesConverter.ConvertToUnit(v));
			addApplyStyleHandler("bottom", v => margin.bottom = SlidesConverter.ConvertToUnit(v));
		}

		public void applyStyle(Style style)
		{
			handleApplyStyle(style);
			foreach (var field in style.GetMainStyle().Properties)
			{
				if (applyStyleHandlers.ContainsKey(field.Key))
					applyStyleHandlers[field.Key].Invoke(field.Value);
			}
			//foreach (var field in applyStyleHandlers.Keys)
			//	style.ModifiedFields.Remove(field);
			if (!(style is Substyle))
				appliedStyles.Push(style);
		}

		protected virtual void handleApplyStyle(Style style) { }

		protected void addApplyStyleHandler(string name, Action<object> handler)
		{
			applyStyleHandlers[name] = handler;
		}
		public UnitPair center() => relativePos(Orientation.Center);
		public UnitPair relativePos(Orientation o)
		{
			Unit x;
			Unit y;

			switch (SlidesHelper.GetHorizontal(o))
			{
				case Horizontal.Left:
					x = left;
					break;
				case Horizontal.Stretch:
				case Horizontal.Center:
					x = left + get_ActualWidth() * 0.5f;
					break;
				case Horizontal.Right:
					x = rightSide;
					break;
				default:
					throw new NotImplementedException(); ;
			}

			switch (SlidesHelper.GetVertical(o))
			{
				case Vertical.Top:
					y = top;
					break;
				case Vertical.Stretch:
				case Vertical.Center:
					y = top + get_ActualHeight() * 0.5f;
					break;
				case Vertical.Bottom:
					y = bottomSide;
					break;
				default:
					throw new NotImplementedException(); ;
			}

			return new UnitPair(x, y);
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

		public Style[] get_AppliedStyles()
		{
			return appliedStyles.ToArray();
		}

		public static void SetStdStyle(StdStyle style)
		{
			StdStyle = style;
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
			if(StdStyle != null)
				foreach (var field in StdStyle.GetMainStyle().Properties)
					if (field.Key == name)
						styleValue = field.Value;
			foreach (var style in appliedStyles)
				foreach (var field in style.GetMainStyle().Properties)
					if (field.Key == name)
						styleValue = field.Value;
			switch (name)
			{
				case "borderColor":
					return borderColor ?? styleValue;
				case "borderThickness":
					if (borderThickness == new Thickness()) return styleValue;
					return borderThickness;
				case "borderStyle":
					if (borderStyle == BorderStyle.Unset) return styleValue;
					return borderStyle;
				case "background":
					return background ?? styleValue;
				case "color":
					if (color.Equals(Color.Transparent))
						return styleValue;
					return color;
				case "orientation":
					//TODO: Shouldn't we return the style value? And not null? And why do we check the Stylevalue?
					if (orientation == Orientation.LeftTop && styleValue != null) return null;
					return orientation;
				case "margin":
					if (margin == new Thickness()) return styleValue;
					return margin;
				case "padding":
					if (padding == new Thickness()) return styleValue;
					return padding;
				case "filter": return filter ?? styleValue;
				default:
					return styleValue;
			}
		}

		internal abstract Unit get_InitialWidth();
		internal abstract Unit get_InitialHeight();

		//TODO: should orientation be more important than width? idk
		protected Unit get_ActualWidth()
		{
			if (_width != null)
				return _width;
			var m = get_ActualMargin();
			if (SlidesHelper.GetHorizontal(orientation) == Horizontal.Stretch && h_AllowsHorizontalStretching)
				return new Unit(100, Unit.UnitKind.Percent) - m.Horizontal;
			return get_InitialWidth();
		}

		protected Unit get_ActualHeight()
		{
			if (_height != null)
				return _height;
			var m = get_ActualMargin();
			if (SlidesHelper.GetVertical(orientation) == Vertical.Stretch && h_AllowsVerticalStretching)
				return new Unit(100, Unit.UnitKind.Percent) - m.Vertical;
			return get_InitialHeight();
		}

		private Thickness get_ActualMargin()
		{
			return get_FieldAsThickness("margin") ?? h_parent?.get_ActualMargin() ?? new Thickness();
		}

		public Unit get_StyleWidth()
		{
			if (_width != null)
				return _width;
			if (NeedsInitialSizeCalculated)
				return get_InitialWidth();
			return null;
		}

		public Unit get_StyleHeight()
		{
			if (_height != null)
				return _height;
			if (NeedsInitialSizeCalculated)
				return get_InitialHeight();
			return null;
		}

		private Thickness get_FieldAsThickness(string name)
		{
			object result = get_Property(name);
			foreach (var style in appliedStyles)
			{
				if (result != null) break;
				if (style.Substyles.GetRootCustomStyle().HasProperty(name))
					result = style.Substyles.GetRootCustomStyle().Properties[name];
			}
			return result as Thickness;
		}
		public void set_Step(Step step) => this.step = step;
		public Step get_Step() => step;
		public void set_SlideStyle(SlideAttributes slide) => slideStyle = slide;
		public SlideAttributes get_SlideStyle() => slideStyle;
	}
}