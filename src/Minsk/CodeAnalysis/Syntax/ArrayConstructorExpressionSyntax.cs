using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax
{
	internal class ArrayConstructorExpressionSyntax : ExpressionSyntax
	{
		public ArrayConstructorExpressionSyntax(SyntaxToken openBracketToken, ImmutableArray<ExpressionSyntax> contents, SyntaxToken closeBracketToken)
		{
			OpenBracketToken = openBracketToken;
			Contents = contents;
			CloseBracketToken = closeBracketToken;
		}

		public override SyntaxKind Kind => SyntaxKind.ArrayConstructorExpression;

		public SyntaxToken OpenBracketToken { get; }
		public ImmutableArray<ExpressionSyntax> Contents { get; }
		public SyntaxToken CloseBracketToken { get; }
	}
}