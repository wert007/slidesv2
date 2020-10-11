using Minsk.CodeAnalysis.Symbols;
using Slides;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal class SyntaxSugarReplacer : BoundTreeRewriter
	{
		private readonly Stack<BoundExpression> _usedAnonymFor = new Stack<BoundExpression>();
		protected override BoundExpression RewriteAnonymForExpression(BoundAnonymForExpression node)
		{
			var variable = new VariableSymbol("#anonymFor", false, PrimitiveTypeSymbol.Integer);
			return RewriteExpression(new BoundVariableExpression(variable));
		}
		protected override BoundExpression RewriteArrayAccessExpression(BoundArrayAccessExpression node)
		{
			var newChild = RewriteExpression(node.Child);
			var newIndex = RewriteExpression(node.Index);
			if(node.Index.Kind == BoundNodeKind.AnonymForExpression)
				_usedAnonymFor.Push(newChild);
			else if (newChild == node.Child && newIndex == node.Index)
				return node;
			return RewriteExpression(new BoundArrayAccessExpression(newChild, newIndex));
		}
		protected override BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
		{
			_usedAnonymFor.Clear();
			var newExpression = RewriteExpression(node.Expression);
			if (_usedAnonymFor.Count == 0)
				return node;
			else
			{
				var lengthFunction = new BoundFunctionExpression(ArrayTypeSymbol.LenFunction, new BoundExpression[0], null);
				BoundExpression initializer = new BoundFunctionAccessExpression(_usedAnonymFor.Pop(), lengthFunction);
				FunctionSymbol minFunction = null;
				_GlobalFunctionsConverter.Instance.TryGetSymbol("min", out var functions);
				foreach (var function in functions)
				{
					if (function.Parameter.Count == 2 &&
						function.Parameter[0].Type == PrimitiveTypeSymbol.Integer &&
						function.Parameter[1].Type == PrimitiveTypeSymbol.Integer)
						minFunction = function;
				}
				while(_usedAnonymFor.Any())
				{
					initializer = new BoundFunctionExpression(minFunction, new BoundExpression[]
					{
						new BoundFunctionAccessExpression(_usedAnonymFor.Pop(), lengthFunction),
						initializer,
					}, null);
				}
				var iteratorVariable = new VariableSymbol("#anonymFor", false, PrimitiveTypeSymbol.Integer);
				var op = BoundBinaryOperator.Bind(Syntax.SyntaxKind.PeriodPeriodToken, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Integer);
				var collection = new BoundBinaryExpression(new BoundLiteralExpression(0), op, initializer);
				var body = new BoundBlockStatement(new BoundStatement[]
				{
					new BoundExpressionStatement(newExpression),
				});
				var result = new BoundForStatement(iteratorVariable, null, collection, body);
				return RewriteStatement(result);
			}
		}
	}
	internal abstract class BoundTreeRewriter
	{
		public virtual BoundStatement RewriteStatement(BoundStatement node)
		{
			switch (node.Kind)
			{
				case BoundNodeKind.BlockStatement:
					return RewriteBlockStatement((BoundBlockStatement)node);
				case BoundNodeKind.VariableDeclaration:
					return RewriteVariableDeclaration((BoundVariableDeclaration)node);
				case BoundNodeKind.ExpressionStatement:
					return RewriteExpressionStatement((BoundExpressionStatement)node);
				case BoundNodeKind.AnimationStatement:
					return RewriteAnimationStatement((BoundAnimationStatement)node);
				case BoundNodeKind.DataStatement:
					return RewriteDataStatement((BoundStructStatement)node);
				case BoundNodeKind.FilterStatement:
					return RewriteFilterStatement((BoundFilterStatement)node);
				case BoundNodeKind.ForStatement:
					return RewriteForStatement((BoundForStatement)node);
				case BoundNodeKind.GroupStatement:
					return RewriteGroupStatement((BoundGroupStatement)node);
				case BoundNodeKind.IfStatement:
					return RewriteIfStatement((BoundIfStatement)node);
				case BoundNodeKind.JSInsertionStatement:
					return RewriteUseStatement((BoundJSInsertionStatement)node);
				case BoundNodeKind.ParameterBlockStatement:
				case BoundNodeKind.ParameterStatement:
					//TODO: idk.
					return node;
				case BoundNodeKind.SlideStatement:
					return RewriteSlideStatement((BoundSlideStatement)node);
				case BoundNodeKind.StyleStatement:
					return RewriteStyleStatement((BoundStyleStatement)node);
				case BoundNodeKind.SVGStatement:
					return RewriteSVGGroupStatement((BoundSVGStatement)node);
				case BoundNodeKind.TemplateStatement:
					return RewriteTemplateStatement((BoundTemplateStatement)node);
				case BoundNodeKind.TransitionStatement:
					return RewriteTransitionStatement((BoundTransitionStatement)node);
				default:
					throw new Exception($"Unexpected node: {node.Kind}");
			}
		}

		protected virtual BoundStatement RewriteAnimationStatement(BoundAnimationStatement node)
		{
			var newElementParameter = RewriteStatement(node.ElementParameter);
			var newTimeParameter = RewriteStatement(node.TimeParameter);
			List<BoundCaseStatement> builder = null;
			for (var i = 0; i < node.Body.Length; i++)
			{
				var oldStatement = node.Body[i];
				var newStatement = RewriteCaseStatement(oldStatement);
				if (newStatement != oldStatement)
				{
					if (builder == null)
					{
						builder = new List<BoundCaseStatement>();

						for (var j = 0; j < i; j++)
							builder.Add(node.Body[j]);
					}
				}

				if (builder != null)
					builder.Add(newStatement);
			}

			if (builder == null && newElementParameter == node.ElementParameter && newTimeParameter == node.TimeParameter)
				return node;

			return new BoundAnimationStatement(node.Variable, (BoundParameterStatement)newElementParameter, (BoundParameterStatement)newTimeParameter, builder.ToArray());
		}

		protected virtual BoundCaseStatement RewriteCaseStatement(BoundCaseStatement node)
		{
			var newCondition = RewriteExpression(node.Condition);
			var newBody = RewriteStatement(node.Body);
			if (newCondition == node.Condition && newBody == node.Body)
				return node;
			return new BoundCaseStatement(newCondition, newBody);
		}

		protected virtual BoundStatement RewriteDataStatement(BoundStructStatement node)
		{
			return node;
		}

		protected virtual BoundStatement RewriteFilterStatement(BoundFilterStatement node)
		{
			var newBody = RewriteStatement(node.Body);
			var newParameter = RewriteStatement(node.Parameter);
			if (newBody == node.Body && newParameter == node.Parameter)
				return node;
			return new BoundFilterStatement(node.Variable, (BoundParameterBlockStatement)newParameter, (BoundBlockStatement)newBody);
		}

		protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
		{
			var newCollection = RewriteExpression(node.Collection);
			var newBody = RewriteStatement(node.Body);
			if (node.Collection == newCollection && node.Body == newBody)
				return node;
			return new BoundForStatement(node.Variable, node.OptionalIndexer, newCollection, (BoundBlockStatement)newBody);
		}

		protected virtual BoundStatement RewriteGroupStatement(BoundGroupStatement node)
		{
			var newBody = RewriteStatement(node.Body);
			var newParameters = RewriteStatement(node.Parameters);
			if (newBody == node.Body && newParameters == node.Parameters)
				return node;
			return new BoundGroupStatement(node.Type, (BoundParameterBlockStatement)newParameters, (BoundBlockStatement)newBody);
		}

		protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
		{
			var newCondition = RewriteExpression(node.Condition);
			var newBody = RewriteStatement(node.Body);
			BoundStatement newElse = null;
			if (node.Else != null)
				newElse = RewriteStatement(node.Else);
			if (newCondition == node.Condition && newBody == node.Body && newElse == node.Else)
				return node;
			return new BoundIfStatement(newCondition, newBody, newElse);
		}

		protected virtual BoundStatement RewriteUseStatement(BoundJSInsertionStatement node)
		{
			var newBody = RewriteStatement(node.Body);
			//TODO: Rewrite dependencies!!!!!
			if (newBody == node.Body) return node;
			return new BoundJSInsertionStatement(node.Dependencies, newBody);
		}


		protected virtual BoundStatement RewriteSlideStatement(BoundSlideStatement node)
		{
			List<BoundStepStatement> builder = null;

			for (var i = 0; i < node.Statements.Length; i++)
			{
				var oldStatement = node.Statements[i];
				var newStatement = RewriteStepStatement(oldStatement);
				if (newStatement != oldStatement)
				{
					if (builder == null)
					{
						builder = new List<BoundStepStatement>();

						for (var j = 0; j < i; j++)
							builder.Add(node.Statements[j]);
					}
				}

				if (builder != null)
					builder.Add(newStatement);
			}

			if (builder == null)
				return node;
			return new BoundSlideStatement(node.IsVisible, node.Variable, node.Template, builder.ToArray());
		}

		protected virtual BoundStepStatement RewriteStepStatement(BoundStepStatement node)
		{
			var newBody = RewriteStatement(node.Body);
			if (newBody == node.Body)
				return node;
			return new BoundStepStatement(node.Variable, newBody);
		}

		protected virtual BoundStatement RewriteStyleStatement(BoundStyleStatement node)
		{
			var newBody = RewriteStatement(node.BoundBody);
			BoundStatement newParameter = null;
			if(node.BoundParameter != null)
				newParameter = RewriteStatement(node.BoundParameter);
			if (newBody == node.BoundBody && newParameter == node.BoundParameter)
				return node;
			return new BoundStyleStatement(node.Variable, (BoundParameterStatement)newParameter, (BoundBlockStatement)newBody);
		}

		protected virtual BoundStatement RewriteSVGGroupStatement(BoundSVGStatement node)
		{
			var newBody = RewriteStatement(node.Body);
			var newParameters = RewriteStatement(node.Parameters);
			if (newBody == node.Body && newParameters == node.Parameters)
				return node;
			return new BoundSVGStatement(node.Type, (BoundParameterBlockStatement)newParameters, (BoundBlockStatement)newBody);
		}

		protected virtual BoundStatement RewriteTemplateStatement(BoundTemplateStatement node)
		{
			var newBody = RewriteStatement(node.Body);
			var newParameter = RewriteStatement(node.SlideParameter);
			if (newBody == node.Body && newParameter == node.SlideParameter)
				return node;
			return new BoundTemplateStatement(node.Variable, (BoundParameterStatement)newParameter, (BoundBlockStatement)newBody);
		}

		protected virtual BoundStatement RewriteTransitionStatement(BoundTransitionStatement node)
		{
			var newBody = RewriteStatement(node.Body);
			var newFromParameter = RewriteStatement(node.FromParameter);
			var newToParameter = RewriteStatement(node.ToParameter);
			if (newBody == node.Body && newFromParameter == node.FromParameter && newToParameter == node.ToParameter)
				return node;
			return new BoundTransitionStatement(node.Variable, (BoundParameterStatement)newFromParameter, (BoundParameterStatement)newToParameter, (BoundBlockStatement)newBody);
		}

		protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
		{
			List<BoundStatement> builder = null;

			for (var i = 0; i < node.Statements.Length; i++)
			{
				var oldStatement = node.Statements[i];
				var newStatement = RewriteStatement(oldStatement);
				if (newStatement != oldStatement)
				{
					if (builder == null)
					{
						builder = new List<BoundStatement>();

						for (var j = 0; j < i; j++)
							builder.Add(node.Statements[j]);
					}
				}

				if (builder != null)
					builder.Add(newStatement);
			}

			if (builder == null)
				return node;

			return new BoundBlockStatement(builder.ToArray());
		}

		protected virtual BoundStatement RewriteVariableDeclaration(BoundVariableDeclaration node)
		{
			var initializer = RewriteExpression(node.Initializer);
			if (initializer == node.Initializer)
				return node;

			return new BoundVariableDeclaration(node.Variable, initializer);
		}

		protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
		{
			var expression = RewriteExpression(node.Expression);
			if (expression == node.Expression)
				return node;

			return new BoundExpressionStatement(expression);
		}

		public virtual BoundExpression RewriteExpression(BoundExpression node)
		{
			switch (node.Kind)
			{
				case BoundNodeKind.LiteralExpression:
					return RewriteLiteralExpression((BoundLiteralExpression)node);
				case BoundNodeKind.VariableExpression:
					return RewriteVariableExpression((BoundVariableExpression)node);
				case BoundNodeKind.AssignmentExpression:
					return RewriteAssignmentExpression((BoundAssignmentExpression)node);
				case BoundNodeKind.UnaryExpression:
					return RewriteUnaryExpression((BoundUnaryExpression)node);
				case BoundNodeKind.BinaryExpression:
					return RewriteBinaryExpression((BoundBinaryExpression)node);
				case BoundNodeKind.AnonymForExpression:
					return RewriteAnonymForExpression((BoundAnonymForExpression)node);
				case BoundNodeKind.ArrayAccessExpression:
					return RewriteArrayAccessExpression((BoundArrayAccessExpression)node);
				case BoundNodeKind.ArrayExpression:
					return RewriteArrayExpression((BoundArrayExpression)node);
				case BoundNodeKind.ConversionExpression:
					return RewriteConversion((BoundConversion)node);
				case BoundNodeKind.EmptyArrayConstructorExpression:
					return RewriteEmptyArrayConstructorExpression((BoundEmptyArrayConstructorExpression)node);
				case BoundNodeKind.EnumExpression:
					return RewriteEnumExpression((BoundEnumExpression)node);
				case BoundNodeKind.ErrorExpression:
					throw new Exception();
					return node;
				case BoundNodeKind.FieldAccessExpression:
					return RewriteFieldAccessExpression((BoundFieldAccessExpression)node);
				case BoundNodeKind.FunctionAccessExpression:
					return RewriteFunctionAccessExpression((BoundFunctionAccessExpression)node);
				case BoundNodeKind.FunctionExpression:
					return RewriteFunctionExpression((BoundFunctionExpression)node);
				case BoundNodeKind.MathExpression:
					return RewriteMathExpression((BoundMathExpression)node);
				case BoundNodeKind.StringExpression:
					return RewriteStringExpression((BoundStringExpression)node);
				default:
					throw new Exception($"Unexpected node: {node.Kind}");
			}
		}

		protected virtual BoundExpression RewriteAnonymForExpression(BoundAnonymForExpression node)
		{
			return node;
		}

		protected virtual BoundExpression RewriteArrayAccessExpression(BoundArrayAccessExpression node)
		{
			var newChild = RewriteExpression(node.Child);
			var newIndex = RewriteExpression(node.Index);
			if (newChild == node.Child && newIndex == node.Index)
				return node;
			return new BoundArrayAccessExpression(newChild, newIndex);
		}

		protected virtual BoundExpression RewriteArrayExpression(BoundArrayExpression node)
		{
			List<BoundExpression> builder = null;

			for (var i = 0; i < node.Expressions.Length; i++)
			{
				var oldExpression = node.Expressions[i];
				var newExpression = RewriteExpression(oldExpression);
				if (newExpression != oldExpression)
				{
					if (builder == null)
					{
						builder = new List<BoundExpression>();

						for (var j = 0; j < i; j++)
							builder.Add(node.Expressions[j]);
					}
				}

				if (builder != null)
					builder.Add(newExpression);
			}

			if (builder == null)
				return node;

			return new BoundArrayExpression(builder.ToArray(), node.BaseType);
		}

		protected virtual BoundExpression RewriteConversion(BoundConversion node)
		{
			var newExpression = RewriteExpression(node.Expression);
			if (newExpression == node.Expression)
				return node;
			return new BoundConversion(newExpression, node.Type);
		}

		protected virtual BoundExpression RewriteEmptyArrayConstructorExpression(BoundEmptyArrayConstructorExpression node)
		{
			var newLength = RewriteExpression(node.Length);
			if (newLength == node.Length)
				return node;
			return new BoundEmptyArrayConstructorExpression(newLength, node.Type);
		}

		protected virtual BoundExpression RewriteEnumExpression(BoundEnumExpression node)
		{
			return node;
		}

		protected virtual BoundExpression RewriteFieldAccessExpression(BoundFieldAccessExpression node)
		{
			var newParent = RewriteExpression(node.Parent);
			var newField = RewriteExpression(node.Field);
			if (newParent == node.Parent && newField == node.Field)
				return node;
			return new BoundFieldAccessExpression(newParent, (BoundVariableExpression)newField);
		}

		protected virtual BoundExpression RewriteFunctionAccessExpression(BoundFunctionAccessExpression node)
		{
			var newParent = RewriteExpression(node.Parent);
			var newFunction = RewriteFunctionExpression(node.FunctionCall);
			if (newParent == node.Parent && newFunction == node.FunctionCall)
				return node;
			return new BoundFunctionAccessExpression(newParent, (BoundFunctionExpression)newFunction);
		}

		protected virtual BoundExpression RewriteFunctionExpression(BoundFunctionExpression node)
		{
			List<BoundExpression> builder = null;

			for (var i = 0; i < node.Arguments.Length; i++)
			{
				var oldExpression = node.Arguments[i];
				var newExpression = RewriteExpression(oldExpression);
				if (newExpression != oldExpression)
				{
					if (builder == null)
					{
						builder = new List<BoundExpression>();

						for (var j = 0; j < i; j++)
							builder.Add(node.Arguments[j]);
					}
				}

				if (builder != null)
					builder.Add(newExpression);
			}

			if (builder == null)
				return node;
			return new BoundFunctionExpression(node.Function, builder.ToArray(), node.Source);
		}

		protected virtual BoundExpression RewriteMathExpression(BoundMathExpression node)
		{
			return node;
		}

		protected virtual BoundExpression RewriteStringExpression(BoundStringExpression node)
		{
			List<BoundExpression> builder = null;

			for (var i = 0; i < node.Expressions.Length; i++)
			{
				var oldExpression = node.Expressions[i];
				var newExpression = RewriteExpression(oldExpression);
				if (newExpression != oldExpression)
				{
					if (builder == null)
					{
						builder = new List<BoundExpression>();

						for (var j = 0; j < i; j++)
							builder.Add(node.Expressions[j]);
					}
				}

				if (builder != null)
					builder.Add(newExpression);
			}

			if (builder == null)
				return node;

			return new BoundStringExpression(builder.ToArray());
		}

		protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
		{
			return node;
		}

		protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
		{
			return node;
		}

		protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
		{
			var expression = RewriteExpression(node.Expression);
			var lvalue = RewriteExpression(node.LValue);
			if (expression == node.Expression && lvalue == node.LValue)
				return node;

			return new BoundAssignmentExpression(lvalue, expression);
		}

		protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
		{
			var operand = RewriteExpression(node.Operand);
			if (operand == node.Operand)
				return node;

			return new BoundUnaryExpression(node.Op, operand);
		}

		protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
		{
			var left = RewriteExpression(node.Left);
			var right = RewriteExpression(node.Right);
			if (left == node.Left && right == node.Right)
				return node;

			return new BoundBinaryExpression(left, node.Op, right);
		}
	}
}
