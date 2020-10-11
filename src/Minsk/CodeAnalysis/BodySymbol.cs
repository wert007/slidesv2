using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis
{
	public class BodySymbol
	{
		//TODO(Minor): Maybe use TypeSymbol instead of VariableSymbol
		internal BodySymbol(VariableSymbol symbol, BoundBlockStatement body)
		{
			Symbol = symbol;
			Body = body;
		}
		internal BodySymbol(TypeSymbol symbol, BoundBlockStatement body)
		{
			var variable = new VariableSymbol(symbol.Name, true, symbol);
			Symbol = variable;
			Body = body;
		}

		public VariableSymbol Symbol { get; }
		internal BoundBlockStatement Body { get; }
		public LibrarySymbol Source { get; set; }

		public override string ToString() => Symbol.ToString();
	}
}