using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Data;
using Slides.Elements;
using SVGLib.Filters;
using SVGLib.Filters.Lights;
using SVGLib.GraphicsElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Minsk.CodeAnalysis
{
	public abstract class Callable
	{
		public abstract object Call(FunctionSymbol function, object[] args);
	}

	public class GlobalFunctionCallable : Callable
	{
		public override object Call(FunctionSymbol function, object[] args)
		{
			TypeSymbol type;
			PrimitiveTypeSymbol pts;
			var _builtInTypes = BuiltInTypes.Instance;
			Debug.Assert(function.Parameter.Count == args.Length, $"Supplied argument count ({args.Length}) didn't match parameter count ({function.Parameter.Count}).");
			switch (function.Name)
			{
				case "gfont":
					return GlobalFunctions.gfont((string)args[0]);
				case "font":
					return GlobalFunctions.font((string)args[0]);
				case "join":
					return GlobalFunctions.join((string)args[0], (object[])args[1]);
				case "utf32":
					pts = (PrimitiveTypeSymbol)function.Parameter[0].Type;
					switch (pts.PrimitiveType)
					{
						case PrimitiveType.Integer:
							return GlobalFunctions.utf32((int)args[0]);
						case PrimitiveType.String:
							return GlobalFunctions.utf32((string)args[0]);
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with a parameter of type '{pts}'!");
					}
				case "contains":
					return GlobalFunctions.contains((string)args[0], (string)args[1]);
				case "toTime":
					return GlobalFunctions.toTime((int)args[0]);
				case "fixedWidth":
					return GlobalFunctions.fixedWidth(args[0], (int)args[1]);
				case "float":
					pts = (PrimitiveTypeSymbol)function.Parameter[0].Type;
					switch (pts.PrimitiveType)
					{
						case PrimitiveType.Integer:
							return GlobalFunctions.@float((int)args[0]);
						case PrimitiveType.Unit:
							return GlobalFunctions.@float((Unit)args[0]);
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with a parameter of type '{pts}'!");
					}
				case "int":
					return GlobalFunctions.@int((float)args[0]);
				case "mod":
					return GlobalFunctions.mod((int)args[0], (int)args[1]);
				case "min":
					pts = (PrimitiveTypeSymbol)function.Parameter[0].Type;
					switch (pts.PrimitiveType)
					{
						case PrimitiveType.Integer:
							return GlobalFunctions.min((int)args[0], (int)args[1]);
						case PrimitiveType.Float:
							return GlobalFunctions.min((float)args[0], (float)args[1]);
						case PrimitiveType.Unit:
							throw new NotImplementedException("min(Unit, Unit) needs to be implemented!");
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with a parameter of type '{pts}'!");
					}
				case "max":
					pts = (PrimitiveTypeSymbol)function.Parameter[0].Type;
					switch (pts.PrimitiveType)
					{
						case PrimitiveType.Integer:
							return GlobalFunctions.max((int)args[0], (int)args[1]);
						case PrimitiveType.Float:
							return GlobalFunctions.max((float)args[0], (float)args[1]);
						case PrimitiveType.Unit:
							return GlobalFunctions.max((Unit)args[0], (Unit)args[1]);
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with a parameter of type '{pts}'!");
					}
				case "csv":
					return GlobalFunctions.csv((string)args[0]);
				case "stepBy":
					return GlobalFunctions.stepBy((Range)args[0], (int)args[1]);
				case "video":
					return GlobalFunctions.video((string)args[0]);
				case "image":
					return GlobalFunctions.image((string)args[0]);
				case "crop":
					return GlobalFunctions.crop((ImageSource)args[0], (Unit)args[1], (Unit)args[2], (Unit)args[3], (Unit)args[4]);
				case "youtube":
					return GlobalFunctions.youtube((string)args[0], (YouTubeQuality)args[1]);
				case "println":
					switch (args.Length)
					{
						case 0:
							GlobalFunctions.println();
							return null;
						case 1:
							GlobalFunctions.println((string)args[0]);
							return null;
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with '{args.Length}' parameters!");
					}
				case "print":
					GlobalFunctions.print((string)args[0]);
					return null;
				case "hsl":
					return GlobalFunctions.hsl((int)args[0], (int)args[1], (int)args[2]);
				case "hsla":
					return GlobalFunctions.hsla((int)args[0], (int)args[1], (int)args[2], (int)args[3]);
				case "hex":
					return GlobalFunctions.hex((string)args[0]);
				case "rgb":
					return GlobalFunctions.rgb((int)args[0], (int)args[1], (int)args[2]);
				case "rgba":
					return GlobalFunctions.rgba((int)args[0], (int)args[1], (int)args[2], (int)args[3]);
				case "alpha":
					return GlobalFunctions.alpha((Color)args[0], (float)args[1]);
				case "identityMatrix":
					return GlobalFunctions.identityMatrix((int)args[0], (int)args[1]);
				case "matrix":
					return GlobalFunctions.matrix((float[])args[0], (int)args[1], (int)args[2]);
				case "px":
					return GlobalFunctions.px((float)args[0]);
				case "pt":
					return GlobalFunctions.pt((float)args[0]);
				case "pct":
					return GlobalFunctions.pct((float)args[0]);
				case "border":
					return GlobalFunctions.border((Unit)args[0], (BorderStyle)args[1], (Color)args[2]);
				case "padding":
					switch (args.Length)
					{
						case 1:
							return GlobalThicknessFunctions.padding((Unit)args[0]);
						case 2:
							return GlobalThicknessFunctions.padding((Unit)args[0], (Unit)args[1]);
						case 4:
							return GlobalThicknessFunctions.padding((Unit)args[0], (Unit)args[1], (Unit)args[2], (Unit)args[3]);
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with '{args.Length}' parameters!");
					}
				case "margin":
					switch (args.Length)
					{
						case 1:
							return GlobalThicknessFunctions.padding((Unit)args[0]);
						case 2:
							return GlobalThicknessFunctions.padding((Unit)args[0], (Unit)args[1]);
						case 4:
							return GlobalThicknessFunctions.padding((Unit)args[0], (Unit)args[1], (Unit)args[2], (Unit)args[3]);
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with '{args.Length}' parameters!");
					}
				case "flood":
					return GlobalFilterFunctions.flood((Color)args[0], (float)args[1]);
				case "turbulence":
					return GlobalFilterFunctions.turbulence((float)args[0], (int)args[1]);
				case "pointLight":
					return GlobalFilterFunctions.pointLight((float)args[0], (float)args[1], (float)args[2]);
				case "distantLight":
					return GlobalFilterFunctions.distantLight((float)args[0], (float)args[1]);
				case "spotLight":
					return GlobalFilterFunctions.spotLight((float)args[0], (float)args[1], (float)args[2], (float)args[3], (float)args[4], (float)args[5], (float)args[6], (float)args[7]);
				case "diffuseLight":
					return GlobalFilterFunctions.diffuseLight((IFilterInput)args[0], (float)args[1], (float)args[2], (Light)args[3]);
				case "specularLight":
					return GlobalFilterFunctions.specularLight((IFilterInput)args[0], (float)args[1], (float)args[2], (float)args[3], (Light)args[4]);
				case "blend":
					return GlobalFilterFunctions.blend((IFilterInput)args[0], (IFilterInput)args[1], (BlendMode)args[2]);
				case "convolve":
					return GlobalFilterFunctions.convolve((IFilterInput)args[0], (Matrix)args[1]);
				case "erode":
					return GlobalFilterFunctions.erode((IFilterInput)args[0], (float)args[1]);
				case "dilate":
					return GlobalFilterFunctions.dilate((IFilterInput)args[0], (float)args[1]);
				case "linearNode":
					return GlobalFilterFunctions.linearNode((float)args[0]);
				case "identityNode":
					return GlobalFilterFunctions.identityNode();
				case "gammaNode":
					return GlobalFilterFunctions.gammaNode((float)args[0], (float)args[1], (float)args[2]);
				case "tableNode":
					return GlobalFilterFunctions.tableNode((float[])args[0]);
				case "discreteNode":
					return GlobalFilterFunctions.discreteNode((float[])args[0]);
				case "transfer":
					return GlobalFilterFunctions.transfer((IFilterInput)args[0], (ComponentTransferFilterChild)args[1], (ComponentTransferFilterChild)args[2], (ComponentTransferFilterChild)args[3], (ComponentTransferFilterChild)args[4]);
				case "blur":
					switch (args.Length)
					{
						case 1:
							return GlobalFilterFunctions.blur((float)args[0]);
						case 2:
							return GlobalFilterFunctions.blur((IFilterInput)args[0], (float)args[1]);
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with '{args.Length}' parameters!");
					}
				case "saturate":
					switch (args.Length)
					{
						case 1:
							return GlobalFilterFunctions.saturate((float)args[0]);
						case 2:
							return GlobalFilterFunctions.saturate((IFilterInput)args[0], (float)args[1]);
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with '{args.Length}' parameters!");
					}
				case "colorMatrix":
					return GlobalFilterFunctions.colorMatrix((IFilterInput)args[0], (Matrix)args[1]);
				case "brightness":
					return GlobalFilterFunctions.brightness((float)args[0]);
				case "contrast":
					return GlobalFilterFunctions.contrast((float)args[0]);
				case "grayscale":
					return GlobalFilterFunctions.grayscale((float)args[0]);
				case "invert":
					return GlobalFilterFunctions.invert((float)args[0]);
				case "opacity":
					return GlobalFilterFunctions.opacity((float)args[0]);
				case "sepia":
					return GlobalFilterFunctions.sepia((float)args[0]);
				case "dropShadow":
					return GlobalFilterFunctions.dropShadow((int)args[0], (int)args[1], (int)args[2], (int)args[3], (Color)args[4]);
				case "hueRotate":
					return GlobalFilterFunctions.hueRotate((float)args[0]);
				case "rect":
					type = function.Parameter[0].Type; 
					if (type == PrimitiveTypeSymbol.Unit)
						return GlobalSVGFunctions.rect((Unit)args[0], (Unit)args[1]);
					else if (type == _builtInTypes.LookSymbolUp(typeof(UnitPair))) 
						return GlobalSVGFunctions.rect((UnitPair)args[0], (UnitPair)args[1]);
					else
						throw new NotImplementedException($"There is no '{function.Name}' with a parameter of type '{type}'!");
				case "line":
					switch (args.Length)
					{
						case 2:
							return GlobalSVGFunctions.line((UnitPair)args[0], (UnitPair)args[1]);
						case 4:
							return GlobalSVGFunctions.line((Unit)args[0], (Unit)args[1], (Unit)args[2], (Unit)args[3]);
						default:
							throw new NotImplementedException($"There is no '{function.Name}' with '{args.Length}' parameters!");
					}
				case "path":
					return GlobalSVGFunctions.path((string)args[0], (int)args[1], (int)args[2]);
				case "arrow":
					return GlobalSVGFunctions.arrow((Direction)args[0], (int)args[1], (int)args[2], (float)args[3], (float)args[4]);
				case "intersectPaths":
					return GlobalSVGFunctions.intersectPaths((Path)args[0], (Path)args[1]);
				case "unitePaths":
					return GlobalSVGFunctions.unitePaths((Path)args[0], (Path)args[1]);
				case "differPaths":
					return GlobalSVGFunctions.differPaths((Path)args[0], (Path)args[1]);
				case "loadSVG":
					return GlobalSVGFunctions.loadSVG((string)args[0]);
				case "toVertical":
				case "toHorizontal":
					throw new NotImplementedException($"'{function.Name}' is not implemented!");
				default:
					throw new NotImplementedException($"No implementation found for a global function '{function.Name}'!");
			}
		}
	}

	public class _GlobalFunctionsConverter
	{
		private static _GlobalFunctionsConverter _instance;
		public static _GlobalFunctionsConverter Instance
		{
			get
			{
				if (_instance == null)
					_instance = CreateFromXML("GlobalFunctions.xml");
				return _instance;
			}
		}

		private FunctionCallableCollection _functionCallableCollection;

		public _GlobalFunctionsConverter(FunctionCallableCollection functionCallableCollection)
		{
			_functionCallableCollection = functionCallableCollection;
		}

		public object Call(FunctionSymbol symbol, object[] args)
		{
			return _functionCallableCollection[symbol].Call(symbol, args);
		}

		public static _GlobalFunctionsConverter CreateFromXML(string fileName)
		{
			var functionCallableCollection = new FunctionCallableCollection();
			var callable = new GlobalFunctionCallable();

			void ReadUntilElementOrEndElement(XmlReader reader)
			{
				do
				{
					reader.Read();
				} while (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement);
			}

			using (var reader = XmlReader.Create(fileName))
			{
				reader.ReadStartElement("functions");
				ReadUntilElementOrEndElement(reader);
				while (!reader.EOF && reader.Name == "externFunction")
				{
					var fnName = reader["name"] ?? throw new NullReferenceException("No function name found!");
					var returnTypeName = reader["returnType"] ?? throw new NullReferenceException("No function returnType found!");
					var returnType = ResolveToType(fileName, fnName, returnTypeName);

					ReadUntilElementOrEndElement(reader);
					var parameter = new VariableSymbolCollection();
					while(!reader.EOF && reader.Name == "variable")
					{
						var varName = reader["name"] ?? throw new NullReferenceException("No parameter name found!");
						var varTypeName = reader["type"] ?? throw new NullReferenceException("No parameter type found!");
						TypeSymbol varType = ResolveToType(fileName, fnName, varTypeName);
						parameter.Add(new VariableSymbol(varName, true, varType));
						ReadUntilElementOrEndElement(reader);
					}
					parameter.Seal();
					var function = new FunctionSymbol(fnName, parameter, returnType);
					functionCallableCollection.Add(function, callable);
					ReadUntilElementOrEndElement(reader);
				}
			}
			var result = new _GlobalFunctionsConverter(functionCallableCollection);
			return result;
		}

		private static TypeSymbol ResolveToType(string fileName, string fnName, string typeName)
		{
			var _builtInTypes = BuiltInTypes.Instance;
			TypeSymbol type;
			if (typeName == "void")
				type = PrimitiveTypeSymbol.Void;
			else if (!_builtInTypes.TryGetSymbol(typeName.TrimEnd('[', ']', '?'), out type))
				throw new Exception($"Could not find type '{typeName}' for function '{fnName}' in file '{fileName}'");
			while (typeName.Last() == '?' || typeName.Last() == ']')
			{
				var last = typeName.Last();
				if (last == '?')
				{
					type = new NoneableTypeSymbol(type);
					typeName = typeName.Remove(typeName.Length - 1);
				}
				else if (last == ']')
				{
					type = new ArrayTypeSymbol(type);
					typeName = typeName.Remove(typeName.Length - 2);
				}
			}

			return type;
		}


		public bool TryGetSymbol(string name, out FunctionSymbol[] functions) => _functionCallableCollection.TryGetSymbol(name, out functions);
		public bool ContainsKey(FunctionSymbol key) => _functionCallableCollection.HasKey(key);
	}

	/*
	public class GlobalFunctionsConverter
	{
		private static GlobalFunctionsConverter _instance = null;

		public static GlobalFunctionsConverter Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new GlobalFunctionsConverter();
					_instance.Init();
				}
				return _instance;
			}
		}

		private Dictionary<FunctionSymbol, MethodInfo> _toMethod = new Dictionary<FunctionSymbol, MethodInfo>();
		private Dictionary<MethodInfo, FunctionSymbol> _toSymbol = new Dictionary<MethodInfo, FunctionSymbol>();
		private Dictionary<string, FunctionSymbol[]> _fromString = new Dictionary<string, FunctionSymbol[]>();
		private BuiltInTypes _typeConverter = BuiltInTypes.Instance;

		private GlobalFunctionsConverter()
		{
		}

		private void Init()
		{
			var functions = typeof(GlobalFunctions).GetMethods().Concat(typeof(GlobalThicknessFunctions).GetMethods()).Concat(typeof(GlobalFilterFunctions).GetMethods()).Concat(typeof(GlobalSVGFunctions).GetMethods());
			foreach (var function in functions)
			{
				var name = function.Name;
				if (name == "Equals" ||
					name == "ToString" ||
					name == "GetHashCode" ||
					name == "GetType")
					continue;
				Add(function);
			}
		}

		private void Add(MethodInfo function)
		{
			var parameters = new VariableSymbolCollection();
			foreach (var parameter in function.GetParameters())
			{
				var parameterSymbol = new VariableSymbol(parameter.Name, true, _typeConverter.LookSymbolUp(parameter.ParameterType));
				parameters.Add(parameterSymbol);
			}
			parameters.Seal();
			Type returnType = function.ReturnType;
			if(returnType.Name == typeof(ImportExpression<>).Name)
			{
				returnType = function.ReturnType.GenericTypeArguments[0];
			}
			var symbol = new FunctionSymbol(function.Name, parameters, _typeConverter.LookSymbolUp(returnType));
			Add(function, symbol);
		}

		private void Add(MethodInfo function, FunctionSymbol symbol)
		{
			_toMethod.Add(symbol, function);
			_toSymbol.Add(function, symbol);
			if(_fromString.ContainsKey(symbol.Name))
				_fromString[symbol.Name] = _fromString[symbol.Name].Concat(new FunctionSymbol[] { symbol }).ToArray();
			else
			_fromString.Add(symbol.Name, new FunctionSymbol[] { symbol});
		}

		public bool ContainsSymbol(FunctionSymbol function)
		{
			if (_toMethod.ContainsKey(function))
				return true;
			if (_toSymbol.ContainsValue(function))
				return true;
			return false;
		}

		public bool ContainsMethod(MethodInfo method)
		{
			if (_toMethod.ContainsValue(method))
				return true;
			if (_toSymbol.ContainsKey(method))
				return true;
			return false;
		}

		public void AddType(TypeSymbol type)
		{

		}

		public FunctionSymbol[] FromString(string name)
		{
			return _fromString[name];
		}

		public FunctionSymbol LookSymbolUp(MethodInfo method)
		{
			return _toSymbol[method];
		}
		public MethodInfo LookMethodInfoUp(FunctionSymbol symbol)
		{
			var result = _toMethod[symbol];
			return result;
		}

		public bool TryGetSymbol(string name, out FunctionSymbol[] function)
		{
			return _fromString.TryGetValue(name, out function);
		}
	}*/
}
