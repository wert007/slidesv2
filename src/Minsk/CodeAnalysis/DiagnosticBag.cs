using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;

namespace Minsk.CodeAnalysis
{
	internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
	{
		private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

		public string FileName { get; }

		public static void OutputToConsole(ImmutableArray<Diagnostic> diagnostics, SourceText text)
		{
			foreach (var diagnostic in diagnostics)
			{
				diagnostic.OutputToConsole(text);
			}
		}

		public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public DiagnosticBag(string fileName)
		{
			FileName = fileName;
		}

		public void AddRange(DiagnosticBag diagnostics)
		{
			_diagnostics.AddRange(diagnostics._diagnostics);
		}

		private void Report(TextSpan span, string message, DiagnosticLevel level)
		{
			var diagnostic = new Diagnostic(span, message, FileName, level);
			_diagnostics.Add(diagnostic);
		}

		public void ReportPresentationDoesNotExist(string path)
		{
			var span = new TextSpan(0, 0);
			var message = $"Presentation '{path}' does not exist.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type)
		{
			if (type == PrimitiveTypeSymbol.Error)
				return;
			var message = $"The number {text} isn't valid {type}.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportBadCharacter(int position, char character)
		{
			var span = new TextSpan(position, 1);
			var message = $"Bad character input: '{character}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportBadTopLevelStatement(SyntaxToken token)
		{
			//TODO: Maybe say, which Tokens (Keywords) are expected/allowed..
			var message = $"Bad top level token: '{token}'";
			Report(token.Span, message, DiagnosticLevel.Error);
		}

		public void ReportMissingCharacter(int position, char givenCharacter, char missingCharacter)
		{
			var span = new TextSpan(position, 1);
			var message = $"Character '{givenCharacter}' expects a second character '{missingCharacter}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
		{
			var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
		{
			if (operandType == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Unary operator '{operatorText}' is not defined for type '{operandType}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
		{
			if (leftType == PrimitiveTypeSymbol.Error || rightType == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUndefinedEnumValue(TextSpan span, EnumTypeSymbol enumType, string memberName)
		{
			var message = $"Enum '{enumType}' doesn't contain value named '{memberName}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUndefinedVariable(TextSpan span, string name)
		{
			var message = $"Variable '{name}' doesn't exist.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportUndefinedVariable(TextSpan span, string name, TypeSymbol parent)
		{
			if (parent == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Field '{parent}.{name}' doesn't exist.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportUndefinedStyle(TextSpan span, string name, LibrarySymbol library)
		{
			var message = $"Library '{library.Name}' doesn't contain a Style named '{name}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUndefinedFunction(TextSpan span, string name)
		{
			var message = $"Function '{name}' doesn't exist.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportUndefinedFunction(TextSpan span, string name, TypeSymbol parent)
		{
			if (parent == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Function '{parent}.{name}' doesn't exist.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportUndefinedType(TextSpan span, string name)
		{
			var message = $"Type '{name}' doesn't exist.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
		{
			if (fromType == PrimitiveTypeSymbol.Error || toType == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Cannot convert type '{fromType}' to '{toType}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol[] toTypes)
		{
			//TODO: What to do if toTypes contains an ErrorTypeSymbol?
			if (fromType == PrimitiveTypeSymbol.Error)
				return;
			if (toTypes.Contains(PrimitiveTypeSymbol.Error))
				throw new Exception();
			var message = $"Cannot convert type '{fromType}' to '{(string.Join("', '", toTypes.Select(t => t.ToString()).ToArray(), 0, toTypes.Length - 1))}' or '{toTypes[toTypes.Length - 1]}'";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportCannotFindLibrary(TextSpan span, string name)
		{
			var message = $"No library named '{name}' exists.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportVariableAlreadyDeclared(TextSpan span, string name)
		{
			var message = $"Variable '{name}' is already declared.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportCannotAssign(TextSpan span, string name)
		{
			var message = $"Variable '{name}' is read-only and cannot be assigned to.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportTypeAlreadyDeclared(TextSpan span, string name)
		{
			var message = $"Type '{name}' is already declared.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportOnlyCaseStatementsAllowed(TextSpan span, SyntaxKind kind)
		{
			//TODO: Better message
			var message = $"Expected a 'CaseStatement' but actual statement was '{kind}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportCannotMatchArguments(TextSpan span, int expectedCount, int actualCount)
		{
			if (expectedCount == actualCount)
				throw new Exception();
			var message = $"Excpected {expectedCount} Argument{(expectedCount == 1 ? "" : "s")}, but received {actualCount}.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportEmptyArray(TextSpan span)
		{
			var message = "Cannot create an empty Array.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportVariableVisibility(SyntaxToken preTildeToken, SyntaxToken identifier, SyntaxToken postTildeToken)
		{
			var message = $"Variable '{identifier.Text} cannot be defined as invisible (pre tilde) and as data (post tilde).";
			var span = TextSpan.FromBounds(preTildeToken.Span.Start, postTildeToken.Span.End);
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportCannotBeInvisible(TextSpan span, TypeSymbol type)
		{
			if (type == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Type '{type}' cannot be invisible, since it cannot be shown on a slide.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportVariableCannotBeFlaggedAsData(TextSpan span, TypeSymbol type)
		{
			if (type == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Type '{type}' is no data type. Therefore you cannot declare it as such.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportVariableMustBeFlaggedAsData(TextSpan span, TypeSymbol type)
		{
			if (type == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Type '{type}' is a data type. You must declare it as such and use post tilde.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportCannotChangeVisibility(TextSpan span, VariableSymbol expectedVariable, VariableSymbol actualVariable)
		{
			var message = $"Variable '{expectedVariable.Name}' cannot change visibility.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportInvalidStringFormat(int start)
		{
			var message = $"The dollar sign must be placed directly in front of quotes.";
			var span = new TextSpan(start, 2);
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportCannotFindFunction(TextSpan span, string name, int argumentLength, FunctionSymbol minParameterCount, FunctionSymbol maxParameterCount)
		{
			if (minParameterCount == null && maxParameterCount == null)
				throw new Exception();
			var message = $"No function named '{name}' with {argumentLength} parameters.";
			if (minParameterCount != null)
				message += $"Try '{minParameterCount}'";
			if (maxParameterCount != null && minParameterCount != null)
				message += $" or '{maxParameterCount}'";
			else
				message += $"Try '{maxParameterCount}'";
			message += ".";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportCannotFindFunction(TextSpan span, string name, FunctionSymbol bestMatch, DiagnosticBag parameterDiagnostics)
		{
			var message = $"No overload of function '{name}' could be converted. Best match was '{bestMatch}'.";
			Report(span, message, DiagnosticLevel.Error);
			foreach (var diagnostic in parameterDiagnostics)
			{
				_diagnostics.Add(diagnostic);
			}
		}

		public void ReportVariableMustBeAssigned(TextSpan span, string variable)
		{
			var message = $"Variable '{variable}' must be assigned in this block.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportFileDoesNotExist(TextSpan span, string path)
		{
			var message = $"File '{path}' doesn't exist. Couldn't load it.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportParameterNeedsTypeDeclaration(VariableExpressionSyntax variable)
		{
			var message = $"Parameter '{variable.Identifier.Text}' needs a TypeDeclaration (like this: {variable.Identifier.Text}: TypeName).";
			Report(variable.Span, message, DiagnosticLevel.Error);
		}

		internal void ReportBadLibrary(TextSpan span, string name)
		{
			var message = $"Could not load library '{name}'. Make sure it exists and compiles.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportNoInlineOperatorForTuples(SyntaxToken token)
		{
			var message = $"It is not possible to use '{token.Text}' with tuples.";
			Report(token.Span, message, DiagnosticLevel.Error);
		}

		internal void ReportUnusedVariable(VariableSymbol unusedVariable, TextSpan span)
		{
			var message = $"No references to '{unusedVariable}' found.";
			Report(span, message, DiagnosticLevel.Warning);
		}
	}
}