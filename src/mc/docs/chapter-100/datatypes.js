class Thickness
{
	constructor(top, right, bottom, left)
	{
		this.top = top;
		this.right = right;
		this.bottom = bottom;
		this.left = left;
	}

	toString()
	{
		return "" + this.top.toString() + " " + this.right.toString() + " " + this.bottom.toString() + " " + this.left.toString();
	}

	static lerp(a, b, t)
	{
		return new Thickness(StyleUnit.lerp(a.top, b.top, t), 
							StyleUnit.lerp(a.right, b.right, t), 
							StyleUnit.lerp(a.bottom, b.bottom, t), 
							StyleUnit.lerp(a.left, b.left, t));
	}

	static parse(s)
	{
		let parts = s.split(' ');
		if(parts.length == 1)
		{
			let valV = StyleUnit.parse(s, true);
			let valH = StyleUnit.parse(s, false);
			return new Thickness(valV, valH, valV, valH);
		}
		else if(parts.length == 2)
		{
			let valV = StyleUnit.parse(parts[0], true);
			let valH = StyleUnit.parse(parts[1], false);
			return new Thickness(valV, valH, valV, valH);
		}
		else if(parts.length == 3)
		{
			let valTop = StyleUnit.parse(parts[0], true);
			let valH = StyleUnit.parse(parts[1], false);
			let valBottom = StyleUnit.parse(parts[2], true);
			return new Thickness(valTop, valH, valBottom, valH);
		}
		else if(parts.length == 4)
		{
			let valTop = StyleUnit.parse(parts[0], true);
			let valRight = StyleUnit.parse(parts[1], false);
			let valBottom = StyleUnit.parse(parts[2], true);
			let valLeft = StyleUnit.parse(parts[3], false);
			return new Thickness(valTop, valRight, valBottom, valLeft);
		}
		console.log("ERROR!");
		return new Thickness();
	}
}
class StyleUnit
{
    constructor(value, unit, isVertical)
    {
        this.value = value;
        this.unit = unit;
        this.isVertical = isVertical;
    }

    toString()
    {
        return "" + this.value + this.unit;
    }

    static lerp(a, b, t)
    {
        if(a.unit == b.unit)
            return new StyleUnit(lerp(a.value, b.value, t), a.unit, undefined);
        else
        {
            let aNeedsChanging = true;
            let changingUnit = a;
            if(b.unit == "%")
            {
                changingUnit = b;
                aNeedsChanging = false;
            }
            let maxSize = document.documentElement.clientWidth;
            if(changingUnit.isVertical)
                maxSize = document.documentElement.clientHeight;
            let value = changingUnit.value / 100.0 * maxSize;
            if(aNeedsChanging)
                //TODO: Maybe its different.
                return new StyleUnit(lerp(value, b.value, t), b.unit, undefined);
            else
                return new StyleUnit(lerp(a.value, value, t), a.unit, undefined);
        }
    }

    static parse(s, isVertical)
    {
        let value = 0;
        let unit = "px";
        let split = -1;
        for (let i = 0; i < s.length; i++) {
            const c = s[i];
            if(c >= '0' && c <= '9' || c == '.' || c == '-') //c is digit
            {

            }
            else if(split < 0)
            {
                split = i;
            }
        }
        value = parseFloat(s.substr(0, split));
        unit = s.substr(split);
        return new StyleUnit(value, unit, isVertical);
    }
}

class PercentalFilter {
	constructor(name, value)
	{
		this.name = name;
		this.value = value;
	}

	static lerp(a, b, t)
	{
		return new PercentalFilter(a.name, lerp(a.value, b.value, t));
	}

	toString()
	{
		return name + '(' + value + ')';
	}
}

class BlurFilter {
	constructor(value)
	{
		this.value = value;
	}

	static lerp(a, b, t)
	{
		return new BlurFilter(lerp(a.value, b.value, t));
	}

	
	toString()
	{
		return 'blur(' + this.value + 'px)';
	}
}

class DropShadowFilter {
	constructor(horizontal, vertical, blur, spread, color)
	{
		this.horizontal = horizontal;
		this.vertical = vertical;
		this.blur = blur;
		this.spread = spread;
		this.color = color;
	}

	static lerp(a, b, t)
	{
		let h = lerp(a.horizontal, b.horizontal, t);
		let v = lerp(a.vertical, b.vertical, t);
		let bl = lerp(a.blur, b.blur, t);
		let s = lerp(a.spread, b.spread, t);
		let c = Color.lerp(a.color, b.color, t);
		return new DropShadowFilter(h, v, bl, s, c);
	}

	
	toString()
	{
		return 'drop-shadow(' + this.horizontal + ' ' + this.vertical + ' ' + this.blur + ' ' + this.spread + ' ' + this.color.toString() + ')';
	}
}

