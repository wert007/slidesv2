namespace Minsk.CodeAnalysis.Syntax
{
	internal class AnimationStatementSyntax : StatementSyntax
	{
		public AnimationStatementSyntax(SyntaxToken animationKeyword, SyntaxToken identifier, AnimationParameterStatementSyntax parameters, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endAnimationKeyword)
		{
			AnimationKeyword = animationKeyword;
			Identifier = identifier;
			Parameters = parameters;
			ColonToken = colonToken;
			Body = body;
			EndAnimationKeyword = endAnimationKeyword;
		}

		public override SyntaxKind Kind => SyntaxKind.AnimationStatement;

		public SyntaxToken AnimationKeyword { get; }
		public SyntaxToken Identifier { get; }
		public AnimationParameterStatementSyntax Parameters { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndAnimationKeyword { get; }
	}
}