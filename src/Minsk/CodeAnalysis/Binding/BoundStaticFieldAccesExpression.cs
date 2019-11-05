using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundStaticFieldAccesExpression : BoundExpression
	{
		public BoundStaticFieldAccesExpression(AdvancedTypeSymbol baseType, VariableSymbol field)
		{
			BaseType = baseType;
			Field = field;
		}

		public override TypeSymbol Type => Field.Type;

		public override BoundNodeKind Kind => BoundNodeKind.StaticFieldAccessExpression;

		public AdvancedTypeSymbol BaseType { get; }
		public VariableSymbol Field { get; }
	}
}