//TODO: Move to color.js
var Color_t = (function(window){

	var Events = {
		RGB_UPDATED : 'RGBUpdated',
		HSL_UPDATED : 'HSLUpdated',
		HSV_UPDATED : 'HSVUpdated',
		HEX_UPDATED : 'HexUpdated',
		INT_UPDATED : 'IntUpdated',
		UPDATED : 'updated',
		PARSED: 'parsed'
	};

	var namedColors = {
		'transparent':'rgba(0, 0, 0, 0)','aliceblue':'#F0F8FF','antiquewhite':'#FAEBD7','aqua':'#00FFFF','aquamarine':'#7FFFD4',
		'azure':'#F0FFFF','beige':'#F5F5DC','bisque':'#FFE4C4','black':'#000000','blanchedalmond':'#FFEBCD','blue':'#0000FF','blueviolet':'#8A2BE2',
		'brown':'#A52A2A','burlywood':'#DEB887','cadetblue':'#5F9EA0','chartreuse':'#7FFF00','chocolate':'#D2691E','coral':'#FF7F50',
		'cornflowerblue':'#6495ED','cornsilk':'#FFF8DC','crimson':'#DC143C','cyan':'#00FFFF','darkblue':'#00008B','darkcyan':'#008B8B','darkgoldenrod':'#B8860B',
		'darkgray':'#A9A9A9','darkgrey':'#A9A9A9','darkgreen':'#006400','darkkhaki':'#BDB76B','darkmagenta':'#8B008B','darkolivegreen':'#556B2F',
		'darkorange':'#FF8C00','darkorchid':'#9932CC','darkred':'#8B0000','darksalmon':'#E9967A','darkseagreen':'#8FBC8F','darkslateblue':'#483D8B',
		'darkslategray':'#2F4F4F','darkslategrey':'#2F4F4F','darkturquoise':'#00CED1','darkviolet':'#9400D3','deeppink':'#FF1493','deepskyblue':'#00BFFF',
		'dimgray':'#696969','dimgrey':'#696969','dodgerblue':'#1E90FF','firebrick':'#B22222','floralwhite':'#FFFAF0','forestgreen':'#228B22',
		'fuchsia':'#FF00FF','gainsboro':'#DCDCDC','ghostwhite':'#F8F8FF','gold':'#FFD700','goldenrod':'#DAA520','gray':'#808080','grey':'#808080',
		'green':'#008000','greenyellow':'#ADFF2F','honeydew':'#F0FFF0','hotpink':'#FF69B4','indianred':'#CD5C5C','indigo':'#4B0082','ivory':'#FFFFF0',
		'khaki':'#F0E68C','lavender':'#E6E6FA','lavenderblush':'#FFF0F5','lawngreen':'#7CFC00','lemonchiffon':'#FFFACD','lightblue':'#ADD8E6',
		'lightcoral':'#F08080','lightcyan':'#E0FFFF','lightgoldenrodyellow':'#FAFAD2','lightgray':'#D3D3D3','lightgrey':'#D3D3D3','lightgreen':'#90EE90',
		'lightpink':'#FFB6C1','lightsalmon':'#FFA07A','lightseagreen':'#20B2AA','lightskyblue':'#87CEFA','lightslategray':'#778899',
		'lightslategrey':'#778899','lightsteelblue':'#B0C4DE','lightyellow':'#FFFFE0','lime':'#00FF00','limegreen':'#32CD32','linen':'#FAF0E6',
		'magenta':'#FF00FF','maroon':'#800000','mediumaquamarine':'#66CDAA','mediumblue':'#0000CD','mediumorchid':'#BA55D3','mediumpurple':'#9370D8',
		'mediumseagreen':'#3CB371','mediumslateblue':'#7B68EE','mediumspringgreen':'#00FA9A','mediumturquoise':'#48D1CC','mediumvioletred':'#C71585',
		'midnightblue':'#191970','mintcream':'#F5FFFA','mistyrose':'#FFE4E1','moccasin':'#FFE4B5','navajowhite':'#FFDEAD','navy':'#000080','oldlace':'#FDF5E6',
		'olive':'#808000','olivedrab':'#6B8E23','orange':'#FFA500','orangered':'#FF4500','orchid':'#DA70D6','palegoldenrod':'#EEE8AA',
		'palegreen':'#98FB98','paleturquoise':'#AFEEEE','palevioletred':'#D87093','papayawhip':'#FFEFD5','peachpuff':'#FFDAB9','peru':'#CD853F',
		'pink':'#FFC0CB','plum':'#DDA0DD','powderblue':'#B0E0E6','purple':'#800080','red':'#FF0000','rosybrown':'#BC8F8F','royalblue':'#4169E1',
		'saddlebrown':'#8B4513','salmon':'#FA8072','sandybrown':'#F4A460','seagreen':'#2E8B57','seashell':'#FFF5EE','sienna':'#A0522D','silver':'#C0C0C0',
		'skyblue':'#87CEEB','slateblue':'#6A5ACD','slategray':'#708090','slategrey':'#708090','snow':'#FFFAFA','springgreen':'#00FF7F',
		'steelblue':'#4682B4','tan':'#D2B48C','teal':'#008080','thistle':'#D8BFD8','tomato':'#FF6347','turquoise':'#40E0D0','violet':'#EE82EE','white':'#FFFFFF'
	};


	// helpers
	var absround = function(number){
		return (0.5 + number) << 0;
	};

	var hue2rgb = function(a, b, c) {  // http://www.w3.org/TR/css3-color/#hsl-color
		if(c < 0) c += 1;
		if(c > 1) c -= 1;
		if(c < 1/6) return a + (b - a) * 6 * c;
		if(c < 1/2) return b;
		if(c < 2/3) return a + (b - a) * (2/3 - c) * 6;
		return a;
	};

	var p2v = function(p){
		return isPercent.test(p) ? absround(parseInt(p) * 2.55) : parseInt(p);
	};
	
	var isNamedColor = function(key){
		var lc = ('' + key).toLowerCase();
		return namedColors.hasOwnProperty(lc)
			? namedColors[lc]
			: null;
	};

	// patterns
	var isHex = /^#?([0-9a-f]{3}|[0-9a-f]{6})$/i;
	var isHSL = /^hsla?\((\d{1,3}?),\s*(\d{1,3}%),\s*(\d{1,3}%)(,\s*[01]?\.?\d*)?\)$/;
	var isRGB = /^rgba?\((\d{1,3}%?),\s*(\d{1,3}%?),\s*(\d{1,3}%?)(,\s*[01]?\.?\d*)?\)$/;
	var isPercent = /^\d+(\.\d+)*%$/;

	var hexBit = /([0-9a-f])/gi;
	var leadHex = /^#/;

	var matchHSL = /^hsla?\((\d{1,3}),\s*(\d{1,3})%,\s*(\d{1,3})%(,\s*([01]?\.?\d*))?\)$/;
	var matchRGB = /^rgba?\((\d{1,3}%?),\s*(\d{1,3}%?),\s*(\d{1,3}%?)(,\s*([01]?\.?\d*))?\)$/;

	/**
	* Color instance - get, update and output a Color between structures.
	* @constructor
	* @param {mixed} value Accepts any valid CSS color value e.g., #FF9900, rgb(255, 153, 0), rgba(100%, 40%, 0%, 0.8);
	* a hash with properties mapped to the Color instance e.g., red, green, saturation, brightness;
	* another Color instance; a numeric color value; a named CSS color
	* @class Instances of the Color class serve as abstract reprentations of the color itself, and don't need to be 
	* transformed from one format to another.  A single Color instance can have any component (red, green, blue, hue, saturation, lightness, brightness,
	* alpha) updated regardless of the source.  Further, all other components will be normalized automatically.  If a Color is instanced using a hex value,
	* it can have it's lightness component updated directly despite lightness being a HSL component.  Further, the same color instance can output it's
	* component values in any format without any extra conversions.  Conversion methods (getRGB, getHex) are provided just as helpers, and don't perform any
	* actual transformations.  They are not required for use or translation.  The standard component parts are available as instance methods - passing a value
	* argument to set, and each return the value as well (with or without setter arguments).  These components perform transformations and dispatch events, and
	* can be used without any sugar to manage the Color instance.
	* Component methods include:
	* .red()
	* .green()
	* .blue()
	* .hue()
	* .saturation()
	* .lightness()
	* .brightness()
	* .hex()
	* .decimal()
	* </ul>
	* @example
	* // instancing...
	* new Color();
	* new Color('#FF9900');
	* new Color(element.style.color);
	* new Color('pink');
	* new Color(123456);
	* new Color({ red : 255, green : 100, blue : 0 });
	* new Color(colorInstance);
	* // usage...
	* var color = new Color('#FF9900');
	* color.brightness(20);
	* element.style.backgroundColor = color;
	* console.log(color.getRGB());
	* console.log(color.saturation());
	*/
	function Color_t(value){

		this._listeners = {};

		//this.subscribe(Events.RGB_UPDATED, this._RGBUpdated);
		//this.subscribe(Events.HEX_UPDATED, this._HEXUpdated);
		//this.subscribe(Events.HSL_UPDATED, this._HSLUpdated);
		//this.subscribe(Events.HSV_UPDATED, this._HSVUpdated);
		//this.subscribe(Events.INT_UPDATED, this._INTUpdated);
		
		this.parse(value);

	};

	Color_t.prototype._decimal = 0;  // 0 - 16777215
	Color_t.prototype._hex = '#000000';  // #000000 - #FFFFFF
	Color_t.prototype._red = 0;  // 0 - 255
	Color_t.prototype._green = 0;  // 0 - 255
	Color_t.prototype._blue = 0;  // 0 - 255
	Color_t.prototype._hue = 0;  // 0 - 360
	Color_t.prototype._saturation = 0;  // 0 - 100
	Color_t.prototype._lightness = 0;  // 0 - 100
	Color_t.prototype._brightness = 0;  // 0 - 100
	Color_t.prototype._alpha = 1;  // 0 - 1
	
	/**
	* Convert mixed variable to Color component properties, and adopt those properties.
	* @function
	* @param {mixed} value Accepts any valid CSS color value e.g., #FF9900, rgb(255, 153, 0), rgba(100%, 40%, 0%, 0.8);
	* a hash with properties mapped to the Color instance e.g., red, green, saturation, brightness;
	* another Color instance; a numeric color value; a named CSS color
	* @returns this
	* @example
	* var color = new Color();
	* color.parse();
	* color.parse('#FF9900');
	* color.parse(element.style.color);
	* color.parse('pink');
	* color.parse(123456);
	* color.parse({ red : 255, green : 100, blue : 0 });
	* color.parse(colorInstance);
	*/
	Color_t.prototype.parse = function(value){
		if(typeof value == 'undefined'){
			return this;
		};
		switch(true){
			case isFinite(value) :
				this.decimal(value);
				this.output = Color_t.INT;
				//this.broadcast(Events.PARSED);
				return this;
			case (value instanceof Color_t) :
				this.copy(value);
				//this.broadcast(Events.PARSED);
				return this;
			default : 
				switch(typeof value) {
					case 'object' :
						this.set(value);
					//	this.broadcast(Events.PARSED);
						return this;
					case 'string' :
						  value = namedColors.hasOwnProperty(value)?namedColors[value]:value;
						switch(true){
							case isHex.test(value) :
								var stripped = value.replace(leadHex, '');
								if(stripped.length == 3) {
									stripped = stripped.replace(hexBit, '$1$1');
								};
								this.decimal(parseInt(stripped, 16));
						//		this.broadcast(Events.PARSED);
								return this;
							case isRGB.test(value) :
								var parts = value.match(matchRGB);
								this.red(p2v(parts[1]));
								this.green(p2v(parts[2]));
								this.blue(p2v(parts[3]));
								var alpha = parseFloat(parts[5]);
								if (isNaN(alpha)) alpha = 1;
								this.alpha(alpha);
								this.output = (isPercent.test(parts[1]) ? 2 : 1) + (parts[5] ? 2 : 0);
							//	this.broadcast(Events.PARSED);
								return this;
							case isHSL.test(value) :  
								var parts = value.match(matchHSL);
								this.hue(parseInt(parts[1]));
								this.saturation(parseInt(parts[2]));
								this.lightness(parseInt(parts[3]));
								var alpha = parseFloat(parts[5]);
								if (isNaN(alpha)) alpha = 1;
								this.alpha(alpha);
								this.output = parts[5] ? 6: 5;
							//	this.broadcast(Events.PARSED);
								return this;
						};
				};		
			
		};
		return this;
	};

	/**
	* Create a duplicate of this Color instance
	* @function
	* @returns Color
	*/
	Color_t.prototype.clone = function(){
		var c = new Color_t(this.decimal());
		c.alpha(this.alpha());
		return c;
	};

	/**
	* Copy values from another Color instance
	* @function
	* @param {Color} color Color instance to copy values from
	* @returns this
	*/
	Color_t.prototype.copy = function(color){
		return this.set(color.decimal()).alpha(color.alpha());
	};

	/**
	* Set a color component value
	* @function
	* @param {string|object|number} key Name of the color component to defined, or a hash of key:value pairs, or a single numeric value
	* @param {string|number} value - Value of the color component to be set
	* @returns this
	* @example
	* var color = new Color();
	* color.set('lightness', 100);
	* color.set({ red : 255, green : 100 });
	* color.set(123456);
	*/
	Color_t.prototype.set = function(key, value){
		if(arguments.length == 1){
			if(typeof key == 'object'){
				for(var p in key){
					if(typeof this[p] == 'function'){
						this[p](key[p]);					
					};
				};
			} else if(isFinite(key)){
				this.decimal(key);
			}
		} else if(typeof this[key] == 'function'){
			this[key](value);
		};
		return this;
	};

	/**
	* sets the invoking Color instance component values to a point between the original value and the destination Color instance component value, multiplied by the factor
	* @function
	* @param {Color} destination Color instance to serve as the termination of the interpolation
	* @param {number} factor 0-1, where 0 is the origin Color and 1 is the destination Color, and 0.5 is halfway between.  This method will "blend" the colors.
	* @returns this
	* @example
	* var orange = new Color('#FF9900');
	* var white = new Color('#FFFFFF');
	* orange.interpolate(white, 0.5);
	*/
	Color_t.prototype.interpolate = function(destination, factor){
		if(!(destination instanceof Color_t)){
			destination = new Color_t(destination);
		};
		this._red = absround( +(this._red) + (destination._red - this._red) * factor );
		this._green = absround( +(this._green) + (destination._green - this._green) * factor );
		this._blue = absround( +(this._blue) + (destination._blue - this._blue) * factor );
		this._alpha = absround( +(this._alpha) + (destination._alpha - this._alpha) * factor );
	//	this.broadcast(Events.RGB_UPDATED);
	//	this.broadcast(Events.UPDATED);
		return this;
	};

	Color_t.prototype._RGB2HSL = function(){

		var r = this._red / 255;
		var g = this._green / 255;
		var b = this._blue / 255;

		var max = Math.max(r, g, b);
		var min = Math.min(r, g, b);
		var l = (max + min) / 2;
		var v = max;

		if(max == min) {
			this._hue = 0;
			this._saturation = 0;
			this._lightness = absround(l * 100);
			this._brightness = absround(v * 100);
			return;
		};

		var d = max - min;
		var s = d / ( ( l <= 0.5) ? (max + min) : (2 - max - min) );
		var h = ((max == r)
			? (g - b) / d + (g < b ? 6 : 0)
			: (max == g)
			 ? ((b - r) / d + 2)
			 : ((r - g) / d + 4)) / 6;

		this._hue = absround(h * 360);
		this._saturation = absround(s * 100);
		this._lightness = absround(l * 100);
		this._brightness = absround(v * 100);
	};
	Color_t.prototype._HSL2RGB = function(){
		var h = this._hue / 360;
		var s = this._saturation / 100;
		var l = this._lightness / 100;
		var q = l < 0.5	? l * (1 + s) : (l + s - l * s);
		var p = 2 * l - q;
		this._red = absround(hue2rgb(p, q, h + 1/3) * 255);
		this._green = absround(hue2rgb(p, q, h) * 255);
		this._blue = absround(hue2rgb(p, q, h - 1/3) * 255);
	};
	Color_t.prototype._HSV2RGB = function(){  // http://mjijackson.com/2008/02/rgb-to-hsl-and-rgb-to-hsv-color-model-conversion-algorithms-in-javascript
		var h = this._hue / 360;
		var s = this._saturation / 100;
		var v = this._brightness / 100;
		var r = 0;
		var g = 0;
		var b = 0;
		var i = Math.floor(h * 6);
		var f = h * 6 - i;
		var p = v * (1 - s);
		var q = v * (1 - f * s);
		var t = v * (1 - (1 - f) * s);
		switch(i % 6){
			case 0 :
				r = v, g = t, b = p;
				break;
			case 1 :
				r = q, g = v, b = p;
				break;
			case 2 :
				r = p, g = v, b = t;
				break;
			case 3 :
				r = p, g = q, b = v;
				break;
			case 4 :
				r = t, g = p, b = v
				break;
			case 5 :
				r = v, g = p, b = q;
				break;
		}
		this._red = absround(r * 255);
		this._green = absround(g * 255);
		this._blue = absround(b * 255);
	};
	Color_t.prototype._INT2HEX = function(){
		var x = this._decimal.toString(16);
		x = '000000'.substr(0, 6 - x.length) + x;
		this._hex = '#' + x.toUpperCase();
	};
	Color_t.prototype._INT2RGB = function(){
		this._red = this._decimal >> 16;
		this._green = (this._decimal >> 8) & 0xFF;
		this._blue = this._decimal & 0xFF;
	};
	Color_t.prototype._HEX2INT = function(){
		this._decimal = parseInt(this._hex, 16);
	};
	Color_t.prototype._RGB2INT = function(){
		this._decimal = (this._red << 16 | (this._green << 8) & 0xffff | this._blue);
	};


	Color_t.prototype._RGBUpdated = function(){
		this._RGB2INT();  // populate INT values
		this._RGB2HSL();  // populate HSL values
		this._INT2HEX();  // populate HEX values
	};
	Color_t.prototype._HSLUpdated = function(){
		this._HSL2RGB();  // populate RGB values
		this._RGB2INT();  // populate INT values
		this._INT2HEX();  // populate HEX values
	};
	Color_t.prototype._HSVUpdated = function(){
		this._HSV2RGB();  // populate RGB values
		this._RGB2INT();  // populate INT values
		this._INT2HEX();  // populate HEX values
	};
	Color_t.prototype._HEXUpdated = function(){
		this._HEX2INT();  // populate INT values
		this._INT2RGB();  // populate RGB values
		this._RGB2HSL();  // populate HSL values
	};
	Color_t.prototype._INTUpdated = function(){
		this._INT2RGB();  // populate RGB values
		this._RGB2HSL();  // populate HSL values
		this._INT2HEX();  // populate HEX values
	};

	Color_t.prototype._broadcastUpdate = function(){
		//this.broadcast(Event.UPDATED);
	};

	/**
	* Set the decimal value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 (black) to 16777215 (white) - the decimal value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.decimal(123456);
	*/
	Color_t.prototype.decimal = function(value){
		return this._handle('_decimal', value, Events.INT_UPDATED);
	};

	/**
	* Set the hex value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {string} value Hex value to be set
	* @returns String
	* @example
	* var color = new Color();
	* color.hex('#FF9900');
	* color.hex('#CCC');
	*/
	Color_t.prototype.hex = function(value){
		return this._handle('_hex', value, Events.HEX_UPDATED);
	};

	/**
	* Set the red component value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 - 255 red component value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.red(125);
	*/
	Color_t.prototype.red = function(value){
		return this._handle('_red', value, Events.RGB_UPDATED);
	};
	/**
	* Set the green component value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 - 255 green component value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.green(125);
	*/
	Color_t.prototype.green = function(value){
		return this._handle('_green', value, Events.RGB_UPDATED);
	};
	/**
	* Set the blue component value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 - 255 blue component value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.blue(125);
	*/
	Color_t.prototype.blue = function(value){
		return this._handle('_blue', value, Events.RGB_UPDATED);
	};

	/**
	* Set the hue component value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 - 360 hue component value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.hue(280);
	*/
	Color_t.prototype.hue = function(value){
		return this._handle('_hue', value, Events.HSL_UPDATED);
	};
	
	/**
	* Set the saturation component value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 - 100 saturation component value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.saturation(280);
	*/
	Color_t.prototype.saturation = function(value){
		return this._handle('_saturation', value, Events.HSL_UPDATED);
	};
	
	/**
	* Set the lightness component value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 - 100 lightness component value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.lightness(80);
	*/	
	Color_t.prototype.lightness = function(value){
		return this._handle('_lightness', value, Events.HSL_UPDATED);
	};	
	
	/**
	* Set the brightness component value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 - 100 brightness component value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.brightness(80);
	*/
	Color_t.prototype.brightness = function(value){
		return this._handle('_brightness', value, Events.HSV_UPDATED);
	};

	/**
	* Set the opacity value of the color, updates all other components, and dispatches Event.UPDATED
	* @function
	* @param {number} value 0 - 1 opacity component value to set
	* @returns Number
	* @example
	* var color = new Color();
	* color.alpha(0.5);
	*/
	Color_t.prototype.alpha = function(value){
		return this._handle('_alpha', value);
	};

	
	Color_t.prototype._handle = function(prop, value, event){
		if(typeof this[prop] != 'undefined'){
			if(typeof value != 'undefined'){
				if(value != this[prop]){
					this[prop] = value;
					if(event){
			//			this.broadcast(event);
					};
				};
				//this.broadcast(Events.UPDATED);
			};
		};
		return this[prop];
	};
	
	/**
	* Returns a CSS-formatted hex string [e.g., #FF9900] from the Color's component values
	* @function
	* @returns String
	* @example
	* var color = new Color();
	* element.style.backgroundColor = color.getHex();
	*/
	Color_t.prototype.getHex = function(){
		return this._hex;
	};
	/**
	* Returns a CSS-formatted RGB string [e.g., rgb(255, 153, 0)] from the Color's component values
	* @function
	* @returns String
	* @example
	* var color = new Color();
	* element.style.backgroundColor = color.getRGB();
	*/
	Color_t.prototype.getRGB = function(){
		var components = [absround(this._red), absround(this._green), absround(this._blue)];
		return 'rgb(' + components.join(', ') + ')';
	};
	/**
	* Returns a CSS-formatted percentile RGB string [e.g., rgb(100%, 50%, 0)] from the Color's component values
	* @function
	* @returns String
	* @example
	* var color = new Color();
	* element.style.backgroundColor = color.getPRGB();
	*/
	Color_t.prototype.getPRGB = function(){
		var components = [absround(100 * this._red / 255) + '%', absround(100 * this._green / 255) + '%', absround(100 * this._blue / 255) + '%'];
		return 'rgb(' + components.join(', ') + ')';
	};
	/**
	* Returns a CSS-formatted RGBA string [e.g., rgba(255, 153, 0, 0.5)] from the Color's component values
	* @function
	* @returns String
	* @example
	* var color = new Color();
	* element.style.backgroundColor = color.getRGBA();
	*/
	Color_t.prototype.getRGBA = function(){
		var components = [absround(this._red), absround(this._green), absround(this._blue), this._alpha];
		return 'rgba(' + components.join(', ') + ')';
	};
	/**
	* Returns a CSS-formatted percentile RGBA string [e.g., rgba(100%, 50%, 0%, 0.5)] from the Color's component values
	* @function
	* @returns String
	* @example
	* var color = new Color();
	* element.style.backgroundColor = color.getPRGBA();
	*/
	Color_t.prototype.getPRGBA = function(){
		var components = [absround(100 * this._red / 255) + '%', absround(100 * this._green / 255) + '%', absround(100 * this._blue / 255) + '%', this._alpha];
		return 'rgba(' + components.join(', ') + ')';
	};
	/**
	* Returns a CSS-formatted HSL string [e.g., hsl(360, 100%, 100%)] from the Color's component values
	* @function
	* @returns String
	* @example
	* var color = new Color();
	* element.style.backgroundColor = color.getHSL();
	*/
	Color_t.prototype.getHSL = function(){
		var components = [absround(this._hue), absround(this._saturation) + '%', absround(this._lightness) + '%'];
		return 'hsl(' + components.join(', ') + ')';
	};
	/**
	* Returns a CSS-formatted HSLA string [e.g., hsl(360, 100%, 100%, 0.5)] from the Color's component values
	* @function
	* @returns String
	* @example
	* var color = new Color();
	* element.style.backgroundColor = color.getHSLA();
	*/
	Color_t.prototype.getHSLA = function(){
		var components = [absround(this._hue), absround(this._saturation) + '%', absround(this._lightness) + '%', this._alpha];
		return 'hsla(' + components.join(', ') + ')';
	};

	/**
	* Returns a tokenized string from the Color's component values
	* @function
	* @param {string} string The string to return, with tokens expressed as %token% that are replaced with component values.  Tokens are as follows:
	* r : red
	* g : green
	* b : blue
	* h : hue
	* s : saturation
	* l : lightness
	* v : brightness
	* a : alpha
	* x : hex
	* i : value
	* @returns String
	* @example
	* var color = new Color('#FF9900');
	* console.log(color.format('red=%r%, green=%g%, blue=%b%));
	*/
	Color_t.prototype.format = function(string){
		var tokens = {
			r : this._red,
			g : this._green,
			b : this._blue,
			h : this._hue,
			s : this._saturation,
			l : this._lightness,
			v : this._brightness,
			a : this._alpha,
			x : this._hex,
			d : this._decimal
		};
		for(var token in tokens){
			string = string.split('%' + token + '%').join(tokens[token]);
		};
		return string;
	};

	/**
	* Sets the format used by the native toString method
	* Color.HEX outputs #FF9900
	* Color.RGB outputs rgb(255, 153, 0)
	* Color.PRGB outputs rgb(100%, 50%, 0)
	* Color.RGBA outputs rgba(255, 153, 0, 0.5)
	* Color.PRGBA outputs rgba(100%, 50%, 0, 0.5)
	* Color.HSL outputs hsl(360, 100%, 80%)
	* Color.HSLA outputs hsla(360, 100%, 80%, 0.5)
	* @example
	* var color = new Color('#FF9900');
	* color.format = Color.RGB;
	* element.style.backgroundColor = color;
	* element.style.color = color;
	*/
	Color_t.prototype.output = 0;

	Color_t.HEX = 0;  // toString returns hex: #ABC123
	Color_t.RGB = 1;  // toString returns rgb: rgb(0, 100, 255)
	Color_t.PRGB = 2;  // toString returns percent rgb: rgb(0%, 40%, 100%)
	Color_t.RGBA = 3;  // toString returns rgba: rgba(0, 100, 255, 0.5)
	Color_t.PRGBA = 4;  // toString returns percent rgba: rgba(0%, 40%, 100%, 0.5)
	Color_t.HSL = 5;  // toString returns hsl: hsl(360, 50%, 50%)
	Color_t.HSLA = 6;  // toString returns hsla: hsla(360, 50%, 50%, 0.5)
	Color_t.INT = 7;  // toString returns decimal value

	Color_t.prototype.toString = function(){
		switch(this.output){
			case 0 :  // Color.HEX
				return this.getHex();
			case 1 :  // Color.RGB
				return this.getRGB();
			case 2 :  // Color.PRGB
				return this.getPRGB();
			case 3 :  // Color.RGBA
				return this.getRGBA();
			case 4 :  // Color.PRGBA
				return this.getPRGBA();
			case 5 :  // Color.HSL
				return this.getHSL();
			case 6 :  // Color.HSLA
				return this.getHSLA();
			case 7 :  // Color.INT
				return this._decimal;
		};
		return this.getHex();
	};
	
	

	/**
	* Blends the color from it's current state to the target Color over the duration
	* @function
	* @param {number} duration duration of tween in millisecond
	* @param {Color} color destination color
	* @returns number
	* @example 
	* var color = new Color('#FF9900');
	* color.tween(2000, '#FFFFFF');
	*/
	Color_t.prototype.tween = function(duration, color){
		if(!(color instanceof Color_t)){
			color = new Color_t(color);
		};
		var start = +(new Date());
		var ref = this;
		//this.broadcast('tweenStart');
		var interval = setInterval(function(){
			var ellapsed = +(new Date()) - start;
			var delta = Math.min(1, ellapsed / duration);
			ref.interpolate(color, delta);
			//ref.broadcast('tweenProgress');
			if(delta == 1){
				clearInterval(interval);
				//ref.broadcast('tweenComplete');
			};
		}, 20);
		return interval;  // return so it can be cancelled early
	};
	
    Color_t.lerp = function(a, b, t)
    {
        return a.interpolate(b, t);
    }
	
	Color_t.Events = Events;
	
	if (typeof define === 'function') {
		define('Color', [], function() {
			return Color_t;
		});
	};
	
	return Color_t;	
	
})(window);


