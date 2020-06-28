using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Text;
using Slides;

namespace Minsk.CodeAnalysis.Syntax
{
	internal sealed class Parser
	{
		private readonly DiagnosticBag _diagnostics;
		private readonly SourceText _text;
		private readonly SyntaxToken[] _tokens;
		private int _position;

		public Parser(SourceText text)
		{
			var tokens = new List<SyntaxToken>();

			var lexer = new Lexer(text);
			SyntaxToken token;
			do
			{
				token = lexer.Lex();

				if (token.Kind != SyntaxKind.WhitespaceToken &&
					 token.Kind != SyntaxKind.BadToken &&
					 token.Kind != SyntaxKind.CommentToken)
				{
					tokens.Add(token);
				}
			} while (token.Kind != SyntaxKind.EndOfFileToken);

			_text = text;
			_tokens = tokens.ToArray();
			_diagnostics = new DiagnosticBag(text.FileName);
			_diagnostics.AddRange(lexer.Diagnostics);
		}

		public DiagnosticBag Diagnostics => _diagnostics;

		private SyntaxToken Peek(int offset)
		{
			var index = _position + offset;
			if (index >= _tokens.Length)
				return _tokens[_tokens.Length - 1];

			return _tokens[index];
		}

		private SyntaxToken Current => Peek(0);

		private SyntaxToken NextToken()
		{
			var current = Current;
			_position++;
			return current;
		}

		private SyntaxToken MatchToken(SyntaxKind kind)
		{
			if (Current.Kind == kind)
				return NextToken();

			_diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
			return new SyntaxToken(kind, Current.Position, null, null);
		}

		public CompilationUnitSyntax ParseCompilationUnit()
		{
			var statement = ParseFile();
			var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
			return new CompilationUnitSyntax(statement, endOfFileToken);
		}

		private StatementSyntax ParseTopLevelStatement()
		{
			switch (Current.Kind)
			{
				case SyntaxKind.SlideKeyword:
					return ParseSlideStatement();
				case SyntaxKind.StepKeyword:
					return ParseStepStatement();
				case SyntaxKind.TemplateKeyword:
					return ParseTemplateStatement();
				case SyntaxKind.GroupKeyword:
					return ParseGroupStatement();
				case SyntaxKind.SVGKeyword:
					return ParseSVGGroupStatement();
				case SyntaxKind.StyleKeyword:
					return ParseStyleStatement();
				case SyntaxKind.AnimationKeyword:
					return ParseAnimationStatement();
				case SyntaxKind.TransitionKeyword:
					return ParseTransitionStatement();
				case SyntaxKind.LibraryKeyword:
					return ParseLibraryStatement();
				case SyntaxKind.StructKeyword:
					return ParseDataStatement();
				case SyntaxKind.ImportKeyword:
					return ParseImportStatement();
				case SyntaxKind.FilterKeyword:
					return ParseFilterStatement();
				default:
					_diagnostics.ReportBadTopLevelStatement(Current);
					return ParseStatement();
					//					throw new Exception();
			}
		}

		private StatementSyntax ParseStatement()
		{
			switch (Current.Kind)
			{
				case SyntaxKind.CaseKeyword:
					return ParseCaseBlockStatement();
				case SyntaxKind.IfKeyword:
					return ParseIfStatement();
				case SyntaxKind.ForKeyword:
					return ParseForStatement();
				case SyntaxKind.LetKeyword:
					return ParseVariableDeclaration();
				case SyntaxKind.JSInsertionKeyword:
					return ParseJSInsertionStatement();
				default:
					return ParseExpressionStatement();
			}
		}

		private FileBlockStatementSyntax ParseFile()
		{
			var statements = new List<StatementSyntax>();


			while (Current.Kind != SyntaxKind.EndOfFileToken)
			{
				var startToken = Current;

				var statement = ParseTopLevelStatement();
				statements.Add(statement);

				if (Current == startToken)
					NextToken();
			}


			return new FileBlockStatementSyntax(statements.ToArray());
		}

		private FunctionExpressionSyntax ParseFunctionExpression()
		{
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);

			var arguments = new List<ExpressionSyntax>();
			var first = true;

			while (Current.Kind != SyntaxKind.EndOfFileToken &&
				Current.Kind != SyntaxKind.CloseParenthesisToken)
			{
				var startToken = Current;
				if (!first)
					MatchToken(SyntaxKind.CommaToken);

				first = false;

				var expression = ParseExpression();
				arguments.Add(expression);

				//if (startToken == Current)
				//	NextToken();
			}

			var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);

