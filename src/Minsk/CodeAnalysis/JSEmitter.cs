using System;
using System.Collections.Generic;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.MathExpressions;
using Color = Slides.Color;
using System.Text;
using Slides.Elements;
using System.IO;
using System.CodeDom.Compiler;
using Slides.Helpers;

namespace Minsk.CodeAnalysis
{
	internal sealed class JSEmitter
	{
		private IndentedTextWriter _jsCode = new IndentedTextWriter(new StringWriter());
		private readonly Dictionary<string, string> _variableDefinitions = new Dictionary<string, string>();
		private string _idBase;
		private VariableValueCollection _variables;
		private StatementEvaluator _evaluator;
		private readonly HashSet<string> _registeredVariables = new HashSet<string>();
		private StringBuilder JSCodeStringBuilder => ((StringWriter)_jsCode.InnerWriter).GetStringBuilder();

		public JSEmitter()
		{
		}

		public JavaScriptCode Emit(BoundStatement node, VariableValueCollection inputVariables, string idBase)
		{
			_registeredVariables.Clear();
			_registeredVariables.Add("totalTime");
			_idBase = idBase;
			_variables = inputVariables;
			JSCodeStringBuilder.Clear();
			_jsCode.WriteLine();
			_variableDefinitions.Clear();
			_evaluator = new StatementEvaluator(node, _variables);
			EmitStatement(node);
			return new JavaScriptCode(JSCodeStringBuilder.ToString(), new Dictionary<string, string>(_variableDefinitions));
		}

		private void EmitStatement(BoundStatement node)
		{
			switch (node.Kind)
			{
				case BoundNodeKind.VariableDeclaration:
					EmitVariableDeclaration((BoundVariableDeclaration)node);
					break;
				case BoundNodeKind.ExpressionStatement:
					EmitExpressionStatement((BoundExpressionStatement)node);
					break;
				case BoundNodeKind.IfStatement:
					EmitIfStatement((BoundIfStatement)node);
					break;
				case BoundNodeKind.ForStatement:
					EmitForStatement((BoundForStatement)node);
					break;
				case BoundNodeKind.BlockStatement:
					EmitBlockStatement((BoundBlockStatement)node);
					break;
				default:
					throw new Exception($"Unexpected node {node.Kind}");
			}
		}

		private void EmitVariableDeclaration(BoundVariableDeclaration node)
		{
			var baseTmp = new StringWriter();
			var tmp = new IndentedTextWriter(baseTmp);
			EmitExpression(node.Initializer, tmp);
			_variableDefinitions.Add(node.Variable.Name, baseTmp.ToString());
		}

		private void EmitExpressionStatement(BoundExpressionStatement node)
		{
			EmitExpression(node.Expression);
			_jsCode.WriteLine(";");
		}

		private void EmitIfStatement(BoundIfStatement node)
		{
			_jsCode.Write("if(");
			EmitExpression(node.Condition);
			_jsCode.Write(") ");
			EmitStatement(node.Body);
			if (node.Else != null)
			{
				_jsCode.Write("else ");
				EmitStatement(node.Else);

			}
		}

		private void EmitForStatement(BoundForStatement node)
		{
			_jsCode.Write("for(");
			//var isForOfLoop = node.Collection.Type != BuiltInTypes.Instance.LookSymbolUp(typeof(Range));
			//if (!isForOfLoop) throw new NotImplementedException();
			//if (isForOfLoop) 
			_jsCode.Write("const ");
			//else _jsCode.Write("let ");
			_registeredVariables.Add(node.Variable.Name);
			_jsCode.Write(node.Variable.Name);
			_jsCode.Write(" of ");
			EmitExpression(node.Collection);
			_jsCode.Write(") ");
			EmitStatement(node.Body);
		}

		private void EmitBlockStatement(BoundBlockStatement node)
		{
			_jsCode.WriteLine("{");
			_jsCode.Indent++;
			foreach (var statement in node.Statements)
			{
				EmitStatement(statement);
			}
			_jsCode.Indent--;
			_jsCode.WriteLine("}");
		}

