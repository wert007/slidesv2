using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Lowering;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Slides.Debug;

namespace Minsk.CodeAnalysis
{

	public sealed class Compilation
	{
		private BoundGlobalScope _globalScope;
		public LibrarySymbol[] References { get;
			set; }
		private string _fileName;

		public Compilation(SyntaxTree syntaxTree, bool offlineView)
			 : this(null, syntaxTree, offlineView)
		{

		}

		private Compilation(Compilation previous, SyntaxTree syntaxTree, bool offlineView)
		{
			Previous = previous;
			SyntaxTree = syntaxTree;
			OfflineView = offlineView;
			_fileName = syntaxTree.Text.FileName;
		}

		public Compilation Previous { get; }
		public SyntaxTree SyntaxTree { get; }
		public bool OfflineView { get; }

		internal BoundGlobalScope GlobalScope
		{
			get
			{
				if (_globalScope == null)
				{
					var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree, References, OfflineView);
					Interlocked.CompareExchange(ref _globalScope, globalScope, null);
				}

				return _globalScope;
			}
		}

		
		public Compilation ContinueWith(SyntaxTree syntaxTree)
		{
			return new Compilation(this, syntaxTree, OfflineView);
		}

		public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables, TimeWatcher timewatch)
		{
			var diagnostics = SyntaxTree.Diagnostics.ToArray();
			if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
				return new EvaluationResult(diagnostics, null, new TimeWatcher());
			diagnostics = GlobalScope.Diagnostics;
			if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
				return new EvaluationResult(diagnostics, null, new TimeWatcher());


			var statement = GetStatement();
			var declarations = GetDeclarations();

			var syntacticSugar = new SyntaxSugarReplacer();
			statement = (BoundBlockStatement)syntacticSugar.RewriteStatement(statement);

			var evaluator = new Evaluator(statement, variables, References, declarations);
			timewatch.Record("create new evaluator");
			var value = evaluator.Evaluate();
			timewatch.Record("evaluate");
			return new EvaluationResult(diagnostics, value, timewatch);
		}

		public void EmitTree(TextWriter writer)
		{
			var statement = GetStatement();
			statement.WriteTo(writer);
		}

		private BoundBlockStatement GetStatement()
		{
			var result = GlobalScope.Statement;
			// return Lowerer.Lower(result);
			return (BoundBlockStatement)result;
		}

		private Dictionary<VariableSymbol, BoundStatement> GetDeclarations()
		{
			var result = GlobalScope.Declarations;
			return result;
		}
	}
}