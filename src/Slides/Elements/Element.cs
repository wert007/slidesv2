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
	public class ElementStyling
	{
		public Brush h_Background { get; private set; }
		public Color h_Color { get; private set; }
		public Filter h_Filter { get; private set; }
		public Thickness h_Margin { get; private set; }
		public Thickness h_Padding { get; private set; }
		public Unit h_Width { get; private set; }
		public Unit h_Height { get; private set; }
		public bool? h_IsVisible { get; private set; }
		public ParentElement h_parent { get; set; }
		public bool h_IsDefault { get; private set; }
		
		private Stack<Transform> _transforms = new Stack<Transform>();


		//Maybe temporary. Could be that we need to support all css attributes. 
		//If that happens we should think about something a little smarter..
		//
		//Actually. That should NEVER happen!
		public string position { get; set; } = "";

		public Border border { get; set; }
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
			
			get => _orientation ?? (Orientation?)Element.StdStyle?.GetMainStyle().GetValue(nameof(orientation)) ?? Orientation.LeftTop;
			set
			{
				_orientation = value;
				h_Element.UpdateLayout();
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
				h_Element.UpdateLayout();
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
				h_Element.UpdateLayout();
			}
		}
		protected Unit _width = null;
		protected Unit _height = null;

		public bool isVisible
		{
			get => h_IsVisible ?? h_parent == null || h_parent.isVisible;
			set => h_IsVisible = value;
		}

		public ElementStyling hover { get; }


		protected readonly Element _element;
		protected virtual Element h_Element => _element;


		private Stack<Style> appliedStyles;

		private readonly Dictionary<string, Action<object>> _applyStyleHandlers = new Dictionary<string, Action<object>>();

		public static ElementStyling h_CreateElementStyling(Element e) => new ElementStyling(e);

		protected ElementStyling(Element element)
		{
			_element = element;
			border = new Border();
			background = null;
			color = null;
			//_orientation = Orientation.LeftTop;
			margin = null;
			padding = null;
			h_IsVisible = null;
			appliedStyles = new Stack<Style>();
			hover = new ElementStyling();
			_width = null;
			_height = null;
			// TODO: Set to false!
			h_IsDefault = true;
		}


		// TODO Actually something like element.hover.hover.hover is completely
		// illegal but we just expect the user to don't try..
		protected ElementStyling()
		{
			border = new Border();
			background = null;
			color = null;
			//_orientation = Orientation.LeftTop;
			margin = null;
			padding = null;
			h_IsVisible = null;
			appliedStyles = new Stack<Style>();
			_width = null;
			_height = null;
			// TODO: Set to false!
			h_IsDefault = true;

		}

		public void applyStyle(Style style)
		{
			h_Element.h_HandleApplyStyle(style);
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

		public void h_AddApplyStyleHandler(string name, Action<object> handler)
		{
			_applyStyleHandlers[name] = handler;
		}

		public Style[] get_AppliedStyles() => appliedStyles.ToArray();

		public object get_ActualField(string name)
		{
			switch (name)
			{
				case nameof(position):
					return string.IsNullOrEmpty(position) ? null : position;
				case nameof(border):
					return border;
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

		public void translate(Unit x, Unit y)
		{
			_transforms.Push(new SingleValueTransform(TransformKind.TranslateX, x));
			_transforms.Push(new SingleValueTransform(TransformKind.TranslateY, y));
		}

		public void rotate(float degree)
		{
			_transforms.Push(new RotationTransform(TransformKind.RotateZ, degree));
		}

		public Transform[] get_Transforms()
		{
			return _transforms.ToArray();
		}



		internal Unit get_UserDefinedWidth() => _width;
		internal Unit get_UserDefinedHeight() => _height;

		public Unit get_ActualWidth()
		{
			var result = get_StatedWidth();
			if (result != null) return result;
			return h_Element.get_InitialWidth();
		}

		//TODO: should orientation be more important than width? idk
		internal Unit get_StatedWidth()
		{
			if (_width != null)
				return _width;
			var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
			if (SlidesHelper.GetHorizontal(orientation) == Horizontal.Stretch && h_Element.h_AllowsHorizontalStretching)
				return new Unit(100, Unit.UnitKind.Percent) - m.Horizontal - padding.Horizontal;
			return null;
		}

		public Unit get_ActualHeight()
		{
			var result = get_StatedHeight();
			if (result != null) return result;
			return h_Element.get_InitialHeight();
		}

		internal Unit get_StatedHeight()
		{
			if (_height != null)
				return _height;
			var m = get_ActualValue(nameof(margin)) as Thickness ?? new Thickness();
			if (SlidesHelper.GetVertical(orientation) == Vertical.Stretch && h_Element.h_AllowsVerticalStretching)
				return new Unit(100, Unit.UnitKind.Percent) - m.Vertical - padding.Vertical;
			return null;
		}

		public Unit get_StyleWidth()
		{
			if (_width != null)
				return _width;
			return h_Element.get_UninitializedStyleWidth();
		}

		public Unit get_StyleHeight()
		{
			if (_height != null)
				return _height;
			return h_Element.get_UninitializedStyleHeight();
		}



	}
	public abstract class Element : IFilterInput
	{
		public ElementStyling styling { get; private set; }
		public virtual ElementStyling h_Styling => styling;

		public Thickness margin { get => h_Styling.margin; set => h_Styling.margin = value; }
		public Thickness padding { get => h_Styling.padding; set => h_Styling.padding = value; }
		public Orientation orientation { 
			get => h_Styling.orientation; 
			set => h_Styling.orientation = value; }
		public Unit width { get => h_Styling.width; set => h_Styling.width = value; }
		public Unit height { get => h_Styling.height; set => h_Styling.height = value; }
		public Color color { get => h_Styling.color; set => h_Styling.color = value; }
		public Filter n_filter { get => h_Styling.n_filter; set => h_Styling.n_filter = value; }
		public bool isVisible { get => h_Styling.isVisible; set => h_Styling.isVisible = value; }
		public ParentElement h_Parent { get => h_Styling.h_parent; set => h_Styling.h_parent = value; }
		public Border border { get => h_Styling.border; set => h_Styling.border = value; }
		public string position { get => h_Styling.position; set => h_Styling.position = value; }
		public Brush background { get => h_Styling.background; set => h_Styling.background = value; }


		public ElementStyling hover { get => h_Styling.hover; }
		public Thickness h_Margin { get => h_Styling.h_Margin; }
		public Brush h_Background { get => h_Styling.h_Background;}




		public Thickness marginAndPadding => margin + padding;
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


		private static int index = 0;

		private Step step { get; set; }
		private SlideAttributes slideStyle { get; set; }
		public static StdStyle StdStyle { get; private set; }

		public string get_Id() => $"{h_Parent?.get_Id() ?? step.get_Id()}-{name}";
		public Element()
		{
			Init();
		}

		protected Element(object noInit)
		{
		}

		protected void Init()
		{
			styling = ElementStyling.h_CreateElementStyling(this);
			name = index.ToString();
			index++;

			addApplyStyleHandler("orientation", v => orientation = (Orientation)v);
			addApplyStyleHandler("margin", v => margin = (Thickness)v);
			addApplyStyleHandler("left", v => margin.left = SlidesConverter.ConvertToUnit(v));
			addApplyStyleHandler("top", v => margin.top = SlidesConverter.ConvertToUnit(v));
			addApplyStyleHandler("right", v => margin.right = SlidesConverter.ConvertToUnit(v));
			addApplyStyleHandler("bottom", v => margin.bottom = SlidesConverter.ConvertToUnit(v));

		}

		public void applyStyle(Style style) => h_Styling.applyStyle(style);
		internal virtual void h_HandleApplyStyle(Style style) { }
		public virtual void UpdateLayout() { }

		protected void addApplyStyleHandler(string name, Action<object> handler)
		{
			h_Styling.h_AddApplyStyleHandler(name, handler);
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

		public void translate(Unit x, Unit y) => h_Styling.translate(x, y);
		public void rotate(float degree) => h_Styling.rotate(degree);
		public Transform[] get_Transforms() => h_Styling.get_Transforms();

		public Style[] get_AppliedStyles() => h_Styling.get_AppliedStyles();

		public static void SetStdStyle(StdStyle style)
		{
			StdStyle = style;
		}

		internal abstract Unit get_InitialWidth();
		internal abstract Unit get_InitialHeight();
		public Unit get_ActualWidth() => h_Styling.get_ActualWidth();
		public Unit get_ActualHeight() => h_Styling.get_ActualHeight();
		public Unit get_StyleWidth() => h_Styling.get_StyleWidth();
		public Unit get_StyleHeight() => h_Styling.get_StyleHeight();

		public object get_ActualField(string name) => h_Styling.get_ActualField(name);
		public object get_ActualValue(string name) => h_Styling.get_ActualValue(name);

		public virtual Unit get_UninitializedStyleHeight() => null;
		public virtual Unit get_UninitializedStyleWidth() => null;

		public void set_Step(Step step) => this.step = step;
		public Step get_Step() => step;
		public void set_SlideStyle(SlideAttributes slide) => slideStyle = slide;
		public SlideAttributes get_SlideStyle() => slideStyle;
	}
}