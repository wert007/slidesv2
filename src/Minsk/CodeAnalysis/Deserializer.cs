﻿using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Slides;
using Slides.Debug;
using Slides.Filters;
using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace Minsk.CodeAnalysis
{
	internal class Deserializer
	{
		private int _position = 0;
		private string _text;
		private TypeSymbolTypeConverter _builtInTypes = TypeSymbolTypeConverter.Instance;

		public Deserializer(string text)
		{
			_text = text.Replace("\n", string.Empty).Replace(" ", string.Empty);
			_position = 0;
		}

		private string PeekToken()
		{
			int start = _position;
			int index = _position;

			if (_text[index] == ':' &&
				_text[index + 1] == ':')
				index += 2;
			else if (_text[index] == '[' &&
				_text[index + 1] == ']')
				index += 2;
			else if (_text[index] == '\'')
			{
				index++;
				while (_text[index] != '\'')
					index++;
				index++;
			}
			else if (char.IsDigit(_text[index]))
			{
				while (char.IsDigit(_text[index]))
					index++;
				if (_text[index] == '.')
				{
					index++;
					while (char.IsDigit(_text[index]))
						index++;
				}
				else if (_text[index] == '%')
					index++;
				else while (char.IsLetter(_text[index]))
						index++;
			}
			else if (char.IsLetter(_text[index]))
			{
				while (char.IsLetter(_text[index]))
					index++;
			}
			else index++;
			var result = _text.Substring(start, index - start);
			return result;
		}

		private string ConsumeToken()
		{
			var result = PeekToken();
			_position += result.Length;
			return result;
		}

		public BoundStatement Deserialize()
		{
			return DeserializeStatement();
		}


		private BoundNodeKind ConsumeStatementHeader()
		{
			var kind = ConsumeToken(); //Kind
			ConsumeToken(); //::
			return (BoundNodeKind)Enum.Parse(typeof(BoundNodeKind), kind);
		}
		private void ConsumeStatementTail()
		{
			ConsumeToken(); //;
		}

		private BoundStatement DeserializeStatement()
		{
			var kind = Enum.Parse(typeof(BoundNodeKind), PeekToken());
			switch (kind)
			{
				case BoundNodeKind.GroupStatement:
					return DeserializeGroupStatement();
				case BoundNodeKind.AnimationStatement:
					return DeserializeAnimationStatement();
				case BoundNodeKind.BlockStatement:
					return DeserializeBlockStatement();
				case BoundNodeKind.CaseStatement:
					return DeserializeCaseStatement();
				case BoundNodeKind.DataStatement:
					return DeserializeDataStatement();
				case BoundNodeKind.ExpressionStatement:
					return DeserializeExpressionStatement();
				case BoundNodeKind.FilterStatement:
					return DeserializeFilterStatement();
				case BoundNodeKind.ForStatement:
					return DeserializeForStatement();
				case BoundNodeKind.IfStatement:
					return DeserializeIfStatement();
				case BoundNodeKind.LibraryStatement:
					return DeserializeLibraryStatement();
				case BoundNodeKind.ParameterBlockStatement:
					return DeserializeParameterBlockStatement();
				case BoundNodeKind.ParameterStatement:
					return DeserializeParameterStatement();
				case BoundNodeKind.SlideStatement:
					return DeserializeSlideStatement();
				case BoundNodeKind.StepStatement:
					return DeserializeStepStatement();
				case BoundNodeKind.StyleStatement:
					return DeserializeStyleStatement();
				case BoundNodeKind.TransitionStatement:
					return DeserializeTransitionStatement();
				case BoundNodeKind.VariableDeclaration:
					return DeserializeVariableDeclaration();
				default:
					throw new NotImplementedException();
			}
		}

		private BoundExpression DeserializeExpression()
		{
			var kind = Enum.Parse(typeof(BoundNodeKind), PeekToken());
			switch (kind)
			{
				case BoundNodeKind.ArrayExpression:
					return DeserializeArrayExpression();
				case BoundNodeKind.AssignmentExpression:
					return DeserializeAssignmentExpression();
				case BoundNodeKind.BinaryExpression:
					return DeserializeBinaryExpression();
				case BoundNodeKind.Conversion:
					return DeserializeConversion();
				case BoundNodeKind.EnumExpression:
					return DeserializeEnumExpression();
				case BoundNodeKind.ErrorExpression:
					Logger.LogUnexpectedErrorExpression(typeof(Deserializer));
					return new BoundErrorExpression();
				case BoundNodeKind.FieldAccessExpression:
					return DeserializeFieldAccessExpression();
				case BoundNodeKind.FieldAssignmentExpression:
					return DeserializeFieldAssignmentExpression();
				case BoundNodeKind.FunctionAccessExpression:
					return DeserializeFunctionAccessExpression();
				case BoundNodeKind.FunctionExpression:
					return DeserializeFunctionExpression();
				case BoundNodeKind.IndexedArrayExpression:
					return DeserializeIndexedArrayExpression();
				case BoundNodeKind.LiteralExpression:
					return DeserializeLiteralExpression();
				case BoundNodeKind.StaticFieldAccessExpression:
					return DeserializeStaticFieldAccessExpression();
				case BoundNodeKind.StringExpression:
					return DeserializeStringExpression();
				case BoundNodeKind.UnaryExpression:
					return DeserializeUnaryExpression();
				case BoundNodeKind.VariableExpression:
					return DeserializeVariableExpression();
				default:
					throw new NotImplementedException();
			}
		}

		private BoundAssignmentExpression DeserializeAssignmentExpression()
		{
			ConsumeStatementHeader();
			var variables = ImmutableArray.CreateBuilder<VariableSymbol>();
			ConsumeToken(); //(
			variables.Add(DeserializeVariableSymbol());
			while (PeekToken() == ",")
			{
				ConsumeToken(); //,
				variables.Add(DeserializeVariableSymbol());
			}
			ConsumeToken(); //)
			ConsumeToken(); //=
			var expression = DeserializeExpression();
			ConsumeStatementTail();
			return new BoundAssignmentExpression(variables.ToImmutable(), expression);
		}

		private BoundBinaryExpression DeserializeBinaryExpression()
		{
			ConsumeStatementHeader();
			var left = DeserializeExpression();
			var opToken = ConsumeToken();
			var right = DeserializeExpression();
			var op = BoundBinaryOperator.Bind(SyntaxFacts.GetSyntaxKind(opToken), left.Type, right.Type);
			if (op == null)
				throw new Exception();
			ConsumeStatementTail();
			return new BoundBinaryExpression(left, op, right);
		}

		private BoundConversion DeserializeConversion()
		{
			ConsumeStatementHeader();
			var expression = DeserializeExpression();
			ConsumeToken(); //:
			var type = DeserializeTypeSymbol();

			if (type == null)
				throw new Exception();
			ConsumeStatementTail();
			return new BoundConversion(expression, type);
		}

		private BoundEnumExpression DeserializeEnumExpression()
		{
			ConsumeStatementHeader();
			var type = (EnumTypeSymbol)DeserializeTypeSymbol();
			if (type == null)
				throw new Exception();
			ConsumeToken(); //.
			var value = ConsumeToken();
			ConsumeStatementTail();
			return new BoundEnumExpression(type, value);
		}

		private BoundFieldAccesExpression DeserializeFieldAccessExpression()
		{
			ConsumeStatementHeader();
			var parent = DeserializeExpression();
			ConsumeToken(); //.
			var field = DeserializeVariableExpression();
			ConsumeStatementTail();
			return new BoundFieldAccesExpression(parent, field);
		}

		private BoundFieldAssignmentExpression DeserializeFieldAssignmentExpression()
		{
			ConsumeStatementHeader();
			var field = DeserializeFieldAccessExpression();
			ConsumeToken(); //=
			var initializer = DeserializeExpression();
			ConsumeStatementTail();
			return new BoundFieldAssignmentExpression(field, initializer);
		}

		private BoundFunctionAccessExpression DeserializeFunctionAccessExpression()
		{
			ConsumeStatementHeader();
			var parent = DeserializeExpression();
			ConsumeToken(); //.
			var function = DeserializeFunctionExpression();
			ConsumeStatementTail();
			return new BoundFunctionAccessExpression(parent, function);
		}

		private BoundFunctionExpression DeserializeFunctionExpression()
		{
			ConsumeStatementHeader();
			LibrarySymbol source = null;
			if (PeekToken() == "%")
			{
				source = DeserializeReference();
				ConsumeToken(); //:
			}
			var function = DeserializeFunctionSymbol();
			ConsumeToken(); //<
			ConsumeToken(); //(
			var arguments = ImmutableArray.CreateBuilder<BoundExpression>();
			while (PeekToken() != ")")
			{
				arguments.Add(DeserializeExpression());
				if (PeekToken() != ")")
					ConsumeToken(); //,
				else
					break;
			}
			if (PeekToken() == ")")
				ConsumeToken();
			ConsumeStatementTail();
			return new BoundFunctionExpression(function, arguments.ToImmutable(), source);
		}

		private FunctionSymbol DeserializeFunctionSymbol()
		{
			var identifier = ConsumeToken();
			ConsumeToken(); //<
			ConsumeToken(); //(
			var parameter = new VariableSymbolCollection();
			while (PeekToken() != ")")
			{
				parameter.Add(DeserializeVariableSymbol());

				if (PeekToken() != ")")
					ConsumeToken(); //,
				else
					break;
			}
			if (PeekToken() == ")")
				ConsumeToken();
			parameter.Seal();
			ConsumeToken(); //:
			var returnType = DeserializeTypeSymbol();

			return new FunctionSymbol(identifier, parameter, returnType);
		}

		private LibrarySymbol DeserializeReference()
		{
			ConsumeToken(); //%
			var name = ConsumeToken();
			//TODO!!!!!!!!!!!!!!!!!!!!!!!!!!
			return new LibrarySymbol(name);
		}

		private BoundIndexedArrayExpression DeserializeIndexedArrayExpression()
		{
			ConsumeStatementHeader();
			var variable = DeserializeVariableSymbol();
			ConsumeToken(); //<
			var boundIndex = DeserializeExpression();
			ConsumeStatementTail();
			return new BoundIndexedArrayExpression(variable, boundIndex);
		}

		private BoundLiteralExpression DeserializeLiteralExpression()
		{
			ConsumeStatementHeader();
			var value = DeserializeObject();
			ConsumeStatementTail();
			return new BoundLiteralExpression(value);
		}
		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");

		private object DeserializeObject()
		{
			var token = ConsumeToken();
			if (int.TryParse(token, out int intResult))
				return intResult;
			if (float.TryParse(token, NumberStyles.Float, _usCulture, out float floatResult))
				return floatResult;
			if (bool.TryParse(token, out bool boolResult))
				return boolResult;
			if (token.StartsWith("'") && token.EndsWith("'"))
			{
				return token.Trim('\'');
			}
			if(Unit.TryParse(token, _usCulture, out Unit unitResult))
			{
				return unitResult;
			}
			if(Time.TryParse(token, _usCulture, out Time timeResult))
			{
				return timeResult;
			}
			return token;
		}

		private BoundStaticFieldAccesExpression DeserializeStaticFieldAccessExpression()
		{
			ConsumeStatementHeader();
			var type = (AdvancedTypeSymbol)DeserializeTypeSymbol();
			if (type == null)
				throw new Exception();
			ConsumeToken(); //.
			var field = DeserializeVariableSymbol();
			ConsumeStatementTail();
			return new BoundStaticFieldAccesExpression(type, field);
		}

		private BoundStringExpression DeserializeStringExpression()
		{
			ConsumeStatementHeader();
			var expressions = ImmutableArray.CreateBuilder<BoundExpression>();
			expressions.Add(DeserializeExpression());
			while (PeekToken() == ",")
			{
				ConsumeToken();
				expressions.Add(DeserializeExpression());
			}
			ConsumeStatementTail();
			return new BoundStringExpression(expressions.ToImmutable());
		}

		private BoundUnaryExpression DeserializeUnaryExpression()
		{
			ConsumeStatementHeader();
			var opText = ConsumeToken();
			var operand = DeserializeExpression();
			var op = BoundUnaryOperator.Bind(SyntaxFacts.GetSyntaxKind(opText), operand.Type);
			if (op == null)
				throw new Exception();
			ConsumeStatementTail();
			return new BoundUnaryExpression(op, operand);
		}

		private BoundVariableExpression DeserializeVariableExpression()
		{
			ConsumeStatementHeader();
			var variable = DeserializeVariableSymbol();
			BoundArrayIndex boundArrayIndex = null;
			if (PeekToken() == "<")
			{
				ConsumeToken();
				boundArrayIndex = DeserializeArrayIndex();
			}
			ConsumeStatementTail();
			return new BoundVariableExpression(variable, boundArrayIndex, variable.Type);
		}

		private BoundArrayIndex DeserializeArrayIndex()
		{
			var boundIndex = DeserializeExpression();
			BoundArrayIndex child = null;
			if (PeekToken() == "<")
			{
				ConsumeToken();
				child = DeserializeArrayIndex();
			}
			return new BoundArrayIndex(boundIndex, child);
		}

		private BoundArrayExpression DeserializeArrayExpression()
		{
			ConsumeStatementHeader();
			var expressions = ImmutableArray.CreateBuilder<BoundExpression>();
			expressions.Add(DeserializeExpression());
			while (PeekToken() == ",")
			{
				ConsumeToken();
				expressions.Add(DeserializeExpression());
			}
			ConsumeStatementTail();
			return new BoundArrayExpression(expressions.ToImmutable());
		}

		private BoundGroupStatement DeserializeGroupStatement()
		{
			ConsumeStatementHeader();
			var identifier = ConsumeToken();
			ConsumeToken(); //<
			var parameters = DeserializeParameterBlockStatement();
			ConsumeToken(); //:
			var body = DeserializeBlockStatement();

			var fields = new VariableSymbolCollection(parameters.Statements.Select(p => p.Variable));
			var constructor = new FunctionSymbolCollection();
			constructor.Add(new FunctionSymbol("constructor", fields, null));
			constructor.Seal();
			var type = new AdvancedTypeSymbol(identifier, fields, constructor, FunctionSymbolCollection.Empty, _builtInTypes.LookSymbolUp(typeof(Element)));
			constructor[0].Type = type;
			if (type == null)
				throw new Exception();
			ConsumeStatementTail();
			return new BoundGroupStatement(type, parameters, body);
		}

		private BoundAnimationStatement DeserializeAnimationStatement()
		{
			ConsumeStatementHeader();
			var identifier = ConsumeToken();
			var variable = new VariableSymbol(identifier, true, _builtInTypes.LookSymbolUp(typeof(AnimationSymbol)), false);
			ConsumeToken(); //<
			var elementIdentifier = ConsumeToken();
			var elementVariable = new VariableSymbol(elementIdentifier, false, _builtInTypes.LookSymbolUp(typeof(Element)), false);
			var element = new BoundParameterStatement(elementVariable, null);
			ConsumeToken(); //<
			var timeIdentifier = ConsumeToken();
			var timeVariable = new VariableSymbol(timeIdentifier, true, _builtInTypes.LookSymbolUp(typeof(Time)), false);
			var time = new BoundParameterStatement(timeVariable, null);
			ConsumeToken(); //:
			var body = ImmutableArray.CreateBuilder<BoundCaseStatement>();
			while (PeekToken() != ";")
			{
				body.Add(DeserializeCaseStatement());
			}
			ConsumeStatementTail();
			return new BoundAnimationStatement(variable, element, time, body.ToImmutable());
		}

		private BoundCaseStatement DeserializeCaseStatement()
		{
			ConsumeStatementHeader();
			var condition = DeserializeExpression();
			ConsumeToken(); //>
			var body = DeserializeStatement();
			ConsumeStatementTail();
			return new BoundCaseStatement(condition, body);
		}

		private BoundDataStatement DeserializeDataStatement()
		{
			ConsumeStatementHeader();
			var identifier = ConsumeToken();
			ConsumeToken(); //=
			var fields = new VariableSymbolCollection(); // ImmutableArray.CreateBuilder<VariableSymbol>();
			fields.Add(DeserializeVariableSymbol());
			while (PeekToken() == ",")
			{
				ConsumeToken();
				fields.Add(DeserializeVariableSymbol());
			}
			fields.Seal();
			var constructor = new FunctionSymbolCollection();
			constructor.Add(new FunctionSymbol("constructor", VariableSymbolCollection.Empty, null));
			constructor.Add(new FunctionSymbol("constructor", fields, null));
			var type = new AdvancedTypeSymbol(identifier, fields, constructor);
			foreach (var c in constructor)
				c.Type = type;
			ConsumeStatementTail();
			return new BoundDataStatement(type);
		}

		private BoundExpressionStatement DeserializeExpressionStatement()
		{
			ConsumeStatementHeader();
			var expression = DeserializeExpression();
			ConsumeStatementTail();
			return new BoundExpressionStatement(expression);
		}


		private BoundFilterStatement DeserializeFilterStatement()
		{
			ConsumeStatementHeader();
			var identifier = ConsumeToken();
			var variable = new VariableSymbol(identifier, true, _builtInTypes.LookSymbolUp(typeof(Filter)), false);
			ConsumeToken(); // <
			var parameter = DeserializeParameterBlockStatement();
			ConsumeToken(); //:
			var body = DeserializeBlockStatement();
			ConsumeStatementTail();
			return new BoundFilterStatement(variable, parameter, body);
		}

		private BoundForStatement DeserializeForStatement()
		{
			ConsumeStatementHeader();
			var variable = DeserializeVariableSymbol();
			ConsumeToken(); //:
			var collection = DeserializeExpression();
			ConsumeToken(); //>
			var body = DeserializeBlockStatement();
			ConsumeStatementTail();
			return new BoundForStatement(variable, collection, body);
		}

		private BoundIfStatement DeserializeIfStatement()
		{
			ConsumeStatementHeader();
			var boundCondition = DeserializeExpression();
			ConsumeToken(); //>
			var boundBody = DeserializeStatement();
			BoundStatement boundElse = null;
			if (PeekToken() == ">")
			{
				ConsumeToken(); //>
				boundElse = DeserializeStatement();
			}
			ConsumeStatementTail();
			return new BoundIfStatement(boundCondition, boundBody, boundElse);
		}

		private BoundLibraryStatement DeserializeLibraryStatement()
		{
			ConsumeStatementHeader();
			var identifier = ConsumeToken();
			var variable = new VariableSymbol(identifier, true, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol)), false);
			ConsumeToken(); //:
			var body = DeserializeStatement();
			ConsumeStatementTail();
			return new BoundLibraryStatement(variable, body);
		}

		private BoundSlideStatement DeserializeSlideStatement()
		{
			ConsumeStatementHeader();
			var identifier = ConsumeToken();
			var variable = new VariableSymbol(identifier, true, _builtInTypes.LookSymbolUp(typeof(SlideAttributes)), false);
			var statements = ImmutableArray.CreateBuilder<BoundStepStatement>();
			ConsumeToken(); //:
			while (PeekToken() != ";")
			{
				statements.Add(DeserializeStepStatement());
			}
			ConsumeStatementTail();
			return new BoundSlideStatement(variable, statements.ToImmutable());
		}

		private BoundStepStatement DeserializeStepStatement()
		{
			ConsumeStatementHeader();
			var name = ConsumeToken();
			if (name == "step")
				name = null;
			ConsumeToken(); //:
			var body = DeserializeStatement();
			ConsumeStatementTail();
			return new BoundStepStatement(name, body);
		}

		private BoundStyleStatement DeserializeStyleStatement()
		{
			ConsumeStatementHeader();
			var identifier = ConsumeToken();
			BoundParameterStatement boundParameter = null;
			VariableSymbol variable = null;
			if (identifier != "std")
			{
				ConsumeToken(); //<
				boundParameter = DeserializeParameterStatement();
				variable = new VariableSymbol(identifier, true, _builtInTypes.LookSymbolUp(typeof(Style)), false);
			}
			ConsumeToken(); //:
			var boundBody = DeserializeBlockStatement();
			ConsumeStatementTail();
			return new BoundStyleStatement(variable, boundParameter, boundBody);
		}

		private BoundBlockStatement DeserializeBlockStatement()
		{
			ConsumeStatementHeader();
			ConsumeToken(); //(
			var statements = ImmutableArray.CreateBuilder<BoundStatement>();
			while (PeekToken() != ")")
			{
				statements.Add(DeserializeStatement());

			}
			ConsumeToken(); //)
			ConsumeStatementTail();
			return new BoundBlockStatement(statements.ToImmutable());
		}

		private BoundParameterBlockStatement DeserializeParameterBlockStatement()
		{
			ConsumeStatementHeader();
			var statements = ImmutableArray.CreateBuilder<BoundParameterStatement>();
			if (PeekToken() != ";")
			{
				statements.Add(DeserializeParameterStatement());
				while (PeekToken() == ",")
				{
					ConsumeToken(); //,
					statements.Add(DeserializeParameterStatement());
				}
			}
			ConsumeStatementTail();
			return new BoundParameterBlockStatement(statements.ToImmutable());
		}

		private BoundParameterStatement DeserializeParameterStatement()
		{
			ConsumeStatementHeader();
			var variable = DeserializeVariableSymbol();
			BoundExpression initializer = null;
			var token = PeekToken();
			if (token == "=")
			{
				ConsumeToken(); //=
				initializer = DeserializeExpression();
			}
			ConsumeStatementTail();
			return new BoundParameterStatement(variable, initializer);
		}

		private BoundTransitionStatement DeserializeTransitionStatement()
		{
			ConsumeStatementHeader();
			var name = ConsumeToken();
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Transition)), false);
			ConsumeToken(); //<
			var boundParameters = DeserializeParameterBlockStatement();
			ConsumeToken(); //:
			var boundBody = DeserializeBlockStatement();

			ConsumeStatementTail();
			return new BoundTransitionStatement(variable, boundParameters, boundBody);
		}

		private BoundVariableDeclaration DeserializeVariableDeclaration()
		{
			ConsumeStatementHeader();

			var variables = ImmutableArray.CreateBuilder<VariableSymbol>();
			BoundExpression initializer = null;
			variables.Add(DeserializeVariableSymbol());
			while (PeekToken() == ",")
			{
				ConsumeToken();
				variables.Add(DeserializeVariableSymbol());
			}
			ConsumeToken();//=
			initializer = DeserializeExpression();

			ConsumeStatementTail();
			return new BoundVariableDeclaration(variables.ToImmutable(), initializer);
		}

		private VariableSymbol DeserializeVariableSymbol()
		{
			bool isVisible = true;
			if (PeekToken() == "~")
			{
				isVisible = false;
				ConsumeToken();
			}
			var identifier = ConsumeToken();
			ConsumeToken(); //:
			var type = DeserializeTypeSymbol();
			return new VariableSymbol(identifier, false, type, type.IsData)
			{
				IsVisible = isVisible
			};
		}

		private TypeSymbol DeserializeTypeSymbol()
		{
			var identifier = ConsumeToken();

			if (identifier == "Tuple")
			{
				ConsumeToken(); //(
				var childTypes = ImmutableArray.CreateBuilder<TypeSymbol>();
				childTypes.Add(DeserializeTypeSymbol());
				while (PeekToken() == ",")
				{
					ConsumeToken(); //,
					childTypes.Add(DeserializeTypeSymbol());
				}
				ConsumeToken(); //)
				return new TupleTypeSymbol(childTypes.ToArray());

			}
			else
			{
				var type = TypeSymbol.FromString(identifier);
				if (type == null)
					throw new Exception();
				if (PeekToken() == "?" ||
					PeekToken() == "[]")
					return DeserializeSubTypeSymbol(type);
				return type;
			}

		}

		private TypeSymbol DeserializeSubTypeSymbol(TypeSymbol type)
		{
			var token = ConsumeToken();
			if (token == "?")
				return new NullableTypeSymbol(type);
			if (token == "[]")
			{
				if (PeekToken() == "?" ||
					PeekToken() == "[]")
					return DeserializeSubTypeSymbol(new ArrayTypeSymbol(type));
				else return new ArrayTypeSymbol(type);
			}
				throw new Exception();
		}

		//private TypeSymbol ToType(string text)
		//{
		//	//Trying our luck here..
		//	if (text.EndsWith("?"))
		//		return new NullableTypeSymbol(ToType(text.Remove(text.Length - 1)));
		//	if (text.EndsWith("[]"))
		//		return new ArrayTypeSymbol(ToType(text.Remove(text.Length - 2)));
		//	return TypeSymbol.FromString(text);
		//}
	}
}
