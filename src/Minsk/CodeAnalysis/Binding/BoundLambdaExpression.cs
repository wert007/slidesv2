using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundLambdaExpression : BoundExpression
	{
		public BoundLambdaExpression(VariableSymbol variable, BoundExpression expression)
		{
			Variable = variable;
			Expression = expression;
		}

		public override TypeSymbol Type => TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(Slides.MathTypes.LambdaFunction));

		public override BoundNodeKind Kind => BoundNodeKind.LambdaExpression;

		public VariableSymbol Variable { get; }
		public BoundExpression Expression { get; }
	}
}