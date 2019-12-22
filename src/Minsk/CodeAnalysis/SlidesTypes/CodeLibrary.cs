using System;
using System.Text;
using Github;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Code;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	public static class CodeLibrary
	{
		public static LibrarySymbol GetLibrary()
		{
			var name = "code";
			var libraries = new LibrarySymbol[0];
			var customTypes = new BodySymbol[0];
			var styles = new Style[0];
			var globalVariables = new VariableValueCollection(null);
			var globalFunctions = new FunctionSymbol[]
			{
				new FunctionSymbol("github", new VariableSymbol("path", false, PrimitiveTypeSymbol.String, true), TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(GitRepository))),
				new FunctionSymbol("codeblock", new VariableSymbolCollection(new VariableSymbol[]
				{
					new VariableSymbol("file", false, TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(GitFile)), true),
					new VariableSymbol("lines", false, TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(Range)), true)
				}), TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(CodeBlock))),
				new FunctionSymbol("codeblock", new VariableSymbolCollection(new VariableSymbol[]
				{
					new VariableSymbol("repository", false, TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(GitRepository)), true),
					new VariableSymbol("fileName", false, PrimitiveTypeSymbol.String, true),
					new VariableSymbol("lines", false, TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(Range)), true)
				}), TypeSymbolTypeConverter.Instance.LookSymbolUp(typeof(CodeBlock))),
			};
			var globalFunctionsReflections = new string[]
			{
				nameof(GetGitRepository),
				nameof(CreateCodeBlockFromFile),
				nameof(CreateCodeBlockFromRep),
			};
			var imports = new string[0];
			var result = new LibrarySymbol(name, libraries, customTypes, styles, globalVariables, globalFunctions, globalFunctionsReflections, imports);
			result.SourceType = typeof(CodeLibrary);
			return result;
		}

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
			var builder = new StringBuilder();
			int start = 0;
			int end = 0;
			int foundLines = 1;
			int index = 0;
			int minWhitespaces = int.MaxValue;
			bool prevWasLineBreakOrWhitespace = false;
			while(foundLines < lines.From)
			{
				if (source[index] == '\n')
					foundLines++;
				index++;
				if (foundLines == lines.From)
					start = index;
			}
			while (foundLines < lines.To)
			{
				if (source[index] == '\n')
				{
					foundLines++;
					prevWasLineBreakOrWhitespace = true;
				}
				if (foundLines == lines.To)
					end = index;
				int curWhitespaces = 0;
				while (prevWasLineBreakOrWhitespace && char.IsWhiteSpace(source[index + 1]))
				{
					curWhitespaces++;
					index++;
				}
				if(prevWasLineBreakOrWhitespace)
					minWhitespaces = Math.Min(minWhitespaces, curWhitespaces);
				prevWasLineBreakOrWhitespace = false;
				index++;
			}
			for (int i = start + minWhitespaces; i < end; i++)
			{
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
			var result = new CodeBlock(code, file.language, $"{file.name} Lines {lines.From}-{lines.To}");
			result.lineStart = lines.From;
			return result;
		}

		//code.codeblock(repository~, 'main.c', 23..54);
		public static CodeBlock CreateCodeBlockFromRep(GitRepository repository, string fileName, Range lines)
		{
			var file = repository.file(fileName);
			return CreateCodeBlockFromFile(file, lines);
		}
	}
}
