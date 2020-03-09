using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	//TODO: Not really supported!
	internal class BoundLambdaExpression : BoundExpression
	{
		public BoundLambdaExpression(VariableSymbol variable, BoundExpression expression)
		{
			Variable = variable;
			Expression = expression;
		}

		public override TypeSymbol Type => TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(Slides.MathTypes.Polynom));

		public override BoundNodeKind Kind => BoundNodeKind.LambdaExpression;

		public VariableSymbol Variable { get; }
		public BoundExpression Expression { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			return false;
		}
	}
}