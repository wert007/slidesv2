namespace Minsk.CodeAnalysis.Syntax
{
	public class ArrayAccessExpressionSyntax : LExpressionSyntax
	{
		public ArrayAccessExpressionSyntax(ExpressionSyntax expression, SyntaxToken openBracketToken, ExpressionSyntax indexExpression, SyntaxToken closeBracketToken)
		{
			Child = expression;
			OpenBracketToken = openBracketToken;
			IndexExpression = indexExpression;
			CloseBracketToken = closeBracketToken;
		}


		public override SyntaxKind Kind => SyntaxKind.ArrayAccessExpression;
		public SyntaxKind ChildKind
		{
			get
			{
				if(Child.Kind == SyntaxKind.ArrayAccessExpression)
					return ((ArrayAccessExpressionSyntax)Child).ChildKind;
				return Child.Kind;
			}
		}
		public ExpressionSyntax Child { get; }
		public SyntaxToken OpenBracketToken { get; }
		public ExpressionSyntax IndexExpression { get; }
		public SyntaxToken CloseBracketToken { get; }

		public override bool IsLValue => Child.IsLValue;
	}
}