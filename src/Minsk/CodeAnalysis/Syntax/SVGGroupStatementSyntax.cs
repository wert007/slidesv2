namespace Minsk.CodeAnalysis.Syntax
{
	internal class SVGStatementSyntax : StatementSyntax
	{
		public SVGStatementSyntax(SyntaxToken svgGroupKeyword, SyntaxToken identifier, ParameterBlockStatementSyntax parameterStatement, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endSVGGroupKeyword)
		{
			SVGGroupKeyword = svgGroupKeyword;
			Identifier = identifier;
			ParameterStatement = parameterStatement;
			ColonToken = colonToken;
			Body = body;
			EndSVGGroupKeyword = endSVGGroupKeyword;
		}

		public override SyntaxKind Kind => SyntaxKind.SVGStatement;

		public SyntaxToken SVGGroupKeyword { get; }
		public SyntaxToken Identifier { get; }
		public ParameterBlockStatementSyntax ParameterStatement { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndSVGGroupKeyword { get; }
	}
}