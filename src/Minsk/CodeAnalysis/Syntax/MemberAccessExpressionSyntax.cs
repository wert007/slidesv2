namespace Minsk.CodeAnalysis.Syntax
{
	internal class MemberAccessExpressionSyntax : LExpressionSyntax
	{
		public MemberAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken periodToken, ExpressionSyntax member)
		{
			Expression = expression;
			PeriodToken = periodToken;
			Member = member;
		}

		public ExpressionSyntax Expression { get; }
		public SyntaxToken PeriodToken { get; }
		public ExpressionSyntax Member { get; }

		public override SyntaxKind Kind => SyntaxKind.MemberAccessExpression;

		public override bool IsLValue => Expression.IsLValue && Member.IsLValue;
	}
}