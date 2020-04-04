namespace Minsk.CodeAnalysis.Binding
{
	internal enum BoundNodeKind
	{
		// Statements
		BlockStatement,
		VariableDeclaration,
		ForStatement,
		ExpressionStatement,

		SlideStatement,
		ParameterStatement,
		ParameterBlockStatement,
		GroupStatement,
		StyleStatement,
		AnimationStatement,
		LibraryStatement,
		DataStatement,
		IfStatement,
		CaseStatement,
		StepStatement,
		TransitionStatement,
		FilterStatement,
		TemplateStatement,
		SVGGroupStatement,

		// Expressions
		LiteralExpression,
		EnumExpression,
		VariableExpression,
		UnaryExpression,
		BinaryExpression,
		FunctionExpression,
		//IndexedArrayExpression,
		ArrayExpression,
		FunctionAccessExpression,
		ErrorExpression,
		FieldAccessExpression,
		StringExpression,
		Conversion,
		MathExpression,
		EmptyArrayConstructorExpression,
		AssignmentExpression,

		LambdaExpression,
		AnonymForExpression,
		ArrayAccessExpression,
	}
}
