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

		// Expressions
		LiteralExpression,
		VariableExpression,
		AssignmentExpression,
		UnaryExpression,
		BinaryExpression,
		FunctionExpression,
		IndexedArrayExpression,
		ArrayExpression,
		FunctionAccessExpression,
		ErrorExpression,
		EnumExpression,
		FieldAccessExpression,
		FieldAssignmentExpression,
		IfStatement,
		StringExpression,
		CaseStatement,
		StaticFieldAccessExpression,
		StepStatement,
		TransitionStatement,
		Conversion,
		FilterStatement,
		TemplateStatement,
		LambdaExpression,
		MathExpression,
	}
}