		private void EmitExpression(BoundExpression node, IndentedTextWriter writer = null)
		{
			if (writer == null)
			{
				writer = _jsCode;
			}
			switch (node.Kind)
			{
				case BoundNodeKind.StringExpression:
					EmitStringExpression((BoundStringExpression)node, writer);
					break;
				case BoundNodeKind.LiteralExpression:
					EmitLiteralExpression((BoundLiteralExpression)node, writer);
					break;
				case BoundNodeKind.VariableExpression:
					EmitVariableExpression((BoundVariableExpression)node, writer);
					break;
				case BoundNodeKind.AssignmentExpression:
					EmitAssignmentExpression((BoundAssignmentExpression)node, writer);
					break;
				case BoundNodeKind.UnaryExpression:
					EmitUnaryExpression((BoundUnaryExpression)node, writer);
					break;
				case BoundNodeKind.BinaryExpression:
					EmitBinaryExpression((BoundBinaryExpression)node, writer);
					break;
				case BoundNodeKind.FunctionExpression:
					EmitFunctionExpression((BoundFunctionExpression)node, writer);
					break;
				case BoundNodeKind.EmptyArrayConstructorExpression:
					EmitEmptyArrayConstructorExpression((BoundEmptyArrayConstructorExpression)node, writer);
					break;
				case BoundNodeKind.ArrayExpression:
					EmitArrayExpression((BoundArrayExpression)node, writer);
					break;
				case BoundNodeKind.EnumExpression:
					EmitEnumExpression((BoundEnumExpression)node, writer);
					break;
				case BoundNodeKind.FieldAccessExpression:
					EmitFieldAccessExpression((BoundFieldAccessExpression)node, writer);
					break;
				case BoundNodeKind.FunctionAccessExpression:
					EmitFunctionAccessExpression((BoundFunctionAccessExpression)node, writer);
					break;
				case BoundNodeKind.ConversionExpression:
					EmitConversion((BoundConversion)node, writer);
					break;
				case BoundNodeKind.MathExpression:
					EmitMathExpression((BoundMathExpression)node, writer);
					break;
				case BoundNodeKind.ArrayAccessExpression:
					EmitArrayAccessExpression((BoundArrayAccessExpression)node, writer);
					break;
				default:
					throw new Exception($"Unexpected node {node.Kind}");
			}
		}

		private void EmitStringExpression(BoundStringExpression node, IndentedTextWriter writer)
		{
			var isFirst = true;
			foreach (var expression in node.Expressions)
			{
				if (isFirst) isFirst = false;
				else writer.Write(" + ");
				EmitExpression(expression, writer);
			}
		}

		private void EmitLiteralExpression(BoundLiteralExpression node, IndentedTextWriter writer)
		{
			writer.EmitObject(node.Value);
		}

		private void EmitVariableExpression(BoundVariableExpression node, IndentedTextWriter writer)
		{
			EmitVariableSymbol(node.Variable, writer);
		}

		//TODO: It should be allowed to have variables named background, if they don't interfere
		//      with e.g. slide attributes. Like you can have 'let background' in a template
		//      Right now we assume there are never variables like this!
		private bool IsSlideAttribute(VariableSymbol variable)
		{
			switch (variable.Name)
			{
				case "background":
					return true;
				default:
					return false;
			}
		}

		private void EmitVariableSymbol(VariableSymbol variable, IndentedTextWriter writer)
		{

			if (IsSlideAttribute(variable))
			{
				if (!_variableDefinitions.ContainsKey("$slideAttributes"))
					_variableDefinitions.Add("$slideAttributes", $"document.getElementById('{_idBase}')");
				writer.Write("$slideAttributes.");
			}
			else if (!_registeredVariables.Contains(variable.Name))
			{
				var value = _evaluator.LookVariableUp(variable);
				//Emit variable initializer to _jsCode. Down there -------
				var tmp = new StringWriter();
				tmp.EmitObject(value);                                   //  |
				if (!_variableDefinitions.ContainsKey(variable.Name))//  v
					_variableDefinitions.Add(variable.Name, tmp.ToString());
			}

			writer.Write(variable.Name);
		}

		private void EmitAssignmentExpression(BoundAssignmentExpression node, IndentedTextWriter writer)
		{
			EmitExpression(node.LValue, writer);
			writer.Write(" = ");
			EmitExpression(node.Expression, writer);
		}

		private void EmitUnaryExpression(BoundUnaryExpression node, IndentedTextWriter writer)
		{
			switch (node.Op.Kind)
			{
				case BoundUnaryOperatorKind.Identity:
					break;
				case BoundUnaryOperatorKind.Negation:
					writer.Write("-");
					break;
				case BoundUnaryOperatorKind.LogicalNegation:
					writer.Write("!");
					break;
				default:
					throw new NotImplementedException();
			}
			EmitExpression(node.Operand, writer);
		}

