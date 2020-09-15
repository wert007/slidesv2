using Slides.Data;
using Slides.Helpers;
using Slides.Styling;
using Slides.Transforms;
using SVGLib.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace Slides.Elements
{
	public abstract class Element : IFilterInput
	{
		public ColorQuadruple h_BorderColor { get; private set; }
		public Thickness h_BorderWidth { get; private set; }
		public Brush h_Background { get; private set; }
		public Color h_Color { get; private set; }
		public Filter h_Filter { get; private set; }
		public Thickness h_Margin { get; private set; }
		public Thickness h_Padding { get; private set; }
		public Unit h_Width { get; private set; }
		public Unit h_Height { get; private set; }
		public bool? h_IsVisible { get; private set; }


		//Maybe temporary. Could be that we need to support all css attributes. 
		//If that happens we should think about something a little smarter..
		//
		//Actually. That should NEVER happen!
		public string position { get; set; } = "";

		public ColorQuadruple borderColor
		{
			get
			{
				if(h_BorderColor == null)
					h_BorderColor = new ColorQuadruple(color, color, color, color);
				return h_BorderColor;
			}

			set => h_BorderColor = value;
		}
		public Thickness borderWidth
		{
			get
			{
				if(h_BorderWidth == null)
					h_BorderWidth = new Thickness(Unit.Medium, Unit.Medium, Unit.Medium, Unit.Medium);
				return h_BorderWidth;
			}

			set => h_BorderWidth = value;
		}
		public BorderStyleQuadruple borderStyle { get; set; }
		public Brush background { get => h_Background ?? new Brush(Color.Transparent); set => h_Background = value; }
		public Color color
		{
			get
			{
				if (h_Color == null) return h_parent?.color ?? Color.Black;
				return h_Color;
			}
			set => h_Color = value;
		}
		public Filter n_filter
		{
			get => h_Filter ?? h_parent?.n_filter;
			set => h_Filter = value;
		}
		private Orientation? _orientation;
		public Orientation orientation
		{
			get => _orientation ?? (Orientation?)StdStyle?.GetMainStyle().GetValue(nameof(orientation)) ?? Orientation.LeftTop;
			set
			{
				_orientation = value;
				UpdateLayout();
			}
		}
		//NOTE: The getter on those two is a little bit wrong.
		//		  you don't want to set the margin to initialized if 
		//      you read from it.. but we have to because the user
		//      may change the returned value, and they should change
		//      h_Margin and not some newly created object!
		public Thickness margin
		{
			get { if (h_Margin == null) h_Margin = new Thickness(); return h_Margin; }
			set => h_Margin = value;
		}
		public Thickness padding
		{
			get { if (h_Padding == null) h_Padding = new Thickness(); return h_Padding; }
			set => h_Padding = value;
		}

		public Unit width
		{
			get
			{
				var paddingHorizontal = (get_ActualValue("padding") as Thickness)?.Horizontal ?? new Unit();
				if (_width != null)
					return _width + paddingHorizontal;
				return get_ActualWidth() + paddingHorizontal;
			}
			set
			{
				_width = value;
				UpdateLayout();
			}
		}
		public Unit height
		{
			get
			{
				var paddingVertical = (get_ActualValue("padding") as Thickness)?.Vertical ?? new Unit();
				if (_height != null)
					return _height + paddingVertical;
				return get_ActualHeight() + paddingVertical;
			}
			set
			{
				_height = value;
				UpdateLayout();
			}
		}
		protected Unit _width = null;
		protected Unit _height = null;

		public bool isVisible
		{
			get => h_IsVisible ?? h_parent == null || h_parent.isVisible;
			set => h_IsVisible = value;
		}
		public CustomStyle hover { get; set; }

		public Thickness marginAndPadding => margin + padding;
		public ParentElement h_parent { get; set; }
		public virtual Unit top
		{
			get
			{
				var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
				var verticalOrientation = SlidesHelper.GetVertical(orientation);
				switch (verticalOrientation)
				{
					case Vertical.Top:
					case Vertical.Stretch:
						return m.top;
					case Vertical.Center:
						return (Unit.HundredPercent - m.bottom + m.top - height) * 0.5f;
					case Vertical.Bottom:
						return Unit.HundredPercent - height - m.bottom;
					default:
						throw new Exception();
				}
			}
		}
		public virtual Unit left
		{
			get
			{
				var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
				var horizontalOrientation = SlidesHelper.GetHorizontal(orientation);
				switch (horizontalOrientation)
				{
					case Horizontal.Left:
					case Horizontal.Stretch:
						return m.left;
					case Horizontal.Center:
						return (Unit.HundredPercent - m.right + m.left - width) * 0.5f;
					case Horizontal.Right:
						return Unit.HundredPercent - width - margin.right;
					default:
						throw new Exception();
				}
			}
		}
		public virtual Unit bottom
		{
			get
			{
				var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
				var verticalOrientation = SlidesHelper.GetVertical(orientation);
				switch (verticalOrientation)
				{
					case Vertical.Bottom:
					case Vertical.Stretch:
						return m.bottom;
					case Vertical.Center:
						return (Unit.HundredPercent - m.top + m.bottom - height) * 0.5f;
					case Vertical.Top:
						return Unit.HundredPercent - height - m.top;
					default:
						throw new Exception();
				}
			}
		}
		public virtual Unit right
		{
			get
			{
				var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
				var horizontalOrientation = SlidesHelper.GetHorizontal(orientation);
				switch (horizontalOrientation)
				{
					case Horizontal.Right:
					case Horizontal.Stretch:
						return m.right;
					case Horizontal.Center:
						return (Unit.HundredPercent - m.left + m.right - width) * 0.5f;
					case Horizontal.Left:
						return Unit.HundredPercent - width - margin.left;
					default:
						throw new Exception();
				}
			}
		}
		public Unit rightSide
		{
			get { return left + get_ActualWidth() + padding.Horizontal; }
		}
		public Unit bottomSide
		{
			get { return top + get_ActualHeight() + padding.Vertical; }
		}

		public string name { get; set; }
		public abstract ElementKind kind { get; }
		public virtual bool h_AllowsVerticalStretching { get; } = true;
		public virtual bool h_AllowsHorizontalStretching { get; } = true;

		private Stack<Style> appliedStyles;

		private readonly Dictionary<string, Action<object>> _applyStyleHandlers = new Dictionary<string, Action<object>>();

		private Stack<Transform> _transforms = new Stack<Transform>();

		private static int index = 0;

		private Step step { get; set; }
		private SlideAttributes slideStyle { get; set; }
		public static StdStyle StdStyle { get; private set; }

		public string get_Id() => $"{h_parent?.get_Id() ?? step.get_Id()}-{name}";
		public Element()
		{
			borderColor = null;
			borderWidth = null;
			borderStyle = new BorderStyleQuadruple();
			background = null;
			color = null;
			//_orientation = Orientation.LeftTop;
			margin = null;
			padding = null;
			h_IsVisible = null;
			appliedStyles = new Stack<Style>();

			_width = null;
			_height = null;
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
			HandleApplyStyle(style);
			foreach (var field in style.GetMainStyle().Properties)
			{
				if (_applyStyleHandlers.ContainsKey(field.Key))
					_applyStyleHandlers[field.Key].Invoke(field.Value);
			}
			//foreach (var field in applyStyleHandlers.Keys)
			//	style.ModifiedFields.Remove(field);
			if (!(style is Substyle))
				appliedStyles.Push(style);
		}

		protected virtual void HandleApplyStyle(Style style) { }
		protected virtual void UpdateLayout() { }

		protected void addApplyStyleHandler(string name, Action<object> handler)
		{
			_applyStyleHandlers[name] = handler;
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

		///// <summary>
		///// Gets the elements property.
		///// Return null if the name couldn't be matched to any property.
		///// </summary>
		///// <param name="name">Must be the element property name.</param>
		///// <returns></returns>
		//public object get_Property(string name)
		//{
		//	object styleValue = null;
		//	if (StdStyle != null)
		//	{
		//		foreach (var field in StdStyle.GetMainStyle().Properties)
		//			if (field.Key == name)
		//				styleValue = field.Value;

		//		//This all is dead weight. 
		//		//We look for the name of the Class. So in the case of a captioned
		//		//we look for Captioned.Label but actually we would need
		//		//Captioned.caption.
		//		//
		//		//So either the label 'caption' needs to know that it is a caption
		//		//or the Captioned must look for such styles and notify the caption! 
		//		var curParent = h_parent;
		//		var parentClassNames = new List<string>();
		//		parentClassNames.Add(GetType().Name);
		//		while (curParent != null)
		//		{
		//			parentClassNames.Add(curParent.GetType().Name);
		//			curParent = curParent.h_parent;
		//		}
		//		parentClassNames.Reverse();
		//		foreach (var substyle in StdStyle.Substyles.GetIterator())
		//		{
		//			if (substyle.Selector.Kind == SelectorKind.Type && substyle.Selector.Name == parentClassNames[0].ToLower())
		//			{
		//				var currentSelector = substyle.Selector;
		//				var index = 0;
		//				while (currentSelector != null)
		//				{
		//					if (index >= parentClassNames.Count)
		//						break;
		//					if (currentSelector.Name != parentClassNames[index].ToLower())
		//						break;
		//					index++;
		//					currentSelector = currentSelector.Child;
		//				}
		//				//We found something!
		//				if (currentSelector == null && index == parentClassNames.Count && substyle.HasProperty(name))
		//					styleValue = substyle.GetValue(name);
		//			}
		//		}
		//	}
		//	foreach (var style in appliedStyles)
		//		foreach (var field in style.GetMainStyle().Properties)
		//			if (field.Key == name)
		//				styleValue = field.Value;
		//	switch (name)
		//	{
		//		case "borderColor":
		//			return borderColor ?? styleValue;
		//		case "borderWidth":
		//			if (borderWidth == new Thickness()) return styleValue;
		//			return borderWidth;
		//		case "borderStyle":
		//			if (borderStyle == BorderStyle.Unset) return styleValue;
		//			return borderStyle;
		//		case "background":
		//			return background ?? styleValue;
		//		case "color":
		//			if (color.Equals(Color.Transparent))
		//				return styleValue;
		//			return color;
		//		case "orientation":
		//			//TODO: Shouldn't we return the style value? And not null? And why do we check the Stylevalue?
		//			if (orientation == Orientation.LeftTop && styleValue != null) return null;
		//			return orientation;
		//		case "margin":
		//			if (margin == new Thickness()) return styleValue ?? new Thickness();
		//			return margin;
		//		case "padding":
		//			if (padding == new Thickness()) return styleValue ?? new Thickness();
		//			return padding;
		//		case "filter": return n_filter ?? styleValue;
		//		default:
		//			return styleValue;
		//	}
		//}

		internal abstract Unit get_InitialWidth();
		internal abstract Unit get_InitialHeight();

		protected Unit get_ActualWidth()
		{
			var result = get_StatedWidth();
			if (result != null) return result;
			return get_InitialWidth();
		}

		//TODO: should orientation be more important than width? idk
		internal Unit get_StatedWidth()
		{
			if (_width != null)
				return _width;
			var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
			if (SlidesHelper.GetHorizontal(orientation) == Horizontal.Stretch && h_AllowsHorizontalStretching)
				return new Unit(100, Unit.UnitKind.Percent) - m.Horizontal - padding.Horizontal;
			return null;
		}

		protected Unit get_ActualHeight()
		{
			var result = get_StatedHeight();
			if (result != null) return result;
			return get_InitialHeight();
		}

		internal Unit get_StatedHeight()
		{
			if (_height != null)
				return _height;
			var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
			if (SlidesHelper.GetVertical(orientation) == Vertical.Stretch && h_AllowsVerticalStretching)
				return new Unit(100, Unit.UnitKind.Percent) - m.Vertical - padding.Vertical;
			return null;
		}

		public Unit get_StyleWidth()
		{
			if (_width != null)
				return _width;
			return get_UninitializedStyleWidth();
		}

		public Unit get_StyleHeight()
		{
			if (_height != null)
				return _height;
			return get_UninitializedStyleHeight();
		}

		public object get_ActualValue(string name)
		{
			var result = get_ActualField(name);
			if (result != null) return result;
			foreach (var style in appliedStyles)
			{
				if (!style.GetMainStyle().HasProperty(name)) continue;
				result = style.GetMainStyle().GetValue(name);
				if (result != null) return result;
			}
			if (h_parent != null)
				result = h_parent.get_ActualValue(name);
			if (result != null) return result;
			//TODO: How do we look up shit in the std-Style?
			return null;
		}

		public object get_ActualField(string name)
		{
			switch (name)
			{
				case nameof(position):
					return string.IsNullOrEmpty(position) ? null : position;
				case nameof(borderColor):
					return h_BorderColor;
				case nameof(borderWidth):
					return h_BorderWidth;
				case nameof(borderStyle):
					return borderStyle.h_IsUnset ? null : borderStyle;
				case nameof(background):
					return h_Background;
				case nameof(color):
					return h_Color;
				case "filter": //todo: idrk..
				case nameof(n_filter):
					return h_Filter;
				case nameof(orientation):
					return orientation;
				case nameof(margin):
					return h_Margin;
				case nameof(padding):
					return h_Padding;
				case nameof(width):
					return h_Width;
				case nameof(height):
					return h_Height;
				case nameof(isVisible):
					return h_IsVisible;
				default:
					throw new Exception();
			}
		}

		protected virtual Unit get_UninitializedStyleHeight() => null;
		protected virtual Unit get_UninitializedStyleWidth() => null;

		public void set_Step(Step step) => this.step = step;
		public Step get_Step() => step;
		public void set_SlideStyle(SlideAttributes slide) => slideStyle = slide;
		public SlideAttributes get_SlideStyle() => slideStyle;
	}
}