			return new FunctionExpressionSyntax(identifier, openParenthesisToken, arguments.ToArray(), closeParenthesisToken);
		}

		private IfStatementSyntax ParseIfStatement(bool needsEndIf = true)
		{
			var ifKeyword = MatchToken(SyntaxKind.IfKeyword);
			var condition = ParseExpression();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.IfKeyword);
			var elseClause = ParseElseClause();
			var endIfKeyword = needsEndIf ? MatchToken(SyntaxKind.EndIfKeyword) : null;
			return new IfStatementSyntax(ifKeyword, condition, colonToken, body, elseClause, endIfKeyword);
		}

		private ElseClauseSyntax ParseElseClause()
		{
			if (Current.Kind != SyntaxKind.ElseKeyword)
				return null;
			var elseKeyword = NextToken();
			StatementSyntax body;
			if (Current.Kind == SyntaxKind.IfKeyword)
				body = ParseIfStatement(false);
			else
				body = ParseBlockStatement(SyntaxKind.ElseKeyword);
			return new ElseClauseSyntax(elseKeyword, body);
		}

		private ForStatementSyntax ParseForStatement()
		{
			var forKeyword = MatchToken(SyntaxKind.ForKeyword);
			var variable = ParseVariableExpression();
			//TODO(Major): Implement a [0..10] element.
			//[0..10]		Normal
			//[0..10:2]		Step 2
			//[2:0..10]		Step 2
			//[0..10>3]		Step 2
			//[0..10->3]	Step 2
			//(old todo) No indexing in the for-loop..
			var inKeyword = MatchToken(SyntaxKind.InKeyword);
			var collection = ParseExpression();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.ForKeyword);
			var endForKeyword = MatchToken(SyntaxKind.EndForKeyword);

			return new ForStatementSyntax(forKeyword, variable, inKeyword, collection, colonToken, body, endForKeyword);
		}

		private ImportStatementSyntax ParseImportStatement()
		{
			var importKeyword = MatchToken(SyntaxKind.ImportKeyword);
			var functionCall = ParseFunctionExpression();
			var asKeyword = MatchToken(SyntaxKind.AsKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);

			return new ImportStatementSyntax(importKeyword, functionCall, asKeyword, identifier, semicolonToken);
		}

		private FilterStatementSyntax ParseFilterStatement()
		{
			var filterKeyword = MatchToken(SyntaxKind.FilterKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var parameter = ParseFilterParameterStatement();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.FilterKeyword);
			var endFilterKeyword = MatchToken(SyntaxKind.EndFilterKeyword);
			return new FilterStatementSyntax(filterKeyword, identifier, parameter, colonToken, body, endFilterKeyword);
		}

		private ParameterBlockStatementSyntax ParseFilterParameterStatement()
		{
			var openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
			var parameter = ParseParameterStatement();
			var closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
			return new ParameterBlockStatementSyntax(openParenthesisToken, new ParameterStatementSyntax[] { parameter }, closeParenthesisToken);
		}

		private VariableTypeDeclarationStatement ParseVariableTypeDeclaration()
		{
			var parameter = ParseParameterStatement();
			var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
			return new VariableTypeDeclarationStatement(parameter, semicolonToken);
		}

		private StructBlockStatementSyntax ParseDataBlockStatement()
		{
			var statements = new List<VariableTypeDeclarationStatement>();

			while (Current.Kind != SyntaxKind.EndOfFileToken &&
				Current.Kind != SyntaxKind.EndStructKeyword)
			{
				var startToken = Current;
				var statement = ParseVariableTypeDeclaration();
				statements.Add(statement);

				if (Current == startToken)
					NextToken();
			}

			return new StructBlockStatementSyntax(statements.ToArray());
		}

		private StructStatementSyntax ParseDataStatement()
		{
			var dataKeyword = MatchToken(SyntaxKind.StructKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseDataBlockStatement();
			var endDataKeyword = MatchToken(SyntaxKind.EndStructKeyword);

			return new StructStatementSyntax(dataKeyword, identifier, colonToken, body, endDataKeyword);
		}

		private UseStatement ParseUseStatement()
		{
			var keyword = MatchToken(SyntaxKind.UseKeyword);
			var token = NextToken();
			var semicolon = MatchToken(SyntaxKind.SemicolonToken);
			return new UseStatement(keyword, token, semicolon);
		}

		private LibraryBlockStatementSyntax ParseLibraryBlockStatement()
		{
			var statements = new List<UseStatement>();

			while (Current.Kind != SyntaxKind.EndOfFileToken &&
				Current.Kind != SyntaxKind.EndLibraryKeyword)
			{
				var startToken = Current;
				var statement = ParseUseStatement();
				statements.Add(statement);

				if (Current == startToken)
					NextToken();
			}

			return new LibraryBlockStatementSyntax(statements.ToArray());
		}


		private LibraryStatementSyntax ParseLibraryStatement()
		{
			var libraryKeyword = MatchToken(SyntaxKind.LibraryKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseLibraryBlockStatement();
			var endLibraryKeyword = MatchToken(SyntaxKind.EndLibraryKeyword);

			return new LibraryStatementSyntax(libraryKeyword, identifier, colonToken, body, endLibraryKeyword);
		}

		private AnimationStatementSyntax ParseAnimationStatement()
		{
			var animationKeyword = MatchToken(SyntaxKind.AnimationKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);

			var parameters = ParseAnimationParameterStatement();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.AnimationKeyword);
			var endAnimationKeyword = MatchToken(SyntaxKind.EndAnimationKeyword);

			return new AnimationStatementSyntax(animationKeyword, identifier, parameters, colonToken, body, endAnimationKeyword);
		}

		private AnimationParameterStatementSyntax ParseAnimationParameterStatement()
		{
			var openParenthesis = MatchToken(SyntaxKind.OpenParenthesisToken);
			var elementParameter = ParseParameterStatement();
			var commaToken = MatchToken(SyntaxKind.CommaToken);
			var timeParameter = ParseParameterStatement();
			var closeParenthesis = MatchToken(SyntaxKind.CloseParenthesisToken);

			return new AnimationParameterStatementSyntax(openParenthesis, elementParameter, commaToken, timeParameter, closeParenthesis);
		}

		private StatementSyntax ParseCaseBlockStatement()
		{
			var caseKeyword = MatchToken(SyntaxKind.CaseKeyword);
			var condition = ParseExpression();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.CaseKeyword);

			return new CaseBlockStatementSyntax(caseKeyword, condition, colonToken, body);
		}

		//transition todo(from: Slide, to: Slide):
		private TransitionStatementSyntax ParseTransitionStatement()
		{
			var transitionKeyword = MatchToken(SyntaxKind.TransitionKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var parameters = ParseTransitionParameter();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.TransitionKeyword);
			var endTransitionKeyword = MatchToken(SyntaxKind.EndTransitionKeyword);

			return new TransitionStatementSyntax(transitionKeyword, identifier, parameters, colonToken, body, endTransitionKeyword);
		}

		private TransitionParameterSyntax ParseTransitionParameter()
		{
			var openParenthesis = MatchToken(SyntaxKind.OpenParenthesisToken);
			var fromParameter = ParseParameterStatement();
			var commaToken = MatchToken(SyntaxKind.CommaToken);
			var toParameter = ParseParameterStatement();
			var closeParenthesis = MatchToken(SyntaxKind.CloseParenthesisToken);

			return new TransitionParameterSyntax(openParenthesis, fromParameter, commaToken, toParameter, closeParenthesis);
		}

		private StyleStatementSyntax ParseStyleStatement()
		{
			var styleKeyword = MatchToken(SyntaxKind.StyleKeyword);
			var expected = Current.Kind == SyntaxKind.StdKeyword ? SyntaxKind.StdKeyword : SyntaxKind.IdentifierToken;
			var identifier = MatchToken(expected);
			SingleParameterStatement parameter = null;
			if (expected == SyntaxKind.IdentifierToken)
			{
				var openParenthesis = MatchToken(SyntaxKind.OpenParenthesisToken);
				var parameterStatement = ParseParameterStatement();
				var closeParenthesis = MatchToken(SyntaxKind.CloseParenthesisToken);
				parameter = new SingleParameterStatement(openParenthesis, parameterStatement, closeParenthesis);
			}
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.StyleKeyword);
			var endStyleKeyword = MatchToken(SyntaxKind.EndStyleKeyword);

			return new StyleStatementSyntax(styleKeyword, identifier, parameter, colonToken, body, endStyleKeyword);
		}

		private TemplateInheritance ParseTemplateInheritance()
		{
			var lessToken = MatchToken(SyntaxKind.LessToken);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);

			return new TemplateInheritance(lessToken, identifier);
		}

		private SlideStatementSyntax ParseSlideStatement()
		{
			var slideKeyword = MatchToken(SyntaxKind.SlideKeyword);
			SyntaxToken optionalPretildeToken = null;
			if (Current.Kind == SyntaxKind.TildeToken)
				optionalPretildeToken = NextToken();
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			TemplateInheritance template = null;
			if (Current.Kind == SyntaxKind.LessToken)
				template = ParseTemplateInheritance();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var statements = ParseSlideBlockStatement();
			var endslideKeyword = MatchToken(SyntaxKind.EndSlideKeyword);

			return new SlideStatementSyntax(slideKeyword, optionalPretildeToken, identifier, template, colonToken, statements, endslideKeyword);
		}


		private StepStatementSyntax[] ParseSlideBlockStatement()
		{
			var statements = new List<StepStatementSyntax>();
			if (TryInsertStepStatement(out var inserted))
				statements.Add(inserted);
			while (Current.Kind == SyntaxKind.StepKeyword)
				statements.Add(ParseStepStatement());
			return statements.ToArray();
		}

		private bool TryInsertStepStatement(out StepStatementSyntax value)
		{
			value = null;
			if (Current.Kind == SyntaxKind.StepKeyword)
				return false;
			var body = ParseBlockStatement(SyntaxKind.StepKeyword);
			value = new StepStatementSyntax(null, null, null, body);
			return true;
		}

		private StepStatementSyntax ParseStepStatement()
		{
			var stepKeyword = MatchToken(SyntaxKind.StepKeyword);
			SyntaxToken optionalIdentifier = null;
			if (Current.Kind == SyntaxKind.IdentifierToken)
				optionalIdentifier = NextToken();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.StepKeyword);
			//var expected = Current.Kind == SyntaxKind.StepKeyword ? SyntaxKind.StepKeyword : SyntaxKind.EndSlideKeyword;
			//var endKeyword = MatchToken(expected);

			return new StepStatementSyntax(stepKeyword, optionalIdentifier, colonToken, body);
		}

		private VariableExpressionSyntax ParseVariableExpression(bool allowFieldKeywords = false)
		{
			SyntaxToken preTildeToken = null;
			if (Current.Kind == SyntaxKind.TildeToken)
				preTildeToken = NextToken();

			SyntaxToken identifier;
			if(allowFieldKeywords)
			{
				identifier = NextToken();
				if (identifier.Kind != SyntaxKind.IdentifierToken && identifier.Kind != SyntaxKind.FilterKeyword && identifier.Kind != SyntaxKind.TransitionKeyword)
					_diagnostics.ReportUnexpectedToken(identifier.Span, identifier.Kind, SyntaxKind.IdentifierToken);
			}
			else identifier = MatchToken(SyntaxKind.IdentifierToken);   
			
			return new VariableExpressionSyntax(preTildeToken, identifier);
		}

		private TypeDeclarationSyntax ParseTypeDeclaration()
		{
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var expected = Current.Kind == SyntaxKind.AnyKeyword ? SyntaxKind.AnyKeyword : SyntaxKind.IdentifierToken;
			var type = MatchToken(expected);
			SyntaxToken questionMarkToken = null;
			if (Current.Kind == SyntaxKind.QuestionMarkToken)
				questionMarkToken = NextToken();

			var bracketPairs = new List<SyntaxToken>();
			while (Current.Kind == SyntaxKind.BracketPairToken)
			{
				var bracketPairToken = MatchToken(SyntaxKind.BracketPairToken);
				bracketPairs.Add(bracketPairToken);
			}
			return new TypeDeclarationSyntax(colonToken, type, questionMarkToken, bracketPairs.ToArray());
		}

		private TypeDeclarationSyntax TryParseTypeDeclaration()
		{
			if (Current.Kind != SyntaxKind.ColonToken)
				return null;
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var expected = Current.Kind == SyntaxKind.AnyKeyword ? SyntaxKind.AnyKeyword : SyntaxKind.IdentifierToken;

			if (Current.Kind != expected)
				return null;
			var type = MatchToken(expected);
			SyntaxToken questionMarkToken = null;
			if (Current.Kind == SyntaxKind.QuestionMarkToken)
				questionMarkToken = NextToken();


			var bracketPairs = new List<SyntaxToken>();
			while (Current.Kind == SyntaxKind.BracketPairToken)
			{
				var bracketPairToken = MatchToken(SyntaxKind.BracketPairToken);
				bracketPairs.Add(bracketPairToken);
			}
			return new TypeDeclarationSyntax(colonToken, type, questionMarkToken, bracketPairs.ToArray());
		}

		private ParameterStatementSyntax ParseParameterStatement()
		{
			var variable = ParseVariableExpression();
			var typeDeclaration = TryParseTypeDeclaration();
			SyntaxToken equalsToken = null;
			ExpressionSyntax initializer = null;
			if (Current.Kind == SyntaxKind.EqualsToken)
			{
				equalsToken = NextToken();
				initializer = ParseBinaryExpression();
			}
			if (typeDeclaration == null && initializer == null)
			{
				_diagnostics.ReportParameterNeedsTypeDeclaration(variable);
			}
			return new ParameterStatementSyntax(variable, typeDeclaration, equalsToken, initializer);
		}

		private ParameterBlockStatementSyntax ParseParameterBlockStatement()
		{
			var openParenthesis = MatchToken(SyntaxKind.OpenParenthesisToken);
			var statements = new List<ParameterStatementSyntax>();

			var first = true;
			while (Current.Kind != SyntaxKind.EndOfFileToken &&
				Current.Kind != SyntaxKind.CloseParenthesisToken)
			{
				if (!first)
					MatchToken(SyntaxKind.CommaToken);
				first = false;
				statements.Add(ParseParameterStatement());
			}

			var closeParenthesis = MatchToken(SyntaxKind.CloseParenthesisToken);

			return new ParameterBlockStatementSyntax(openParenthesis, statements.ToArray(), closeParenthesis);
		}

		private TemplateStatementSyntax ParseTemplateStatement()
		{
			var templateKeyword = MatchToken(SyntaxKind.TemplateKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var parameterStatement = ParseTemplateParameterStatement();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.TemplateKeyword);
			var endTemplateKeyword = MatchToken(SyntaxKind.EndTemplateKeyword);

			return new TemplateStatementSyntax(templateKeyword, identifier, parameterStatement, colonToken, body, endTemplateKeyword);
		}

		private SingleParameterStatement ParseTemplateParameterStatement()
		{
			var openParenthesis = MatchToken(SyntaxKind.OpenParenthesisToken);
			var parameterStatement = ParseParameterStatement();
			var closeParenthesis = MatchToken(SyntaxKind.CloseParenthesisToken);
			return new SingleParameterStatement(openParenthesis, parameterStatement, closeParenthesis);
		}

		private GroupStatementSyntax ParseGroupStatement()
		{
			var groupKeyword = MatchToken(SyntaxKind.GroupKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var parameterStatement = ParseParameterBlockStatement();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.GroupKeyword);
			var endGroupKeyword = MatchToken(SyntaxKind.EndGroupKeyword);

			return new GroupStatementSyntax(groupKeyword, identifier, parameterStatement, colonToken, body, endGroupKeyword);
		}

		private SVGStatementSyntax ParseSVGGroupStatement()
		{
			var svgGroupKeyword = MatchToken(SyntaxKind.SVGKeyword);
			var identifier = MatchToken(SyntaxKind.IdentifierToken);
			var parameterStatement = ParseParameterBlockStatement();
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.SVGKeyword);
			var endSVGGroupKeyword = MatchToken(SyntaxKind.EndSVGKeyword);

			return new SVGStatementSyntax(svgGroupKeyword, identifier, parameterStatement, colonToken, body, endSVGGroupKeyword);
		}

		private BlockStatementSyntax ParseBlockStatement(SyntaxKind starter)
		{
			var statements = new List<StatementSyntax>();


			while (Current.Kind != SyntaxKind.EndOfFileToken)
			{
				if (Current.Kind == SyntaxFacts.GetEndKeywordKind(starter))
					break;

				if (starter == SyntaxKind.IfKeyword && Current.Kind == SyntaxKind.ElseKeyword)
					break;

				if (starter == SyntaxKind.CaseKeyword && Current.Kind == SyntaxKind.CaseKeyword)
					break;
				if (starter == SyntaxKind.StepKeyword && Current.Kind == SyntaxKind.StepKeyword)
					break;

				var startToken = Current;

				var statement = ParseStatement();
				if(starter == SyntaxKind.JSInsertionKeyword && statement.Kind == SyntaxKind.UseStatement)
				{
					//TODO:
					throw new NotImplementedException();
				}
				statements.Add(statement);

				// If ParseStatement() did not consume any tokens,
				// we need to skip the current token and continue
				// in order to avoid an infinite loop.
				//
				// We don't need to report an error, because we'll
				// already tried to parse an expression statement
				// and reported one.
				if (Current == startToken)
					NextToken();
			}


			return new BlockStatementSyntax(statements.ToArray());
		}


		private StatementSyntax ParseVariableDeclaration()
		{
			var keyword = MatchToken(SyntaxKind.LetKeyword);

			var variable = ParseVariableExpression();
			var optionalTypeDeclaration = TryParseTypeDeclaration();
			var equals = MatchToken(SyntaxKind.EqualsToken);
			var initializer = ParseExpression();
			var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
			return new VariableDeclarationSyntax(keyword, variable, optionalTypeDeclaration, equals, initializer, semicolonToken);
		}

		private StatementSyntax ParseJSInsertionStatement()
		{
			var keyword = MatchToken(SyntaxKind.JSInsertionKeyword);
			var colonToken = MatchToken(SyntaxKind.ColonToken);
			var body = ParseBlockStatement(SyntaxKind.JSInsertionKeyword);
			var endKeyword = MatchToken(SyntaxKind.EndJSInsertionKeyword);

			return new JSInsertionStatementSyntax(keyword, colonToken, body, endKeyword);
		}




		private ExpressionStatementSyntax ParseExpressionStatement()
		{
			var expression = ParseExpression();
			var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
			return new ExpressionStatementSyntax(expression, semicolonToken);
		}

		private ExpressionSyntax ParseExpression()
		{
			return ParseAssignmentExpression();
		}

		private ExpressionSyntax ParseMemberExpression()
		{
			//Field -> identifierToken + (TildeToken)
			//FunctionCall -> identifierToken + openParenthesisToken + "whatever" + closeParenthesisToken
			return ParseVariableOrFunctionExpression();
		}

		private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
		{
			ExpressionSyntax left;

			var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
			if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
			{
				var operatorToken = NextToken();
				var operand = ParseBinaryExpression(unaryOperatorPrecedence);
				left = new UnaryExpressionSyntax(operatorToken, operand);
			}
			else
			{
				left = ParseFieldAccessExpression();
			}

			//while (true)
			//{
			//	if (Current.Kind != SyntaxKind.PeriodToken)
			//		break;
			//	var periodToken = NextToken();
			//	var member = ParseMemberExpression();
			//	left = new MemberAccessExpressionSyntax(left, periodToken, member);
			//}

			while (true)
			{
				var precedence = Current.Kind.GetBinaryOperatorPrecedence();
				if (precedence == 0 || precedence <= parentPrecedence)
					break;

				var operatorToken = NextToken();
				var right = ParseBinaryExpression(precedence);
				left = new BinaryExpressionSyntax(left, operatorToken, right);
			}

			return left;
		}

		private ExpressionSyntax TryParseBinaryExpression(int parentPrecedence = 0)
		{
			ExpressionSyntax left = null;


			var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
			if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
			{
				var operatorToken = NextToken();
				var operand = TryParseBinaryExpression(unaryOperatorPrecedence);
				if (operand == null)
					return left;
				left = new UnaryExpressionSyntax(operatorToken, operand);
			}
			else
			{
				left = TryParseArrayAccessExpression();
			}

			while (true)
			{
				//What the heck is this???
				if (Current.Kind != SyntaxKind.PeriodToken)
					break;
				var periodToken = NextToken();
				var member = ParseMemberExpression();
				left = new MemberAccessExpressionSyntax(left, periodToken, member);
			}

			while (true)
			{
				var precedence = Current.Kind.GetBinaryOperatorPrecedence();
				if (precedence == 0 || precedence <= parentPrecedence)
					break;

				var operatorToken = NextToken();
				var right = TryParseBinaryExpression(precedence);
				if (right == null)
					return left;
				left = new BinaryExpressionSyntax(left, operatorToken, right);
			}

			return left;
		}

		private ExpressionSyntax ParseArrayAccessExpression()
		{
			var result = ParsePrimaryExpression();
			while (Current.Kind == SyntaxKind.OpenBracketToken)
			{
				var openBracketToken = MatchToken(SyntaxKind.OpenBracketToken);
				ExpressionSyntax indexExpression = null;
				if (Current.Kind == SyntaxKind.PeriodPeriodToken)
					indexExpression = new AnonymForExpressionSyntax(MatchToken(SyntaxKind.PeriodPeriodToken));
				else
					indexExpression = ParseExpression();
				var closeBracketToken = MatchToken(SyntaxKind.CloseBracketToken);
				result = new ArrayAccessExpressionSyntax(result, openBracketToken, indexExpression, closeBracketToken);
			}
			return result;
		}

		private ExpressionSyntax ParsePrimaryExpression()
		{
			switch (Current.Kind)
			{
				case SyntaxKind.OpenParenthesisToken:
					return ParseParenthesizedExpression();

				case SyntaxKind.FalseKeyword:
				case SyntaxKind.TrueKeyword:
					return ParseBooleanLiteral();

				case SyntaxKind.DollarSignToken:
					return ParseStringExpression();
				case SyntaxKind.StringToken:
					return ParseStringLiteral();
				case SyntaxKind.MathFormulaToken:
					return ParseMathFormulaLiteral();

				case SyntaxKind.NumberToken:
					return ParseNumberLiteral();

				case SyntaxKind.NoneKeyword:
					return ParseNoneLiteral();

				case SyntaxKind.OpenBracketToken:
					return ParseArrayExpression();

				case SyntaxKind.NewKeyword:
					return ParseConstructorExpression();

				case SyntaxKind.TildeToken:
				case SyntaxKind.IdentifierToken:
				case SyntaxKind.FilterKeyword:
				case SyntaxKind.TransitionKeyword:
					return ParseVariableOrFunctionExpression(true);
				default:
					_diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, SyntaxKind.IdentifierToken);
					return new LiteralExpressionSyntax(NextToken(), null);
			}
		}

		private ExpressionSyntax TryParseArrayAccessExpression()
		{
			var result = TryParsePrimaryExpression();
			while (result != null && Current.Kind == SyntaxKind.OpenBracketToken)
			{
				var openBracketToken = MatchToken(SyntaxKind.OpenBracketToken);
				ExpressionSyntax indexExpression = null;
				if (Current.Kind == SyntaxKind.PeriodPeriodToken)
					indexExpression = new AnonymForExpressionSyntax(MatchToken(SyntaxKind.PeriodPeriodToken));
				else
					indexExpression = ParseExpression();
				var closeBracketToken = MatchToken(SyntaxKind.CloseBracketToken);
				result = new ArrayAccessExpressionSyntax(result, openBracketToken, indexExpression, closeBracketToken);
			}
			return result;
		}
		private ExpressionSyntax TryParsePrimaryExpression()
		{
			switch (Current.Kind)
			{
				case SyntaxKind.OpenParenthesisToken:
					return ParseParenthesizedExpression();

				case SyntaxKind.FalseKeyword:
				case SyntaxKind.TrueKeyword:
					return ParseBooleanLiteral();

				case SyntaxKind.DollarSignToken:
					return ParseStringExpression();
				case SyntaxKind.StringToken:
					return ParseStringLiteral();

				case SyntaxKind.NumberToken:
					return ParseNumberLiteral();

				case SyntaxKind.NoneKeyword:
					return ParseNoneLiteral();

				case SyntaxKind.OpenBracketToken:
					return ParseArrayExpression();

				case SyntaxKind.NewKeyword:
					return ParseConstructorExpression();

				case SyntaxKind.TildeToken:
				case SyntaxKind.IdentifierToken:
				case SyntaxKind.FilterKeyword:
				case SyntaxKind.TransitionKeyword:
					return ParseVariableOrFunctionExpression(true);
				default:
					return null;
			}
		}

		private enum PVOFEReturnType
		{
			Variable,
			Function,
		//	Array,
			//FieldAccess,
		}
		private ExpressionSyntax ParseVariableOrFunctionExpression(bool allowFieldKeywords = false)
		{
			if(Current.Kind != SyntaxKind.IdentifierToken && Current.Kind != SyntaxKind.TildeToken)
			{
				if (!allowFieldKeywords || Current.Kind != SyntaxKind.FilterKeyword && Current.Kind != SyntaxKind.TransitionKeyword)
				{
					_diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, SyntaxKind.IdentifierToken);
					return ParseNameExpression();
				}
			}
			var returnType = PVOFEReturnType.Variable;
			int i = 0;
			var peek = Peek(i);
			var keepGoing = true;
			while (keepGoing)
			{
				switch (peek.Kind)
				{
					case SyntaxKind.IdentifierToken:
					case SyntaxKind.TildeToken:
						break;
					case SyntaxKind.FilterKeyword:
					case SyntaxKind.TransitionKeyword:
						keepGoing = allowFieldKeywords;
						break;
					case SyntaxKind.OpenParenthesisToken:
						returnType = PVOFEReturnType.Function;
						break;
					default:
						keepGoing = false;
						break;
				}

				i++;
				peek = Peek(i);
			}
			switch (returnType)
			{
				case PVOFEReturnType.Variable:        return ParseVariableExpression(allowFieldKeywords);
				case PVOFEReturnType.Function:        return ParseFunctionExpression();
				//case PVOFEReturnType.Array:           return ParseArrayAccessExpression();
				//case PVOFEReturnType.FieldAccess:     return ParseFieldAccessExpression();
				default:                              throw new Exception();
			}
		}

		private bool IsSettable()
		{
			var i = 0;
			var openParenthesis = 0;
			var openBrackets = 0;
			var peek = Peek(i);
			var loopOn = true;
			while (peek.Kind != SyntaxKind.EndOfFileToken && loopOn)
			{
				switch (peek.Kind)
				{
					case SyntaxKind.EqualsToken:
					case SyntaxKind.PlusEqualsToken:
					case SyntaxKind.MinusEqualsToken:
					case SyntaxKind.StarEqualsToken:
					case SyntaxKind.SlashEqualsToken:
					case SyntaxKind.PipeEqualsToken:
					case SyntaxKind.AmpersandEqualsToken:
					case SyntaxKind.ColonToken:
					case SyntaxKind.SemicolonToken:
					case SyntaxKind.LetKeyword:
					case SyntaxKind.CaseKeyword:
					case SyntaxKind.ElseKeyword:
					case SyntaxKind.ForKeyword:
					case SyntaxKind.IfKeyword:
					case SyntaxKind.StepKeyword:
						loopOn = false;
						break;
					case SyntaxKind.OpenParenthesisToken:
						openParenthesis++;
						break;
					case SyntaxKind.CloseParenthesisToken:
						openParenthesis--;
						break;
					case SyntaxKind.OpenBracketToken:
						openBrackets++;
						break;
					case SyntaxKind.CloseBracketToken:
						openBrackets--;
						break;
				}
				if (openParenthesis < 0 || openBrackets < 0)
					break;
				if (!loopOn)
					break;
				i++;
				peek = Peek(i);
			}
			return Peek(i).Kind.IsAssignmentOperator();
		}

		private ExpressionSyntax ParseAssignmentExpression()
		{
			if (IsSettable())
			{
				ExpressionSyntax left = ParseFieldAccessExpression();
				if (!left.IsLValue)
				{
					//TODO: Beautify diagnostics!
					_diagnostics.ReportCannotAssign(left.Span, $"is no valid l value ({left.ToString()})");
					left = new VariableExpressionSyntax(null, new SyntaxToken(SyntaxKind.IdentifierToken, left.Span.Start, null, null));
				}
				if (!Current.Kind.IsAssignmentOperator())
				{
					//TODO(Minor): Not all possible operators. Maybe we should report them too!
					_diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, SyntaxKind.EqualsToken);
				}
				var operatorToken = NextToken();
				var right = ParseAssignmentExpression();
				return new AssignmentExpressionSyntax((LExpressionSyntax)left, operatorToken, right);
			}
			return ParseBinaryExpression();
		}

		private ExpressionSyntax ParseFieldAccessExpression()
		{
			var left = ParseArrayAccessExpression();
			while(Current.Kind == SyntaxKind.PeriodToken || 
				   Current.Kind == SyntaxKind.OpenBracketToken)
			{
				if (Current.Kind == SyntaxKind.PeriodToken)
				{
					var periodToken = NextToken();
					var right = ParseVariableOrFunctionExpression(true);
					left = new MemberAccessExpressionSyntax(left, periodToken, right);
				}
				else if (Current.Kind == SyntaxKind.OpenBracketToken)
				{
					var openBracket = NextToken();
					ExpressionSyntax index;
					if (Current.Kind == SyntaxKind.PeriodPeriodToken)
						index = new AnonymForExpressionSyntax(MatchToken(SyntaxKind.PeriodPeriodToken));
					else
						index = ParseExpression();
					var closeBracket = MatchToken(SyntaxKind.CloseBracketToken);
					left = new ArrayAccessExpressionSyntax(left, openBracket, index, closeBracket);
				}
				else throw new Exception();
			}
			return left;
		}

		private ExpressionSyntax ParseConstructorExpression()
		{
			var newKeyword = MatchToken(SyntaxKind.NewKeyword);
			ExpressionSyntax functionCall = null;
				//TODO: What is this? Why is this check here and not in MemberAccess/FieldAccess??
			if (Peek(1).Kind == SyntaxKind.PeriodToken)
			{
				functionCall = ParseVariableExpression();
				var periodToken = NextToken();
				var member = ParseMemberExpression();
				functionCall = new MemberAccessExpressionSyntax(functionCall, periodToken, member);
			}
			else
				functionCall = ParseFunctionExpression();

			return new ConstructorExpressionSyntax(newKeyword, functionCall);
		}

		private ExpressionSyntax ParseArrayExpression()
		{
			if (Peek(1).Kind == SyntaxKind.ColonToken)
				return ParseEmptyArrayConstructorExpression();
			return ParseArrayConstructorExpression();
		}

		private ExpressionSyntax ParseArrayConstructorExpression()
		{
			var openBracketToken = MatchToken(SyntaxKind.OpenBracketToken);

			var expressions = new List<ExpressionSyntax>();
			var first = true;
			while (Current.Kind != SyntaxKind.EndOfFileToken &&
				Current.Kind != SyntaxKind.CloseBracketToken)
			{
				if (!first)
					MatchToken(SyntaxKind.CommaToken);
				first = false;
				if (Current.Kind == SyntaxKind.CloseBracketToken)
					break;
				var expression = ParseExpression();
				expressions.Add(expression);
			}
			var closeBracketToken = MatchToken(SyntaxKind.CloseBracketToken);

			return new ArrayConstructorExpressionSyntax(openBracketToken, expressions.ToArray(), closeBracketToken);
		}

		private ExpressionSyntax ParseEmptyArrayConstructorExpression()
		{
			var openBracketToken = MatchToken(SyntaxKind.OpenBracketToken);
			var typeDeclaration = TryParseTypeDeclaration();
			var semicolonToken = MatchToken(SyntaxKind.SemicolonToken);
			var lengthExpression = ParseExpression();
			var closeBracketToken = MatchToken(SyntaxKind.CloseBracketToken);

			return new EmptyArrayConstructorExpressionSyntax(openBracketToken, typeDeclaration, semicolonToken, lengthExpression, closeBracketToken);
		}

		private ExpressionSyntax ParseParenthesizedExpression()
		{
			var left = MatchToken(SyntaxKind.OpenParenthesisToken);
			var expression = ParseExpression();
			var right = MatchToken(SyntaxKind.CloseParenthesisToken);
			return new ParenthesizedExpressionSyntax(left, expression, right);
		}

		private ExpressionSyntax ParseBooleanLiteral()
		{
			var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
			var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
			return new LiteralExpressionSyntax(keywordToken, isTrue);
		}



		private ExpressionSyntax ParseStringExpression()
		{
			var dollarSignToken = MatchToken(SyntaxKind.DollarSignToken);
			var literalExpressions = new List<LiteralExpressionSyntax>();
			var insertions = new List<StringInsertionExpressionSyntax>();
			while (true)
			{
				if (Current.Kind == SyntaxKind.StringToken)
					literalExpressions.Add(ParseStringLiteral());
				else if (Current.Kind == SyntaxKind.OpenBraceToken)
					insertions.Add(ParseStringInsertion());
				else
					break;
			}
			return new StringExpressionSyntax(dollarSignToken, literalExpressions.ToArray(), insertions.ToArray());
		}

		private StringInsertionExpressionSyntax ParseStringInsertion()
		{
			var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);
			var expression = ParseExpression();
			var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);
			return new StringInsertionExpressionSyntax(openBraceToken, expression, closeBraceToken);
		}

		private LiteralExpressionSyntax ParseStringLiteral()
		{
			var stringToken = MatchToken(SyntaxKind.StringToken);
			return new LiteralExpressionSyntax(stringToken);
		}

		private LiteralExpressionSyntax ParseMathFormulaLiteral()
		{
			var mathFormulaToken = MatchToken(SyntaxKind.MathFormulaToken);
			return new LiteralExpressionSyntax(mathFormulaToken);
		}

		private ExpressionSyntax ParseNumberLiteral()
		{
			var numberToken = MatchToken(SyntaxKind.NumberToken);
			if (Current.Kind == SyntaxKind.IdentifierToken ||
				Current.Kind == SyntaxKind.PercentToken)
			{
				var textToken = NextToken();
				object value = null;
				SyntaxKind kind = SyntaxKind.BadToken;
				switch (textToken.Text)
				{
					case "pt":
						value = new Unit(Convert.ToSingle(numberToken.Value), Unit.UnitKind.Point);
						kind = SyntaxKind.UnitToken;
						break;
					case "px":
						value = new Unit(Convert.ToSingle(numberToken.Value), Unit.UnitKind.Pixel);
						kind = SyntaxKind.UnitToken;
						break;
					case "%":
						value = new Unit(Convert.ToSingle(numberToken.Value), Unit.UnitKind.Percent);
						kind = SyntaxKind.UnitToken;
						break;
					case "ms":
						value = new Time(Convert.ToSingle(numberToken.Value), Time.TimeUnit.Milliseconds);
						kind = SyntaxKind.TimeToken;
						break;
					case "s":
						value = new Time(Convert.ToSingle(numberToken.Value), Time.TimeUnit.Seconds);
						kind = SyntaxKind.TimeToken;
						break;
					case "m":
						value = new Time(Convert.ToSingle(numberToken.Value), Time.TimeUnit.Minutes);
						kind = SyntaxKind.TimeToken;
						break;
					case "h":
						value = new Time(Convert.ToSingle(numberToken.Value), Time.TimeUnit.Hours);
						kind = SyntaxKind.TimeToken;
						break;
					default:
						return new LiteralExpressionSyntax(numberToken);
				}
				var text = numberToken.Text + textToken.Text;
				var token = new SyntaxToken(kind, numberToken.Position, text, value);
				return new LiteralExpressionSyntax(token);
			}
			return new LiteralExpressionSyntax(numberToken);
		}

		private ExpressionSyntax ParseNoneLiteral()
		{
			var noneToken = MatchToken(SyntaxKind.NoneKeyword);
			return new LiteralExpressionSyntax(noneToken, null);
		}

		private ExpressionSyntax ParseNameExpression()
		{
			var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
			return new NameExpressionSyntax(identifierToken);
		}
	}
}