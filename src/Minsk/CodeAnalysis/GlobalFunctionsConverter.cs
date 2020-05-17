using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Minsk.CodeAnalysis
{
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
				var parameterSymbol = new VariableSymbol(parameter.Name, true, _typeConverter.LookSymbolUp(parameter.ParameterType), false);
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
	}
}
