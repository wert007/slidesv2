Element : IFilterInput
	> Color h_BorderColor;
	> Unit h_BorderWidth;
	> Brush h_Background;
	> Color h_Color;
	> Filter h_Filter;
	> Thickness h_Margin;
	> Thickness h_Padding;
	> Unit h_Width;
	> Unit h_Height;
	> bool? h_IsVisible	



    will set a defined value for css::position 
    (mostly relative or absolute) overrides any defaults
	default value is "empty string"
	> string position;

    if unset (is null) then it returns "color", will not be inherited 
    > Color borderColor;
	if unset (is null) then it returns compile-time constant "medium" (3px), will not be inherited 
	> Unit borderWidth;
	default value is BorderStyle.None, will not be inherited
	> BorderStyle borderStyle;
	default value is Brush("transparent"), will not be inherited
	> Brush background;
	default value is:
		"black" if there is no parent,
		parent.color otherwise
	> Color color;
	# Undecided what would be right here? Should we just always return none (null)?
	default value is:
		none if there is no parent,
		parent.filter otherwise
	> Filter n_filter;
	default value is Orientation.TopLeft, will not be inherited
	> Orientation orientation;
	default value is Thickness(0), will not be inherited
	> Thickness margin;
	default value is Thickness(0), will not be inherited
	> Thickness padding;
	default value is defined in get_ActualWidth(), will not be inherited
    > Unit width;
	default value is defined in get_ActualHeight(), will not be inherited
    > Unit height
	default vaule is:
		true if there is no parent,
		parent.isVisible otherwise
    > bool isVisible;
	default value is none, will not be inherited
    > CustomStyle n_hover;

    get only:
	addition of margin and padding
	> Thickness marginAndPadding;
	depending on orientation it will return either:
		Orientation.Top | Orientation.Stretch => margin.top
		Orientation.Bottom => 100% - height - margin.bottom
		Orientation.Center => (100% - margin.bottom + margin.top - height) * 0.5f
	> Unit top;
	depending on orientation it will return either:
		Orientation.Left | Orientation.Stretch => margin.left
		Orientation.Right => 100% - width - margin.right
		Orientation.Center => (100% - margin.right + margin.left - width) * 0.5f
	> Unit left;
	depending on orientation it will return either:
		Orientation.Bottom | Orientation.Stretch => margin.bottom
		Orientation.Top => 100% - height - margin.top
		Orientation.Center => (100% - margin.top + margin.bottom - height) * 0.5f
	> Unit bottom;
	depending on orientation it will return either:
		Orientation.Right | Orientation.Stretch => margin.right
		Orientation.Left => 100% - width - margin.left
		Orientation.Center => (100% - margin.left + margin.righ5 - width) * 0.5f
	> Unit right;
	equals left + width
	> Unit rightSide;
	equals top + height
	> Unit bottomSide;
 
    not to be used in sld:
	the parent of this element, only used internally
	> ParentElement h_parent;
	the variable name to which this is assigned, used for the html-id later
    > string h_name;
	only used internally to determine the actual class
    > ElementKind h_kind;
	if false, Orientation.Stretch will behave like Orientation.Center
    > bool h_AllowsVerticalStretching;
	if false, Orientation.Stretch will behave like Orientation.Center
	> bool h_AllowsHorizontalStretching;


TextElement : Element


	default value is:
		Font(Arial, Helvetica, sans-serif) if there is no parent
		parent.fontsize otherwise
	> Font font;
	default value is:
		14pt if there is no parent
		parent.fontsize otherwise
	> Unit fontsize;
