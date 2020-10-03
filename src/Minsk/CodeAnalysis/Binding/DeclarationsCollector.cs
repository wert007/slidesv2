using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using Slides;
using Slides.Elements;
using Slides.Styling;
using SVGLib.ContainerElements;

namespace Minsk.CodeAnalysis.Binding
{
	internal class DeclarationsCollector
	{
		private readonly BuiltInTypes _builtInTypes = BuiltInTypes.Instance;
		private readonly DiagnosticBag _diagnostics;
		private BoundScope _scope;
		private readonly StatementSyntax _root;
		private Dictionary<VariableSymbol, BoundStatement> _declarations = new Dictionary<VariableSymbol, BoundStatement>();
	
		public DeclarationsCollector(DiagnosticBag diagnostics, BoundScope scope, StatementSyntax root)
		{
			_scope = scope;
			_root = root;
			_diagnostics = diagnostics;
		}

		public Dictionary<VariableSymbol, BoundStatement> CollectDeclarations()
		{
			_declarations.Clear();
			CollectDeclarations(_root);
			return _declarations;
		}

		private void CollectDeclarations(StatementSyntax syntax)
		{
			switch (syntax.Kind)
			{
				case SyntaxKind.FileBlockStatement:
					CollectFileBlockStatementDeclarations((FileBlockStatementSyntax)syntax);
					break;
				case SyntaxKind.GroupStatement:
					CollectGroupStatementDeclarations((GroupStatementSyntax)syntax);
					break;
				case SyntaxKind.SVGStatement:
					CollectSVGStatementDeclarations((SVGStatementSyntax)syntax);
					break;
				case SyntaxKind.StructStatement:
					CollectStructStatementDeclarations((StructStatementSyntax)syntax);
					break;
				case SyntaxKind.StyleStatement:
					CollectStyleStatementDeclarations((StyleStatementSyntax)syntax);
					break;
				case SyntaxKind.TransitionStatement:
					CollectTransitionStatmentDeclarations((TransitionStatementSyntax)syntax);
					break;
				case SyntaxKind.FilterStatement:
					CollectFilterStatementDeclarations((FilterStatementSyntax)syntax);
					break;
				case SyntaxKind.AnimationStatement:
					CollectAnimationStatementDeclarations((AnimationStatementSyntax)syntax);
					break;
				case SyntaxKind.TemplateStatement:
					CollectTemplateStatementDeclarations((TemplateStatementSyntax)syntax);
					break;
				case SyntaxKind.SlideStatement:
					CollectSlideStatementDeclarations((SlideStatementSyntax)syntax);
					break;
				case SyntaxKind.LibraryStatement:
					break; //TODO: Why is there an exception? Just ignore it if its there.
					//It's fine, it's no mistake...
					throw new Exception();
				case SyntaxKind.VariableDeclaration:
				case SyntaxKind.ImportStatement:
					//ImportStatements should always be the first thing you do in 
					//your code. So we don't collect them from anywhere else.
					//the same is true for variable declarations!
					break;
				default:
					_diagnostics.ReportBadTopLevelStatement(syntax);
					return;
			}
		}

		private void CollectFileBlockStatementDeclarations(FileBlockStatementSyntax syntax)
		{
			foreach (var statement in syntax.Statements)
			{
				CollectDeclarations(statement);
			}
		}

		private void CollectGroupStatementDeclarations(GroupStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope = new BoundScope(_scope);
			var binder = new Binder(_scope, null, _diagnostics.FileName, true);
			var boundParameters = binder.BindParameterBlockStatement(syntax.ParameterStatement);
			_scope = _scope.Parent;
			var fields = new VariableSymbolCollection(boundParameters.Statements.Select(p => p.Variable));
			var constructor = new FunctionSymbolCollection();
			constructor.Add(new FunctionSymbol("constructor", fields, null));
			constructor.Seal();
			var type = new AdvancedTypeSymbol(name, fields, constructor, FunctionSymbolCollection.Empty, _builtInTypes.LookSymbolUp(typeof(Element)));
			constructor[0].Type = type;

			if (!_scope.TryDeclare(type))
				_diagnostics.ReportTypeAlreadyDeclared(syntax.Identifier.Span, name);
			_declarations.Add(new VariableSymbol(name, true, type), null);
		}
		private void CollectSVGStatementDeclarations(SVGStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope = new BoundScope(_scope);
			var binder = new Binder(_scope, null, _diagnostics.FileName, true);
			var boundParameters = binder.BindParameterBlockStatement(syntax.ParameterStatement);
			_scope = _scope.Parent;
			var fields = new VariableSymbolCollection(boundParameters.Statements.Select(p => p.Variable));
			var constructor = new FunctionSymbolCollection();
			constructor.Add(new FunctionSymbol("constructor", fields, null));
			constructor.Seal();
			var type = new AdvancedTypeSymbol(name, fields, constructor, FunctionSymbolCollection.Empty, _builtInTypes.LookSymbolUp(typeof(SVGTag)));
			constructor[0].Type = type;

			if (!_scope.TryDeclare(type))
				_diagnostics.ReportTypeAlreadyDeclared(syntax.Identifier.Span, name);
			_declarations.Add(new VariableSymbol(name, true, type), null);
		}

