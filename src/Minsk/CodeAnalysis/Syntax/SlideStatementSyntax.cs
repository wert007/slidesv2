﻿namespace Minsk.CodeAnalysis.Syntax
{
	public sealed class SlideStatementSyntax : StatementSyntax
	{
		public SlideStatementSyntax(SyntaxToken slideKeyword, SyntaxToken pretildeToken, SyntaxToken identifier, TemplateInheritance template, SyntaxToken colonToken, StepStatementSyntax[] statements, SyntaxToken endslideKeyword)
		{
			SlideKeyword = slideKeyword;
			PretildeToken = pretildeToken;
			Identifier = identifier;
			Template = template;
			ColonToken = colonToken;
			Statements = statements;
			EndslideKeyword = endslideKeyword;
		}

		public SyntaxToken SlideKeyword { get; }
		public SyntaxToken PretildeToken { get; }
		public SyntaxToken Identifier { get; }
		public TemplateInheritance Template { get; }
		public SyntaxToken ColonToken { get; }
		public StepStatementSyntax[] Statements { get; }
		public SyntaxToken EndslideKeyword { get; }

		public override SyntaxKind Kind => SyntaxKind.SlideStatement;
	}
}