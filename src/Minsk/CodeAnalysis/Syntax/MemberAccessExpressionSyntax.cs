﻿namespace Minsk.CodeAnalysis.Syntax
{
	public interface ISettableExpression
	{

	}
	internal class MemberAccessExpressionSyntax : ExpressionSyntax, ISettableExpression
	{
		public MemberAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken periodToken, ExpressionSyntax member)
		{
			Expression = expression;
			PeriodToken = periodToken;
			Member = member;
		}
		public MemberAccessExpressionSyntax(ISettableExpression expression, SyntaxToken periodToken, ExpressionSyntax member)
		{
			Expression = (ExpressionSyntax)expression; 
							//TODO: THIS IS A FRICKING HACK!
							//Is it? I don't quite understand. ISettableExpression SHOULD be a Expression.
							//And every expression inherits from ExpressionSyntax. So I don't see the hack
							//here.
			PeriodToken = periodToken;
			Member = member;
		}

		public ExpressionSyntax Expression { get; }
		public SyntaxToken PeriodToken { get; }
		public ExpressionSyntax Member { get; }

		public override SyntaxKind Kind => SyntaxKind.MemberAccessExpression;
	}
}