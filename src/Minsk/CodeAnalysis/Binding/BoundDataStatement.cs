using Minsk.CodeAnalysis.Symbols;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundDataStatement : BoundStatement
	{
		public BoundDataStatement(TypeSymbol type)
		{
			Type = type;
		}

		public TypeSymbol Type { get; }

		public override BoundNodeKind Kind => BoundNodeKind.DataStatement;

	}
}