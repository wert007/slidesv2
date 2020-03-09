using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundSVGGroupStatement : BoundStatement
	{
		public BoundSVGGroupStatement(AdvancedTypeSymbol type, BoundParameterBlockStatement parameters, BoundBlockStatement body)
		{
			Type = type;
			Parameters = parameters;
			Body = body;
		}

		public override BoundNodeKind Kind => BoundNodeKind.SVGGroupStatement;

		public AdvancedTypeSymbol Type { get; }
		public BoundParameterBlockStatement Parameters { get; }
		public BoundBlockStatement Body { get; }
	}
}