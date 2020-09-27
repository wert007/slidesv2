using System;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class CompileTimeEvaluator : BasicEvaluator
	{
		private VariableValueCollection _variables = new VariableValueCollection(null);
		public CompileTimeEvaluator()
			: base(null)
		{

		}

		public void SetVariable(VariableSymbol symbol, object value)
		{
			_variables[symbol] = value;
		}

		public bool TryGetValue(BoundExpression expression, out object value)
		{
			switch (expression.Kind)
			{
				case BoundNodeKind.LiteralExpression:
					return TryGetLiteralValue((BoundLiteralExpression)expression, out value);
				case BoundNodeKind.EnumExpression:
					return TryGetEnumValue((BoundEnumExpression)expression, out value);
				case BoundNodeKind.VariableExpression:
					return TryGetVariableValue((BoundVariableExpression)expression, out value);
				case BoundNodeKind.UnaryExpression:
					return TryGetUnaryValue((BoundUnaryExpression)expression, out value);
				case BoundNodeKind.BinaryExpression:
					return TryGetBinaryValue((BoundBinaryExpression)expression, out value);
				case BoundNodeKind.FunctionExpression:
					return TryGetFunctionValue((BoundFunctionExpression)expression, out value);
				case BoundNodeKind.ArrayExpression:
					return TryGetArrayValue((BoundArrayExpression)expression, out value);
				case BoundNodeKind.FunctionAccessExpression:
					return TryGetFunctionAccessValue((BoundFunctionAccessExpression)expression, out value);
				case BoundNodeKind.ErrorExpression:
					value = null;
					return false;
				case BoundNodeKind.FieldAccessExpression:
					return TryGetFieldAccessValue((BoundFieldAccessExpression)expression, out value);
				case BoundNodeKind.StringExpression:
					return TryGetStringValue((BoundStringExpression)expression, out value);
				case BoundNodeKind.ConversionExpression:
					return TryGetConversionValue((BoundConversion)expression, out value);
				case BoundNodeKind.MathExpression:
					return TryGetMathValue((BoundMathExpression)expression, out value);
				case BoundNodeKind.EmptyArrayConstructorExpression:
					return TryGetEmptyArrayConstructorValue((BoundEmptyArrayConstructorExpression)expression, out value);
				case BoundNodeKind.AssignmentExpression:
					return TryGetAssignmentValue((BoundAssignmentExpression)expression, out value);
				case BoundNodeKind.AnonymForExpression:
					value = null;
					return false;
				case BoundNodeKind.ArrayAccessExpression:
					return TryGetArrayAccessValue((BoundArrayAccessExpression)expression, out value);
				default:
					throw new Exception();
			}
		}

		private bool TryGetLiteralValue(BoundLiteralExpression expression, out object value)
		{
			value = EvaluateLiteralExpression(expression);
			return true;
		}

		private bool TryGetEnumValue(BoundEnumExpression expression, out object value)
		{
			value = EvaluateEnumExpression(expression);
			return value != null;
		}

		public override object LookVariableUp(VariableSymbol variable)
		{
			var result = _constants[variable];
			if (result == null) result = _variables[variable];
			return result;
		}

		private bool TryGetVariableValue(BoundVariableExpression expression, out object value)
		{
			value = EvaluateVariableExpression(expression);
			return value != null;
		}

		private bool TryGetUnaryValue(BoundUnaryExpression expression, out object value)
		{
			if (!TryGetValue(expression.Operand, out value))
				return false;
			value = EvaluateUnaryExpressionValue(expression.Op, value);
			return value != null;
		}

		private bool TryGetBinaryValue(BoundBinaryExpression expression, out object value)
		{
			value = null;
			if (!TryGetValue(expression.Left, out var left)) return false;
			if (!TryGetValue(expression.Right, out var right)) return false;
			value = EvaluateBinaryExpressionValue(left, expression.Op, right);
			return value != null;
		}

		private bool TryGetFunctionValue(BoundFunctionExpression expression, out object value)
		{
			value = null;
			var args = new object[expression.Function.Parameter.Count];
			for (int i = 0; i < args.Length; i++)
			{
				if (!TryGetValue(expression.Arguments[i], out args[i])) return false;
			}
			value = EvaluateFunctionExpressionValue(expression.Function, args, expression.Source);
			return value != null;
		}

		private bool TryGetArrayValue(BoundArrayExpression expression, out object value)
		{
			value = null;
			var result = new object[expression.Expressions.Length];
			for (int i = 0; i < result.Length; i++)
			{
				if (!TryGetValue(expression.Expressions[i], out result[i])) return false;
			}
			value = result;
			return true;
		}

		private bool TryGetFunctionAccessValue(BoundFunctionAccessExpression expression, out object value)
		{
			value = null;
			if (!TryGetValue(expression.Parent, out var parent)) return false;
			var args = new object[expression.FunctionCall.Arguments.Length];
			for (int i = 0; i < args.Length; i++)
			{
				if (!TryGetValue(expression.FunctionCall.Arguments[i], out args[i])) return false;
			}
			value = EvaluateFunctionAccessCall(parent, expression.FunctionCall.Function, args);
			return true;
		}

		private bool TryGetFieldAccessValue(BoundFieldAccessExpression expression, out object value)
		{
			value = null;
			if (!TryGetValue(expression.Parent, out var parent)) return false;
			value = EvaluateFieldAccessExpressionValue(parent, expression.Field.Variable.Name, _builtInTypes.LookTypeUp(expression.Field.Variable.Type));
			return true;
		}

		private bool TryGetStringValue(BoundStringExpression expression, out object value)
		{
			value = null;
			var values = new object[expression.Expressions.Length];
			for (int i = 0; i < values.Length; i++)
			{
				if (!TryGetValue(expression.Expressions[i], out values[i])) return false;
			}
			value = EvaluateStringExpressionValue(values);
			return true;
		}

		private bool TryGetConversionValue(BoundConversion expression, out object value)
		{
			value = null;
			if (!TryGetValue(expression.Expression, out var sourceValue)) return false;
			value = EvaluateConversionValue(expression.Type, sourceValue, expression.Expression.Type);
			return true;
		}

		private bool TryGetMathValue(BoundMathExpression expression, out object value)
		{
			value = EvaluateMathExpression(expression);
			return true;
		}

		private bool TryGetEmptyArrayConstructorValue(BoundEmptyArrayConstructorExpression expression, out object value)
		{
			value = null;
			if (!TryGetValue(expression.Length, out var length)) return false;
			value = EvaluateEmptyArrayConstructorExpressionValue((ArrayTypeSymbol)expression.Type, (int)length);
			return true;
		}

		private bool TryGetAssignmentValue(BoundAssignmentExpression expression, out object value)
		{
			return TryGetValue(expression.Expression, out value);
		}

		private bool TryGetArrayAccessValue(BoundArrayAccessExpression expression, out object value)
		{
			value = null;
			if (!TryGetValue(expression.Child, out var array)) return false;
			if (!TryGetValue(expression.Index, out var index)) return false;
			value = EvaluateArrayAccessExpressionValue((object[])array, (int)index);
			return true;
		}
	}
}
