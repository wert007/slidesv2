using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class SlideStatementSyntax : StatementSyntax
	{
		public SlideStatementSyntax(SyntaxToken slideKeyword, SyntaxToken identifier, SyntaxToken colonToken, ImmutableArray<StepStatementSyntax> statements, SyntaxToken endslideKeyword)
		{
			SlideKeyword = slideKeyword;
			Identifier = identifier;
			ColonToken = colonToken;
			Statements = statements;
			EndslideKeyword = endslideKeyword;
		}

		public SyntaxToken SlideKeyword { get; }
		public SyntaxToken Identifier { get; }
		public SyntaxToken ColonToken { get; }
		public ImmutableArray<StepStatementSyntax> Statements { get; }
		public SyntaxToken EndslideKeyword { get; }

		public override SyntaxKind Kind => SyntaxKind.SlideStatement;
	}
}