using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope(BoundGlobalScope previous, Diagnostic[] diagnostics, VariableSymbol[] variables, BoundStatement statement)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            Variables = variables;
            Statement = statement;
        }

        public BoundGlobalScope Previous { get; }
        public Diagnostic[] Diagnostics { get; }
        public VariableSymbol[] Variables { get; }
        public BoundStatement Statement { get; }
    }
}
