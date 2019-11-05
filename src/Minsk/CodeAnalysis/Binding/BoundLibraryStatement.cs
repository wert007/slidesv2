using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundLibraryStatement : BoundStatement
	{
		public BoundLibraryStatement(VariableSymbol variable, BoundStatement boundBody)
		{
			Variable = variable;
			BoundBody = boundBody;
		}

		public VariableSymbol Variable { get; }
		public BoundStatement BoundBody { get; }

		public override BoundNodeKind Kind => BoundNodeKind.LibraryStatement;
	}
}