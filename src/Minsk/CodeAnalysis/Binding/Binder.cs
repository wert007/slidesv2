using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using Slides;
using Slides.Debug;
using Slides.Filters;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class Binder
	{
		private readonly DiagnosticBag _diagnostics;

		private LibrarySymbol[] _references;
		private BoundScope _scope;
		private Dictionary<string, VariableSymbol> _builtInConstants;
		private TypeSymbolTypeConverter _builtInTypes;

		private Dictionary<VariableSymbol, BoundStatement> _declarations = new Dictionary<VariableSymbol, BoundStatement>();
		private HashSet<VariableSymbol> _assignedVariables = new HashSet<VariableSymbol>();
		private PresentationFlags _flags = new PresentationFlags();
		public DiagnosticBag Diagnostics => _diagnostics;

		public Binder(BoundScope parent, LibrarySymbol[] references, string fileName)
		{
			_scope = new BoundScope(parent);
			_references = references.Concat(new LibrarySymbol[]
				{
					LibrarySymbol.Seperator,
					LibrarySymbol.Code,
				}).ToArray();
			_diagnostics = new DiagnosticBag(fileName);

			_builtInTypes = TypeSymbolTypeConverter.Instance;
			_builtInConstants = new Dictionary<string, VariableSymbol>();

			_builtInConstants.Add("seperator", new VariableSymbol("seperator", true, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol)), false));
			_builtInConstants.Add("code", new VariableSymbol("code", true, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol)), false));

			_builtInConstants.Add("auto", new VariableSymbol("auto", true, _builtInTypes.LookSymbolUp(typeof(Unit)), false));
			foreach (var color in Color.GetStaticColors())
			{
				_builtInConstants.Add(color.Key, new VariableSymbol(color.Key, true, _builtInTypes.LookSymbolUp(typeof(Color)), false));
			}
		}

		public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, SyntaxTree syntax, LibrarySymbol[] references)
		{
			var parentScope = CreateParentScope(previous);
			var binder = new Binder(parentScope, references, syntax.Text.FileName);
			binder.CollectDeclarations(syntax.Root.Statement);
			var expression = binder.BindStatement(syntax.Root.Statement);
			var variables = binder._scope.GetDeclaredVariables();
			var diagnostics = binder.Diagnostics.ToList();
			var declarations = binder._declarations;

			if (previous != null)
				diagnostics.InsertRange(0, previous.Diagnostics);

			return new BoundGlobalScope(previous, diagnostics.ToArray(), variables, expression, declarations);
		}

		private static BoundScope CreateParentScope(BoundGlobalScope previous)
		{
			var stack = new Stack<BoundGlobalScope>();
			while (previous != null)
			{
				stack.Push(previous);
				previous = previous.Previous;
			}

			BoundScope parent = null;

			while (stack.Count > 0)
			{
				previous = stack.Pop();
				var scope = new BoundScope(parent);
				foreach (var v in previous.Variables)
					scope.TryDeclare(v, null);

				parent = scope;
			}



			return parent;
		}

		private void CollectDeclarations(StatementSyntax syntax)
		{
			switch (syntax.Kind)
			{
				case SyntaxKind.FileBlockStatement:
					CollectDeclarations((FileBlockStatementSyntax)syntax);
					break;
				case SyntaxKind.GroupStatement:
					CollectDeclarations((GroupStatementSyntax)syntax);
					break;
				case SyntaxKind.DataStatement:
					CollectDeclarations((DataStatementSyntax)syntax);
					break;
				case SyntaxKind.StyleStatement:
					CollectDeclarations((StyleStatementSyntax)syntax);
					break;
				case SyntaxKind.TransitionStatement:
					CollectDeclarations((TransitionStatementSyntax)syntax);
					break;
				case SyntaxKind.FilterStatement:
					CollectDeclarations((FilterStatementSyntax)syntax);
					break;
				case SyntaxKind.AnimationStatement:
					CollectDeclarations((AnimationStatementSyntax)syntax);
					break;
				case SyntaxKind.TemplateStatement:
					CollectDeclarations((TemplateStatementSyntax)syntax);
					break;
				case SyntaxKind.SlideStatement:
					CollectDeclarations((SlideStatementSyntax)syntax);
					break;
				case SyntaxKind.LibraryStatement:
				//TODO: Set the presentation-flags during binding time!
				case SyntaxKind.ImportStatement:
					//ImportStatements should always be the first thing you do in 
					//your code. So we don't collect them from anywhere else.
					break;
				default:
					throw new Exception();
			}
		}

		private void CollectDeclarations(FileBlockStatementSyntax syntax)
		{
			foreach (var statement in syntax.Statements)
			{
				CollectDeclarations(statement);
			}
		}

		private void CollectDeclarations(GroupStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope = new BoundScope(_scope);
			var boundParameters = BindParameterBlockStatement(syntax.ParameterStatement);
			_scope = _scope.Parent;
			var fields = new VariableSymbolCollection(boundParameters.Statements.Select(p => p.Variable));
			var constructor = new FunctionSymbolCollection();
			constructor.Add(new FunctionSymbol("constructor", fields, null));
			constructor.Seal();
			var type = new AdvancedTypeSymbol(name, fields, constructor, FunctionSymbolCollection.Empty, _builtInTypes.LookSymbolUp(typeof(Element)));
			constructor[0].Type = type;

			if (!_scope.TryDeclare(type))
				_diagnostics.ReportTypeAlreadyDeclared(syntax.Identifier.Span, name);
			_declarations.Add(new VariableSymbol(name, true, type, false), null);
		}

		private void CollectDeclarations(DataStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var fields = BindDataBlockStatement(syntax.Body);
			var constructor = new FunctionSymbolCollection();
			constructor.Add(new FunctionSymbol("constructor", VariableSymbolCollection.Empty, null));
			constructor.Add(new FunctionSymbol("constructor", fields, null));
			constructor.Seal();

			var customType = new AdvancedTypeSymbol(name, fields, constructor, FunctionSymbolCollection.Empty);
			customType.SetData(true);
			constructor[0].Type = customType;
			constructor[1].Type = customType;

			if (!_scope.TryDeclare(customType))
				_diagnostics.ReportTypeAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(new VariableSymbol(name, true, customType, false), null);
		}

		private void CollectDeclarations(StyleStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			VariableSymbol variable = null;
			if (syntax.Identifier.Kind != SyntaxKind.StdKeyword)
			{
				variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Style)), true);
				TextSpan? span = null;
				if (!_flags.IsLibrarySymbol)
					span = syntax.Identifier.Span;
				if (!_flags.StyleAllowed)
					//todo;
					_flags.StyleAllowed = true;

				if (!_scope.TryDeclare(variable, span))
					_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

				_declarations.Add(variable, null);
			}
		}

		private void CollectDeclarations(TransitionStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Transition)), false);

			if (!_scope.TryDeclare(variable, syntax.Identifier.Span))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private void CollectDeclarations(FilterStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Filter)), true);

			if (!_scope.TryDeclare(variable, syntax.Identifier.Span))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private void CollectDeclarations(AnimationStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(AnimationSymbol)), false);

			if (!_scope.TryDeclare(variable, syntax.Identifier.Span))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private void CollectDeclarations(TemplateStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Template)), false);

			if (!_scope.TryDeclare(variable, null))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private void CollectDeclarations(SlideStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(SlideAttributes)), false);

			if (!_scope.TryDeclare(variable, null))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private BoundStatement BindStatement(StatementSyntax syntax)
		{
			switch (syntax.Kind)
			{
				case SyntaxKind.FileBlockStatement:
					return BindFileBlockStatement((FileBlockStatementSyntax)syntax);
				case SyntaxKind.BlockStatement:
					return BindBlockStatement((BlockStatementSyntax)syntax);
				case SyntaxKind.TemplateStatement:
					return BindTemplateStatement((TemplateStatementSyntax)syntax);
				case SyntaxKind.SlideStatement:
					return BindSlideStatement((SlideStatementSyntax)syntax);
				case SyntaxKind.GroupStatement:
					return BindGroupStatement((GroupStatementSyntax)syntax);
				case SyntaxKind.StyleStatement:
					return BindStyleStatement((StyleStatementSyntax)syntax);
				case SyntaxKind.AnimationStatement:
					return BindAnimationStatement((AnimationStatementSyntax)syntax);
				case SyntaxKind.CaseBlockStatement:
					return BindCaseBlockStatement((CaseBlockStatementSyntax)syntax);
				case SyntaxKind.TransitionStatement:
					return BindTransitionStatement((TransitionStatementSyntax)syntax);
				case SyntaxKind.FilterStatement:
					return BindFilterStatement((FilterStatementSyntax)syntax);
				case SyntaxKind.LibraryStatement:
					return BindLibraryStatement((LibraryStatementSyntax)syntax);
				case SyntaxKind.DataStatement:
					return BindDataStatement((DataStatementSyntax)syntax);
				case SyntaxKind.ImportStatement:
					return BindImportStatement((ImportStatementSyntax)syntax);
				case SyntaxKind.IfStatement:
					return BindIfStatement((IfStatementSyntax)syntax);
				case SyntaxKind.ForStatement:
					return BindForStatement((ForStatementSyntax)syntax);

				case SyntaxKind.VariableDeclaration:
					return BindVariableDeclaration((VariableDeclarationSyntax)syntax);
				case SyntaxKind.ExpressionStatement:
					return BindExpressionStatement((ExpressionStatementSyntax)syntax);
				default:
					throw new Exception($"Unexpected syntax {syntax.Kind}");
			}
		}

		private BoundStatement BindForStatement(ForStatementSyntax syntax)
		{
			_scope = new BoundScope(_scope);
			var name = syntax.Variable.Identifier.Text;
			var boundCollection = BindExpression(syntax.Collection);
			var isRange = boundCollection.Type == _builtInTypes.LookSymbolUp(typeof(Range));
			if (boundCollection.Type.Type != TypeType.Array && !isRange)
			{
				_diagnostics.ReportCannotConvert(syntax.Collection.Span, boundCollection.Type, new ArrayTypeSymbol(boundCollection.Type));
				return new BoundExpressionStatement(new BoundErrorExpression());
			}
			VariableSymbol variable = null;
			if (isRange)
			{
				variable = CheckGlobalVariableExpression(syntax.Variable, PrimitiveTypeSymbol.Integer, true);
			}
			else
			{
				var type = boundCollection.Type as ArrayTypeSymbol;
				variable = CheckGlobalVariableExpression(syntax.Variable, type.Child, true);
			}
			var boundBody = BindBlockStatement(syntax.Body);
			CheckUnusedSymbols(_scope);
			_scope = _scope.Parent;
			return new BoundForStatement(variable, boundCollection, boundBody);
		}

		private BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax, bool useSeperateScope = true)
		{
			var statements = new List<BoundStatement>();
			if (useSeperateScope)
				_scope = new BoundScope(_scope);

			foreach (var statementSyntax in syntax.Statements)
			{
				var statement = BindStatement(statementSyntax);
				statements.Add(statement);
			}

			if (useSeperateScope)
			{
				//CheckUnusedSymbols(_scope.Parent);
				_scope = _scope.Parent;
			}
			return new BoundBlockStatement(statements.ToArray());
		}

		private VariableSymbolCollection BindDataBlockStatement(DataBlockStatementSyntax syntax)
		{
			var statements = new VariableSymbolCollection();
			_scope = new BoundScope(_scope);

			foreach (var statementSyntax in syntax.Statements)
			{
				var statement = BindParameterStatement(statementSyntax.Parameter);
				statements.Add(statement.Variable);
			}

			_scope = _scope.Parent;

			return statements;
		}

		private BoundStatement BindImportStatement(ImportStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var initializer = BindExpression(syntax.Initializer);
			//Make sure referenced libraries actually work!
			//EDIT: I don't know if we do. But i think we do..
			var libraryType = _builtInTypes.LookSymbolUp(typeof(LibrarySymbol));
			if (initializer.Type != _builtInTypes.LookSymbolUp(typeof(Font)) && initializer.Type != libraryType)
				_diagnostics.ReportCannotConvert(syntax.Initializer.Span, initializer.Type, new TypeSymbol[] { _builtInTypes.LookSymbolUp(typeof(Font)), libraryType });

			var variable = new VariableSymbol(name, false, initializer.Type, initializer.Type.IsData);

			TextSpan? span = syntax.Identifier.Span;

			if (!_scope.TryDeclare(variable, span))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			return new BoundVariableDeclaration(variable, initializer);
		}

		private BoundStatement BindIfStatement(IfStatementSyntax syntax)
		{
			var boundCondition = BindExpression(syntax.Condition);
			VariableSymbol variableSymbol = null;
			if (boundCondition.Type is NullableTypeSymbol n)
			{
				variableSymbol = ExtractVariableSymbol(boundCondition);
				variableSymbol.Type = n.BaseType;
			}
			else if (boundCondition.Type != PrimitiveTypeSymbol.Bool)
			{
				var toTypes = new TypeSymbol[]
				{
					PrimitiveTypeSymbol.Bool,
					new NullableTypeSymbol(boundCondition.Type),
				};
				_diagnostics.ReportCannotConvert(syntax.Condition.Span, boundCondition.Type, toTypes);
				boundCondition = new BoundErrorExpression();
			}
			var boundBody = BindStatement(syntax.Body);
			if (variableSymbol != null)
			{
				variableSymbol.Type = new NullableTypeSymbol(variableSymbol.Type);
			}
			var boundElse = BindElseClause(syntax.ElseClause);
			return new BoundIfStatement(boundCondition, boundBody, boundElse);
		}

		private BoundStatement BindElseClause(ElseClauseSyntax syntax)
		{
			if (syntax == null)
				return null;
			var boundBody = BindStatement(syntax.Body);
			return boundBody;
		}

		private static VariableSymbol ExtractVariableSymbol(BoundExpression boundCondition)
		{
			VariableSymbol variableSymbol;
			switch (boundCondition.Kind)
			{
				case BoundNodeKind.AssignmentExpression:
					var ae = (BoundAssignmentExpression)boundCondition;
					//Maybe hacky?
					//Right now we only call this function, when we only CAN have one 
					//variable (the if-extraction, and the library-accessor).
					variableSymbol = ae.Variables[0];

					break;
				case BoundNodeKind.VariableExpression:
					var ve = (BoundVariableExpression)boundCondition;
					variableSymbol = ve.Variable;
					break;
				case BoundNodeKind.FieldAccessExpression:
					var facc = (BoundFieldAccesExpression)boundCondition;
					variableSymbol = facc.Field.Variable;
					break;
				case BoundNodeKind.FieldAssignmentExpression:
					var fass = (BoundFieldAssignmentExpression)boundCondition;
					variableSymbol = fass.Field.Field.Variable;
					break;
				default:
					Logger.LogUnmatchedIfConditionVariableExtractment(boundCondition);
					//TODO(Time): Add diagnostics for this case. Maybe. Let time decide.
					//If we will never run into this, then we should add a diagnostics
					//here.
					return null;
			}

			return variableSymbol;
		}

		private BoundStatement BindDataStatement(DataStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope.TryLookup(name, out TypeSymbol customType);
			var result = new BoundDataStatement(customType);
			_declarations[new VariableSymbol(name, true, customType, false)] = result;
			return result;
		}

		private BoundStatement BindLibraryStatement(LibraryStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol)), false);
			_flags.IsLibrarySymbol = true;

			if (!_scope.TryDeclare(variable, null))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			var boundBody = BindBlockStatement(syntax.Body);

			return new BoundLibraryStatement(variable, boundBody);
		}

		private BoundParameterBlockStatement BindAnimationParameterStatement(AnimationParameterStatementSyntax syntax)
		{
			var boundElementParameter = BindParameterStatement(syntax.ElementParameter, null, false);
			var boundTimeParameter = BindParameterStatement(syntax.TimeParameter, _builtInTypes.LookSymbolUp(typeof(Time)), false);

			var parameters = new BoundParameterStatement[] { boundElementParameter, boundTimeParameter };
			var result = new BoundParameterBlockStatement(parameters);
			return result;
		}

		private BoundStatement BindAnimationStatement(AnimationStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope.TryLookup(name, out VariableSymbol variable);

			_scope = new BoundScope(_scope);
			var boundParameters = BindAnimationParameterStatement(syntax.Parameters);

			_scope.TryDeclare(new VariableSymbol("interpolation", false, _builtInTypes.LookSymbolUp(typeof(Interpolation)), false), null);
			_scope.TryDeclare(AnimationSymbol.DoneSymbol, null);
			_scope.TryDeclare(AnimationSymbol.InitSymbol, null);
			_scope.TryDeclare(new VariableSymbol("progress", true, PrimitiveTypeSymbol.Float, true)
			{ NeedsDataFlag = false }, null);

			var boundBody = BindAnimationBodyStatement(syntax.Body);
			CheckUnusedSymbols(_scope);
			_scope = _scope.Parent;

			var result = new BoundAnimationStatement(variable, boundParameters.Statements[0], boundParameters.Statements[1], boundBody);
			_declarations[variable] = result;
			return result;
		}

		private BoundCaseStatement[] BindAnimationBodyStatement(BlockStatementSyntax body)
		{
			var result = new List<BoundCaseStatement>();
			foreach (var statement in body.Statements)
			{
				if (statement.Kind != SyntaxKind.CaseBlockStatement)
				{
					_diagnostics.ReportOnlyCaseStatementsAllowed(statement.Span, statement.Kind);
					continue;
				}
				var boundStatement = BindCaseBlockStatement((CaseBlockStatementSyntax)statement);
				result.Add(boundStatement);
			}
			return result.ToArray();
		}

		private BoundCaseStatement BindCaseBlockStatement(CaseBlockStatementSyntax syntax)
		{
			var boundCondition = BindExpression(syntax.Condition, PrimitiveTypeSymbol.Float);
			var boundBody = BindStatement(syntax.Body);

			return new BoundCaseStatement(boundCondition, boundBody);
		}

		private BoundStatement BindTransitionStatement(TransitionStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope.TryLookup(name, out VariableSymbol variable);

			_scope = new BoundScope(_scope);
			var boundParameters = BindTransitionParameters(syntax.Parameters);
			foreach (var field in ((AdvancedTypeSymbol)_builtInTypes.LookSymbolUp(typeof(Transition))).Fields)
			{
				_scope.TryDeclare(field, null);
			}
			var boundBody = BindBlockStatement(syntax.Body);
			_scope = _scope.Parent;

			var result = new BoundTransitionStatement(variable, boundParameters, boundBody);
			_declarations[variable] = result;
			return result;
		}

		private BoundStatement BindFilterStatement(FilterStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope.TryLookup(name, out VariableSymbol variable);

			_scope = new BoundScope(_scope);
			var boundParameters = BindParameterBlockStatement(syntax.Parameter);
			var boundBody = BindBlockStatement(syntax.Body);
			CheckUnusedSymbols(_scope);
			_scope = _scope.Parent;

			var result = new BoundFilterStatement(variable, boundParameters, boundBody);
			_declarations[variable] = result;
			return result;
		}

		private BoundParameterBlockStatement BindTransitionParameters(TransitionParameterSyntax syntax)
		{
			var boundFromParameter = BindParameterStatement(syntax.FromParameter, _builtInTypes.LookSymbolUp(typeof(SlideAttributes)));
			var boundToParameter = BindParameterStatement(syntax.ToParameter, _builtInTypes.LookSymbolUp(typeof(SlideAttributes)));

			var parameters = new BoundParameterStatement[] { boundFromParameter, boundToParameter };
			var result = new BoundParameterBlockStatement(parameters);
			return result;
		}

		private BoundStatement BindStyleStatement(StyleStatementSyntax syntax)
		{

			_scope = new BoundScope(_scope);

			BoundParameterStatement boundParameter = null;
			if (syntax.Parameter != null)
				boundParameter = BindParameterStatement(syntax.Parameter.ParameterStatement);
			else
			{
				foreach (var field in (_builtInTypes.LookSymbolUp(typeof(SlideAttributes)) as AdvancedTypeSymbol).Fields)
				{
					_scope.TryDeclare(field, null);
				}
				_scope.TryDeclare(new VariableSymbol("Slide", false, _builtInTypes.LookSymbolUp(typeof(SlideAttributes)), false));
			}
			var boundBody = BindBlockStatement(syntax.Body);
			CheckUnusedSymbols(_scope);
			_scope = _scope.Parent;
			var name = syntax.Identifier.Text;
			VariableSymbol variable = null;
			if (syntax.Identifier.Kind != SyntaxKind.StdKeyword)
				_scope.TryLookup(name, out variable);
			var result = new BoundStyleStatement(variable, boundParameter, boundBody);
			if (variable != null)
				_declarations[variable] = result;
			return result;
		}

		private TypeSymbol BindTypeDeclaration(TypeDeclarationSyntax syntax, TypeSymbol targetType = null)
		{
			var name = syntax.Type.Text;
			var type = TypeSymbol.FromString(name);
			if (type == null)
			{
				if (!_scope.TryLookup(name, out type))
				{
					_diagnostics.ReportUndefinedType(syntax.Type.Span, name);
					type = PrimitiveTypeSymbol.Error;
				}
			}
			if (syntax.QuestionMarkToken != null)
			{
				type = new NullableTypeSymbol(type);
			}
			foreach (var bracket in syntax.BracketPairs)
			{
				type = new ArrayTypeSymbol(type);
			}

			if (targetType != null && !type.CanBeConvertedTo(targetType))
				_diagnostics.ReportCannotConvert(syntax.Type.Span, type, targetType);
			return type;
		}

		private BoundParameterStatement BindParameterStatement(ParameterStatementSyntax syntax, TypeSymbol targetType = null, bool countReferences = true)
		{
			TypeSymbol type = null;
			if (syntax.TypeDeclaration != null)
				type = BindTypeDeclaration(syntax.TypeDeclaration, targetType);
			BoundExpression expression = null;
			if (syntax.Initializer != null)
				expression = BindExpression(syntax.Initializer);

			if (type == null)
				type = expression?.Type ?? PrimitiveTypeSymbol.Error;

			if (expression != null)
			{
				if (!expression.Type.CanBeConvertedTo(type))
					_diagnostics.ReportCannotConvert(syntax.Span, expression.Type, type);
			}

			var variable = CheckGlobalVariableExpression(syntax.Variable, type, true, countReferences);

			return new BoundParameterStatement(variable, expression);
		}

		private BoundParameterBlockStatement BindParameterBlockStatement(ParameterBlockStatementSyntax syntax)
		{
			var statements = new List<BoundParameterStatement>();

			foreach (var statementSyntax in syntax.Statements)
			{
				var statement = BindParameterStatement(statementSyntax);
				statements.Add(statement);
			}

			return new BoundParameterBlockStatement(statements.ToArray());
		}

		private BoundStatement BindGroupStatement(GroupStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;

			_scope = new BoundScope(_scope);
			var boundParameters = BindParameterBlockStatement(syntax.ParameterStatement);
			_scope = new BoundScope(_scope);
			foreach (var field in ((AdvancedTypeSymbol)_builtInTypes.LookSymbolUp(typeof(Element))).Fields)
			{
				//Throw no warnings
				//Erm.. i think we don't?

				_scope.Declare(field, true);
			}
			_scope.TryDeclare(new VariableSymbol("fontsize", false, _builtInTypes.LookSymbolUp(typeof(Unit)), false), null);
			foreach (var function in ((AdvancedTypeSymbol)_builtInTypes.LookSymbolUp(typeof(Element))).Functions)
			{
				_scope.TryDeclare(function);
			}
			_assignedVariables.Clear();
			var boundBody = BindBlockStatement(syntax.Body);
			if (!_assignedVariables.Any(a => a.Name == "initWidth"))
			{
				var variable = _assignedVariables.FirstOrDefault(a => a.Name == "initWidth");
				_diagnostics.ReportVariableMustBeAssigned(syntax.Identifier.Span, "initWidth");
			}
			if (!_assignedVariables.Any(a => a.Name == "initHeight"))
			{
				var variable = _assignedVariables.FirstOrDefault(a => a.Name == "initHeight");
				_diagnostics.ReportVariableMustBeAssigned(syntax.Identifier.Span, "initHeight");
			}

			CheckUnusedSymbols(_scope);
			CheckUnusedSymbols(_scope.Parent);
			_scope = _scope.Parent.Parent;

			//Already declared while collecting declarations.
			_scope.TryLookup(name, out TypeSymbol type);

			var result = new BoundGroupStatement(type, boundParameters, boundBody);
			_declarations[new VariableSymbol(name, true, type, false)] = result;
			return result;
		}

		private void CheckUnusedSymbols(BoundScope scope)
		{
			var unusedVariables = scope.GetUnusedVariables();
			foreach (var unusedVariable in unusedVariables)
			{
				var span = scope.GetDeclarationSpan(unusedVariable);
				if (span == null)
					throw new Exception();
				if (unusedVariable.IsVisible && !unusedVariable.NeedsDataFlag)
					continue;
				_diagnostics.ReportUnusedVariable(unusedVariable, span.Value);
			}

			foreach (var singleUseVariable in scope.GetVariablesReferenced(1))
			{
				//There is a hell of a lot of them. so we dont do anything for now..
				//Logger.LogSingleUseVariable(singleUseVariable);
			}
		}

		private BoundTemplateStatement BindTemplateStatement(TemplateStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope.TryLookup(name, out VariableSymbol variable);

			var parameter = BindParameterStatement(syntax.ParameterStatement.ParameterStatement, _builtInTypes.LookSymbolUp(typeof(SlideAttributes)), false);
			_scope.TryDeclare(new VariableSymbol("slideCount", true, PrimitiveTypeSymbol.Integer, false));
			var body = BindBlockStatement(syntax.Body);
			var result = new BoundTemplateStatement(variable, parameter, body);
			_declarations[variable] = result;
			return result;
		}

		private BoundStatement BindSlideStatement(SlideStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope.TryLookup(name, out VariableSymbol variable);

			VariableSymbol template = null;
			if (syntax.Template != null)
			{
				var templateName = syntax.Template.Identifier.Text;
				if (!_scope.TryLookup(templateName, out template))
					_diagnostics.ReportUndefinedVariable(syntax.Template.Identifier.Span, templateName);
			}

			_scope = new BoundScope(_scope);
			foreach (var field in ((AdvancedTypeSymbol)_builtInTypes.LookSymbolUp(typeof(SlideAttributes))).Fields)
			{
				_scope.TryDeclare(field, null);
			}
			var boundStatements = new List<BoundStepStatement>();
			foreach (var statement in syntax.Statements)
			{
				boundStatements.Add(BindStepStatement(statement));
			}
			CheckUnusedSymbols(_scope);
			_scope = _scope.Parent;
			var result = new BoundSlideStatement(variable, template, boundStatements.ToArray());
			_declarations[variable] = result;
			return result;
		}

		private BoundStepStatement BindStepStatement(StepStatementSyntax statement)
		{
			var boundBody = BindBlockStatement(statement.Body, useSeperateScope: false);

			var name = statement.OptionalIdentifier?.Text;
			VariableSymbol variable = null;
			if (name != null)
			{
				variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Step)), false);
				if (!_scope.TryDeclare(variable))
				{
					_diagnostics.ReportVariableAlreadyDeclared(TextSpan.FromBounds(statement.StepKeyword.Span.Start, statement.ColonToken.Span.End), name);
				}
			}
			return new BoundStepStatement(variable, boundBody);
		}

		private BoundStatement BindFileBlockStatement(FileBlockStatementSyntax syntax)
		{
			var statements = new List<BoundStatement>();
			_scope = new BoundScope(_scope);

			foreach (var statementSyntax in syntax.Statements)
			{
				var statement = BindStatement(statementSyntax);
				statements.Add(statement);
			}

			_scope = _scope.Parent;

			return new BoundBlockStatement(statements.ToArray());
		}

		private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
		{
			var names = syntax.Variables.Select(v => v.Identifier.Text).ToArray();
			var initializer = BindExpression(syntax.Initializer);
			var variables = new List<VariableSymbol>();

			var type = initializer.Type;
			if (syntax.Variables.Length > 1)
			{
				var children = new TypeSymbol[syntax.Variables.Length];
				for (int i = 0; i < children.Length; i++)
					children[i] = TypeSymbol.Undefined;
				var targetType = new TupleTypeSymbol(children);

				if (type.Type != TypeType.Tuple)
				{
					_diagnostics.ReportCannotConvert(syntax.Initializer.Span, type, targetType);
					return new BoundExpressionStatement(new BoundErrorExpression());
				}

				var t = (TupleTypeSymbol)type;
				if (t.Length != targetType.Length)
				{
					_diagnostics.ReportCannotConvert(syntax.Initializer.Span, t, targetType);
					return new BoundExpressionStatement(new BoundErrorExpression());
				}
			}

			for (int i = 0; i < syntax.Variables.Length; i++)
			{
				var variable = syntax.Variables[i];
				if (syntax.Variables.Length > 1)
					type = ((TupleTypeSymbol)initializer.Type).Children[i];
				variables.Add(CheckGlobalVariableExpression(variable, type, true));
			}
			return new BoundVariableDeclaration(variables.ToArray(), initializer);
		}

		private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
		{
			var expression = BindExpression(syntax.Expression);
			return new BoundExpressionStatement(expression);
		}

		private BoundExpression BindExpression(ExpressionSyntax syntax, TypeSymbol targetType)
		{
			BoundExpression result = null;
			if (targetType != null)
			{
				if (syntax.Kind == SyntaxKind.LiteralExpression)
					result = BindLiteralExpression((LiteralExpressionSyntax)syntax, targetType);
			}
			if (result == null)
				result = BindExpression(syntax);
			if (targetType != null && !result.Type.CanBeConvertedTo(targetType))
				_diagnostics.ReportCannotConvert(syntax.Span, result.Type, targetType);

			return result;
		}

		private BoundExpression BindExpression(ExpressionSyntax syntax)
		{
			switch (syntax.Kind)
			{
				case SyntaxKind.ParenthesizedExpression:
					return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
				case SyntaxKind.StringExpression:
					return BindStringExpression((StringExpressionSyntax)syntax);
				case SyntaxKind.LiteralExpression:
					return BindLiteralExpression((LiteralExpressionSyntax)syntax);
				case SyntaxKind.VariableExpression:
					return BindGlobalVariableExpression((VariableExpressionSyntax)syntax);
				case SyntaxKind.AssignmentExpression:
					return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
				case SyntaxKind.FieldAssignmentExpression:
					return BindFieldAssignmentExpression((FieldAssignmentExpressionSyntax)syntax);
				case SyntaxKind.UnaryExpression:
					return BindUnaryExpression((UnaryExpressionSyntax)syntax);
				case SyntaxKind.BinaryExpression:
					return BindBinaryExpression((BinaryExpressionSyntax)syntax);
				case SyntaxKind.FunctionExpression:
					return BindGlobalFunctionExpression((FunctionExpressionSyntax)syntax);
				case SyntaxKind.ArrayConstructorExpression:
					return BindArrayConstructorExpression((ArrayConstructorExpressionSyntax)syntax);
				case SyntaxKind.ConstructorExpression:
					return BindConstructorExpression((ConstructorExpressionSyntax)syntax);
				case SyntaxKind.MemberAccessExpression:
					return BindMemberAccessExpression((MemberAccessExpressionSyntax)syntax);
				case SyntaxKind.FieldAccessExpression:
					return BindFieldAccessExpression((FieldAccessExpressionSyntax)syntax);
				case SyntaxKind.LambdaExpression:
					return BindLambdaExpression((LambdaExpressionSyntax)syntax);
				case SyntaxKind.NameExpression:
					return new BoundErrorExpression(); //Let's hope somebody informed the diagnostics!
				default:
					throw new Exception($"Unexpected syntax {syntax.Kind}");
			}
		}

		private BoundExpression BindConstructorExpression(ConstructorExpressionSyntax syntax)
		{
			FunctionExpressionSyntax functionCall = null;
			BoundVariableExpression boundLibrary = null;
			LibrarySymbol source = null;
			if (syntax.FunctionCall is FunctionExpressionSyntax f)
				functionCall = f;
			else if (syntax.FunctionCall is MemberAccessExpressionSyntax m)
			{
				boundLibrary = (BoundVariableExpression)BindExpression(m.Expression, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol)));

				if (m.Member is FunctionExpressionSyntax mf)
					functionCall = mf;
				else throw new Exception();
				//TODO(Time): Add Diagnostics
				//Hasn't caused an issue as of today.
				//But let's wait and see what time is having
			}
			else
				throw new Exception();
			//TODO(Time): Maybe add diagnostics.
			//Same as above.
			var name = functionCall.Identifier.Text;

			AdvancedTypeSymbol type = null;
			if (boundLibrary == null)
			{
				if (!_scope.TryLookup(name, out TypeSymbol _type))
				{
					_diagnostics.ReportUndefinedType(syntax.Span, name);
					return new BoundErrorExpression();
				}
				if (_type.Type != TypeType.Advanced)
					throw new Exception();
				type = (AdvancedTypeSymbol)_type;
			}
			else
			{
				foreach (var library in _references)
				{
					if (library.Name == boundLibrary.Variable.Name)
					{
						var group = library.CustomTypes.FirstOrDefault(g => g.Symbol.Name == name);
						if (group == null)
						{
							_diagnostics.ReportUndefinedFunction(syntax.Span, name);
							return new BoundErrorExpression();
						}
						type = (AdvancedTypeSymbol)group.Symbol.Type;
						source = library;
						break;
					}
				}
				if (boundLibrary != null && type == null)
				{
					_diagnostics.ReportBadLibrary((syntax.FunctionCall as MemberAccessExpressionSyntax).Expression.Span, boundLibrary.Variable.Name);
					return new BoundErrorExpression();
				}
				if (type == null)
				{
					_diagnostics.ReportUndefinedType(syntax.Span, name);
					return new BoundErrorExpression();
				}

			}
			return BindFunctionExpression(functionCall, type.Constructor.ToArray(), source);
		}

		private BoundExpression BindArrayConstructorExpression(ArrayConstructorExpressionSyntax syntax)
		{
			if (syntax.Contents.Length <= 0)
			{
				_diagnostics.ReportEmptyArray(syntax.Span);
				return new BoundErrorExpression();
			}
			TypeSymbol targetType = null;
			var boundExpressions = new List<BoundExpression>();
			foreach (var expressionSyntax in syntax.Contents)
			{
				var boundExpression = BindExpression(expressionSyntax, targetType);
				boundExpressions.Add(boundExpression);

				if (targetType == null)
					targetType = boundExpression.Type;
			}
			return new BoundArrayExpression(boundExpressions.ToArray());
		}

		private BoundArrayIndex BindArrayIndexExpression(ArrayIndexExpressionSyntax syntax, TypeSymbol type)
		{
			if (type.Type != TypeType.Array)
			{
				_diagnostics.ReportCannotConvert(syntax.Span, type, new ArrayTypeSymbol(type));

			}
			var boundIndex = BindExpression(syntax.Index);
			if (boundIndex.Type != PrimitiveTypeSymbol.Integer)
				_diagnostics.ReportCannotConvert(syntax.Span, boundIndex.Type, PrimitiveTypeSymbol.Integer);

			BoundArrayIndex boundChild = null;
			if (syntax.ArrayIndex != null)
			{
				boundChild = BindArrayIndexExpression(syntax.ArrayIndex, ((ArrayTypeSymbol)type).Child);
			}
			return new BoundArrayIndex(boundIndex, boundChild);
		}

		private BoundExpression BindMemberAccessExpression(MemberAccessExpressionSyntax syntax)
		{
			if (TryBindEnumExpression(syntax, out var result))
				return result;
			if (TryBindStaticMemberAccess(syntax, out result))
				return result;

			var boundExpression = BindExpression(syntax.Expression);

			if (boundExpression.Type == _builtInTypes.LookSymbolUp(typeof(LibrarySymbol)))
				return BindLibraryAccessExpression(boundExpression, syntax);


			BoundExpression boundMember;
			if (syntax.Member.Kind == SyntaxKind.FunctionExpression)
			{
				boundMember = BindMemberFunctionExpression((FunctionExpressionSyntax)syntax.Member, boundExpression.Type);
				if (boundMember is BoundFunctionExpression functionExpression)
					return new BoundFunctionAccessExpression(boundExpression, functionExpression);
				return new BoundErrorExpression();
			}
			else if (syntax.Member.Kind == SyntaxKind.VariableExpression)
			{
				boundMember = BindMemberVariableExpression((VariableExpressionSyntax)syntax.Member, boundExpression.Type);
				return new BoundFieldAccesExpression(boundExpression, (BoundVariableExpression)boundMember);
			}
			else
				throw new Exception();
		}

		private bool TryBindEnumExpression(MemberAccessExpressionSyntax syntax, out BoundExpression expression)
		{
			expression = null;
			if (!(syntax.Expression is VariableExpressionSyntax v))
				return false;
			var name = v.Identifier.Text;
			if (!_scope.TryLookup(name, out TypeSymbol type))
				return false;
			if (type.Type != TypeType.Enum)
				return false;
			var enumType = (EnumTypeSymbol)type;
			if (syntax.Member.Kind != SyntaxKind.VariableExpression)
			{
				expression = new BoundErrorExpression();
				_diagnostics.ReportUnexpectedMemberKind(syntax.Span, syntax.Member.Kind, enumType);
				return true;
			}
			var member = (VariableExpressionSyntax)syntax.Member;
			var memberName = member.Identifier.Text;
			if (!enumType.Values.Contains(memberName))
			{
				_diagnostics.ReportUndefinedEnumValue(syntax.Span, enumType, memberName);
				expression = new BoundErrorExpression();
				return true;
			}
			expression = new BoundEnumExpression(enumType, memberName);
			return true;
		}

		public BoundExpression BindLibraryAccessExpression(BoundExpression libraryExpression, MemberAccessExpressionSyntax syntax)
		{
			var libraryVariable = ExtractVariableSymbol(libraryExpression);
			var library = _references.FirstOrDefault(l => l.Name == libraryVariable.Name);
			if (library == null)
			{
				_diagnostics.ReportBadLibrary(syntax.Expression.Span, libraryVariable.Name);
				return new BoundErrorExpression();
			}
			if (syntax.Member.Kind == SyntaxKind.VariableExpression)
			{
				var variableExpression = (VariableExpressionSyntax)syntax.Member;
				var style = library.Styles.FirstOrDefault(s => s.Name == variableExpression.Identifier.Text);
				if (style == null)
				{
					_diagnostics.ReportUndefinedStyle(variableExpression.Span, variableExpression.Identifier.Text, library);
					return new BoundErrorExpression();
				}
				return new BoundVariableExpression(new VariableSymbol(style.Name, true, _builtInTypes.LookSymbolUp(typeof(Style)), false), null, _builtInTypes.LookSymbolUp(typeof(Style)));
			}
			else if (syntax.Member.Kind == SyntaxKind.FunctionExpression)
			{
				var functionExpression = (FunctionExpressionSyntax)syntax.Member;
				return BindLibraryFunctionExpression(functionExpression, library);
			}
			else throw new Exception();
		}

		private bool TryBindStaticMemberAccess(MemberAccessExpressionSyntax syntax, out BoundExpression expression)
		{
			expression = null;
			if (!(syntax.Expression is VariableExpressionSyntax v))
				return false;
			var name = v.Identifier.Text;
			if (!_scope.TryLookup(name, out TypeSymbol type))
				return false;
			if (type.Type != TypeType.Advanced)
				return false;
			var advancedType = (AdvancedTypeSymbol)type;
			if (syntax.Member.Kind != SyntaxKind.VariableExpression)
			{
				expression = new BoundErrorExpression();
				_diagnostics.ReportUnexpectedMemberKind(syntax.Span, syntax.Member.Kind, advancedType);
				return true;
			}
			var member = (VariableExpressionSyntax)syntax.Member;
			var memberName = member.Identifier.Text;
			if (!advancedType.TryLookUpStaticField(memberName, out var field))
			{
				_diagnostics.ReportUndefinedVariable(syntax.Span, memberName, advancedType);
				expression = new BoundErrorExpression();
				return true;
			}
			expression = new BoundStaticFieldAccesExpression(advancedType, field);
			return true;
		}

		public BoundVariableExpression BindMemberVariableExpression(VariableExpressionSyntax syntax, TypeSymbol type)
		{
			VariableSymbol variable = CheckMemberVariableExpression(syntax, null, type);
			return BindVariableExpression(syntax, variable);
		}

		private VariableSymbol CheckMemberVariableExpression(VariableExpressionSyntax syntax, TypeSymbol target, TypeSymbol parent)
		{
			var name = syntax.Identifier.Text;
			var varType = target;
			if (varType == null)
				varType = PrimitiveTypeSymbol.Error;
			var variable = new VariableSymbol(name, false, varType, varType.IsData);

			if (!parent.TryLookUpField(name, out var targetVariable) &&
				!_builtInConstants.TryGetValue(name, out targetVariable))
			{
				_diagnostics.ReportUndefinedVariable(syntax.Span, name, parent);
				varType = PrimitiveTypeSymbol.Error;
			}
			else
			{
				variable = targetVariable;
				varType = variable.Type;

				if (varType == null)
					throw new Exception();
			}

			return variable;
		}

		private BoundVariableExpression BindGlobalVariableExpression(VariableExpressionSyntax syntax)
		{
			VariableSymbol variable = CheckGlobalVariableExpression(syntax, null, false);
			return BindVariableExpression(syntax, variable);
		}

		private VariableSymbol CheckGlobalVariableExpression(VariableExpressionSyntax syntax, TypeSymbol type, bool declare, bool countReferences = true)
		{
			if (declare && type == null)
				throw new Exception();
			var name = syntax.Identifier.Text;
			var varType = type;
			if (varType == null)
				varType = PrimitiveTypeSymbol.Error;
			var variable = new VariableSymbol(name, false, varType, varType.IsData);

			if (declare)
			{
				TextSpan? span = syntax.Span;
				if (!countReferences)
					span = null;
				if (!_scope.TryDeclare(variable, span))
					_diagnostics.ReportVariableAlreadyDeclared(syntax.Span, name);
			}
			else
			{
				if (!_scope.TryLookup(name, out VariableSymbol targetVariable) &&
					!_builtInConstants.TryGetValue(name, out targetVariable))
				{
					_diagnostics.ReportUndefinedVariable(syntax.Span, name);
					varType = PrimitiveTypeSymbol.Error;

				}
				else
				{
					if (targetVariable.IsVisible != variable.IsVisible && varType != PrimitiveTypeSymbol.Error)
						_diagnostics.ReportCannotChangeVisibility(syntax.Span, targetVariable, variable);

					variable = targetVariable;
					varType = variable.Type;
				}
			}

			if (varType == null)
				throw new Exception();
			else if (varType == PrimitiveTypeSymbol.Error)
				return variable;

			if (syntax.PreTildeToken != null)
			{
				if (varType.IsData)
					_diagnostics.ReportCannotBeInvisible(syntax.Span, varType);
				else
					variable.IsVisible = false;
			}
			if (syntax.PostTildeToken != null)
			{
				if (!variable.NeedsDataFlag)
					_diagnostics.ReportVariableCannotBeFlaggedAsData(syntax.Identifier.Span, varType);
			}
			else
			{
				if (varType.IsData && variable.NeedsDataFlag)
					_diagnostics.ReportVariableMustBeFlaggedAsData(syntax.Span, varType);
			}
			return variable;
		}

		private BoundVariableExpression BindVariableExpression(VariableExpressionSyntax syntax, VariableSymbol variable)
		{
			TypeSymbol type = variable.Type;
			BoundArrayIndex boundArrayIndex = null;
			if (syntax.ArrayIndex != null && type != PrimitiveTypeSymbol.Error)
			{
				boundArrayIndex = BindArrayIndexExpression(syntax.ArrayIndex, variable.Type);
				var currentArrayIndex = boundArrayIndex;
				while (currentArrayIndex != null)
				{
					type = ((ArrayTypeSymbol)type).Child;
					currentArrayIndex = currentArrayIndex.BoundChild;
				}
			}

			if (type == null)
				throw new Exception();
			return new BoundVariableExpression(variable, boundArrayIndex, type);
		}

		private BoundExpression BindFunctionExpression(FunctionExpressionSyntax syntax, TypeSymbol parent = null)
		{
			if (parent == null)
				return BindGlobalFunctionExpression(syntax);
			else
				return BindMemberFunctionExpression(syntax, parent);
		}

		private BoundExpression BindLibraryFunctionExpression(FunctionExpressionSyntax syntax, LibrarySymbol library)
		{
			var name = syntax.Identifier.Text;
			if (!library.TryLookUpFunction(name, out var functions))
			{
				_diagnostics.ReportUndefinedFunction(syntax.Span, name, library);
				return new BoundErrorExpression();
			}
			return BindFunctionExpression(syntax, functions, library);
		}

		private BoundExpression BindMemberFunctionExpression(FunctionExpressionSyntax syntax, TypeSymbol parent)
		{
			var name = syntax.Identifier.Text;
			if (!parent.TryLookUpFunction(name, out var function))
			{
				_diagnostics.ReportUndefinedFunction(syntax.Span, name, parent);
				return new BoundErrorExpression();
			}
			if (name == "constructor")
				throw new Exception();
			return BindFunctionExpression(syntax, function);

		}

		private BoundExpression BindFunctionExpression(FunctionExpressionSyntax syntax, FunctionSymbol[] allFunctions, LibrarySymbol source = null)
		{
			var name = syntax.Identifier.Text;
			var argumentCount = syntax.Arguments.Length;
			if (allFunctions.Length == 0)
			{
				_diagnostics.ReportUndefinedFunction(syntax.Span, name);
				return new BoundErrorExpression();
			}
			var functions = new List<FunctionSymbol>();
			FunctionSymbol minParameterCount = null;
			FunctionSymbol maxParameterCount = null;
			foreach (var function in allFunctions)
			{
				if (function.Parameter.Count == argumentCount)
				{
					functions.Add(function);
				}
				else
				{
					if (minParameterCount == null && argumentCount > function.Parameter.Count)
						minParameterCount = function;
					if (maxParameterCount == null && argumentCount < function.Parameter.Count)
						maxParameterCount = function;
					if (minParameterCount != null && minParameterCount.Parameter.Count > function.Parameter.Count)
						minParameterCount = function;
					if (maxParameterCount != null && maxParameterCount.Parameter.Count < function.Parameter.Count)
						maxParameterCount = function;
				}
			}
			if (!functions.Any())
			{
				_diagnostics.ReportCannotFindFunction(syntax.Span, name, argumentCount, minParameterCount, maxParameterCount);
				return new BoundErrorExpression();
			}
			if (argumentCount == 0)
			{
				return new BoundFunctionExpression(functions.Single(), new BoundExpression[0], source);
			}

			var arguments = new List<BoundExpression>();
			var parameterDiagnostics = new DiagnosticBag[functions.Count];
			for (int i = 0; i < argumentCount; i++)
			{
				BoundExpression boundArgument = new BoundLiteralExpression(null, TypeSymbol.Undefined);
				if (syntax.Arguments[i] != null)
					boundArgument = BindExpression(syntax.Arguments[i]);
				for (int j = functions.Count - 1; j >= 0; j--)
				{
					if (parameterDiagnostics[j] == null)
						parameterDiagnostics[j] = new DiagnosticBag(_diagnostics.FileName);
					var parameterType = functions[j].Parameter[i].Type;
					if (!boundArgument.Type.CanBeConvertedTo(parameterType))
					{
						parameterDiagnostics[j].ReportCannotConvert(syntax.Arguments[i].Span, boundArgument.Type, parameterType);
					}
					else
					{
						boundArgument = BindConversion(boundArgument, parameterType);
					}
				}
				arguments.Add(boundArgument);
			}
			var min = parameterDiagnostics.OrderBy(d => d.Count()).First();
			var index = Array.IndexOf(parameterDiagnostics, min);
			var bestMatch = functions[index];
			if (parameterDiagnostics[index].Any())
			{
				_diagnostics.ReportCannotFindFunction(syntax.Span, name, bestMatch, parameterDiagnostics[index]);
				return new BoundErrorExpression();
			}

			switch (bestMatch.Name)
			{
				case "svg":
				case "image":
					BindImageFunction(syntax.Span, bestMatch, arguments.ToArray());
					break;
				case "font":
					BindFontFunction(syntax.Span, bestMatch, arguments.ToArray());
					break;
			}
			return new BoundFunctionExpression(bestMatch, arguments.ToArray(), source);
		}

		private BoundExpression BindConversion(BoundExpression expression, TypeSymbol targetType)
		{
			if ((expression.Type == PrimitiveTypeSymbol.Integer || expression.Type == PrimitiveTypeSymbol.Float) && targetType == PrimitiveTypeSymbol.Unit)
				return new BoundConversion(expression, targetType);
			return expression;
		}

		private void BindFontFunction(TextSpan span, FunctionSymbol function, BoundExpression[] immutableArguments)
		{
			//System.Drawing.Font f = new System.Drawing.Font(name, 0f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
		}

		private void BindImageFunction(TextSpan span, FunctionSymbol function, BoundExpression[] expression)
		{
			var pathExpression = expression[0];
			if (pathExpression is BoundLiteralExpression l)
			{
				var path = l.Value.ToString();

				if (!File.Exists(path))
				{
					_diagnostics.ReportFileDoesNotExist(span, path);
				}
			}
			else
			{
				Logger.LogCannotTestImageFunction(pathExpression.Kind.ToString());

			}
		}



		private BoundExpression BindGlobalFunctionExpression(FunctionExpressionSyntax syntax)
		{
			var name = syntax.Identifier.Text;

			if (!_scope.TryLookup(name, out FunctionSymbol[] functions))
			{
				_diagnostics.ReportUndefinedFunction(syntax.Span, name);
				return new BoundErrorExpression();
			}
			return BindFunctionExpression(syntax, functions);
		}

		private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
		{
			return BindExpression(syntax.Expression);
		}

		private BoundExpression BindStringExpression(StringExpressionSyntax syntax)
		{
			var boundExpressions = new List<BoundExpression>();

			for (int i = 0; i < syntax.Literals.Length + syntax.Insertions.Length; i++)
			{
				if (i % 2 == 0)
				{
					if (syntax.FirstElementIsLiteral)
						boundExpressions.Add(BindLiteralExpression(syntax.Literals[i / 2]));
					else
						boundExpressions.Add(BindExpression(syntax.Insertions[i / 2].Expression));
				}
				else
				{
					if (!syntax.FirstElementIsLiteral)
						boundExpressions.Add(BindLiteralExpression(syntax.Literals[i / 2]));
					else
						boundExpressions.Add(BindExpression(syntax.Insertions[i / 2].Expression));
				}
			}
			return new BoundStringExpression(boundExpressions.ToArray());
		}

		private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax, TypeSymbol targetType = null)
		{
			var value = syntax.Value;
			if (value == null)
			{
				if (targetType != null && targetType.Type != TypeType.Nullable)
				{
					//_diagnostics.ReportValueCanNotBeNone();
					return new BoundErrorExpression();
				}

				return new BoundLiteralExpression(value, targetType ?? TypeSymbol.Undefined);
			}
			return new BoundLiteralExpression(value);

		}

		private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
		{
			var boundVariables = new List<VariableSymbol>();
			foreach (var variable in syntax.Variables)
			{
				boundVariables.Add(BindGlobalVariableExpression(variable).Variable);
				_assignedVariables.Add(boundVariables.Last());
			}
			BoundExpression boundExpression = null; // BindExpression(syntax.Expression);
			if (syntax.Variables.Length == 1)
			{

				switch (syntax.OperatorToken.Kind)
				{
					case SyntaxKind.PlusEqualsToken:
						boundExpression = BindBinaryExpression(new BinaryExpressionSyntax(syntax.Variables[0], new SyntaxToken(SyntaxKind.PlusToken, syntax.OperatorToken.Position, "+", null), syntax.Expression));
						break;
					case SyntaxKind.MinusEqualsToken:
						boundExpression = BindBinaryExpression(new BinaryExpressionSyntax(syntax.Variables[0], new SyntaxToken(SyntaxKind.MinusToken, syntax.OperatorToken.Position, "-", null), syntax.Expression));
						break;
					case SyntaxKind.StarEqualsToken:
						boundExpression = BindBinaryExpression(new BinaryExpressionSyntax(syntax.Variables[0], new SyntaxToken(SyntaxKind.StarToken, syntax.OperatorToken.Position, "*", null), syntax.Expression));
						break;
					case SyntaxKind.SlashEqualsToken:
						boundExpression = BindBinaryExpression(new BinaryExpressionSyntax(syntax.Variables[0], new SyntaxToken(SyntaxKind.SlashToken, syntax.OperatorToken.Position, "/", null), syntax.Expression));
						break;
					case SyntaxKind.EqualsToken:
						boundExpression = BindExpression(syntax.Expression);
						break;
					default:
						throw new Exception();
				}

				var variable = boundVariables.First();
				if (!boundExpression.Type.CanBeConvertedTo(variable.Type))
				{
					_diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
					return new BoundErrorExpression();
				}
			}
			else
			{
				if (syntax.OperatorToken.Kind != SyntaxKind.EqualsToken)
				{
					_diagnostics.ReportNoInlineOperatorForTuples(syntax.OperatorToken);
					return new BoundErrorExpression();
				}
				boundExpression = BindExpression(syntax.Expression);

				var targetType = new TupleTypeSymbol(boundVariables.Select(v => v.Type).ToArray());
				if (boundExpression.Type.Type != TypeType.Tuple)
				{
					_diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, targetType);
					return new BoundErrorExpression();
				}
				var type = (TupleTypeSymbol)boundExpression.Type;
				if (type.Length != targetType.Length)
				{
					_diagnostics.ReportCannotConvert(syntax.Expression.Span, type, targetType);
					return new BoundErrorExpression();
				}
			}

			return new BoundAssignmentExpression(boundVariables.ToArray(), boundExpression);
		}

		private BoundExpression BindFieldAssignmentExpression(FieldAssignmentExpressionSyntax syntax)
		{
			var boundField = BindFieldAccessExpression(syntax.Left);
			var boundExpression = BindExpression(syntax.Right);

			if (!boundExpression.Type.CanBeConvertedTo(boundField.Type))
			{
				_diagnostics.ReportCannotConvert(syntax.Right.Span, boundExpression.Type, boundField.Type);
				//Maybe return BoundErrorExpression();
				//I mean, we probably never use this feature. But yeah. Just do it 
				//So we dont have cascading errors;
				return new BoundErrorExpression();
			}

			return new BoundFieldAssignmentExpression(boundField, boundExpression);
		}

		private BoundFieldAccesExpression BindFieldAccessExpression(FieldAccessExpressionSyntax syntax)
		{
			var boundParent = BindExpression(syntax.Parent);
			var boundVariable = BindMemberVariableExpression(syntax.Variable, boundParent.Type);
			return new BoundFieldAccesExpression(boundParent, boundVariable);
		}

		private BoundExpression BindLambdaExpression(LambdaExpressionSyntax syntax)
		{
			_scope = new BoundScope(_scope);
			var name = syntax.Variable.Identifier.Text;
			var variable = new VariableSymbol(name, true, PrimitiveTypeSymbol.Float, true);
			if (!_scope.TryDeclare(variable, syntax.Variable.Span))
			{
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Variable.Span, name);
				return new BoundErrorExpression();
			}
			var expression = BindExpression(syntax.Expression);
			_scope = _scope.Parent;
			return new BoundLambdaExpression(variable, expression);
		}

		private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
		{
			var boundOperand = BindExpression(syntax.Operand);
			var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

			if (boundOperator == null)
			{
				_diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
				return boundOperand;
			}

			return new BoundUnaryExpression(boundOperator, boundOperand);
		}

		private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
		{
			//TODO(Major): what to do if you have int + float or something like that???
			var boundLeft = BindExpression(syntax.Left);
			var boundRight = BindExpression(syntax.Right);
			var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

			if (boundLeft.Type == PrimitiveTypeSymbol.Error || boundRight.Type == PrimitiveTypeSymbol.Error)
			{
				return new BoundErrorExpression();
			}
			else if (boundOperator == null)
			{
				_diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
				return new BoundErrorExpression();
			}

			return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
		}
	}
}
