using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Slides;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Minsk.CodeAnalysis
{
	public sealed class Linker
	{
		//TODO(Improvement): sort fields
		//I dont know what that (^) means. but make clear when you use what!
		private string _presentationName = null;
		private readonly Dictionary<string, string[]> _references = new Dictionary<string, string[]>();
		private readonly Dictionary<string, Compilation> _loadedCompilations = new Dictionary<string, Compilation>();
		private readonly Dictionary<string, LibrarySymbol> _collectedLibraries = new Dictionary<string, LibrarySymbol>();
		private readonly List<string> _referencedInFile = new List<string>();
		private readonly Queue<Compilation> _toCollectReferences = new Queue<Compilation>();
		private readonly bool _completeRebuild;
		private readonly bool _offlineView;
		private DiagnosticBag _diagnostics;

		public Linker(bool completeRebuild, bool offlineView)
		{
			_completeRebuild = completeRebuild;
			_offlineView = offlineView;
		}


		//private void CreateTree(string file)
		//{
		//	Compilation compilation = null;
		//	if (_loadedCompilations.ContainsKey(file))
		//		compilation = _loadedCompilations[file];
		//	else
		//		compilation = Loader.LoadCompilationFromFile(file);
		//	_referencedInFile.Clear();
		//	CreateTree(compilation, file);

		//	_references.Add(file, _referencedInFile.ToArray());
		//	_referencedInFile.Clear();
		//}

		private LibrarySymbol[] Bind(TimeWatcher timewatcher)
		{
			var bindedFiles = new List<string>();
			while (bindedFiles.Count < _references.Count - 1)
			{
				foreach (var reference in _references)
				{
					if (_collectedLibraries.ContainsKey(reference.Key))
						continue;
					if (reference.Key == _presentationName)
						continue;
					if (reference.Value.All(r => bindedFiles.Contains(r)))
					{
						bindedFiles.Add(reference.Key);
						_collectedLibraries.Add(reference.Key, Bind(reference.Key, timewatcher));
					}
				}
				//timewatcher.Record($"bind {bindedFiles.Last()}");

			}
			return _collectedLibraries.Values.ToArray();
		}

		private LibrarySymbol Bind(string key, TimeWatcher timewatch)
		{
			var referenced = new LibrarySymbol[_references[key].Length]; // _collectedLibraries.Values.ToArray();
			for (int i = 0; i < referenced.Length; i++)
			{
				referenced[i] = _collectedLibraries[_references[key][i]];
			}
			_loadedCompilations[key].References = referenced;
			var result = _loadedCompilations[key].Evaluate(new Dictionary<VariableSymbol, object>(), timewatch);
			timewatch.Record($"evaluate {key}");

			var syntaxTree = _loadedCompilations[key].SyntaxTree;
			DiagnosticBag.OutputToConsole(result.Diagnostics, syntaxTree.Text, 100);

			if (result.Diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
				return new LibrarySymbol(key);

			IFormatter formatter = new BinaryFormatter();
			var serializableLibrary = new SerializableLibrarySymbol((LibrarySymbol)result.Value);

			var fileName = Path.GetFileNameWithoutExtension(key);
			using (var stream = new FileStream($@".\{fileName}.bsld", FileMode.Create, FileAccess.Write))
				formatter.Serialize(stream, serializableLibrary);
			return (LibrarySymbol)result.Value;
		}

		public LibrarySymbol[] Link(Compilation root, TimeWatcher timewatch, string fileName)
		{
			_diagnostics = new DiagnosticBag(fileName);
			_diagnostics.AddRange(root.SyntaxTree.Diagnostics);
			CreateTree(root, fileName);
			timewatch.Record("generate linktree");
			return Bind(timewatch);
		}

		private void CreateTree(Compilation root, string fileName)
		{
			if (_referencedInFile.Any())
				throw new Exception();
			if (_presentationName == null)
				_presentationName = fileName;

			_loadedCompilations.Add(fileName, root);

			foreach (var child in root.SyntaxTree.Root.GetChildren())
			{
				CreateTree(child);
			}

			_references.Add(fileName, _referencedInFile.ToArray());
			_referencedInFile.Clear();

			while (_toCollectReferences.Count > 0)
			{
				var reference = _toCollectReferences.Dequeue();
				CreateTree(reference, reference.SyntaxTree.Text.FileName);
			}
		}

		private void CreateTree(SyntaxNode syntax)
		{
			switch (syntax.Kind)
			{
				case SyntaxKind.ImportStatement:
					var import = (ImportStatementSyntax)syntax;
					CreateTree(import.Initializer);
					break;
				default:
					foreach (var child in syntax.GetChildren())
					{
						CreateTree(child);
					}
					break;
			}
		}

		private void CreateTree(FunctionExpressionSyntax expression)
		{
			var functionName = expression.Identifier.Text;
			switch (expression.Arguments[0].Kind)
			{
				case SyntaxKind.LiteralExpression:
					if (functionName == "lib")
					{
						var literal = (LiteralExpressionSyntax)expression.Arguments[0];
						var fileName = (string)literal.Value;
						AddImport(fileName);
					}
					break;
				default:
					//Logger.LogUnexpectedBoundNodeKind(expression.Kind);
					break;
			}
		}

		private void AddImport(string path)
		{
			_referencedInFile.Add(path);
			if (!_loadedCompilations.ContainsKey(path))
			{
				//TODO(Major): Find out when we need to rebuild our bsld-file
				string fileName = Path.GetFileNameWithoutExtension(path);
				if (!_completeRebuild && File.Exists(fileName + ".bsld"))
				{
					using (var stream = new FileStream($@".\{fileName}.bsld", FileMode.Open, FileAccess.Read))
					{
						var formatter = new BinaryFormatter();
						var library = (SerializableLibrarySymbol)formatter.Deserialize(stream);
						_collectedLibraries.Add(path, library.ToLibrarySymbol());
					}
					return;
				}
				var compilation = Loader.LoadCompilationFromFile(CompilationFlags.Directory, path, _offlineView);
				if (compilation == null)
				{
					//This will hopefully be reported later!
					//_diagnostics.ReportCannotFindLibrary();
					return;
				}
				_toCollectReferences.Enqueue(compilation);



				//using (FileStream fs = new FileStream(fileName + ".bsld", FileMode.Create))
				//using (StreamWriter sw = new StreamWriter(fs))
				//	sw.Write(Serializer.Serialize(compilation.GlobalScope.Statement));
			}
		}

	}
}