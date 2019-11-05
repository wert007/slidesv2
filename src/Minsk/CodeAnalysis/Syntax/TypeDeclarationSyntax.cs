using System.Collections.Immutable;

namespace Minsk.CodeAnalysis.Syntax
{
	internal class TypeDeclarationSyntax
	{
		public TypeDeclarationSyntax(SyntaxToken colonToken, SyntaxToken type, SyntaxToken questionMarkToken, ImmutableArray<SyntaxTokenPair>.Builder bracketPairs)
		{
			ColonToken = colonToken;
			Type = type;
			QuestionMarkToken = questionMarkToken;
			BracketPairs = bracketPairs;
		}

		public SyntaxToken ColonToken { get; }
		public SyntaxToken Type { get; }
		public SyntaxToken QuestionMarkToken { get; }
		public ImmutableArray<SyntaxTokenPair>.Builder BracketPairs { get; }
		
	}
}