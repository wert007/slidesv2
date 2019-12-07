using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class SyntaxTree
	{
		private SyntaxTree(SourceText text)
		{
			var parser = new Parser(text);
			var root = parser.ParseCompilationUnit();

			Text = text;
			Diagnostics = parser.Diagnostics.ToArray();
			Root = root;
		}

		public SourceText Text { get; }
		public Diagnostic[] Diagnostics { get; }
		public CompilationUnitSyntax Root { get; }

		internal SyntaxTree(CompilationUnitSyntax root)
		{
			Text = SourceText.From(string.Empty);
			Diagnostics = new Diagnostic[0];
			Root = root;
		}

		public static SyntaxTree Parse(string text, string fileName)
		{
			var sourceText = SourceText.From(text);
			sourceText.FileName = fileName;
			return Parse(sourceText);
		}

		public static SyntaxTree Parse(SourceText text)
		{
			return new SyntaxTree(text);
		}

		public static IEnumerable<SyntaxToken> ParseTokens(string text)
		{
			var sourceText = SourceText.From(text);
			return ParseTokens(sourceText);
		}

		public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
		{
			var lexer = new Lexer(text);
			while (true)
			{
				var token = lexer.Lex();
				if (token.Kind == SyntaxKind.EndOfFileToken)
					break;

				yield return token;
			}
		}
	}
}