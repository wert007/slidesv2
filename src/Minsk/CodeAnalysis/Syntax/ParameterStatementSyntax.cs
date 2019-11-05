using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax
{
	internal class ParameterStatementSyntax : StatementSyntax
	{
		public ParameterStatementSyntax(VariableExpressionSyntax variable, TypeDeclarationSyntax typeDeclaration, SyntaxToken equalsToken, ExpressionSyntax initializer)
		{
			Variable = variable;
			TypeDeclaration = typeDeclaration;
			EqualsToken = equalsToken;
			Initializer = initializer;
		}

		public override SyntaxKind Kind => SyntaxKind.ParameterStatement;

		public VariableExpressionSyntax Variable { get; }
		public TypeDeclarationSyntax TypeDeclaration { get; }
		public SyntaxToken EqualsToken { get; }
		public ExpressionSyntax Initializer { get; }
	}
}