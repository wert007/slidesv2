namespace Minsk.CodeAnalysis.Binding
{
	internal enum BoundBinaryOperatorKind
	{
		//Arithmetic
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Exponentiation,

		//Logic
		LogicalAnd,
		LogicalOr,
		Equals,
		NotEquals,
		Less,
		LessOrEquals,
		Greater,
		GreaterOrEquals,
		
		Concatination,
		EnumAddition,
		FilterAddition,
		Range,
		NotNoneValue,
		RangeMultiplication,
	}
}