class NumberRange
{
	constructor(from, to, step)
	{
		this.a = [];
		let index = 0;
		for(let i = from; i < to; i += step)
		{
			this.a[index] = i;
			index++;
		}
	}

	[Symbol.iterator]() { return this.a.values() }
}

//TODO: Move to functions.js or smth
//
//  functions
//

function lerp(v0, v1, t) {
    return v0*(1-t)+v1*t
}

//source: https://stackoverflow.com/questions/596467/how-do-i-convert-a-float-number-to-a-whole-number-in-javascript
function castToInt(value)
{
	return value | 0;
}

function toTime(timeInMilliseconds)
{
	timeInMilliseconds = castToInt(timeInMilliseconds);
	let result = "";
	if(timeInMilliseconds % 1000 != 0)
		result = timeInMilliseconds % 1000 + "ms ";
	timeInMilliseconds = castToInt(timeInMilliseconds / 1000);
	if(timeInMilliseconds % 60 != 0)
		result = timeInMilliseconds % 60 + "s " + result;
	timeInMilliseconds = castToInt(timeInMilliseconds / 60);
	if(timeInMilliseconds % 60 != 0)
		result = timeInMilliseconds % 60 + "m " + result;
	timeInMilliseconds = castToInt(timeInMilliseconds / 60);
	if (timeInMilliseconds % 24 != 0)
		result = timeInMilliseconds % 24 + "h " + result;
	timeInMilliseconds = castToInt(timeInMilliseconds / 24);
	if (timeInMilliseconds != 0)
		result = timeInMilliseconds + "d " + result;
	return result.trim();
}

function fixedWidthAny(source, length)
{
	let str = new Array(length + 1).join(" ");
	return String(str + source).slice(length);
}

function fixedWidthInt(source, length)
{
	let str = new Array(length + 1).join("0");
	return String(str + source).slice(length);
}

function stepBy(range, step)
{
	return new NumberRange(range.from, range.to, step);
}