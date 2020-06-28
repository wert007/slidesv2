namespace Minsk.CodeAnalysis.Syntax
{
	public enum SyntaxKind
	{
		// Tokens
		EndOfFileToken,
		BadToken,
		WhitespaceToken,
		SlashSlashToken,

		//Single Tokens
		SemicolonToken,
		TildeToken,
		CommaToken,
		ColonToken,
		PeriodToken,
		QuestionMarkToken,
		DollarSignToken,
		PercentToken,
		
		//Variable Length Tokens
		NumberToken,
		IdentifierToken,
		StringToken,
		
		//Constructed Tokens
		MathFormulaToken,
		UnitToken,
		TimeToken,
		CommentToken,

		//Binary Tokens
		PlusToken,
		MinusToken,
		StarToken,
		SlashToken,
		HatToken,
		BangToken,
		PipeToken,
		QuestionMarkQuestionMarkToken,
		PeriodPeriodToken,
		
		//Binary Tokens
		AmpersandAmpersandToken,
		PipePipeToken,
		EqualsEqualsToken,
		BangEqualsToken,
		LessToken,
		LessOrEqualsToken,
		GreaterToken,
		GreaterOrEqualsToken,

		//Assignment Tokens
		EqualsToken,
		PlusEqualsToken,
		MinusEqualsToken,
		StarEqualsToken,
		SlashEqualsToken,
		PipeEqualsToken,
		AmpersandEqualsToken,

		//Pairs
		OpenParenthesisToken,
		CloseParenthesisToken,
		OpenBracketToken,
		CloseBracketToken,
		BracketPairToken,
		OpenBraceToken,
		CloseBraceToken,

		// Keywords
		LetKeyword,
		TrueKeyword,
		FalseKeyword,
		StdKeyword,
		NewKeyword,
		NoneKeyword,
		AnyKeyword,
		
		//Block Keywords
		AnimationKeyword,
		CaseKeyword,
		EndAnimationKeyword,
		FilterKeyword, // -> Replace by IdentifierToken???
		EndFilterKeyword,
		GroupKeyword,
		EndGroupKeyword,
		LibraryKeyword,
		EndLibraryKeyword,
		SlideKeyword,
		StepKeyword,
		EndSlideKeyword,
		StyleKeyword,
		EndStyleKeyword,
		StructKeyword,
		EndStructKeyword,
		SVGKeyword,
		EndSVGKeyword,
		TemplateKeyword,
		EndTemplateKeyword,
		TransitionKeyword,
		EndTransitionKeyword,
		//TODO: Tbc
		JSInsertionKeyword,
		EndJSInsertionKeyword,
		
		UseKeyword,
		ImportKeyword,
		AsKeyword,
		ForKeyword,
		InKeyword,
		EndForKeyword,
		IfKeyword,
		ElseKeyword,
		EndIfKeyword,

		// Nodes
		CompilationUnit,
		ElseClause,
		SyntaxTokenPair,
		TransitionParameter,
		TypeDeclarationSyntax,


		// Statements
		FileBlockStatement,
		BlockStatement,
		VariableDeclaration,
		ExpressionStatement,
		ForStatement,
		IfStatement,
		UseStatement,
		
		//Top Level Statements
		AnimationStatement,
		CaseBlockStatement,
		StructStatement,
		DataBlockStatement,
		FilterStatement,
		GroupStatement,
		ImportStatement,
		LibraryStatement,
		SlideStatement,
		StepStatement,
		StyleStatement,
		SVGStatement,
		TemplateStatement,
		TransitionStatement,
		JSInsertionStatement,

		//Parameter Statements
		ParameterBlockStatement,
		ParameterStatement,
		SingleParameterStatement,
		AnimationParameterStatement,

		// Expressions
		LiteralExpression,
		EmptyArrayConstructorExpression,
		AnonymForExpression,
		NameExpression,
		UnaryExpression,
		BinaryExpression,
		ParenthesizedExpression,
		AssignmentExpression,
		FunctionExpression,
		IndexedArrayExpression,
		ArrayConstructorExpression,
		ConstructorExpression,
		VariableExpression,
		ArrayAccessExpression,
		MemberAccessExpression,
		StringExpression,
		StringInsertionExpression,
	}
}