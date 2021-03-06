﻿using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Minsk.CodeAnalysis
{

	internal static class Serializer
	{
		public static string Serialize(BoundStatement statement)
		{
			var result = string.Empty;
			switch (statement.Kind)
			{
				case BoundNodeKind.GroupStatement:
					result = SerializeGroupStatement((BoundGroupStatement)statement);
					break;
				case BoundNodeKind.AnimationStatement:
					result = SerializeAnimationStatement((BoundAnimationStatement)statement);
					break;
				case BoundNodeKind.BlockStatement:
					result = SerializeBlockStatement((BoundBlockStatement)statement);
					break;
				case BoundNodeKind.CaseStatement:
					result = SerializeCaseStatement((BoundCaseStatement)statement);
					break;
				case BoundNodeKind.DataStatement:
					result = SerializeDataStatement((BoundStructStatement)statement);
					break;
				case BoundNodeKind.ExpressionStatement:
					result = SerializeExpressionStatement((BoundExpressionStatement)statement);
					break;
				case BoundNodeKind.FilterStatement:
					result = SerializeFilterStatement((BoundFilterStatement)statement);
					break;
				case BoundNodeKind.ForStatement:
					result = SerializeForStatement((BoundForStatement)statement);
					break;
				case BoundNodeKind.IfStatement:
					result = SerializeIfStatement((BoundIfStatement)statement);
					break;
				case BoundNodeKind.ParameterBlockStatement:
					result = SerializeParameterBlockStatement((BoundParameterBlockStatement)statement);
					break;
				case BoundNodeKind.ParameterStatement:
					result = SerializeParameterStatement((BoundParameterStatement)statement);
					break;
				case BoundNodeKind.SlideStatement:
					result = SerializeSlideStatement((BoundSlideStatement)statement);
					break;
				case BoundNodeKind.StepStatement:
					result = SerializeStepStatement((BoundStepStatement)statement);
					break;
				case BoundNodeKind.StyleStatement:
					result = SerializeStyleStatement((BoundStyleStatement)statement);
					break;
				case BoundNodeKind.TransitionStatement:
					result = SerializeTransitionStatement((BoundTransitionStatement)statement);
					break;
				case BoundNodeKind.VariableDeclaration:
					result = SerializeVariableDeclaration((BoundVariableDeclaration)statement);
					break;
				default:
					throw new NotImplementedException();
			}
			return $"{statement.Kind}::{result};\n";
		}

		public static string Serialize(BoundExpression expression)
		{
			var result = string.Empty;

			switch (expression.Kind)
			{
				case BoundNodeKind.ArrayExpression:
					result = SerializeArrayExpression((BoundArrayExpression)expression);
					break;
				case BoundNodeKind.AssignmentExpression:
					result = SerializeAssignmentExpression((BoundAssignmentExpression)expression);
					break;
				case BoundNodeKind.BinaryExpression:
					result = SerializeBinaryExpression((BoundBinaryExpression)expression);
					break;
				case BoundNodeKind.ConversionExpression:
					result = SerializeConversion((BoundConversion)expression);
					break;
				case BoundNodeKind.EnumExpression:
					result = SerializeEnumExpression((BoundEnumExpression)expression);
					break;
				case BoundNodeKind.ErrorExpression:
					result = "#ERROR";
					Logger.Log("Unexpected use of Serializer. (Actually we have a ErrorExpression..)");
					break;
				case BoundNodeKind.FieldAccessExpression:
					result = SerializeFieldAccessExpression((BoundFieldAccessExpression)expression);
					break;
				case BoundNodeKind.FunctionAccessExpression:
					result = SerializeFunctionAccessExpression((BoundFunctionAccessExpression)expression);
					break;
				case BoundNodeKind.FunctionExpression:
					result = SerializeFunctionExpression((BoundFunctionExpression)expression);
					break;
				case BoundNodeKind.LiteralExpression:
					result = SerializeLiteralExpression((BoundLiteralExpression)expression);
					break;
				case BoundNodeKind.StringExpression:
					result = SerializeStringExpression((BoundStringExpression)expression);
					break;
				case BoundNodeKind.UnaryExpression:
					result = SerializeUnaryExpression((BoundUnaryExpression)expression);
					break;
				case BoundNodeKind.VariableExpression:
					result = SerializeVariableExpression((BoundVariableExpression)expression);
					break;
				default:
					result = "";
					break;
			}
			return $"{expression.Kind}::{result};";
		}

		private static string SerializeMin(VariableSymbol symbol)
		{
			var postfix = string.Empty;
			if (symbol.IsReadOnly)
				postfix = "*";
			return $"{symbol.Name}{postfix}:{symbol.Type}";
		}

		private static string Serialize(FunctionSymbol symbol)
			=> $"{symbol.Name}<{symbol.Index}({string.Join(",", symbol.Parameter.Select(s => SerializeMin(s)))}):{symbol.Type}";
		private static string SerializeReference(LibrarySymbol symbol) => $"%{symbol.Name}";


		private static string Serialize(BoundUnaryOperator op) => $"{SyntaxFacts.GetText(op.SyntaxKind)}";
		private static string Serialize(BoundBinaryOperator op) => $"{SyntaxFacts.GetText(op.SyntaxKind)}";

		private static string SerializeGroupStatement(BoundGroupStatement statement)
			=> $"{statement.Type}<{Serialize(statement.Parameters)}:{Serialize(statement.Body)}";
		private static string SerializeAnimationStatement(BoundAnimationStatement statement)
			=> $"{statement.Variable.Name}<{statement.ElementParameter.Variable.Name}<{statement.TimeParameter.Variable.Name}:{string.Join("", statement.Body.Select(s => Serialize(s)))}";
		private static string SerializeBlockStatement(BoundBlockStatement statement)
			=> $"({string.Join("", statement.Statements.Select(s => Serialize(s)))})";
		private static string SerializeCaseStatement(BoundCaseStatement statement)
			=> $"{Serialize(statement.Condition)}>{Serialize(statement.Body)}";

		private static string SerializeDataStatement(BoundStructStatement statement)
		{
			var type = statement.Type as AdvancedTypeSymbol;
			return $"{type}={string.Join(",", (type as AdvancedTypeSymbol).Fields.Symbols.Select(s => SerializeMin(s)))}";
		}

		private static string SerializeExpressionStatement(BoundExpressionStatement statement) => Serialize(statement.Expression);
		private static string SerializeFilterStatement(BoundFilterStatement statement)
			=> $"{statement.Variable.Name}<{Serialize(statement.Parameter)}:{Serialize(statement.Body)}";
		private static string SerializeForStatement(BoundForStatement statement)
			=> $"{SerializeMin(statement.Variable)}:{Serialize(statement.Collection)}>{Serialize(statement.Body)}";

		private static string SerializeIfStatement(BoundIfStatement statement)
		{
			var elseClause = string.Empty;
			if (statement.Else != null)
				elseClause = ">" + Serialize(statement.Else);
			return $"{Serialize(statement.Condition)}>{Serialize(statement.Body)}{elseClause}";
		}

		private static string SerializeParameterBlockStatement(BoundParameterBlockStatement statement)
			=> $"{string.Join(",", statement.Statements.Select(s => Serialize(s)))}";

		private static string SerializeParameterStatement(BoundParameterStatement statement)
		{
			var initializer = string.Empty;
			if (statement.Initializer != null)
				initializer = $"={Serialize(statement.Initializer)}";
			return $"{SerializeMin(statement.Variable)}{initializer}";
		}

		private static string SerializeSlideStatement(BoundSlideStatement statement)
			=> $"{statement.Variable.Name}{SerializeTemplateStatement(statement.Template)}:{string.Join("", statement.Statements.Select(s => Serialize(s)))}";

		private static string SerializeTemplateStatement(VariableSymbol variable) => $"<{variable.Name}";

		private static string SerializeStepStatement(BoundStepStatement statement)
		{
			var name = "step";
			if (statement.Variable != null)
				name = statement.Variable.Name;
			return $"{name}:{Serialize(statement.Body)}";
		}

		private static string SerializeStyleStatement(BoundStyleStatement statement)
		{
			var name = "std";
			var parameter = string.Empty;
			if (statement.Variable != null)
			{
				name = statement.Variable.Name;
				parameter = $"<{Serialize(statement.BoundParameter)}";
			}
			return $"{name}{parameter}:{Serialize(statement.BoundBody)}";
		}

		private static string SerializeTransitionStatement(BoundTransitionStatement statement)
			=> $"{statement.Variable.Name}<{Serialize(new BoundParameterBlockStatement(new []{statement.FromParameter, statement.ToParameter}))}:{Serialize(statement.Body)}";
		private static string SerializeVariableDeclaration(BoundVariableDeclaration declaration)
			=> $"{SerializeMin(declaration.Variable)}={Serialize(declaration.Initializer)}";






		private static string SerializeArrayExpression(BoundArrayExpression expression)
			=> $"{string.Join(",", expression.Expressions.Select(e => Serialize(e)))}";
		private static string SerializeAssignmentExpression(BoundAssignmentExpression expression)
			=> $"({ Serialize(expression.LValue) })={Serialize(expression.Expression)}";
		private static string SerializeBinaryExpression(BoundBinaryExpression expression)
			=> $"{Serialize(expression.Left)}{Serialize(expression.Op)}{Serialize(expression.Right)}";
		private static string SerializeConversion(BoundConversion conversion) => $"{Serialize(conversion.Expression)}:{conversion.Type}";
		private static string SerializeEnumExpression(BoundEnumExpression expression) => $"{expression.Type}.{expression.Value}";
		private static string SerializeFieldAccessExpression(BoundFieldAccessExpression expression)
			=> $"{Serialize(expression.Parent)}.{Serialize(expression.Field)}";

		private static string SerializeFunctionAccessExpression(BoundFunctionAccessExpression expression)
			=> $"{Serialize(expression.Parent)}.{Serialize(expression.FunctionCall)}";
		private static string SerializeFunctionExpression(BoundFunctionExpression expression)
		{
			var source = string.Empty;
			if (expression.Source != null)
				source = SerializeReference(expression.Source) + ":";
			return $"{source}{Serialize(expression.Function)}<({string.Join(",", expression.Arguments.Select(e => Serialize(e)))})";
		}
		private static string SerializeLiteralExpression(BoundLiteralExpression expression) => Serialize(expression.ConstantValue);

		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");
		private static string Serialize(object value)
		{
			if(value is string str)
			{
				return $"'{str}'";
			}
			if (value is float f)
			{
				return f.ToString(_usCulture);
			}
			if (value is double d)
			{
				return d.ToString(_usCulture);
			}
			return value.ToString();
		}

		private static string SerializeStringExpression(BoundStringExpression expression)
			=> $"{string.Join(",", expression.Expressions.Select(e => Serialize(e)))}";
		private static string SerializeUnaryExpression(BoundUnaryExpression expression)
			=> $"{Serialize(expression.Op)}{Serialize(expression.Operand)}";

		private static string SerializeVariableExpression(BoundVariableExpression expression)
		{
			return $"{SerializeMin(expression.Variable)}";
		}

		private static string Serialize(BoundArrayAccessExpression arrayIndex)
		{
			var child = string.Empty;
			if (arrayIndex.Child != null)
				child = "<" + Serialize(arrayIndex.Child);
			return $"{Serialize(arrayIndex.Index)}{child}";
		}
	}
}
