namespace Minsk.CodeAnalysis.Syntax
{
	public class VariableExpressionSyntax : MemberExpressionSyntax, ISettableExpression
	{
		public VariableExpressionSyntax(SyntaxToken preTildeToken, SyntaxToken identifier, SyntaxToken postTildeToken, ArrayIndexExpressionSyntax arrayIndex)
		{
			PreTildeToken = preTildeToken;
			Identifier = identifier;
			ArrayIndex = arrayIndex;
			PostTildeToken = postTildeToken;
		}

		public SyntaxToken PreTildeToken { get; }
		public override SyntaxToken Identifier { get; }
		public ArrayIndexExpressionSyntax ArrayIndex { get; }
		public SyntaxToken PostTildeToken { get; }
		


		public override SyntaxKind Kind => SyntaxKind.VariableExpression;
	}

	public class ArrayIndexExpressionSyntax : ExpressionSyntax
	{
		public ArrayIndexExpressionSyntax(SyntaxToken openBracketToken, ExpressionSyntax index, SyntaxToken closeBracketToken, ArrayIndexExpressionSyntax arrayIndex)
		{
			OpenBracketToken = openBracketToken;
			Index = index;
			CloseBracketToken = closeBracketToken;
			ArrayIndex = arrayIndex;
		}

		public SyntaxToken OpenBracketToken { get; }
		public ExpressionSyntax Index { get; }
		public SyntaxToken CloseBracketToken { get; }
		public ArrayIndexExpressionSyntax ArrayIndex { get; }

		public override SyntaxKind Kind => SyntaxKind.ArrayIndexExpression;
	}
}