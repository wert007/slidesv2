using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Code;
using Slides.MathExpressions;
using Slides.SVG;
using SVGLib;
using SVGLib.GraphicsElements;
using SVGGroup = SVGLib.ContainerElements.Group;
using SVGPath = SVGLib.GraphicsElements.Path;
using SVGText = SVGLib.GraphicsElements.Text;
using Color = Slides.Data.Color;
using Matrix = Slides.Data.Matrix;
using Vector2 = Slides.Data.Vector2;
using SVGColor = SVGLib.Datatypes.Color;
using SVGMatrix = SVGLib.Datatypes.Matrix;
using SVGVector2 = SVGLib.Datatypes.Vector2;
using SVGTransform = SVGLib.Datatypes.Transform;
using System.Text;
using SVGLib.ContainerElements;
using SVGLib.PathOperations;
using SVGLib.Datatypes;
using Slides.Elements;
using SVGLib.Filters;
using SVGLib.Filters.Lights;
using Slides.Elements.SVG;
using Slides.Styling;
using Slides.Data;

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
			Add(typeof(UnitPair));
			Add(typeof(UnitAddition));
			Add(typeof(Range));
			Add(typeof(MathFormula));

			Add(typeof(Style), CreateEmptySymbol("Style"));
			Add(typeof(CustomStyle), CreateEmptySymbol("Style"));
			Add(typeof(StdStyle), CreateEmptySymbol("Style"));
			Add(typeof(ElementKind), CreateEmptySymbol("ElementType"));
			Add(typeof(ChartType));
			Add(typeof(LibrarySymbol), CreateEmptySymbol("Library"));

			Add(typeof(BorderStyle));
			Add(typeof(Direction));
			Add(typeof(Time.TimeUnit));
			Add(typeof(Time));
			Add(typeof(Thickness));
			Add(typeof(Color));
			Add(typeof(Font));
			Add(typeof(ImageSource));
			Add(typeof(CSVFile));
			Add(typeof(Matrix));
			Add(typeof(Horizontal));
			Add(typeof(Vertical));
			Add(typeof(Orientation));
			Add(typeof(Interpolation));
			Add(typeof(Brush.BrushMode));
			Add(typeof(Brush), new TypeSymbol[] { LookSymbolUp(typeof(Color)), LookSymbolUp(typeof(ImageSource)) });
			Add(typeof(BorderLine));
			Add(typeof(Border));
			Add(typeof(Filter));

			//Add(typeof(FormattedString));
			
			Add(typeof(SVGColor));
			Add(typeof(SVGMatrix));

			Add(typeof(IFilterInput), CreateEmptySymbol("FilterInput"));
			var implementsIFilterInput = new TypeSymbol[] { LookSymbolUp(typeof(IFilterInput)) };
			Add(typeof(SVGFilter), canBeCastedTo: implementsIFilterInput);
			Add(typeof(EdgeMode));
			Add(typeof(ColorMatrixType));
			Add(typeof(GaussianBlurFilter));
			Add(typeof(ColorMatrixFilter));
			Add(typeof(ConvolveMatrixFilter));
			Add(typeof(ComponentTransferType));
			Add(typeof(ComponentTransferFilterChild));
			Add(typeof(ComponentTransferFilter));
			Add(typeof(MorphologyOperator));
			Add(typeof(MorphologyFilter));
			Add(typeof(FloodFilter));
			Add(typeof(StitchTiles));
			Add(typeof(TurbulenceType));
			Add(typeof(TurbulenceFilter));
			Add(typeof(Light));
			Add(typeof(PointLight));
			Add(typeof(DistantLight));
			Add(typeof(SpotLight));
			Add(typeof(DiffuseLightingFilter));
			Add(typeof(SpecularLightingFilter));
			Add(typeof(BlendMode));
			Add(typeof(BlendFilter));
			Add(typeof(CompositeOperator));
			Add(typeof(CompositeFilter));
			Add(typeof(DisplacementMapChannelSelector));
			Add(typeof(DisplacementMapFilter));
			Add(typeof(MergeFilter));
			Add(typeof(OffsetFilter));
			Add(typeof(TileFilter));