		private void EmitBinaryExpression(BoundBinaryExpression node, IndentedTextWriter writer)
		{
			if (node.Op.Kind == BoundBinaryOperatorKind.Range)
			{
				EmitBinaryRangeOperator(node.Left, node.Right, writer);
				return;
			}
			EmitExpression(node.Left, writer);
			switch (node.Op.Kind)
			{
				case BoundBinaryOperatorKind.Concatination:
				case BoundBinaryOperatorKind.Addition:
					writer.Write(" + ");
					break;
				case BoundBinaryOperatorKind.Subtraction:
					writer.Write(" - ");
					break;
				case BoundBinaryOperatorKind.Multiplication:
					writer.Write(" * ");
					break;
				case BoundBinaryOperatorKind.Division:
					writer.Write(" / "); //TODO: Make sure, that if we input an int we get an int out!
					break;
				case BoundBinaryOperatorKind.LogicalAnd:
					writer.Write(" && ");
					break;
				case BoundBinaryOperatorKind.LogicalOr:
					writer.Write(" || ");
					break;
				//TODO: Use right js equals operator
				case BoundBinaryOperatorKind.Equals:
					writer.Write(" === ");
					break;
				case BoundBinaryOperatorKind.NotEquals:
					writer.Write(" !== ");
					break;
				case BoundBinaryOperatorKind.Less:
					writer.Write(" < ");
					break;
				case BoundBinaryOperatorKind.LessOrEquals:
					writer.Write(" <= ");
					break;
				case BoundBinaryOperatorKind.Greater:
					writer.Write(" > ");
					break;
				case BoundBinaryOperatorKind.GreaterOrEquals:
					writer.Write(" >= ");
					break;
				case BoundBinaryOperatorKind.EnumAddition:
					throw new NotImplementedException();
					break;
				case BoundBinaryOperatorKind.FilterAddition:
					throw new NotImplementedException();
					break;
				case BoundBinaryOperatorKind.Exponentiation:
					throw new NotImplementedException();
					break;
				default:
					throw new NotImplementedException();
			}
			EmitExpression(node.Right, writer);
		}

		private void EmitBinaryRangeOperator(BoundExpression from, BoundExpression to, IndentedTextWriter writer)
		{
			writer.Write("new NumberRange(");
			EmitExpression(from, writer);
			writer.Write(", ");
			EmitExpression(to, writer);
			writer.Write(", ");
			writer.Write("1"); //TODO: If from is bigger than to, we need -1 here!
			writer.Write(")");
		}

		private void EmitFunctionExpression(BoundFunctionExpression node, IndentedTextWriter writer)
		{
			bool isFirst = true;
			switch (node.Function.Name)
			{
				case "mod":
					EmitMod(node, writer);
					break;
				case "rgb":
				case "rgba":
					EmitRGBA(node, writer);
					break;
				case "hsl":
				case "hsla":
					EmitHSLA(node, writer);
					break;
				case "fixedWidth":
					EmitFixedWidth(node, writer);
					break;
				case "max":
				case "min":
					EmitMathFunction(node, writer);
					break;
				case "print":
				case "println":
					EmitPrintFunction(node, writer);
					break;
				case "youtube": //this is kind of a constructor. And we don't support these in js!
				case "image": //In slides image() returns path, width and hight
								  //In js ImageSource is just a string. so they wouldn't
								  //behave the same..
				case "csv": //No CSVFile Datatype in js!
					throw new NotImplementedException();
				case "stepBy":
				default:
					writer.Write(node.Function.Name);
					writer.Write("(");
					foreach (var arg in node.Arguments)
					{
						if (isFirst) isFirst = false;
						else writer.Write(", ");
						EmitExpression(arg, writer);
					}
					writer.Write(")");
					break;
			}
		}

		private void EmitMod(BoundFunctionExpression node, IndentedTextWriter writer)
		{
			writer.Write("(");
			if (node.Arguments[0].Kind == BoundNodeKind.BinaryExpression) writer.Write("(");
			EmitExpression(node.Arguments[0], writer);
			if (node.Arguments[0].Kind == BoundNodeKind.BinaryExpression) writer.Write(")");
			writer.Write(" % ");
			if (node.Arguments[1].Kind == BoundNodeKind.BinaryExpression) writer.Write("(");
			EmitExpression(node.Arguments[1], writer);
			if (node.Arguments[1].Kind == BoundNodeKind.BinaryExpression) writer.Write(")");
			writer.Write(")");
		}

		private void EmitRGBA(BoundFunctionExpression node, IndentedTextWriter writer)
		{
			writer.Write("'");
			writer.Write(node.Function.Name);
			writer.Write("(' + ");
			EmitExpression(node.Arguments[0], writer);
			writer.Write(" + ', ' + ");
			EmitExpression(node.Arguments[1], writer);
			writer.Write(" + ', ' + ");
			EmitExpression(node.Arguments[2], writer);
			if (node.Arguments.Length > 3)
			{
				writer.Write(" + ', ' + (");
				EmitExpression(node.Arguments[3], writer);
				writer.Write(" / 255)");
			}
			writer.Write(" + ')'");

		}

