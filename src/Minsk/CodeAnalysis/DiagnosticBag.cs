using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using Slides;

namespace Minsk.CodeAnalysis
{
	internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
	{
		private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

		public string FileName { get; }

		public static void OutputToConsole(Diagnostic[] diagnostics, SourceText text, int max)
		{
			foreach (var diagnostic in diagnostics)
			{
				diagnostic.OutputToConsole(text);
				max--;
				if (max < 0)
					return;
			}
		}

		public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public DiagnosticBag(string fileName)
		{
			FileName = fileName;
		}



		public bool HasErrors() => _diagnostics.Any(d => d.Level == DiagnosticLevel.Error);

		public string Join(object[] obj, string prefix = null, string postfix = null)
		{
			if (prefix == null) prefix = string.Empty;
			if (postfix == null) postfix = string.Empty;
			if (obj.Length > 2)
				return $"{prefix}{string.Join($"{postfix}, {prefix}", obj.Take(obj.Length - 1))}{postfix} or {prefix}{obj.Last()}{postfix}";
			if (obj.Length == 2)
				return $"{prefix}{obj[0]}{postfix} or {prefix}{obj[1]}{postfix}";
			return $"{prefix}{obj[0]}{postfix}";
		}

		private string[] SimiliarValues(string[] values, string memberName)
		{
			string[] mostSimiliarNames = new string[Math.Min(values.Length, 5)];
			int[] sim = PopulateDistances(values, memberName);
			for (int j = 0; j < mostSimiliarNames.Length; j++)
			{
				int lastMin = int.MaxValue;
				int index = -1;
				for (int i = 0; i < sim.Length; i++)
				{
					if (sim[i] < lastMin)
					{
						lastMin = sim[i];
						index = i;
					}
				}
				mostSimiliarNames[j] = values[index];
				sim[index] = int.MaxValue;
			}

			return mostSimiliarNames;
		}

		private int[] PopulateDistances(string[] values, string found)
		{
			int[] sim = new int[values.Length];
			for (int i = 0; i < sim.Length; i++)
			{
				sim[i] = GetStringDistance(values[i], found);
			}
			return sim;
		}

		private int GetStringDistance(string a, string b)
		{
			if (string.IsNullOrEmpty(a))
				return int.MaxValue;
			if (string.IsNullOrEmpty(b))
				return int.MaxValue;
			int[] p = new int[a.Length + 1];
			int[] d = new int[a.Length + 1];
			int i, j;

			for (i = 0; i <= a.Length; i++)
				p[i] = i;
			for (j = 1; j <= b.Length; j++)
			{
				d[0] = j;
				for (i = 1; i <= a.Length; i++)
				{
					int cost = a[i - 1] == b[j - 1] ? 0 : 1;
					d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
				}
				int[] dPlaceholder = p;
				p = d;
				d = dPlaceholder;
			}
			return p[a.Length];
		}

		internal void AddRange(Diagnostic[] diagnostics)
		{
			_diagnostics.AddRange(diagnostics);
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

		public void ReportBadTopLevelStatement(SyntaxNode node)
		{
			//TODO(Improvement): Maybe say, which Tokens (Keywords) are expected/allowed..
			var message = $"Bad top level token: '{node}'";
			Report(node.Span, message, DiagnosticLevel.Error);
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
			var mostSimiliarNames = SimiliarValues(enumType.Values.Distinct().ToArray(), memberName);
			var message = $"Enum '{enumType}' doesn't contain value named '{memberName}'. Maybe try {Join(mostSimiliarNames, "'", "'")}.";
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
			if (parent is AdvancedTypeSymbol advanced)
			{
				var mostSimiliarOnes = SimiliarValues(advanced.Fields.Select(v => v.Name).Distinct().ToArray(), name);
				if(mostSimiliarOnes.Length > 0)
					message += $" Maybe try {Join(mostSimiliarOnes, "'", "'")}.";
			}
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportUndefinedStyle(TextSpan span, string name, LibrarySymbol library)
		{
			var message = $"Library '{library.Name}' doesn't contain a Style named '{name}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		//TODO! Complete suggestions for every undefined function!!

		public void ReportUndefinedFunction(TextSpan span, string name)
		{
			var message = $"Function '{name}' doesn't exist.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUndefinedFunction(TextSpan span, string name, TypeSymbol parent)
		{
			if (parent == PrimitiveTypeSymbol.Error)
				return;
			var message = $"Function '{parent}.{name}' doesn't exist.";
			if (parent is AdvancedTypeSymbol advanced)
			{
				var mostSimiliarOnes = SimiliarValues(advanced.Functions.Select(f => f.Name).Distinct().ToArray(), name);
				if (mostSimiliarOnes.Length > 0)
					message += $" Maybe try {Join(mostSimiliarOnes, "'", "'")}.";
			}
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUndefinedFunction(TextSpan span, string name, LibrarySymbol library)
		{
			var message = $"Function '{library.Name}.{name} doesn't exist in '{library.Name}'.";
			var mostSimiliarOnes = SimiliarValues(library.GlobalFunctions.Select(f => f.Name).Distinct().ToArray(), name);
			if (mostSimiliarOnes.Length > 0)
				message += $" Maybe try {Join(mostSimiliarOnes, "'", "'")}.";
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
			//TODO(Time): What to do if toTypes contains an ErrorTypeSymbol?
			//As of right now toTypes is constant. So i don't know where
			//the ErrorSymbol should come from..
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
			//TODO(Improvement): Better message
			var message = $"Expected a 'CaseStatement' but actual statement was '{kind}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		//public void ReportCannotMatchArguments(TextSpan span, int expectedCount, int actualCount)
		//{
		//	if (expectedCount == actualCount)
		//		throw new Exception();
		//	var message = $"Excpected {expectedCount} Argument{(expectedCount == 1 ? "" : "s")}, but received {actualCount}.";
		//	Report(span, message, DiagnosticLevel.Error);
		//}

		public void ReportEmptyArray(TextSpan span)
		{
			var message = "Cannot create an empty Array.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportStatementNotStackable(TextSpan span, SyntaxKind kind)
		{
			var message = $"'{kind}'-statements can not be used again in itself.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportUnterminatedString(TextSpan span, char endCharacter)
		{
			var message = $"Expected {endCharacter} at the end of the string!";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportInvalidStringFormat(int start)
		{
			var message = $"The dollar sign must be placed directly in front of quotes (').";
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
			else if (maxParameterCount != null)
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

		public void ReportUnrecognizedUseToken(TextSpan span, SyntaxKind token)
		{
			var message = $"Unrecognized token '{token}' in use statement.";
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

		public void ReportCouldNotCheckFile(BoundNodeKind kind, TextSpan span)
		{
			var message = $"Could not check file. {kind} will only be evaluate during runtime.";
			Report(span, message, DiagnosticLevel.Warning);
		}

		public void ReportUnexpectedMemberKind(TextSpan span, SyntaxKind memberKind, TypeSymbol enumType)
		{
			var message = $"Unexpected MemberKind '{memberKind}' on type '{enumType}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportExpectedDifferentUnknownsMathExpression(TextSpan span, BoundMathExpression mathExpression, int expected, int actual)
		{
			var message = $"Expected {expected} unknowns, but actually {actual} were found in '{mathExpression.Expression}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportEmptyStyle(TextSpan span, string name)
		{
			var message = $"Style '{name}' has no body.";
			Report(span, message, DiagnosticLevel.Warning);
		}

		internal void ReportCouldNotDetermineType(TextSpan span)
		{
			//TODO: Improve Errormessage 
			//(like, if you use a array you can use the empty array constructor)
			//or if you have 
			//		let a = none;
			//Something else!
			var message = $"Could not determine type of expression.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportOfflineNotSupported(TextSpan span, string identifier)
		{
			var message = $"'{identifier}' doesn't support offline viewing.";
			Report(span, message, DiagnosticLevel.Warning);
		}

		public void ReportIntegerDivisonByZero(TextSpan span)
		{
			var message = $"Cannot divide integers by zero.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportIndexOutOfRange(TextSpan span, int index, int arrayLength)
		{
			var message = $"Index '{index}' was out ouf range.";
			if(index >= arrayLength) message += $" It has to be lower than '{arrayLength}'.";
			if(index < 0) message += $" It has to be higher than or equal to '0'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportDependencyInArrayIndex(TextSpan span, JSInsertionKind kind)
		{
			var message = $"Only fixed values are as indecies allowed. No '{kind}' dependencies.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportExpressionNotAllowedInJSInsertionStatement(TextSpan span, SyntaxKind kind)
		{
			var message = $"Expression '{kind}' are not allowed inside of a JSInsertion statement.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportJSInsertionNotInJSInsertionStatement(TextSpan span, JSInsertionKind kind, int coveredJSInsertions)
		{
			//TDOD This is deprecated. or it should be.
			var message = $"This is deprecated. Or it should be. The current use statement does not cover '{kind}' JavaScript insertions.";
			if (coveredJSInsertions == 0) message = $"Please wrap this statement with a use statement for '{kind}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportInvalidUseDependency(TextSpan span)
		{
			var message = "This is not a valid use statement dependency.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportCannotAssignLibraries(TextSpan span)
		{
			var message = "Libraries can only be assigned to a variable in the import statement. Otherwise you cannot assign libraries.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportNoDefaultValueForType(TextSpan span, TypeSymbol type)
		{
			var message = $"There is no default value for type '{type}'. Maybe try '{type}?' instead with the default value 'none'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		public void ReportPercentUnitValueNotAllowed(TextSpan span)
		{
			var message = $"You can only use absolute values like 'px' or 'pt' here!";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportCannotWriteToTypeType(TextSpan span, TypeSymbol type)
		{
			var message = $"Cannot write to function return-type '{type}'.";
			Report(span, message, DiagnosticLevel.Error);
		}

		internal void ReportVariableInvalidType(TextSpan span, SyntaxKind kind, TypeSymbol type)
		{
			var message = $"Cannot create a '{kind}' variable here with type '{type}'.";
			Report(span, message, DiagnosticLevel.Error);
		}
	}
}