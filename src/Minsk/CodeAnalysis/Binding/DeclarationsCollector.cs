using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using Slides;
using Slides.Elements;
using Slides.Filters;
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
		private Binder _binder;

		public DeclarationsCollector(Binder binder, BoundScope scope, StatementSyntax root)
		{
			_binder = binder;
			_scope = scope;
			_root = root;
			_diagnostics = binder.Diagnostics;
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
					CollectDeclarations((FileBlockStatementSyntax)syntax);
					break;
				case SyntaxKind.GroupStatement:
					CollectDeclarations((GroupStatementSyntax)syntax);
					break;
				case SyntaxKind.SVGStatement:
					CollectDeclarations((SVGStatementSyntax)syntax);
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
					_diagnostics.ReportBadTopLevelStatement(syntax);
					return;
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
			var boundParameters = _binder.BindParameterBlockStatement(syntax.ParameterStatement);
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
		private void CollectDeclarations(SVGStatementSyntax syntax)
		{
			var name = syntax.Identifier.Text;
			_scope = new BoundScope(_scope);
			var boundParameters = _binder.BindParameterBlockStatement(syntax.ParameterStatement);
			_scope = _scope.Parent;
			var fields = new VariableSymbolCollection(boundParameters.Statements.Select(p => p.Variable));
			var constructor = new FunctionSymbolCollection();
			constructor.Add(new FunctionSymbol("constructor", fields, null));
			constructor.Seal();
			var type = new AdvancedTypeSymbol(name, fields, constructor, FunctionSymbolCollection.Empty, _builtInTypes.LookSymbolUp(typeof(SVGTag)));
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
				variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(StdStyle)), true);
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
			var isVisible = syntax.PretildeToken == null;
			var variable = new VariableSymbol(name, true, _builtInTypes.LookSymbolUp(typeof(SlideAttributes)), false);
			variable.IsVisible = isVisible;

			if (!_scope.TryDeclare(variable, null))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);

			//TODO: What to do, when we have multiple elements with the same name? We need to throw a diagnostic here!
			//Great. Now thats done, we need to improve the diagnostic!
			if (_declarations.ContainsKey(variable))
				_diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
			else
				_declarations.Add(variable, null);
		}


		private VariableSymbolCollection BindDataBlockStatement(DataBlockStatementSyntax syntax)
		{
			var statements = new VariableSymbolCollection();
			_scope = new BoundScope(_scope);

			foreach (var statementSyntax in syntax.Statements)
			{
				var statement = _binder.BindParameterStatement(statementSyntax.Parameter);
				statements.Add(statement.Variable);
			}

			_scope = _scope.Parent;

			return statements;
		}

	}
}
