namespace Minsk.CodeAnalysis.Binding
{
	internal enum BoundNodeKind
	{
		// Statements
		BlockStatement,
		VariableDeclaration,
		ForStatement,
		IfStatement,
		ExpressionStatement,

		//Parameter Statemens
		ParameterStatement,
		ParameterBlockStatement,
		
		//Top Level Statemens
		AnimationStatement,
		CaseStatement,
		DataStatement,
		GroupStatement,
		FilterStatement,
		SlideStatement,
		StepStatement,
		StyleStatement,
		SVGStatement,
		TemplateStatement,
		TransitionStatement,
		JSInsertionStatement,

		// Expressions
		ErrorExpression,
		LiteralExpression,
		
		UnaryExpression,
		BinaryExpression,
		ArrayExpression,
		
		StringExpression,
		MathExpression,
		
		VariableExpression,
		FunctionExpression,
		EnumExpression,
		EmptyArrayConstructorExpression,
		
		ArrayAccessExpression,
		FieldAccessExpression,
		FunctionAccessExpression,
		
		ConversionExpression,
		AssignmentExpression,
		AnonymForExpression,
		NopStatement,
	}
}
