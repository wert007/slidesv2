using System.Collections.Generic;
using System.Reflection;

namespace Minsk.CodeAnalysis
{
	public class ConstructorSymbolConverter
	{
		private static ConstructorSymbolConverter _instance = null;

		public static ConstructorSymbolConverter Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new ConstructorSymbolConverter();
				}
				return _instance;
			}
		}

		private Dictionary<FunctionSymbol, ConstructorInfo> _toConstructor = new Dictionary<FunctionSymbol, ConstructorInfo>();
		private Dictionary<ConstructorInfo, FunctionSymbol> _toConstructorSymbol = new Dictionary<ConstructorInfo, FunctionSymbol>();

		private ConstructorSymbolConverter()
		{

		}

		public void AddConstructor(ConstructorInfo typeConstructor, FunctionSymbol functionSymbol)
		{
			if(!_toConstructor.ContainsKey(functionSymbol))
				_toConstructor.Add(functionSymbol, typeConstructor);
			if(!_toConstructorSymbol.ContainsKey(typeConstructor))
				_toConstructorSymbol.Add(typeConstructor, functionSymbol);
		}

		public FunctionSymbol LookConstructorSymbolUp(ConstructorInfo constructor)
		{
			return _toConstructorSymbol[constructor];
		}

		public ConstructorInfo LookConstructorInfoUp(FunctionSymbol symbol)
		{
			return _toConstructor[symbol];
		}
	}
}
