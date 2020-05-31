using Minsk.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
	internal class JSInsertionStatementSyntax : StatementSyntax
	{
		
		public JSInsertionStatementSyntax(SyntaxToken keyword, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endKeyword)
		{
			Keyword = keyword;
			ColonToken = colonToken;
			Body = body;
			EndKeyword = endKeyword;
		}

		public SyntaxToken Keyword { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndKeyword { get; }

		public TextSpan HeaderSpan => TextSpan.FromBounds(Keyword.Position, ColonToken.Span.End); 

		public override SyntaxKind Kind => SyntaxKind.JSInsertionStatement;
	}
}