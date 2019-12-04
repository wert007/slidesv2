using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Minsk.CodeAnalysis.SlidesTypes
{

	public static class Loader
	{
		private static EvaluationResult CouldnotFindFile(string path)
		{
			var diagnostics = new DiagnosticBag(path);
			diagnostics.ReportPresentationDoesNotExist(path);
			return new EvaluationResult(diagnostics.ToImmutableArray(), null, new TimeWatcher());
		}

		public static Compilation LoadCompilationFromFile(string path)
		{
			Console.WriteLine($"Loading {path}...");
			string fileContent;
			if (!File.Exists(path))
				return null;
			using (FileStream fs = new FileStream(path, FileMode.Open))
			using (StreamReader reader = new StreamReader(fs))
			{
				fileContent = reader.ReadToEnd();
			}


			var text = fileContent;
			var syntaxTree = SyntaxTree.Parse(text, path);
			return new Compilation(syntaxTree);
		}

		internal static BoundGlobalScope LoadBSLDFile(string path)
		{
			Console.WriteLine($"Loading {path}...");
			string fileContent;
			if (!File.Exists(path))
				return null;
			using (FileStream fs = new FileStream(path, FileMode.Open))
			using (StreamReader reader = new StreamReader(fs))
			{
				fileContent = reader.ReadToEnd();
			}
			var deserializer = new Deserializer(fileContent);
			var root = deserializer.Deserialize();
			var boundGlobalScope = new BoundGlobalScope(null, ImmutableArray.Create<Diagnostic>(), ImmutableArray.Create<VariableSymbol>(), root);

			return boundGlobalScope;
		}


		//Entry Point.
		public static EvaluationResult LoadFromFile(string path, bool showTree, bool showProgram)
		{
			var timewatch = new TimeWatcher();
			timewatch.Start();
			var compilation = LoadCompilationFromFile(path);
			timewatch.Record("load Compilation");
			var variables = new Dictionary<VariableSymbol, object>();
			var syntaxTree = compilation.SyntaxTree;
			var linker = new Linker();
			timewatch.Record("initialise linker");
			timewatch.Push();
			compilation.References = linker.Link(compilation, timewatch, path);
			timewatch.Pop();

			timewatch.Record("link libraries");

			if (showTree)
				syntaxTree.Root.WriteTo(Console.Out);

			if (showProgram)
				compilation.EmitTree(Console.Out);

			timewatch.Push();
			var result = compilation.Evaluate(variables, timewatch);
			timewatch.Pop();
			timewatch.Record("evaluate program");
			DiagnosticBag.OutputToConsole(result.Diagnostics, syntaxTree.Text);
			Console.WriteLine();
			return new EvaluationResult(result.Diagnostics, result.Value, timewatch);

		}
	}
}
