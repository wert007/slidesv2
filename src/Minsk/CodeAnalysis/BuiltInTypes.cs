using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Code;
using Slides.Filters;
using Slides.MathExpressions;
using Slides.SVG;
using SVGLib;
using SVGLib.GraphicsElements;
using SVGGroup = SVGLib.ContainerElements.Group;
using SVGPath = SVGLib.GraphicsElements.Path;
using SVGText = SVGLib.GraphicsElements.Text;
using Color = Slides.Color;
using Vector2 = Slides.Vector2;
using SVGColor = SVGLib.Datatypes.Color;
using SVGVector2 = SVGLib.Datatypes.Vector2;
using SVGTransform = SVGLib.Datatypes.Transform;
using System.Text;
using SVGLib.ContainerElements;
using SVGLib.PathOperations;
using SVGLib.Datatypes;
using Slides.Elements;

namespace Minsk.CodeAnalysis
{
	public static class TextHelper
	{


		public static string ToVariableLower(this string str)
		{
			if (char.IsUpper(str[0]))
			{
				var temp = str.ToCharArray();
				temp[0] = char.ToLower(temp[0]);
				return new string(temp);
			}
			else return str;
		}
		public static string ToVariableUpper(this string str)
		{
			if (char.IsLower(str[0]))
			{
				var temp = str.ToCharArray();
				temp[0] = char.ToUpper(temp[0]);
				return new string(temp);
			}
			else return str;
		}
	}
	public class BuiltInTypes
	{
		private Dictionary<TypeSymbol, Type> _toType;
		private Dictionary<Type, TypeSymbol> _toSymbol;

		private static BuiltInTypes _instance = null;

