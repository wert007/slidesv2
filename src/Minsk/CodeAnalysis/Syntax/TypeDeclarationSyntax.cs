using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
	internal class TypeDeclarationSyntax
	{
		public TypeDeclarationSyntax(SyntaxToken colonToken, SyntaxToken type, SyntaxToken questionMarkToken, SyntaxToken[] bracketPairs)
		{
			ColonToken = colonToken;
			Type = type;
			QuestionMarkToken = questionMarkToken;
			BracketPairs = bracketPairs;
		}

		public SyntaxToken ColonToken { get; }
		public SyntaxToken Type { get; }
		public SyntaxToken QuestionMarkToken { get; }
		public SyntaxToken[] BracketPairs { get; }
		
	}
}