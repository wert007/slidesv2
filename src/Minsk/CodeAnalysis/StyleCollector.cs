using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;
using SimpleLogger;
using Slides;
using Slides.Elements;
using Slides.Styling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Minsk.CodeAnalysis
{

	internal class StyleCollector
	{
		private readonly BoundStatement _statement;
		private readonly StatementEvaluator _evaluator;
		private readonly string _name;
		private SubstyleCollection _substyles;
		private readonly TypeSymbol _elementType;
		private readonly TypeSymbol _slideType;


		public StyleCollector(BoundBlockStatement statement, string name, VariableValueCollection variables)
		{
			_statement = statement;
			_evaluator = new StatementEvaluator(statement, new VariableValueCollection(variables));
			_name = name;
			_substyles = new SubstyleCollection();
			_elementType = BuiltInTypes.Instance.LookSymbolUp(typeof(Element));
			_slideType = BuiltInTypes.Instance.LookSymbolUp(typeof(SlideAttributes));
		}

		internal Style CollectFields()
		{
			if (_name == "std") return CollectStdStyle(_statement);
			return CollectCustomStyle(_statement);
		}

		private StdStyle CollectStdStyle(BoundStatement statement)
		{
			_substyles = new SubstyleCollection();
			CollectTypedFieldsFromStatement(statement);
			return new StdStyle(_substyles);
		}

		private void CollectTypedFieldsFromStatement(BoundStatement statement)
		{
			switch (statement.Kind)
			{
				case BoundNodeKind.BlockStatement:
					CollectTypedFieldsFromBlockStatement((BoundBlockStatement)statement);
					break;
				case BoundNodeKind.VariableDeclaration:
					CollectTypedFieldsFromVariableDeclaration((BoundVariableDeclaration)statement);
					break;
				case BoundNodeKind.ExpressionStatement:
					CollectTypedFieldsFromExpressionStatement((BoundExpressionStatement)statement);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void CollectTypedFieldsFromBlockStatement(BoundBlockStatement statement)
		{
			foreach (var s in statement.Statements)
				CollectTypedFieldsFromStatement(s);
		}

		private void CollectTypedFieldsFromVariableDeclaration(BoundVariableDeclaration statement)
		{
			_evaluator.EvaluateStatement(statement);
		}

		private void CollectTypedFieldsFromExpressionStatement(BoundExpressionStatement statement)
		{
			CollectTypedFieldsFromExpression(statement.Expression);
		}

		private void CollectTypedFieldsFromExpression(BoundExpression expression)
		{
			bool isStdStyle = _name == "std";
			switch (expression.Kind)
			{
				case BoundNodeKind.AssignmentExpression:
					CollectPropertiesFromAssignmentExpression((BoundAssignmentExpression)expression, isStdStyle);
					break;
				case BoundNodeKind.FunctionAccessExpression:
					CollectPropertiesFromFunctionAccessExpression((BoundFunctionAccessExpression)expression, isStdStyle);
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private void CollectPropertiesFromAssignmentExpression(BoundAssignmentExpression expression, bool isStdStyle)
		{
			var field = GetFieldFromExpression(expression.LValue, isStdStyle);
			var value = _evaluator.EvaluateExpression(expression.Expression);
			var selector = GetSelectorFromExpression(expression, isStdStyle);
			_substyles.AddProperty(selector, field, value);
		}

		private void CollectPropertiesFromFunctionAccessExpression(BoundFunctionAccessExpression expression, bool isStdStyle)
		{
			var selector = GetSelectorFromExpression(expression, isStdStyle);
			switch (expression.FunctionCall.Function.Name)
			{
				case "setTextmarker":
					//TODO: Introduce a place for such constants maybe??
					var property = "non-css-custom-text-marker";
					var value = _evaluator.EvaluateExpression(expression.FunctionCall.Arguments[0]);
					_substyles.AddProperty(selector, property, value);
					break;
				case "applyStyle":
					Logger.Log("applyStyle not supported");
					break;
				default:
					throw new NotImplementedException();
			}
		}

		private Selector GetSelectorFromExpression(BoundExpression expression, bool isStdStyle)
		{
			switch (expression.Kind)
			{
				case BoundNodeKind.AssignmentExpression:
					return GetSelectorFromAssignmentExpression((BoundAssignmentExpression)expression, isStdStyle);
				case BoundNodeKind.FieldAccessExpression:
					return GetSelectorFromFieldAccessExpression((BoundFieldAccessExpression)expression, isStdStyle);
				case BoundNodeKind.VariableExpression:
					return GetSelectorFromVariableExpression((BoundVariableExpression)expression, isStdStyle);
				case BoundNodeKind.FunctionAccessExpression:
					return GetSelectorFromFunctionAccessExpression((BoundFunctionAccessExpression)expression, isStdStyle);
				default:
					throw new NotImplementedException();
			}
		}

		private Selector GetSelectorFromVariableExpression(BoundVariableExpression expression, bool isStdStyle)
		{
			//TODO: ToLower could be wrong. Maybe you only want the first letter to be lowered..
			if (isStdStyle) return Selector.CreateType(expression.Variable.Name.ToLower());
			return Selector.CreateCustom(_name);
		}

		private Selector GetSelectorFromAssignmentExpression(BoundAssignmentExpression expression, bool isStdStyle)
		{
			switch (expression.LValue.Kind)
			{
				case BoundNodeKind.VariableExpression:
					if (!isStdStyle) throw new Exception();
					return Selector.CreateAll();
				case BoundNodeKind.FieldAccessExpression:
					return GetSelectorFromFieldAccessExpression((BoundFieldAccessExpression)expression.LValue, isStdStyle);
				default:
					throw new NotImplementedException();
			}
		}

		private Selector GetSelectorFromFieldAccessExpression(BoundFieldAccessExpression expression, bool isStdStyle)
		{
			var parentSelector = GetSelectorFromExpression(expression.Parent, isStdStyle);
			if (NeedsTypeAsSelector(expression.Type))
				parentSelector.AddField(expression.Field.Variable.Name);
			return parentSelector;
		}

		private Selector GetSelectorFromFunctionAccessExpression(BoundFunctionAccessExpression expression, bool isStdStyle)
		{
			return GetSelectorFromExpression(expression.Parent, isStdStyle);
		}

		private string GetFieldFromExpression(BoundExpression expression, bool isStdStyle)
		{
			switch (expression.Kind)
			{
				case BoundNodeKind.VariableExpression:
					if (!isStdStyle) throw new Exception();
					return GetFieldFromVariableExpression((BoundVariableExpression)expression);
				case BoundNodeKind.FieldAccessExpression:
					return GetFieldFromFieldAccessExpression((BoundFieldAccessExpression)expression, isStdStyle);
				default:
					throw new NotImplementedException();
			}
		}
		private string GetFieldFromVariableExpression(BoundVariableExpression expression) => expression.Variable.Name;
		private string GetFieldFromFieldAccessExpression(BoundFieldAccessExpression expression, bool isStdStyle)
		{
			var fieldName = expression.Field.Variable.Name;
			if (NeedsTypeAsSelector(expression.Parent.Type))
				return fieldName;
			else
				return $"{GetFieldFromExpression(expression.Parent, isStdStyle)}-{fieldName}";
		}

		private CustomStyle CollectCustomStyle(BoundStatement statement)
		{
			_substyles = new SubstyleCollection();
			CollectTypedFieldsFromStatement(statement);
			return new CustomStyle(_name, _substyles);
		}

		//TODO: Naming! Essentially you ask if a type is
		//      a Element or Slide and know you need to 
		//      create a subtype for it.
		private bool NeedsTypeAsSelector(TypeSymbol type)
		{
			return type.CanBeConvertedTo(_elementType) || type.CanBeConvertedTo(_slideType);
		}
	}
}