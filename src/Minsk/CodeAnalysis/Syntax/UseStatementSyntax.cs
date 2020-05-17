using Minsk.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
	internal class UseStatementSyntax : StatementSyntax
	{
		
		public UseStatementSyntax(SyntaxToken keyword, ExpressionSyntax[] dependencies, SyntaxToken[] commaToken, SyntaxToken colonToken, BlockStatementSyntax body, SyntaxToken endKeyword)
		{
			Keyword = keyword;
			Dependencies = dependencies;
			CommaToken = commaToken;
			ColonToken = colonToken;
			Body = body;
			EndKeyword = endKeyword;
		}

		public SyntaxToken Keyword { get; }
		public ExpressionSyntax[] Dependencies { get; }
		public SyntaxToken[] CommaToken { get; }
		public SyntaxToken ColonToken { get; }
		public BlockStatementSyntax Body { get; }
		public SyntaxToken EndKeyword { get; }

		public TextSpan HeaderSpan => TextSpan.FromBounds(Keyword.Position, ColonToken.Span.End); 

		public override SyntaxKind Kind => SyntaxKind.UseStatement;
	}
}