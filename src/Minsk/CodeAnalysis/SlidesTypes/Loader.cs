using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
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
			return new EvaluationResult(diagnostics.ToArray(), null, new TimeWatcher());
		}

		public static Compilation LoadCompilationFromFile(string directory, string fileName)
		{
			var path = Path.Combine(directory, fileName);
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
			var syntaxTree = SyntaxTree.Parse(text, fileName);
			return new Compilation(syntaxTree);
		}

		//Entry Point.
		public static EvaluationResult LoadFromFile(string directory, string fileName, bool showTree, bool showProgram, bool completeRebuild)
		{
			var timewatch = new TimeWatcher();
			timewatch.Start();
			var compilation = LoadCompilationFromFile(directory, fileName);
			timewatch.Record("load Compilation");
			var variables = new Dictionary<VariableSymbol, object>();
			var syntaxTree = compilation.SyntaxTree;

			if (!syntaxTree.Diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
			{
				var linker = new Linker(completeRebuild);
				timewatch.Record("initialise linker");
				timewatch.Push();
				compilation.References = linker.Link(compilation, timewatch, fileName);
				timewatch.Pop();
			}
			timewatch.Record("link libraries");

			if (showTree)
				syntaxTree.Root.WriteTo(Console.Out);

			if (showProgram)
				compilation.EmitTree(Console.Out);

			timewatch.Push();
			var result = compilation.Evaluate(variables, timewatch);
			timewatch.Pop();
			timewatch.Record("evaluate program");
			DiagnosticBag.OutputToConsole(result.Diagnostics, syntaxTree.Text, 100);
			Console.WriteLine();
			return new EvaluationResult(result.Diagnostics, result.Value, timewatch);

		}
	}
}
