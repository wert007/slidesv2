using System;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;

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

		private static BoundUnaryOperator[] _operators =
		{
				new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, PrimitiveTypeSymbol.Bool),

				new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, PrimitiveTypeSymbol.Integer),
				new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, PrimitiveTypeSymbol.Integer),

				new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.FromString("Unit")),
				new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.FromString("Unit")),
			//	new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, PrimitiveTypeSymbol.Integer),
		  };

		public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol operandType)
		{
			foreach (var op in _operators)
			{
				if (op.SyntaxKind == syntaxKind && op.OperandType == operandType)
					return op;
			}

			return null;
		}
	}
}
