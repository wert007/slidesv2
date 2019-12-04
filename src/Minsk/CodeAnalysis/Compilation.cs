using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

		public Compilation(SyntaxTree syntaxTree)
			 : this(null, syntaxTree)
		{

		}

		private Compilation(Compilation previous, SyntaxTree syntaxTree)
		{
			Previous = previous;
			SyntaxTree = syntaxTree;
			_fileName = syntaxTree.Text.FileName;
		}

		public Compilation Previous { get; }
		public SyntaxTree SyntaxTree { get; }

		internal BoundGlobalScope GlobalScope
		{
			get
			{
				if (_globalScope == null)
				{
					var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree, References);
					Interlocked.CompareExchange(ref _globalScope, globalScope, null);
				}

				return _globalScope;
			}
		}

		
		public Compilation ContinueWith(SyntaxTree syntaxTree)
		{
			return new Compilation(this, syntaxTree);
		}

		public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables, TimeWatcher timewatch)
		{
			var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
			if (diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
				return new EvaluationResult(diagnostics, null, new TimeWatcher());

			var statement = GetStatement();
			//timewatch.Record($"write {SyntaxTree.Text.FileName}.bsld");
			//using(FileStream fs = new FileStream($@".\{SyntaxTree.Text.FileName}.bsld", FileMode.Create))
			//using (StreamWriter sw = new StreamWriter(fs))
			//{
			//	sw.WriteLine(Serializer.Serialize(statement));
			//}
			//timewatch.Record($"read {SyntaxTree.Text.FileName}.bsld");
			//using (FileStream fs = new FileStream($@".\{SyntaxTree.Text.FileName}.bsld", FileMode.Open))
			//using (StreamReader sr = new StreamReader(fs))
			//{
			//	var content = sr.ReadToEnd();
			//	var deserializer = new Deserializer(content);
			//	var root = deserializer.Deserialize();
			//}

			var evaluator = new Evaluator(statement, variables, References);
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
	}
}