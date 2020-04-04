
namespace Minsk.CodeAnalysis.Syntax
{
	internal class ArrayConstructorExpressionSyntax : ExpressionSyntax
	{
		public ArrayConstructorExpressionSyntax(SyntaxToken openBracketToken, ExpressionSyntax[] contents, SyntaxToken closeBracketToken)
		{
			OpenBracketToken = openBracketToken;
			Contents = contents;
			CloseBracketToken = closeBracketToken;
		}

		public override SyntaxKind Kind => SyntaxKind.ArrayConstructorExpression;

		public SyntaxToken OpenBracketToken { get; }
		public ExpressionSyntax[] Contents { get; }
		public SyntaxToken CloseBracketToken { get; }

		public override bool IsLValue => false;
	}
}