		private void EmitHSLA(BoundFunctionExpression node, IndentedTextWriter writer)
		{
			writer.Write("'");
			writer.Write(node.Function.Name);
			writer.Write("(' + ");
			EmitExpression(node.Arguments[0], writer);
			writer.Write(" + ', ' + ((");
			EmitExpression(node.Arguments[1], writer);
			writer.Write(") / 2.55) + '%, ' + ((");
			EmitExpression(node.Arguments[2], writer);
			writer.Write(") / 2.55) + '%'");
			if (node.Arguments.Length > 3)
			{
				writer.Write(" + ', ' + (");
				EmitExpression(node.Arguments[3], writer);
				writer.Write(") / 2.55");
			}
			writer.Write(" + ')'");
		}

		private void EmitFixedWidth(BoundFunctionExpression node, IndentedTextWriter writer)
		{
			if (node.Arguments[0].Type == PrimitiveTypeSymbol.Integer) writer.Write("fixedWidthInt(");
			else writer.Write("fixedWidthAny(");
			EmitExpression(node.Arguments[0], writer);
			writer.Write(", ");
			EmitExpression(node.Arguments[1], writer);
			writer.Write(")");
		}

		private void EmitMathFunction(BoundFunctionExpression node, IndentedTextWriter writer)
		{
			writer.Write("Math.");
			writer.Write(node.Function.Name);
			writer.Write("(");
			var isFirst = true;
			foreach (var arg in node.Arguments)
			{
				if (!isFirst) writer.Write(", ");
				EmitExpression(arg, writer);
			}
			writer.Write(")");
		}

		private void EmitPrintFunction(BoundFunctionExpression node, IndentedTextWriter writer)
		{
			writer.Write("console.log(");
			var isFirst = true;
			foreach (var arg in node.Arguments)
			{
				if (!isFirst) writer.Write(", ");
				EmitExpression(arg, writer);
			}
			writer.Write(")");
		}

		private void EmitEmptyArrayConstructorExpression(BoundEmptyArrayConstructorExpression node, IndentedTextWriter writer)
		{
			writer.Write("[]");
		}

		private void EmitArrayExpression(BoundArrayExpression node, IndentedTextWriter writer)
		{
			writer.Write("[");
			var isFirst = true;
			foreach (var expression in node.Expressions)
			{
				if (isFirst) isFirst = false;
				else writer.Write(", ");
				EmitExpression(expression, writer);
			}
			writer.Write("]");
		}

		private void EmitEnumExpression(BoundEnumExpression node, IndentedTextWriter writer)
		{
			writer.Write(node.Type.Name);
			writer.Write(".");
			writer.Write(node.Value);
		}

		private void EmitFieldAccessExpression(BoundFieldAccessExpression node, IndentedTextWriter writer)
		{
			EmitExpression(node.Parent, writer);
			writer.Write(".");
			writer.Write(ToField(node.Field.Variable.Name, node.Parent.Type));
		}

		private string ToField(string name, TypeSymbol parentType)
		{
			var elementType = BuiltInTypes.Instance.LookSymbolUp(typeof(Element));
			var slideAttributesType = BuiltInTypes.Instance.LookSymbolUp(typeof(SlideAttributes));
			if (!parentType.CanBeConvertedTo(elementType) &&
				!parentType.CanBeConvertedTo(slideAttributesType))
				return name;
			switch (name)
			{
				case "text": return "innerText";
				case "background": return "style.background";
				case "color": return "style.color";
				default:
					return name;
			}
		}

		private void EmitFunctionAccessExpression(BoundFunctionAccessExpression node, IndentedTextWriter writer)
		{
			EmitExpression(node.Parent, writer);
			writer.Write(".");
			EmitExpression(node.FunctionCall, writer);
		}

		//TODO: Fixme
		private void EmitConversion(BoundConversion node, IndentedTextWriter writer)
		{
			EmitExpression(node.Expression, writer);
		}

		private void EmitMathExpression(BoundMathExpression node, IndentedTextWriter writer)
		{
			throw new NotImplementedException();
		}

		private void EmitArrayAccessExpression(BoundArrayAccessExpression node, IndentedTextWriter writer)
		{
			EmitExpression(node.Child, writer);
			writer.Write("[");
			EmitExpression(node.Index, writer);
			writer.Write("]");
		}
	}
}