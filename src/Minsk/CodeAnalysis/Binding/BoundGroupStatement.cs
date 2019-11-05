using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundGroupStatement : BoundStatement
	{

		public BoundGroupStatement(TypeSymbol type, BoundParameterBlockStatement boundParameters, BoundBlockStatement boundBody)
		{
			Type = type;
			BoundParameters = boundParameters;
			BoundBody = boundBody;
		}

		public override BoundNodeKind Kind => BoundNodeKind.GroupStatement;

		public TypeSymbol Type { get; }
		public BoundParameterBlockStatement BoundParameters { get; }
		public BoundBlockStatement BoundBody { get; }
	}
}