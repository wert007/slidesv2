namespace Minsk.CodeAnalysis.Syntax
{
	internal class ConstructorExpressionSyntax : ExpressionSyntax
	{
		public ConstructorExpressionSyntax(SyntaxToken newKeyword, ExpressionSyntax functionCall)
		{
			NewKeyword = newKeyword;
			FunctionCall = functionCall;
		}

		public override SyntaxKind Kind => SyntaxKind.ConstructorExpression;

		public SyntaxToken NewKeyword { get; }
		public ExpressionSyntax FunctionCall { get; }
	}
}