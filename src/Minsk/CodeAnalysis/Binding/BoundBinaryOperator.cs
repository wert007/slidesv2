using System;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Slides;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundBinaryOperator
	{
		private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol type)
			 : this(syntaxKind, kind, type, type, type)
		{
		}

		private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
			 : this(syntaxKind, kind, operandType, operandType, resultType)
		{
		}


		private BoundBinaryOperator(SyntaxKind syntaxKind, BoundBinaryOperatorKind kind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
		{
			SyntaxKind = syntaxKind;
			Kind = kind;
			LeftType = leftType;
			RightType = rightType;
			Type = resultType;
		}

		public SyntaxKind SyntaxKind { get; }
		public BoundBinaryOperatorKind Kind { get; }
		public TypeSymbol LeftType { get; }
		public TypeSymbol RightType { get; }
		public TypeSymbol Type { get; }


		private static TypeSymbol Vertical = BuiltInTypes.Instance.LookSymbolUp(typeof(Vertical));
		private static TypeSymbol Horizontal = BuiltInTypes.Instance.LookSymbolUp(typeof(Horizontal));
		private static TypeSymbol Orientation = BuiltInTypes.Instance.LookSymbolUp(typeof(Orientation));

		private static TypeSymbol Filter = BuiltInTypes.Instance.LookSymbolUp(typeof(Filter));
		private static TypeSymbol Unit = BuiltInTypes.Instance.LookSymbolUp(typeof(Unit));
		private static TypeSymbol Range = BuiltInTypes.Instance.LookSymbolUp(typeof(Range));
		private static BoundBinaryOperator[] _operators =
		  {
				new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, PrimitiveTypeSymbol.Integer),
				new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, PrimitiveTypeSymbol.Integer),
				new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, PrimitiveTypeSymbol.Integer),
				new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, PrimitiveTypeSymbol.Integer),
				new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.Exponentiation, PrimitiveTypeSymbol.Integer),

				new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, PrimitiveTypeSymbol.Float),
				new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, PrimitiveTypeSymbol.Float),
				new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, PrimitiveTypeSymbol.Float),
				new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, PrimitiveTypeSymbol.Float),
				new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.Exponentiation, PrimitiveTypeSymbol.Float),
				new BoundBinaryOperator(SyntaxKind.HatToken, BoundBinaryOperatorKind.Exponentiation, PrimitiveTypeSymbol.Float, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Float),

				new BoundBinaryOperator(SyntaxKind.PeriodPeriodToken, BoundBinaryOperatorKind.Range, PrimitiveTypeSymbol.Integer, Range),

				new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Concatination, PrimitiveTypeSymbol.String),

				new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.LessToken, BoundBinaryOperatorKind.Less, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.LessOrEqualsToken, BoundBinaryOperatorKind.LessOrEquals, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.GreaterToken, BoundBinaryOperatorKind.Greater, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.GreaterOrEqualsToken, BoundBinaryOperatorKind.GreaterOrEquals, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Bool),

				new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, PrimitiveTypeSymbol.String, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, PrimitiveTypeSymbol.String, PrimitiveTypeSymbol.Bool),

				new BoundBinaryOperator(SyntaxKind.AmpersandAmpersandToken, BoundBinaryOperatorKind.LogicalAnd, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.PipePipeToken, BoundBinaryOperatorKind.LogicalOr, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.EqualsEqualsToken, BoundBinaryOperatorKind.Equals, PrimitiveTypeSymbol.Bool),
				new BoundBinaryOperator(SyntaxKind.BangEqualsToken, BoundBinaryOperatorKind.NotEquals, PrimitiveTypeSymbol.Bool),

				new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.EnumAddition, Horizontal, Vertical, Orientation),
				new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.EnumAddition, Vertical, Horizontal, Orientation),

				new BoundBinaryOperator(SyntaxKind.PipeToken, BoundBinaryOperatorKind.FilterAddition, Filter),


				new BoundBinaryOperator(SyntaxKind.PlusToken, BoundBinaryOperatorKind.Addition, Unit),
				new BoundBinaryOperator(SyntaxKind.MinusToken, BoundBinaryOperatorKind.Subtraction, Unit),
				new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, PrimitiveTypeSymbol.Float, Unit, Unit),
				new BoundBinaryOperator(SyntaxKind.StarToken, BoundBinaryOperatorKind.Multiplication, Unit, PrimitiveTypeSymbol.Float, Unit),
				new BoundBinaryOperator(SyntaxKind.SlashToken, BoundBinaryOperatorKind.Division, Unit, PrimitiveTypeSymbol.Float, Unit),
		  };

		public static BoundBinaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol leftType, TypeSymbol rightType)
		{
			if(leftType.Type == TypeType.Noneable)
			{
				var nullableType = (NoneableTypeSymbol)leftType;
				if (nullableType.BaseType == rightType)
					return new BoundBinaryOperator(SyntaxKind.QuestionMarkQuestionMarkToken, BoundBinaryOperatorKind.NotNoneValue, rightType);
			}
			foreach (var op in _operators)
			{
				if (op.SyntaxKind == syntaxKind && op.LeftType == leftType && op.RightType == rightType)
					return op;
			}

			return null;
		}
	}
}
