namespace Minsk.CodeAnalysis.Syntax
{
	public class FunctionExpressionSyntax : MemberExpressionSyntax
	{
		public FunctionExpressionSyntax(SyntaxToken identifier, SyntaxToken openParenthesisToken, ExpressionSyntax[] arguments, SyntaxToken closeParenthesisToken)
		{
			Identifier = identifier;
			OpenParenthesisToken = openParenthesisToken;
			Arguments = arguments;
			CloseParenthesisToken = closeParenthesisToken;
		}

		public override SyntaxKind Kind => SyntaxKind.FunctionExpression;

		public override SyntaxToken Identifier { get; }
		public SyntaxToken OpenParenthesisToken { get; }
		public ExpressionSyntax[] Arguments { get; }
		public SyntaxToken CloseParenthesisToken { get; }
	}
}