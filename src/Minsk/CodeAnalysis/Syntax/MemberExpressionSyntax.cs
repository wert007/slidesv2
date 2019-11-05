namespace Minsk.CodeAnalysis.Syntax
{
	public abstract class MemberExpressionSyntax : ExpressionSyntax
	{
		public abstract SyntaxToken Identifier { get; }
	}
}