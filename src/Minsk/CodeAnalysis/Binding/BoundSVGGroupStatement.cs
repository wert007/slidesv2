using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundSVGStatement : BoundStatement
	{
		public BoundSVGStatement(AdvancedTypeSymbol type, BoundParameterBlockStatement parameters, BoundBlockStatement body)
		{
			Type = type;
			Parameters = parameters;
			Body = body;
		}

		public override BoundNodeKind Kind => BoundNodeKind.SVGStatement;

		public AdvancedTypeSymbol Type { get; }
		public BoundParameterBlockStatement Parameters { get; }
		public BoundBlockStatement Body { get; }
	}
}