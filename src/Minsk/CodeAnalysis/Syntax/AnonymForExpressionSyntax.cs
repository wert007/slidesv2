namespace Minsk.CodeAnalysis.Syntax
{
	internal class AnonymForExpressionSyntax : ExpressionSyntax
	{
		public AnonymForExpressionSyntax(SyntaxToken periodPeriodToken)
		{
			PeriodPeriodToken = periodPeriodToken;
		}

		public SyntaxToken PeriodPeriodToken { get; }

		public override SyntaxKind Kind => SyntaxKind.AnonymForExpression;

		public override bool IsLValue => false;
	}
}