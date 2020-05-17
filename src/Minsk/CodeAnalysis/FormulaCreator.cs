using System;
using System.Linq;
using System.Text;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Elements;

namespace Minsk.CodeAnalysis
{
	internal static class FormulaCreator
	{
		public static JSInsertionKind GetJSInsertionKind(this BoundExpression dependent)
		{
			switch (dependent.Kind)
			{
				case BoundNodeKind.VariableExpression:
					var variableExpression = (BoundVariableExpression)dependent;
					if (variableExpression.Variable.Name == "totalTime")
						return JSInsertionKind.Time;
					break;
				case BoundNodeKind.FieldAccessExpression:
					var fieldAccess = (BoundFieldAccessExpression)dependent;
					if (fieldAccess.Parent.Type == BuiltInTypes.Instance.LookSymbolUp(typeof(Slider)) &&
						fieldAccess.Field.Variable.Name == "value")
						return JSInsertionKind.Slider;
					break;
				default:
					break;
			}
			return JSInsertionKind.None;
		}
		public static JSInsertionKind GetJSInsertionKind(BoundExpression expression, out BoundExpression dependent)
		{
			
			if(expression.GetJSInsertionKind() != JSInsertionKind.None)
			{
				dependent = expression;
				return dependent.GetJSInsertionKind();
			}
			foreach (var child in expression.GetChildren())
			{
				if (child is BoundExpression e && GetJSInsertionKind(e, out BoundExpression childDependent) != JSInsertionKind.None)
				{
					dependent = childDependent;
					return dependent.GetJSInsertionKind();
				}
			}
			dependent = null;
			return JSInsertionKind.None;
		}
		/*
		public static Formula CreateFunction(this Evaluator e, BoundExpression expression, BoundExpression dependent)
		{
			return new Formula(CreateFormula(e, expression, dependent));
		}

		private static string CreateFormula(this Evaluator e, BoundExpression expression, BoundExpression dependent)
		{
			if (expression == dependent)
				return "(%x%)";
			if (!Contains(expression, dependent))
				return ToString(e.EvaluateExpression(expression));
			switch (expression.Kind)
			{
				case BoundNodeKind.ConversionExpression:
					return CreateConversionFormula(e, (BoundConversion)expression, dependent);
				case BoundNodeKind.BinaryExpression:
					return CreateBinaryExpressionFormula(e, (BoundBinaryExpression)expression, dependent);
				case BoundNodeKind.StringExpression:
					return CreateStringExpressionFormula(e, (BoundStringExpression)expression, dependent);
				case BoundNodeKind.FunctionExpression:
					return CreateFunctionExpressionFormula(e, (BoundFunctionExpression)expression, dependent);
				default:
					throw new Exception();
			}
		}

		private static string CreateFunctionExpressionFormula(Evaluator e, BoundFunctionExpression expression, BoundExpression dependent)
		{
			var args = new string[expression.Arguments.Length];
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = CreateFormula(e, expression.Arguments[i], dependent);
			}
			var function = FunctionToString(expression.Function, args);
			if (function == null) throw new Exception();
			return function;
		}

		private static string CreateStringExpressionFormula(Evaluator e, BoundStringExpression expression, BoundExpression dependent)
		{
			return $"({string.Join(" + ", expression.Expressions.Select(ex => CreateFormula(e, ex, dependent)))})";
		}

		private static string CreateConversionFormula(this Evaluator e, BoundConversion expression, BoundExpression dependent)
		{
			return CreateFormula(e, expression.Expression, dependent);
		}

		private static string CreateBinaryExpressionFormula(Evaluator e, BoundBinaryExpression expression, BoundExpression dependent)
		{
			var left = CreateFormula(e, expression.Left, dependent);
			var right = CreateFormula(e, expression.Right, dependent);
			var op = OperatorToString(expression.Op.Kind);
			if (expression.Op.Kind == BoundBinaryOperatorKind.Division && expression.Type == PrimitiveTypeSymbol.Integer)
				return $"castToInt({left} {op} {right})";
			return $"({left} {op} {right})";
		}

		private static string FunctionToString(FunctionSymbol function, string[] args)
		{
			switch (function.Name)
			{
				case "pt":
					if (args.Length != 1)
						return null;
					return $"new StyleUnit({args[0]}, 'pt', undefined)";
				case "px":
					if (args.Length != 1)
						return null;
					return $"new StyleUnit({args[0]}, 'px', undefined)";
				case "pct":
					if (args.Length != 1)
						return null;
					return $"new StyleUnit({args[0]}, '%', undefined)";
				case "margin":
				case "padding":
					if (args.Length != 4)
						//TODO
						throw new NotImplementedException();
					return $"new Thickness({args[0]}, {args[1]}, {args[2]}, {args[3]})";
				case "hsl":
					return $"'hsl(' + {args[0]} + ', ' + {args[1]} + '%, ' + {args[2]} + '%)'";
				case "rgb":
					return $"'rgb(' + {args[0]} + ', ' + {args[1]} + ', ' + {args[2]} + ')'";
				case "mod":
					return $"({args[0]}) % ({args[1]})";
				case "int":
					return $"castToInt({args[0]})";
				case "toTime":
					return $"toTime({args[0]})";
				default:
					return null;
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
				case Unit u:
					var kind = "px";
					switch (u.Kind)
					{
						case Unit.UnitKind.Pixel: break;
						case Unit.UnitKind.Point:
							kind = "pt";
							break;
						case Unit.UnitKind.Percent:
							kind = "%";
							break;
						default:
							throw new Exception();
					}
					return $"new StyleUnit({ToString(u.Value)}, {ToString(kind)}, undefined)";
				default:
					return $"({value})";
			}
		}
		*/
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