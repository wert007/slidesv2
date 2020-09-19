using System;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Slides;
using Slides.Data;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundUnaryOperator
	{
		private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType)
			 : this(syntaxKind, kind, operandType, operandType)
		{
		}

		private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
		{
			SyntaxKind = syntaxKind;
			Kind = kind;
			OperandType = operandType;
			Type = resultType;
		}

		public SyntaxKind SyntaxKind { get; }
		public BoundUnaryOperatorKind Kind { get; }
		public TypeSymbol OperandType { get; }
		public TypeSymbol Type { get; }

		private static TypeSymbol unitTypeSymbol = BuiltInTypes.Instance.LookSymbolUp(typeof(Unit));

		private static BoundUnaryOperator[] _operators =
		{
				new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, PrimitiveTypeSymbol.Bool),

				new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, PrimitiveTypeSymbol.Integer),
				new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, PrimitiveTypeSymbol.Integer),

				new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, PrimitiveTypeSymbol.Float),
				new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, PrimitiveTypeSymbol.Float),

				new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, unitTypeSymbol),
				new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, unitTypeSymbol),
		  };

		public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol operandType)
		{
			if(operandType.Type == TypeType.Noneable && syntaxKind == SyntaxKind.BangToken)
				return new BoundUnaryOperator(syntaxKind, BoundUnaryOperatorKind.NoneableNegation, operandType);
			foreach (var op in _operators)
			{
				if (op.SyntaxKind == syntaxKind && op.OperandType == operandType)
					return op;
			}

			return null;
		}
	}
}
