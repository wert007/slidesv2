namespace Minsk.CodeAnalysis.Syntax
{
	public abstract class LExpressionSyntax : ExpressionSyntax
	{
		public override bool IsLValue => true;
	}
	public class VariableExpressionSyntax : LExpressionSyntax
	{
		public VariableExpressionSyntax(SyntaxToken preTildeToken, SyntaxToken identifier)
		{
			PreTildeToken = preTildeToken;
			Identifier = identifier;
		}

		public SyntaxToken PreTildeToken { get; }
		public SyntaxToken Identifier { get; }

		public override SyntaxKind Kind => SyntaxKind.VariableExpression;
	}
}