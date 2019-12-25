using System;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Slides;

namespace Minsk.CodeAnalysis
{
	internal static class FormulaCreator
	{
		public static bool NeedsDependency(BoundExpression expression, out BoundExpression dependent)
		{
			if (expression is BoundFieldAccesExpression fieldAccess)
			{
				if (fieldAccess.Parent.Type == TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(Slider)) &&
					fieldAccess.Field.Variable.Name == "value")
				{
					dependent = expression;
					return true;
				}
			}
			foreach (var child in expression.GetChildren())
			{
				if (child is BoundExpression e && NeedsDependency(e, out BoundExpression childDependent))
				{
					dependent = childDependent;
					return true;
				}
			}
			dependent = null;
			return false;
		}
		public static Formula CreateFunction(this Evaluator e, BoundExpression expression, BoundExpression dependent)
		{
			return new Formula(CreateFormula(e, expression, dependent));
		}

		private static string CreateFormula(this Evaluator e, BoundExpression expression, BoundExpression dependent)
		{
			if (expression == dependent)
				return "x";
			if (!Contains(expression, dependent))
				return ToString(e.EvaluateExpression(expression));
			switch (expression.Kind)
			{
				case BoundNodeKind.BinaryExpression:
					var binary = (BoundBinaryExpression)expression;
					var left = CreateFormula(e, binary.Left, dependent);
					var right = CreateFormula(e, binary.Right, dependent);
					var op = OperatorToString(binary.Op.Kind);
					return left + op + right;
				case BoundNodeKind.StringExpression:
					var strExp = (BoundStringExpression)expression;
					return string.Join("+", strExp.Expressions.Select(ex => CreateFormula(e, ex, dependent)));
				default:
					throw new Exception();
			}
		}

		private static string OperatorToString(BoundBinaryOperatorKind kind)
		{
			switch (kind)
			{
				case BoundBinaryOperatorKind.Addition:
					return "+";
				case BoundBinaryOperatorKind.Subtraction:
					return "-";
				case BoundBinaryOperatorKind.Multiplication:
					return "*";
				case BoundBinaryOperatorKind.Division:
					return "/";
				case BoundBinaryOperatorKind.LogicalAnd:
					return "&&";
				case BoundBinaryOperatorKind.LogicalOr:
					return "||";
				case BoundBinaryOperatorKind.Equals:
					return "=="; //TODO: JavaScript has === as well..
				case BoundBinaryOperatorKind.NotEquals:
					return "!=";
				case BoundBinaryOperatorKind.Less:
					return "<";
				case BoundBinaryOperatorKind.LessOrEquals:
					return "<=";
				case BoundBinaryOperatorKind.Greater:
					return ">";
				case BoundBinaryOperatorKind.GreaterOrEquals:
					return ">=";
				case BoundBinaryOperatorKind.Concatination:
					return "+";
				default:
					throw new Exception();
			}
		}

		private static string ToString(object value)
		{
			switch (value)
			{
				case string s:
					return $"'{s}'";
				case Color c:
					//Shouldn't happen as of now
					//TODO
					throw new Exception();
				//	return $"'{CSSWriter.GetValue(c)}'";
				case int i:
					return $"{i}";
				case bool b:
					return b.ToString().ToLower();
				default:
					return value.ToString();
			}
		}

		private static bool Contains(BoundExpression a, BoundExpression b)
		{
			if (a == b)
				return true;
			foreach (var child in a.GetChildren())
			{
				if (child is BoundExpression e && Contains(e, b))
					return true;
			}
			return false;
		}
	}
}