		public static BuiltInTypes Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new BuiltInTypes();
					_instance.Init();
				}
				return _instance;
			}
		}

		private BuiltInTypes()
		{
			_toType = new Dictionary<TypeSymbol, Type>();
			_toSymbol = new Dictionary<Type, TypeSymbol>();
		}

		private void Init()
		{
			Add(typeof(int), PrimitiveTypeSymbol.Integer);
			Add(typeof(byte), PrimitiveTypeSymbol.Integer);
			Add(typeof(string), PrimitiveTypeSymbol.String);
			Add(typeof(bool), PrimitiveTypeSymbol.Bool);
			Add(typeof(float), PrimitiveTypeSymbol.Float);
			//This is only for the SVGLibrary. Maybe we should change
			//how we add svg types to the type table?
			Add(typeof(double), PrimitiveTypeSymbol.Float);
			Add(typeof(void), PrimitiveTypeSymbol.Void);
			Add(typeof(object), PrimitiveTypeSymbol.Object);

			//Maybe allow the user to use Font.Measure(string, Unit) -> Vector
			//It's the only reference right now to Vector
			Add(typeof(Vector2), CreateEmptySymbol(nameof(Vector2)));
			Add(typeof(Unit), PrimitiveTypeSymbol.Unit);
			Add(typeof(Unit.UnitKind));
			Add(typeof(UnitAddition));
			Add(typeof(Range), isData:true);
			Add(typeof(MathFormula), isData: true);

			Add(typeof(CustomStyle), CreateEmptySymbol("Style"));
			Add(typeof(StdStyle), CreateEmptySymbol("Style"));
			Add(typeof(ElementKind), CreateEmptySymbol("ElementType"));
			Add(typeof(ChartType));
			Add(typeof(LibrarySymbol), CreateEmptySymbol("Library"));

			Add(typeof(BorderStyle));
			Add(typeof(Direction));
			Add(typeof(Time.TimeUnit));
			Add(typeof(Time), isData: true);
			Add(typeof(Thickness));
			Add(typeof(Color), isData: true);
			Add(typeof(Font), isData: true);
			Add(typeof(ImageSource), isData: true);
			Add(typeof(CSVFile), isData: true);
			Add(typeof(Matrix), isData: true);
			Add(typeof(Horizontal));
			Add(typeof(Vertical));
			Add(typeof(Orientation));
			Add(typeof(Interpolation));

			Add(typeof(Filter), isData: true);
			Add(typeof(IFilterInput), CreateEmptySymbol("FilterInput"));
			var implementsIFilterInput = new TypeSymbol[] { LookSymbolUp(typeof(IFilterInput)) };
			Add(typeof(SVGFilter), isData: true, canBeCastedTo: implementsIFilterInput);
			Add(typeof(EdgeMode), isData: true);
			Add(typeof(ColorMatrixType), isData: true);
			Add(typeof(GaussianBlurFilter), isData: true);
			Add(typeof(ColorMatrixFilter), isData: true);
			Add(typeof(ConvolveMatrixFilter), isData: true);
			Add(typeof(ComponentTransferType), isData: true);
			Add(typeof(ComponentTransferFilterChild), isData: true);
			Add(typeof(ComponentTransferFilter), isData: true);
			Add(typeof(MorphologyOperator), isData: true);
			Add(typeof(MorphologyFilter), isData: true);
			Add(typeof(FloodFilter), isData: true);
			Add(typeof(StitchTiles), isData: true);
			Add(typeof(TurbulenceType), isData: true);
			Add(typeof(TurbulenceFilter), isData: true);
			Add(typeof(Light), isData: true);
			Add(typeof(PointLight), isData: true);
			Add(typeof(DistantLight), isData: true);
			Add(typeof(SpotLight), isData: true);
			Add(typeof(DiffuseLightingFilter), isData: true);
			Add(typeof(SpecularLightingFilter), isData: true);
			Add(typeof(BlendMode), isData: true);
			Add(typeof(BlendFilter), isData: true);
			Add(typeof(CompositeOperator), isData: true);
			Add(typeof(CompositeFilter), isData: true);
			Add(typeof(DisplacementMapChannelSelector), isData: true);
			Add(typeof(DisplacementMapFilter), isData: true);
			Add(typeof(MergeFilter), isData: true);
			Add(typeof(OffsetFilter), isData: true);
			Add(typeof(TileFilter), isData: true);

			Add(typeof(Brush.BrushMode));
			Add(typeof(Brush), new TypeSymbol[] { LookSymbolUp(typeof(Color)), LookSymbolUp(typeof(ImageSource)) });
			Add(typeof(Element), canBeCastedTo: implementsIFilterInput);
			Add(typeof(Element), name: "any", canBeCastedTo: implementsIFilterInput);
			Add(typeof(ImageStretching));
			Add(typeof(Image));
			Add(typeof(FlowAxis));
			Add(typeof(Stack));
			Add(typeof(Alignment));
			Add(typeof(Label));
			Add(typeof(Container));
			Add(typeof(SplittedContainer));
			Add(typeof(List));
			Add(typeof(IFrame));
			Add(typeof(Slider));
			Add(typeof(TableChild));
			Add(typeof(Table));

			Add(typeof(CodeBlock));

			Add(typeof(TransitionCall), CreateEmptySymbol("TransitionCall"));
			Add(typeof(Transition), isData: false);

			Add(typeof(Chart));
			Add(typeof(LineChart));

			Add(typeof(SVGColor));
			Add(typeof(ViewBox));
			Add(typeof(SVGVector2));
			Add(typeof(SVGTransform), CreateEmptySymbol("Transform"));
			Add(typeof(SVGElementKind));
			Add(typeof(SVGElement));
			Add(typeof(SVGGraphicsElement));
			Add(typeof(SVGGroup));
			Add(typeof(SVGTag));
			Add(typeof(BasicShape), CreateSVGShapePlaceholder());
			Add(typeof(PathOperationKind));
			Add(typeof(PathOperation));
			Add(typeof(SVGPath), name: "Path");
			Add(typeof(BasicShape));
			Add(typeof(Rect));
			Add(typeof(Circle));
			Add(typeof(Ellipse));
			Add(typeof(Line));
			Add(typeof(Polyline));
			Add(typeof(Polygon));
			Add(typeof(SVGText), name: "Text");

			Add(typeof(SVGContainer));

			Add(typeof(MathPlot));

			Add(typeof(Step), CreateEmptySymbol(nameof(Step)));
			Add(typeof(Template));
			Add(typeof(AnimationSymbol), CreateAnimationSymbol("Animation"));
			Add(typeof(SlideAttributes), name: "Slide");
			Add(typeof(StyleSlideAttributes),name: "_Slide", fieldsAreNoneable: true);

			Add(typeof(CodeHighlighter));
			Add(typeof(Github.File), CreateEmptySymbol("GithubFile"));
			Add(typeof(GitFile), isData:true);
			Add(typeof(GitRepository), isData:true);
		}

		private TypeSymbol CreateSVGShapePlaceholder()
		{
			var parent = LookSymbolUp(typeof(SVGGraphicsElement));
			var fields = new VariableSymbolCollection(); ;
			var constructor = new FunctionSymbolCollection();
			var functions = new FunctionSymbolCollection();
			return new AdvancedTypeSymbol("SVGShape", fields, constructor, functions, parent);
		}

		public bool ContainsSymbol(TypeSymbol type)
		{
			if (_toType.ContainsKey(type))
				return true;
			if (_toSymbol.ContainsValue(type))
				return true;
			return false;
		}

		public bool ContainsType(Type type)
		{
			if (_toType.ContainsValue(type))
				return true;
			if (_toSymbol.ContainsKey(type))
				return true;
			return false;
		}

		private AdvancedTypeSymbol CreateAnimationSymbol(string name)
		{
			var functions = new FunctionSymbolCollection();
			var parameters = new VariableSymbolCollection();
			parameters.Add(new VariableSymbol("element", false, LookSymbolUp(typeof(Element)), false));
			parameters.Add(new VariableSymbol("time", true, LookSymbolUp(typeof(Time)), false));
			parameters.Seal();
			functions.Add(new FunctionSymbol("play", parameters, PrimitiveTypeSymbol.Void));
			functions.Seal();
			return new AdvancedTypeSymbol(name, VariableSymbolCollection.Empty, FunctionSymbolCollection.Empty, functions);
		}

		private TypeSymbol CreateEmptySymbol(string name, bool isData = false)
		{
			var result = new AdvancedTypeSymbol(name, VariableSymbolCollection.Empty, FunctionSymbolCollection.Empty);
			result.SetData(isData);
			return result;
		}


		private void Add(Type type, TypeSymbol[] canBeCastedTo = null, bool isData = false, string name = null, bool fieldsAreNoneable = false)
		{
			TypeSymbol symbol;
			if (name == null)
				name = type.Name;
			if (!type.IsEnum)
			{
				symbol = CreateAdvancedType(type, name, canBeCastedTo, fieldsAreNoneable);
				((AdvancedTypeSymbol)symbol).SetData(isData);
			}
			else
			{
				symbol = CreateEnumType(type, name);
			}
			Add(type, symbol);

		}

		private static TypeSymbol CreateEnumType(Type type, string name)
		{
			TypeSymbol symbol;
			var values = type.GetEnumValues();
			var names = new string[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				names[i] = values.GetValue(i).ToString();
			}
			symbol = new EnumTypeSymbol(name, names);
			return symbol;
		}

		public bool TryGetSymbol(string name, out TypeSymbol type)
		{
			foreach (var symbol in _toType.Keys)
			{
				if (symbol.Name == name)
				{
					type = symbol;
					return true;
				}
			}
			type = null;
			return false;
		}

		private TypeSymbol CreateAdvancedType(Type type, string name, TypeSymbol[] canBeCastedTo, bool toLower = true)
		{
			TypeSymbol symbol;
			List<string> todoList = new List<string>();
			VariableSymbolCollection fields = new VariableSymbolCollection();
			foreach (var field in type.GetFields())
			{
				TypeSymbol fieldType = null;
				var fieldName = field.Name.ToVariableLower();
				if (field.FieldType != type)
				{
					fieldType = LookSymbolUp(field.FieldType);
					if (fieldName.StartsWith("n_"))
						fieldType = new NoneableTypeSymbol(fieldType);
				}
				else
				{
					todoList.Add(fieldName);
				}
				var toAdd = new VariableSymbol(fieldName, field.IsInitOnly, fieldType, false);
				toAdd.NeedsDataFlag = false;
				fields.Add(toAdd);
			}
			foreach (var prop in type.GetProperties())
			{
				TypeSymbol propertyType = null;
				var propName = prop.Name.ToVariableLower();
				if (prop.PropertyType != type)
				{
					propertyType = LookSymbolUp(prop.PropertyType);
					if (propName.StartsWith("n_"))
						propertyType = new NoneableTypeSymbol(propertyType);
				}
				else
				{
					todoList.Add(propName);
				}
				var toAdd = new VariableSymbol(propName, !prop.CanWrite, propertyType, false);
				toAdd.NeedsDataFlag = false;
				fields.Add(toAdd);
			}
			fields.Seal();
			var constructor = new FunctionSymbolCollection();
			foreach (var typeConstructor in type.GetConstructors())
			{
				var parameter = new VariableSymbolCollection();
				foreach (var para in typeConstructor.GetParameters())
				{
					parameter.Add(new VariableSymbol(para.Name.ToVariableLower(), true, LookSymbolUp(para.ParameterType), false));
				}
				parameter.Seal();
				constructor.Add(new FunctionSymbol("constructor", parameter, null));
				ConstructorSymbolConverter.Instance.AddConstructor(typeConstructor, constructor.Last());
			}
			FunctionSymbolCollection functions = new FunctionSymbolCollection();
			foreach (var method in type.GetMethods())
			{
				var mname = method.Name;
				if (mname == "Equals" ||
					mname == "ToString" ||
					mname == "GetHashCode" ||
					mname == "GetType")
					continue;
				if (mname.StartsWith("get_") || mname.StartsWith("set_") || mname.StartsWith("add_"))
					continue;
				if (mname.StartsWith("op_"))
					continue;
				if (method.IsStatic)
					continue;
				mname = mname.ToVariableLower();
				var parameter = new VariableSymbolCollection();
				foreach (var para in method.GetParameters())
				{
					parameter.Add(new VariableSymbol(para.Name.ToVariableLower(), true, LookSymbolUp(para.ParameterType), false));
				}
				parameter.Seal();
				if (method.ReturnType != type)
				{
					var returnType = LookSymbolUp(method.ReturnType);
					if (mname == "getData")
						returnType = new NoneableTypeSymbol(returnType);
					functions.Add(new FunctionSymbol(mname, parameter, returnType));
				}
				else
				{
					todoList.Add(mname);
				}
			}
			if (typeof(Element).IsAssignableFrom(type))
			{
				functions.Add(new FunctionSymbol("applyStyle", new VariableSymbol("style", true, LookSymbolUp(typeof(StdStyle)), false), PrimitiveTypeSymbol.Void));
			}
			functions.Seal();
			var parent = type.BaseType;
			TypeSymbol parentSymbol = null;
			if (parent != null && parent != typeof(object) && parent != typeof(ValueType))
				parentSymbol = LookSymbolUp(parent);
			symbol = new AdvancedTypeSymbol(name, fields, constructor, functions, parentSymbol, canBeCastedTo);
			foreach (string todo in todoList)
			{
				if (fields.TryLookUp(todo, out VariableSymbol variable) && variable.Type == null)
					variable.Type = symbol;
				if (functions.TryLookUp(todo, out FunctionSymbol[] function))
					foreach (var f in function)
						if (f.Type == null)
							f.Type = symbol;
			}
			foreach (var c in constructor)
			{
				c.Type = symbol;
			}
			return symbol;
		}
		private void Add(Type type, TypeSymbol symbol)
		{
			if (!_toType.ContainsKey(symbol))
				_toType.Add(symbol, type);
			if (!_toSymbol.ContainsKey(type))
				_toSymbol.Add(type, symbol);
			return;
		}

		public TypeSymbol LookSymbolUp(Type type)
		{
			if (type.IsArray)
				return new ArrayTypeSymbol(LookSymbolUp(type.GetElementType()));
			if(type.Name == "Nullable`1")
			{
				return new NoneableTypeSymbol(LookSymbolUp(type.GenericTypeArguments[0]));
			}
			if (!_toSymbol.ContainsKey(type))
				throw new Exception();
			return _toSymbol[type];
		}

		public Type LookTypeUp(TypeSymbol symbol)
		{
			if(symbol.Type == TypeType.Array)
			{
				var arrSym = symbol as ArrayTypeSymbol;
				var res = LookTypeUp(arrSym.Child);
				return res.MakeArrayType();
			}
			if (symbol.Type == TypeType.Tuple)
				throw new Exception();
			if (!_toType.ContainsKey(symbol))
				throw new Exception();
			return _toType[symbol];
		}

		public Type TryLookTypeUp(TypeSymbol symbol)
		{
			if (symbol.Type == TypeType.Tuple)
				throw new Exception();
			if (!_toType.ContainsKey(symbol))
				return null;
			return _toType[symbol];
		}




	}
}