using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundGroupStatement : BoundStatement
	{

		public BoundGroupStatement(TypeSymbol type, BoundParameterBlockStatement boundParameters, BoundBlockStatement boundBody)
		{
			Type = type;
			Parameters = boundParameters;
			Body = boundBody;
		}

		public override BoundNodeKind Kind => BoundNodeKind.GroupStatement;

		public TypeSymbol Type { get; }
		public BoundParameterBlockStatement Parameters { get; }
		public BoundBlockStatement Body { get; }
	}
}