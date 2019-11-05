using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class AssignmentExpressionSyntax : ExpressionSyntax
	{
		public AssignmentExpressionSyntax(ImmutableArray<VariableExpressionSyntax> variables, ImmutableArray<SyntaxToken> commas, SyntaxToken equalsToken, ExpressionSyntax expression)
		{
			Variables = variables;
			Commas = commas;
			OperatorToken = equalsToken;
			Expression = expression;
		}

		public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
		public SyntaxToken OperatorToken { get; }
		public ExpressionSyntax Expression { get; }
		public ImmutableArray<VariableExpressionSyntax> Variables { get; }
		public ImmutableArray<SyntaxToken> Commas { get; }
	}
}