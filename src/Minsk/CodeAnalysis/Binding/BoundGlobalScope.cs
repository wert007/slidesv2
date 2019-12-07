using Minsk.CodeAnalysis.Symbols;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundGlobalScope
	{
		public BoundGlobalScope(BoundGlobalScope previous, Diagnostic[] diagnostics, VariableSymbol[] variables, BoundStatement statement, Dictionary<VariableSymbol, BoundStatement> declarations)
		{
			Previous = previous;
			Diagnostics = diagnostics;
			Variables = variables;
			Statement = statement;
			Declarations = declarations;
		}

		public BoundGlobalScope Previous { get; }
		public Diagnostic[] Diagnostics { get; }
		public VariableSymbol[] Variables { get; }
		public BoundStatement Statement { get; }
		public Dictionary<VariableSymbol, BoundStatement> Declarations { get; }
	}
}