//			Add(typeof(ParentElement), CreateEmptySymbol(nameof(ParentElement)));
			Add(typeof(Element), canBeCastedTo: implementsIFilterInput);
			Add(typeof(Element), name: "any", canBeCastedTo: implementsIFilterInput);
			Add(typeof(TextElement));
			Add(typeof(ParentElement));
			Add(typeof(ImageStretching));
			Add(typeof(Image));
			Add(typeof(FlowAxis));
			Add(typeof(Stack));
			Add(typeof(Alignment));
			Add(typeof(Label));
			Add(typeof(Container));
			Add(typeof(SplittedContainer));
			Add(typeof(List.ListMarkerType));
			Add(typeof(List));
			Add(typeof(IFrame));
			Add(typeof(Slider));
			Add(typeof(YouTubeQuality));
			Add(typeof(YouTubePlayerParameters.ShowControls));
			Add(typeof(YouTubePlayerParameters.ListType));
			Add(typeof(YouTubePlayerParameters));
			Add(typeof(YouTubePlayer));
			Add(typeof(TableChild));
			Add(typeof(Table));
			Add(typeof(CaptionPlacement));
			Add(typeof(Captioned));

			Add(typeof(CodeBlock));

			Add(typeof(TransitionCall), CreateEmptySymbol("TransitionCall"));
			Add(typeof(TransitionSlide));
			Add(typeof(Transition));

			Add(typeof(Chart));
			Add(typeof(LineChart));

			Add(typeof(ViewBox));
			Add(typeof(SVGVector2));
			Add(typeof(LineCap));
			Add(typeof(SVGTransform), CreateEmptySymbol("Transform"));
			Add(typeof(SVGElementKind));
			Add(typeof(SVGElement));
			Add(typeof(SVGGraphicsElement));
			Add(typeof(SVGGroup));
			Add(typeof(AspectRatioAlign));
			Add(typeof(AspectRatioMeetOrSlice));
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
			//Less often used then SVGRect
			Add(typeof(UnitRect));
			Add(typeof(UnitLine));

			Add(typeof(Step), CreateEmptySymbol(nameof(Step)));
			Add(typeof(Template));
			Add(typeof(AnimationSymbol), CreateAnimationSymbol("Animation"));
			Add(typeof(SlideAttributes), name: "Slide");

			Add(typeof(CodeHighlighter));
			Add(typeof(Github.File), CreateEmptySymbol("GithubFile"));
			Add(typeof(GitFile));
			Add(typeof(GitRepository));
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
			parameters.Add(new VariableSymbol("element", false, LookSymbolUp(typeof(Element))));
			parameters.Add(new VariableSymbol("time", true, LookSymbolUp(typeof(Time))));
			parameters.Seal();
			functions.Add(new FunctionSymbol("play", parameters, PrimitiveTypeSymbol.Void));
			functions.Seal();
			return new AdvancedTypeSymbol(name, VariableSymbolCollection.Empty, FunctionSymbolCollection.Empty, functions);
		}

		private TypeSymbol CreateEmptySymbol(string name, bool isData = false)
		{
			var result = new AdvancedTypeSymbol(name, VariableSymbolCollection.Empty, FunctionSymbolCollection.Empty);
			return result;
		}


		private void Add(Type type, TypeSymbol[] canBeCastedTo = null, string name = null)
		{
			TypeSymbol symbol;
			if (name == null)
				name = type.Name;
			if (!type.IsEnum)
			{
				symbol = CreateAdvancedType(type, name, canBeCastedTo);
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

		private TypeSymbol CreateAdvancedType(Type type, string name, TypeSymbol[] canBeCastedTo)
		{
			TypeSymbol symbol;
			List<string> todoList = new List<string>();
			VariableSymbolCollection fields = new VariableSymbolCollection();
			foreach (var field in type.GetFields())
			{
				TypeSymbol fieldType = null;
				var fieldName = field.Name.ToVariableLower();
				if (fieldName.StartsWith("h_")) continue;
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
				var toAdd = new VariableSymbol(fieldName, field.IsInitOnly, fieldType);
				fields.Add(toAdd);
			}
			foreach (var prop in type.GetProperties())
			{
				TypeSymbol propertyType = null;
				var propName = prop.Name.ToVariableLower();
				if (propName.StartsWith("h_")) continue;
				if (prop.PropertyType != type)
				{
					propertyType = LookSymbolUp(prop.PropertyType);
					if (propName.StartsWith("n_"))
					{
						propertyType = new NoneableTypeSymbol(propertyType);
						propName = propName.Substring(2);
					}
				}
				else
				{
					todoList.Add(propName);
				}
				var toAdd = new VariableSymbol(propName, !prop.CanWrite, propertyType);
				fields.Add(toAdd);
			}
			fields.Seal();
			var constructor = new FunctionSymbolCollection();
			foreach (var typeConstructor in type.GetConstructors())
			{
				var parameter = new VariableSymbolCollection();
				foreach (var para in typeConstructor.GetParameters())
				{
					if (para.ParameterType == type)
						parameter.Add(new VariableSymbol(para.Name.ToVariableLower(), true, null));
					else
						parameter.Add(new VariableSymbol(para.Name.ToVariableLower(), true, LookSymbolUp(para.ParameterType)));
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
				if (mname.StartsWith("get_") || mname.StartsWith("set_") || mname.StartsWith("add_") || mname.StartsWith("h_"))
					continue;
				if (mname.StartsWith("op_"))
					continue;
				if (method.IsStatic)
					continue;
				mname = mname.ToVariableLower();
				var parameter = new VariableSymbolCollection();
				foreach (var para in method.GetParameters())
				{
					if (para.ParameterType == type)
					{
						todoList.Add(mname);
						parameter.Add(new VariableSymbol(para.Name.ToVariableLower(), true, null));
					}
					else
						parameter.Add(new VariableSymbol(para.Name.ToVariableLower(), true, LookSymbolUp(para.ParameterType)));
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
					//TODO: Add if array type or normal type...
					todoList.Add(mname);
				}
			}
			/*
			//TODO: Why do we have this? Isn't there already a applyStyle in Element?
			if (typeof(Element).IsAssignableFrom(type))
			{
				functions.Add(new FunctionSymbol("applyStyle", new VariableSymbol("style", true, LookSymbolUp(typeof(Style)), false), PrimitiveTypeSymbol.Void));
			}
			*/
			functions.Seal();
			var parent = type.BaseType;
			TypeSymbol parentSymbol = null;
			if (parent != null && parent != typeof(object) && parent != typeof(ValueType))
				parentSymbol = LookSymbolUp(parent);
			//TODO: remove todoList by adding a almost empty constructor to AdvancedTypeSymbol
			symbol = new AdvancedTypeSymbol(name, fields, constructor, functions, parentSymbol, canBeCastedTo);
			foreach (string todo in todoList)
			{
				if (fields.TryLookUp(todo, out VariableSymbol variable) && variable.Type == null)
					variable.Type = symbol;
				if (functions.TryLookUp(todo, out FunctionSymbol[] function))
					foreach (var f in function)
					{
						if (f.Type == null)
							f.Type = symbol;
						foreach (var p in f.Parameter)
						{
							if (p.Type == null)
								p.Type = symbol;
						}
					}
			}
			foreach (var c in constructor)
				foreach (var p in c.Parameter)
					if (p.Type == null)
						p.Type = symbol;
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