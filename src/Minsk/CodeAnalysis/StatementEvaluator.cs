using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Elements;
using Slides.MathExpressions;

namespace Minsk.CodeAnalysis
{
	internal class StatementEvaluator : BasicEvaluator
	{
		protected VariableValueCollection _variables;

		public StatementEvaluator(BoundStatement root, VariableValueCollection variables) : base(root)
		{
			_variables = variables;
		}

		public override object LookVariableUp(VariableSymbol variable)
		{
			if (_variables.HasKey(variable)) return _variables[variable];
			return base.LookVariableUp(variable);
		}

		public void EvaluateStatement(BoundStatement node)
		{
			switch (node.Kind)
			{
				case BoundNodeKind.BlockStatement:
					EvaluateBlockStatement((BoundBlockStatement)node);
					break;
				case BoundNodeKind.VariableDeclaration:
					EvaluateVariableDeclaration((BoundVariableDeclaration)node);
					break;
				case BoundNodeKind.ForStatement:
					EvaluateForStatement((BoundForStatement)node);
					break;
				case BoundNodeKind.ExpressionStatement:
					EvaluateExpressionStatement((BoundExpressionStatement)node);
					break;
				case BoundNodeKind.SlideStatement:
					EvaluateSlideStatement((BoundSlideStatement)node);
					break;
				case BoundNodeKind.GroupStatement:
					EvaluateGroupStatement((BoundGroupStatement)node);
					break;
				case BoundNodeKind.StyleStatement:
					EvaluateStyleStatement((BoundStyleStatement)node);
					break;
				case BoundNodeKind.AnimationStatement:
					EvaluateAnimationStatement((BoundAnimationStatement)node);
					break;
				case BoundNodeKind.DataStatement:
					EvaluateDataStatement((BoundDataStatement)node);
					break;
				case BoundNodeKind.IfStatement:
					EvaluateIfStatement((BoundIfStatement)node);
					break;
				case BoundNodeKind.CaseStatement:
					EvaluateCaseStatement((BoundCaseStatement)node);
					break;
				case BoundNodeKind.TransitionStatement:
					EvaluateTransitionStatement((BoundTransitionStatement)node);
					break;
				case BoundNodeKind.FilterStatement:
					EvaluateFilterStatement((BoundFilterStatement)node);
					break;
				case BoundNodeKind.TemplateStatement:
					EvaluateTemplateStatement((BoundTemplateStatement)node);
					break;
				case BoundNodeKind.SVGStatement:
					EvaluateSVGStatement((BoundSVGStatement)node);
					break;
				case BoundNodeKind.JSInsertionStatement:
					EvaluateUseStatement((BoundJSInsertionStatement)node);
					break;
				case BoundNodeKind.ParameterBlockStatement:
				case BoundNodeKind.StepStatement:
				case BoundNodeKind.ParameterStatement:
				default:
					throw new NotImplementedException();
			}
		}

		protected virtual void EvaluateBlockStatement(BoundBlockStatement node)
		{
			foreach (var statement in node.Statements)
			{
				EvaluateStatement(statement);
			}
		}


		protected virtual void EvaluateVariableDeclaration(BoundVariableDeclaration node)
		{
			var value = EvaluateExpression(node.Initializer);

			value = CheckIfIsImport(value);

	


			if (value != null && value.GetType() == typeof(LibrarySymbol)) 
				//I don't know why this was here. Do we need it? We shall see..
				// && _referenced != null)
				return;
			var variable = node.Variable;
			TryAddChildren(variable, value);
			_variables[variable] = value;
			if (value is Element e)
			{
				e.name = variable.Name;
				DeclareElement(e);
			}
			if (value is MathFormula m)
				m.Name = variable.Name;
		
		}

		protected virtual void DeclareElement(Element e) { }
		protected virtual object CheckIfIsImport(object value) => value;
		protected virtual void TryAddChildren(VariableSymbol variable, object value) { }


		protected virtual void EvaluateForStatement(BoundForStatement node)
		{
		//	_variables = _variables.Push();
			var collection = EvaluateExpression(node.Collection);
			if (collection is Range r)
			{
				if (r.Step >= 0)
					for (int i = r.From; i < r.To; i += r.Step)
					{
						_variables[node.Variable] = i;
						EvaluateStatement(node.Body);
					}
				else
					for (int i = r.From; i > r.To; i += r.Step)
					{
						_variables[node.Variable] = i;
						EvaluateStatement(node.Body);
					}
			}
			else
			{
				if (!(collection is IEnumerable<object> e))
					throw new Exception();

				foreach (var item in e)
				{
					_variables[node.Variable] = item;
					EvaluateStatement(node.Body);
				}
			}
			//_variables = _variables.Pop(out var _);

		}

		protected virtual void EvaluateExpressionStatement(BoundExpressionStatement node)
		{
			EvaluateExpression(node.Expression);
		}

		protected virtual void EvaluateSlideStatement(BoundSlideStatement node) { }
		protected virtual void EvaluateGroupStatement(BoundGroupStatement node) { }
		protected virtual void EvaluateStyleStatement(BoundStyleStatement node) { }
		protected virtual void EvaluateAnimationStatement(BoundAnimationStatement node) { }
		protected virtual void EvaluateDataStatement(BoundDataStatement node) { }

		protected virtual void EvaluateIfStatement(BoundIfStatement node)
		{
			var condition = EvaluateExpression(node.Condition);
			if (node.Condition.Type.Type == TypeType.Noneable)
			{
				if (condition != null)
					EvaluateStatement(node.Body);
				else if (node.Else != null)
					EvaluateStatement(node.Else);
			}
			else if (condition.Equals(true))
				EvaluateStatement(node.Body);
			else if (node.Else != null)
				EvaluateStatement(node.Else);
		}

		protected virtual void EvaluateCaseStatement(BoundCaseStatement node) { }
		protected virtual void EvaluateTransitionStatement(BoundTransitionStatement node) { }
		protected virtual void EvaluateFilterStatement(BoundFilterStatement node) { }
		protected virtual void EvaluateTemplateStatement(BoundTemplateStatement node) { }
		protected virtual void EvaluateSVGStatement(BoundSVGStatement node) { }
		protected virtual void EvaluateUseStatement(BoundJSInsertionStatement node) { }
	}
}