		private void CollectStructStatementDeclarations(StructStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var fields = BindStructBlockStatement(syntax.Body);
			var fieldVariables = new VariableSymbolCollection(fields.Select(p => p.Variable));
			var constructor = new FunctionSymbolCollection();
			//constructor.Add(new FunctionSymbol("constructor", VariableSymbolCollection.Empty, null));
			constructor.Add(new FunctionSymbol("constructor", fieldVariables, null));
			constructor.Add(new FunctionSymbol("constructor", new VariableSymbolCollection(fields.Where(p => p.Initializer == null).Select(p => p.Variable)), null));
			constructor.Seal();

			var customType = new AdvancedTypeSymbol(name, fieldVariables, fields.Select(p => p.Initializer).ToArray(), constructor, FunctionSymbolCollection.Empty, null, new TypeSymbol[0]);
			for (int i = 0; i < constructor.Count; i++)
				constructor[i].Type = customType;

			if (!_scope.TryDeclare(customType))
				_diagnostics.ReportTypeAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(new VariableSymbol(name, true, customType), null);
		}

		private void CollectStyleStatementDeclarations(StyleStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			VariableSymbol variable = null;
			if (syntax.Identifier.Kind != SyntaxKind.StdKeyword)
			{
				variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(StdStyle)));
				TextSpan? span = null;
				//TODO: How to use Flags right here and in the binder and in the evaluator.
				//if (!_flags.IsLibrarySymbol)
				//	span = syntax.Identifier.Span;
				//if (!_flags.StyleAllowed)
				//	//todo;
				//	_flags.StyleAllowed = true;

				if (!_scope.TryDeclare(variable, span))
					_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

				_declarations.Add(variable, null);
			}
		}

		private void CollectTransitionStatmentDeclarations(TransitionStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Transition)));

			if (!_scope.TryDeclare(variable, syntax.Identifier.Span))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private void CollectFilterStatementDeclarations(FilterStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Filter)));

			if (!_scope.TryDeclare(variable, syntax.Identifier.Span))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private void CollectAnimationStatementDeclarations(AnimationStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(AnimationSymbol)));

			if (!_scope.TryDeclare(variable, syntax.Identifier.Span))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private void CollectTemplateStatementDeclarations(TemplateStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(Template)));

			if (!_scope.TryDeclare(variable, null))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			_declarations.Add(variable, null);
		}

		private void CollectSlideStatementDeclarations(SlideStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			var isVisible = syntax.PretildeToken == null;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(SlideAttributes)));

			if (!_scope.TryDeclare(variable, null))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			//TODO: What to do, when we have multiple elements with the same name? We need to throw a diagnostic here!
			//Great. Now thats done, we need to improve the diagnostic!
			if (_declarations.ContainsKey(variable))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
			else
				_declarations.Add(variable, null);
		}


		private IEnumerable<BoundParameterStatement> BindStructBlockStatement(StructBlockStatementSyntax syntax)
		{
			var statements = new List<BoundParameterStatement>();
			_scope = new BoundScope(_scope);
			var binder = new Binder(_scope, null, _diagnostics.FileName,true);
			foreach (var statementSyntax in syntax.Statements)
			{
				var statement = binder.BindParameterStatement(statementSyntax.Parameter);
				statements.Add(statement);
			}

			_scope = _scope.Parent;

			return statements;
		}

	}
}
