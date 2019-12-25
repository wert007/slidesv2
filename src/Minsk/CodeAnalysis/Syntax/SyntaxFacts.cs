using System;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
	public static class SyntaxFacts
	{
		public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
		{
			switch (kind)
			{
				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
				case SyntaxKind.BangToken:
					return 8;

				default:
					return 0;
			}
		}

		public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
		{
			switch (kind)
			{
				case SyntaxKind.PeriodPeriodToken:
					return 7;

				case SyntaxKind.HatToken:
					return 6;

				case SyntaxKind.StarToken:
				case SyntaxKind.SlashToken:
					return 5;

				case SyntaxKind.PlusToken:
				case SyntaxKind.MinusToken:
					return 4;

				case SyntaxKind.EqualsEqualsToken:
				case SyntaxKind.BangEqualsToken:
				case SyntaxKind.LessToken:
				case SyntaxKind.LessOrEqualsToken:
				case SyntaxKind.GreaterToken:
				case SyntaxKind.GreaterOrEqualsToken:
					return 3;

				case SyntaxKind.AmpersandAmpersandToken:
					return 2;

				case SyntaxKind.PipeToken:
				case SyntaxKind.PipePipeToken:
					return 1;

				default:
					return 0;
			}
		}



		public static SyntaxKind GetKeywordKind(string text)
		{
			switch (text)
			{
				case "false":
					return SyntaxKind.FalseKeyword;
				case "let":
					return SyntaxKind.LetKeyword;
				case "true":
					return SyntaxKind.TrueKeyword;
				case "import":
					return SyntaxKind.ImportKeyword;
				case "if":
					return SyntaxKind.IfKeyword;
				case "else":
					return SyntaxKind.ElseKeyword;
				case "endif":
					return SyntaxKind.EndIfKeyword;
				case "as":
					return SyntaxKind.AsKeyword;
				case "style":
					return SyntaxKind.StyleKeyword;
				case "endstyle":
					return SyntaxKind.EndStyleKeyword;
				case "animation":
					return SyntaxKind.AnimationKeyword;
				case "case":
					return SyntaxKind.CaseKeyword;
				case "endanimation":
					return SyntaxKind.EndAnimationKeyword;
				case "template":
					return SyntaxKind.TemplateKeyword;
				case "endtemplate":
					return SyntaxKind.EndTemplateKeyword;
				case "slide":
					return SyntaxKind.SlideKeyword;
				case "endslide":
					return SyntaxKind.EndSlideKeyword;
				case "step":
					return SyntaxKind.StepKeyword;
				case "new":
					return SyntaxKind.NewKeyword;
				case "library":
					return SyntaxKind.LibraryKeyword;
				case "endlibrary":
					return SyntaxKind.EndLibraryKeyword;
				case "group":
					return SyntaxKind.GroupKeyword;
				case "endgroup":
					return SyntaxKind.EndGroupKeyword;
				case "data":
					return SyntaxKind.DataKeyword;
				case "enddata":
					return SyntaxKind.EndDataKeyword;
				case "std":
					return SyntaxKind.StdKeyword;
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
				case "transition":
					return SyntaxKind.TransitionKeyword;
				case "endtransition":
					return SyntaxKind.EndTransitionKeyword;
				case "filter":
					return SyntaxKind.FilterKeyword;
				case "endfilter":
					return SyntaxKind.EndFilterKeyword;
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
				case SyntaxKind.StyleKeyword:
					return SyntaxKind.EndStyleKeyword;
				case SyntaxKind.CaseKeyword:
				case SyntaxKind.AnimationKeyword:
					return SyntaxKind.EndAnimationKeyword;
				case SyntaxKind.SlideKeyword:
				case SyntaxKind.StepKeyword:
					return SyntaxKind.EndSlideKeyword;
				case SyntaxKind.TemplateKeyword:
					return SyntaxKind.EndTemplateKeyword;
				case SyntaxKind.LibraryKeyword:
					return SyntaxKind.EndLibraryKeyword;
				case SyntaxKind.GroupKeyword:
					return SyntaxKind.EndGroupKeyword;
				case SyntaxKind.DataKeyword:
					return SyntaxKind.EndDataKeyword;
				case SyntaxKind.ForKeyword:
					return SyntaxKind.EndForKeyword;
				case SyntaxKind.IfKeyword:
				case SyntaxKind.ElseKeyword:
					return SyntaxKind.EndIfKeyword;
				case SyntaxKind.TransitionKeyword:
					return SyntaxKind.EndTransitionKeyword;
				case SyntaxKind.FilterKeyword:
					return SyntaxKind.EndFilterKeyword;
				default:
					return kind;
			}
		}

		public static string GetText(SyntaxKind kind)
		{
			switch (kind)
			{
				case SyntaxKind.PlusToken:
					return "+";
				case SyntaxKind.MinusToken:
					return "-";
				case SyntaxKind.StarToken:
					return "*";
				case SyntaxKind.SlashToken:
					return "/";
				case SyntaxKind.BangToken:
					return "!";
				case SyntaxKind.EqualsToken:
					return "=";
				case SyntaxKind.TildeToken:
					return "~";
				case SyntaxKind.ColonToken:
					return ":";
				case SyntaxKind.SemicolonToken:
					return ";";
				case SyntaxKind.CommaToken:
					return ",";
				case SyntaxKind.PeriodToken:
					return ".";
				case SyntaxKind.QuestionMarkToken:
					return "?";
				case SyntaxKind.DollarSignToken:
					return "$";
				case SyntaxKind.OpenBraceToken:
					return "{";
				case SyntaxKind.CloseBraceToken:
					return "}";
				case SyntaxKind.LessToken:
					return "<";
				case SyntaxKind.LessOrEqualsToken:
					return "<=";
				case SyntaxKind.GreaterToken:
					return ">";
				case SyntaxKind.GreaterOrEqualsToken:
					return ">=";
				case SyntaxKind.AmpersandAmpersandToken:
					return "&&";
				case SyntaxKind.PipeToken:
					return "|";
				case SyntaxKind.PipePipeToken:
					return "||";
				case SyntaxKind.HatToken:
					return "^";
				case SyntaxKind.EqualsEqualsToken:
					return "==";
				case SyntaxKind.BangEqualsToken:
					return "!=";
				case SyntaxKind.OpenParenthesisToken:
					return "(";
				case SyntaxKind.CloseParenthesisToken:
					return ")";
				case SyntaxKind.FalseKeyword:
					return "false";
				case SyntaxKind.LetKeyword:
					return "let";
				case SyntaxKind.TrueKeyword:
					return "true";
				case SyntaxKind.ImportKeyword:
					return "import";
				case SyntaxKind.AsKeyword:
					return "as";
				case SyntaxKind.StyleKeyword:
					return "style";
				case SyntaxKind.EndStyleKeyword:
					return "endstyle";
				case SyntaxKind.AnimationKeyword:
					return "animation";
				case SyntaxKind.CaseKeyword:
					return "case";
				case SyntaxKind.EndAnimationKeyword:
					return "endanimation";
				case SyntaxKind.TemplateKeyword:
					return "template";
				case SyntaxKind.EndTemplateKeyword:
					return "endtemplate";
				case SyntaxKind.SlideKeyword:
					return "slide";
				case SyntaxKind.StepKeyword:
					return "step";
				case SyntaxKind.EndSlideKeyword:
					return "endslide";
				case SyntaxKind.NewKeyword:
					return "new";
				case SyntaxKind.LibraryKeyword:
					return "library";
				case SyntaxKind.EndLibraryKeyword:
					return "endlibrary";
				case SyntaxKind.GroupKeyword:
					return "group";
				case SyntaxKind.EndGroupKeyword:
					return "endgroup";
				case SyntaxKind.DataKeyword:
					return "data";
				case SyntaxKind.EndDataKeyword:
					return "enddata";
				case SyntaxKind.StdKeyword:
					return "std";
				case SyntaxKind.NoneKeyword:
					return "none";
				case SyntaxKind.AnyKeyword:
					return "any";
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
				case SyntaxKind.TransitionKeyword:
					return "transition";
				case SyntaxKind.EndTransitionKeyword:
					return "endtransition";
				case SyntaxKind.FilterKeyword:
					return "filter";
				case SyntaxKind.EndFilterKeyword:
					return "endfilter";
				default:
					return null;
			}
		}


		public static SyntaxKind GetSyntaxKind(string text)
		{
			switch (text)
			{
				case "+":
					return SyntaxKind.PlusToken;
				case "-":
					return SyntaxKind.MinusToken;
				case "*":
					return SyntaxKind.StarToken;
				case "/":
					return SyntaxKind.SlashToken;
				case "!":
					return SyntaxKind.BangToken;
				case "=":
					return SyntaxKind.EqualsToken;
				case "~":
					return SyntaxKind.TildeToken;
				case ":":
					return SyntaxKind.ColonToken;
				case ";":
					return SyntaxKind.SemicolonToken;
				case ",":
					return SyntaxKind.CommaToken;
				case ".":
					return SyntaxKind.PeriodToken;
				case "?":
					return SyntaxKind.QuestionMarkToken;
				case "$":
					return SyntaxKind.DollarSignToken;
				case "{":
					return SyntaxKind.OpenBraceToken;
				case "}":
					return SyntaxKind.CloseBraceToken;
				case "<":
					return SyntaxKind.LessToken;
				case "<=":
					return SyntaxKind.LessOrEqualsToken;
				case ">":
					return SyntaxKind.GreaterToken;
				case ">=":
					return SyntaxKind.GreaterOrEqualsToken;
				case "&&":
					return SyntaxKind.AmpersandAmpersandToken;
				case "|":
					return SyntaxKind.PipeToken;
				case "||":
					return SyntaxKind.PipePipeToken;
				case "^":
					return SyntaxKind.HatToken;
				case "==":
					return SyntaxKind.BangEqualsToken;
				case "!=":
					return SyntaxKind.EqualsEqualsToken;
				case "(":
					return SyntaxKind.OpenParenthesisToken;
				case ")":
					return SyntaxKind.CloseParenthesisToken;
				default:
					var keyword = GetKeywordKind(text);
					if (keyword == SyntaxKind.IdentifierToken)
						return SyntaxKind.BadToken;
					return keyword;
			}
		}
	}
}