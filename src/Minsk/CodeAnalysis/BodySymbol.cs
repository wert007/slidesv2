using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis
{
	public class BodySymbol
	{
		//TODO: Maybe use TypeSymbol instead of VariableSymbol
		internal BodySymbol(VariableSymbol symbol, BoundBlockStatement body)
		{
			Symbol = symbol;
			Body = body;
		}
		internal BodySymbol(TypeSymbol symbol, BoundBlockStatement body)
		{
			var variable = new VariableSymbol(symbol.Name, true, symbol, symbol.IsData);
			Symbol = variable;
			Body = body;
		}

		public VariableSymbol Symbol { get; }
		internal BoundBlockStatement Body { get; }
		public LibrarySymbol Source { get; set; }

		public override string ToString() => Symbol.ToString();
	}
}