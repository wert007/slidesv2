using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Github;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Code;
using Slides.Data;
using Slides.Styling;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	public static class CodeLibrary
	{
		private class CodeCallable : Callable
		{

			public static GitRepository GetGitRepository(string path)
			{
				//TODO: Make sure during Compile time, that we have a '/' in our path.
				var owner = path.Split('/')[0];
				var name = path.Split('/')[1];
				var repository = GithubClient.GetRepository(owner, name);
				return new GitRepository(owner, name, repository.Contents, repository.Language);
			}


			private static string Beautify(string source, Range lines)
			{
				source = source.Replace("\r", "");
				var builder = new StringBuilder();
				int start = 0;
				int end = 0;
				int foundLines = 1;
				int index = 0;
				int minWhitespaces = int.MaxValue;
				bool prevWasLineBreakOrWhitespace = false;
				while (foundLines < lines.From)
				{
					if (source[index] == '\n')
						foundLines++;
					index++;
				}
				start = index;
				while (foundLines <= lines.To)
				{
					if (source[index] == '\n')
					{
						foundLines++;
						prevWasLineBreakOrWhitespace = true;
					}
					int curWhitespaces = 0;
					while (prevWasLineBreakOrWhitespace && source[index + 1] != '\n' && char.IsWhiteSpace(source[index + 1]))
					{
						curWhitespaces++;
						index++;
					}
					if (prevWasLineBreakOrWhitespace)
						minWhitespaces = Math.Min(minWhitespaces, curWhitespaces);
					prevWasLineBreakOrWhitespace = false;
					index++;
				}
				end = index;
				for (int i = start + minWhitespaces; i < end; i++)
				{
					if (source[i] == '\\')
						builder.Append('\\');
					builder.Append(source[i]);
					if (source[i] == '\n')
					{
						i += minWhitespaces;
					}
				}
				return builder.ToString();
			}

			public static CodeBlock CreateCodeBlockFromFile(GitFile file, Range lines)
			{
				var code = Beautify(file.content, lines);
				var result = new CodeBlock(code, file.language);
				result.lineStart = lines.From;
				return result;
			}

			//code.codeblock(repository~, 'main.c', 23..54);
			public static CodeBlock CreateCodeBlockFromRep(GitRepository repository, string fileName, Range lines)
			{
				var file = repository.file(fileName);
				return CreateCodeBlockFromFile(file, lines);
			}

			public static GitFile LoadFile(string path, string language)
			{
				var content = System.IO.File.ReadAllText(Path.Combine(CompilationFlags.Directory, path));
				var name = Path.GetFileName(path);
				return new GitFile(name, language, content);
			}

			public override object Call(FunctionSymbol function, object[] args)
			{
				switch (function.Name)
				{
					case "github":
						return GetGitRepository((string)args[0]);
					case "codeblock":
						switch (args.Length)
						{
							case 2:
								return CreateCodeBlockFromFile((GitFile)args[0], (Range)args[1]);
							case 3:
								return CreateCodeBlockFromRep((GitRepository)args[0], (string)args[1], (Range)args[1]);
							default:
								throw new NotImplementedException($"No implementation of function '{function}' with {args.Length} arguments found!");
						}
					case "loadFile":
						return LoadFile((string)args[0], (string)args[1]);
					default:
						throw new NotImplementedException($"No implementation for function '{function}' found!");
				}
			}
		}
		public static LibrarySymbol GetLibrary()
		{
			var name = "coding";
			var libraries = new LibrarySymbol[0];
			var customTypes = new BodySymbol[0];
			var styles = new StdStyle[0]; //TODO: Why is this a array of StdStyle? There always can only be one!
			var globalVariables = new VariableValueCollection(null);
			var globalFunctionSymbols= new FunctionSymbol[]
			{
				new FunctionSymbol("github", new VariableSymbol("path", false, PrimitiveTypeSymbol.String), BuiltInTypes.Instance.LookSymbolUp(typeof(GitRepository))),

				new FunctionSymbol("codeblock", new VariableSymbolCollection(new VariableSymbol[]
				{
					new VariableSymbol("file", false, BuiltInTypes.Instance.LookSymbolUp(typeof(GitFile))),
					new VariableSymbol("lines", false, BuiltInTypes.Instance.LookSymbolUp(typeof(Range)))
				}), BuiltInTypes.Instance.LookSymbolUp(typeof(CodeBlock))),

				new FunctionSymbol("codeblock", new VariableSymbolCollection(new VariableSymbol[]
				{
					new VariableSymbol("repository", false, BuiltInTypes.Instance.LookSymbolUp(typeof(GitRepository))),
					new VariableSymbol("fileName", false, PrimitiveTypeSymbol.String),
					new VariableSymbol("lines", false, BuiltInTypes.Instance.LookSymbolUp(typeof(Range)))
				}), BuiltInTypes.Instance.LookSymbolUp(typeof(CodeBlock))),

				new FunctionSymbol("loadFile", new VariableSymbolCollection(new VariableSymbol[]
				{
					new VariableSymbol("path", false, PrimitiveTypeSymbol.String),
					new VariableSymbol("language", false, PrimitiveTypeSymbol.String),
				}), BuiltInTypes.Instance.LookSymbolUp(typeof(GitFile))),
			};
			var globalFunctions = new FunctionCallableCollection(globalFunctionSymbols, new CodeCallable());
			//var globalFunctionsReflections = new string[]
			//{
			//	//nameof(SetStyle),
			//	nameof(GetGitRepository),
			//	nameof(CreateCodeBlockFromFile),
			//	nameof(CreateCodeBlockFromRep),
			//	nameof(LoadFile),
			//};
			var imports = new string[0];
			var result = new LibrarySymbol(name, libraries, customTypes, styles, globalVariables, globalFunctions, imports);
			result.SourceType = typeof(CodeLibrary);
			return result;
		}

	}
}
