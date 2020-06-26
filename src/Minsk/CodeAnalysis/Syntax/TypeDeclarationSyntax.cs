using Minsk.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis.Syntax
{
	public class TypeDeclarationSyntax
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
		private int End => Math.Max(Type.Span.End, Math.Max(QuestionMarkToken?.Span.End ?? -1, BracketPairs.LastOrDefault()?.Span.End ?? -1));
		public TextSpan Span => TextSpan.FromBounds(ColonToken.Position, End);



	}
}