using System;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
	public static class SyntaxFacts
	{
		public static bool IsAssignmentOperator(this SyntaxKind kind)
		{
			switch(kind)
			{
				case SyntaxKind.EqualsToken:
				case SyntaxKind.PlusEqualsToken:
				case SyntaxKind.MinusEqualsToken:
				case SyntaxKind.StarEqualsToken:
				case SyntaxKind.SlashEqualsToken:
				case SyntaxKind.PipeEqualsToken:
				case SyntaxKind.AmpersandEqualsToken:
					return true;
				default:
					return false;
			}
		}
		public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
		{
			switch (kind)
			{
				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
				case SyntaxKind.BangToken:
					return 9;

				default:
					return 0;
			}
		}

		public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
		{
			switch (kind)
			{
				case SyntaxKind.PeriodPeriodToken:
					return 8;

				case SyntaxKind.HatToken:
					return 7;

				case SyntaxKind.StarToken:
				case SyntaxKind.SlashToken:
					return 6;

				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
					return 5;

				case SyntaxKind.EqualsEqualsToken:
				case SyntaxKind.BangEqualsToken:
				case SyntaxKind.LessToken:
				case SyntaxKind.LessOrEqualsToken:
				case SyntaxKind.GreaterToken:
				case SyntaxKind.GreaterOrEqualsToken:
					return 4;

				case SyntaxKind.AmpersandAmpersandToken:
					return 3;

				case SyntaxKind.PipeToken:
				case SyntaxKind.PipePipeToken:
					return 2;

				case SyntaxKind.QuestionMarkQuestionMarkToken:
					return 1;

				default:
					return 0;
			}
		}



		public static SyntaxKind GetKeywordKind(string text)
		{
			switch (text)
			{
				case "std":
					return SyntaxKind.StdKeyword;
				case "new":
					return SyntaxKind.NewKeyword;
				case "none":
					return SyntaxKind.NoneKeyword;
				case "any":
					return SyntaxKind.AnyKeyword;
				case "for":
					return SyntaxKind.ForKeyword;
				case "in":
					return SyntaxKind.InKeyword;
				case "endfor":
					return SyntaxKind.EndForKeyword;
				case "if":
					return SyntaxKind.IfKeyword;
				case "else":
					return SyntaxKind.ElseKeyword;
				case "endif":
					return SyntaxKind.EndIfKeyword;
				case "false":
					return SyntaxKind.FalseKeyword;
				case "let":
					return SyntaxKind.LetKeyword;
				case "true":
					return SyntaxKind.TrueKeyword;
				case "use":
					return SyntaxKind.UseKeyword;
				case "import":
					return SyntaxKind.ImportKeyword;
				case "as":
					return SyntaxKind.AsKeyword;
				
				case "animation":
					return SyntaxKind.AnimationKeyword;
				case "case":
					return SyntaxKind.CaseKeyword;
				case "endanimation":
					return SyntaxKind.EndAnimationKeyword;
				case "filter":
					return SyntaxKind.FilterKeyword;
				case "endfilter":
					return SyntaxKind.EndFilterKeyword;
				case "group":
					return SyntaxKind.GroupKeyword;
				case "endgroup":
					return SyntaxKind.EndGroupKeyword;
				case "library":
					return SyntaxKind.LibraryKeyword;
				case "endlibrary":
					return SyntaxKind.EndLibraryKeyword;
				case "slide":
					return SyntaxKind.SlideKeyword;
				case "step":
					return SyntaxKind.StepKeyword;
				case "endslide":
					return SyntaxKind.EndSlideKeyword;
				case "struct":
					return SyntaxKind.StructKeyword;
				case "endstruct":
					return SyntaxKind.EndStructKeyword;
				case "style":
					return SyntaxKind.StyleKeyword;
				case "endstyle":
					return SyntaxKind.EndStyleKeyword;
				case "svg":
					return SyntaxKind.SVGKeyword;
				case "endsvg":
					return SyntaxKind.EndSVGKeyword;
				case "template":
					return SyntaxKind.TemplateKeyword;
				case "endtemplate":
					return SyntaxKind.EndTemplateKeyword;
				case "transition":
					return SyntaxKind.TransitionKeyword;
				case "endtransition":
					return SyntaxKind.EndTransitionKeyword;
				case "jsinsertion":
					return SyntaxKind.JSInsertionKeyword;
				case "endjsinsertion":
					return SyntaxKind.EndJSInsertionKeyword;
				default:
					return SyntaxKind.IdentifierToken;
			}
		}

		public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds()
		{
			var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
			foreach (var kind in kinds)
			{
				if (GetUnaryOperatorPrecedence(kind) > 0)
					yield return kind;
			}
		}

		public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds()
		{
			var kinds = (SyntaxKind[])Enum.GetValues(typeof(SyntaxKind));
			foreach (var kind in kinds)
			{
				if (GetBinaryOperatorPrecedence(kind) > 0)
					yield return kind;
			}
		}

		public static SyntaxKind GetEndKeywordKind(SyntaxKind kind)
		{
			switch (kind)
			{
				case SyntaxKind.ForKeyword:
					return SyntaxKind.EndForKeyword;
				case SyntaxKind.IfKeyword:
				case SyntaxKind.ElseKeyword:
					return SyntaxKind.EndIfKeyword;
				case SyntaxKind.AnimationKeyword:
				case SyntaxKind.CaseKeyword:
					return SyntaxKind.EndAnimationKeyword;
				case SyntaxKind.FilterKeyword:
					return SyntaxKind.EndFilterKeyword;
				case SyntaxKind.GroupKeyword:
					return SyntaxKind.EndGroupKeyword;
				case SyntaxKind.SlideKeyword:
				case SyntaxKind.StepKeyword:
					return SyntaxKind.EndSlideKeyword;
				case SyntaxKind.StructKeyword:
					return SyntaxKind.EndStructKeyword;
				case SyntaxKind.StyleKeyword:
					return SyntaxKind.EndStyleKeyword;
				case SyntaxKind.SVGKeyword:
					return SyntaxKind.EndSVGKeyword;
				case SyntaxKind.TemplateKeyword:
					return SyntaxKind.EndTemplateKeyword;
				case SyntaxKind.LibraryKeyword:
					return SyntaxKind.EndLibraryKeyword;
				case SyntaxKind.JSInsertionKeyword:
					return SyntaxKind.EndJSInsertionKeyword;
				case SyntaxKind.TransitionKeyword:
					return SyntaxKind.EndTransitionKeyword;
				default:
					return kind;
			}
		}

		public static string GetText(SyntaxKind kind)
		{
			switch (kind)
			{
				case SyntaxKind.SlashSlashToken:
					return "//";
				case SyntaxKind.SemicolonToken:
					return ";";
				case SyntaxKind.TildeToken:
					return "~";
				case SyntaxKind.CommaToken:
					return ",";
				case SyntaxKind.ColonToken:
					return ":";
				case SyntaxKind.PeriodToken:
					return ".";
				case SyntaxKind.QuestionMarkToken:
					return "?";
				case SyntaxKind.DollarSignToken:
					return "$";
				case SyntaxKind.PercentToken:
					return "%";
				case SyntaxKind.PlusToken:
					return "+";
				case SyntaxKind.MinusToken:
					return "-";
				case SyntaxKind.StarToken:
					return "*";
				case SyntaxKind.SlashToken:
					return "/";
				case SyntaxKind.HatToken:
					return "^";
				case SyntaxKind.BangToken:
					return "!";
				case SyntaxKind.PipeToken:
					return "|";
				case SyntaxKind.QuestionMarkQuestionMarkToken:
					return "??";
				case SyntaxKind.PeriodPeriodToken:
					return "..";
				case SyntaxKind.AmpersandAmpersandToken:
					return "&&";
				case SyntaxKind.PipePipeToken:
					return "||";
				case SyntaxKind.EqualsEqualsToken:
					return "==";
				case SyntaxKind.BangEqualsToken:
					return "!=";
				case SyntaxKind.LessToken:
					return "<";
				case SyntaxKind.LessOrEqualsToken:
					return "<=";
				case SyntaxKind.GreaterToken:
					return ">";
				case SyntaxKind.GreaterOrEqualsToken:
					return ">=";

				case SyntaxKind.EqualsToken:
					return "=";
				case SyntaxKind.PlusEqualsToken:
					return "+=";
				case SyntaxKind.MinusEqualsToken:
					return "-=";
				case SyntaxKind.StarEqualsToken:
					return "*=";
				case SyntaxKind.SlashEqualsToken:
					return "/=";
				case SyntaxKind.PipeEqualsToken:
					return "|=";
				case SyntaxKind.AmpersandEqualsToken:
					return "&=";

				case SyntaxKind.OpenParenthesisToken:
					return "(";
				case SyntaxKind.CloseParenthesisToken:
					return ")";
				case SyntaxKind.OpenBracketToken:
					return "[";
				case SyntaxKind.CloseBracketToken:
					return "]";
				case SyntaxKind.BracketPairToken:
					return "[]";
				case SyntaxKind.OpenBraceToken:
					return "{";
				case SyntaxKind.CloseBraceToken:
					return "}";

				case SyntaxKind.LetKeyword:
					return "let";
				case SyntaxKind.TrueKeyword:
					return "true";
				case SyntaxKind.FalseKeyword:
					return "false";
				case SyntaxKind.StdKeyword:
					return "std";
				case SyntaxKind.NewKeyword:
					return "new";
				case SyntaxKind.NoneKeyword:
					return "none";
				case SyntaxKind.AnyKeyword:
					return "any";

				case SyntaxKind.AnimationKeyword:
					return "animation";
				case SyntaxKind.CaseKeyword:
					return "case";
				case SyntaxKind.EndAnimationKeyword:
					return "endanimation";
				case SyntaxKind.FilterKeyword:
					return "filter";
				case SyntaxKind.EndFilterKeyword:
					return "endfilter";
				case SyntaxKind.GroupKeyword:
					return "group";
				case SyntaxKind.EndGroupKeyword:
					return "endgroup";
				case SyntaxKind.LibraryKeyword:
					return "library";
				case SyntaxKind.EndLibraryKeyword:
					return "endlibrary";
				case SyntaxKind.SlideKeyword:
					return "slide";
				case SyntaxKind.StepKeyword:
					return "step";
				case SyntaxKind.EndSlideKeyword:
					return "endslide";
				case SyntaxKind.StructKeyword:
					return "struct";
				case SyntaxKind.EndStructKeyword:
					return "endstruct";
				case SyntaxKind.StyleKeyword:
					return "style";
				case SyntaxKind.EndStyleKeyword:
					return "endstyle";
				case SyntaxKind.SVGKeyword:
					return "svg";
				case SyntaxKind.EndSVGKeyword:
					return "endsvg";
				case SyntaxKind.TemplateKeyword:
					return "template";
				case SyntaxKind.EndTemplateKeyword:
					return "endtemplate";
				case SyntaxKind.TransitionKeyword:
					return "transition";
				case SyntaxKind.EndTransitionKeyword:
					return "endtransition";
				case SyntaxKind.JSInsertionKeyword:
					return "use";
				case SyntaxKind.EndJSInsertionKeyword:
					return "enduse";
					
				case SyntaxKind.ImportKeyword:
					return "import";
				case SyntaxKind.AsKeyword:
					return "as";
				case SyntaxKind.ForKeyword:
					return "for";
				case SyntaxKind.InKeyword:
					return "in";
				case SyntaxKind.EndForKeyword:
					return "endfor";
				case SyntaxKind.IfKeyword:
					return "if";
				case SyntaxKind.ElseKeyword:
					return "else";
				case SyntaxKind.EndIfKeyword:
					return "endif";
				default:
					return null;
			}
		}


		public static SyntaxKind GetSyntaxKind(string text)
		{
			switch (text)
			{
				case "//":
					return SyntaxKind.SlashSlashToken;
				case ";":
					return SyntaxKind.SemicolonToken;
				case "~":
					return SyntaxKind.TildeToken;
				case ",":
					return SyntaxKind.CommaToken;
				case ":":
					return SyntaxKind.ColonToken;
				case ".":
					return SyntaxKind.PeriodToken;
				case "?":
					return SyntaxKind.QuestionMarkToken;
				case "$":
					return SyntaxKind.DollarSignToken;
				case "%":
					return SyntaxKind.PercentToken;

				case "+":
					return SyntaxKind.PlusToken;
				case "-":
					return SyntaxKind.MinusToken;
				case "*":
					return SyntaxKind.StarToken;
				case "/":
					return SyntaxKind.SlashToken;
				case "^":
					return SyntaxKind.HatToken;
				case "!":
					return SyntaxKind.BangToken;
				case "|":
					return SyntaxKind.PipeToken;
				case "??":
					return SyntaxKind.QuestionMarkQuestionMarkToken;
				case "..":
					return SyntaxKind.PeriodPeriodToken;

				case "&&":
					return SyntaxKind.AmpersandAmpersandToken;
				case "||":
					return SyntaxKind.PipePipeToken;
				case "==":
					return SyntaxKind.BangEqualsToken;
				case "!=":
					return SyntaxKind.EqualsEqualsToken;
				case "<":
					return SyntaxKind.LessToken;
				case "<=":
					return SyntaxKind.LessOrEqualsToken;
				case ">":
					return SyntaxKind.GreaterToken;
				case ">=":
					return SyntaxKind.GreaterOrEqualsToken;
				
				case "=":
					return SyntaxKind.EqualsToken;
				case "+=":
					return SyntaxKind.PlusEqualsToken;
				case "-=":
					return SyntaxKind.MinusEqualsToken;
				case "*=":
					return SyntaxKind.StarEqualsToken;
				case "/=":
					return SyntaxKind.SlashEqualsToken;
				case "|=":
					return SyntaxKind.PipeEqualsToken;
				case "&=":
					return SyntaxKind.AmpersandEqualsToken;

				case "(":
					return SyntaxKind.OpenParenthesisToken;
				case ")":
					return SyntaxKind.CloseParenthesisToken;
				case "[":
					return SyntaxKind.OpenBracketToken;
				case "]":
					return SyntaxKind.CloseBracketToken;
				case "[]":
					return SyntaxKind.BracketPairToken;
				case "{":
					return SyntaxKind.OpenBraceToken;
				case "}":
					return SyntaxKind.CloseBraceToken;
				default:
					var keyword = GetKeywordKind(text);
					if (keyword == SyntaxKind.IdentifierToken)
						return SyntaxKind.BadToken;
					return keyword;
			}
		}
	}
}