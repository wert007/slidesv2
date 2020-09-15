using HTMLWriter;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using SimpleLogger;
using Slides;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Minsk
{
	internal static class Program
	{
		const int STATUS_ERRORS_FOUND = 4;
		const int STATUS_WRONG_USAGE = 3;
		const int STATUS_FILE_NOT_FOUND = 2;
		const int STATUS_WARNINGS_FOUND = 1;
		const int STATUS_ALL_OK = 0;

		private static int Main(string[] args)
		{
			bool completeRebuild = false;
			bool offlineView = false;
			var silentMode = args.Contains("-q") || args.Contains("-quietly");
			if (!silentMode)
				foreach (var arg in args)
					Console.WriteLine($">> arg '{arg}' received");
			if (args.Length >= 2)
			{
				if (args.Contains("-build-fresh"))
					completeRebuild = true;
				if (args.Contains("-offline"))
					offlineView = true;
				var result = LoadFromFile(args[0], args[1], completeRebuild, offlineView, silentMode);
				return result;
			}
			else
			{
				Console.Error.WriteLine("Usage: 'slides <SlidesFile> <targetDirectory>");
				return STATUS_WRONG_USAGE;
			}
		}


		private static int LoadFromFile(string path, string targetDirectory, bool completeRebuild, bool offlineView, bool silentMode)
		{
			var statusCode = STATUS_ERRORS_FOUND;
			var fileName = Path.GetFileName(path);
			var directory = Path.GetDirectoryName(path);
			CompilationFlags.Init(directory, fileName, completeRebuild);
			if (!File.Exists(Path.Combine(directory, fileName)))
			{
				Console.Error.WriteLine($"File '{Path.Combine(directory, fileName)}' not found!");
				return STATUS_FILE_NOT_FOUND;
			}
			var result = Loader.LoadFromFile(directory, fileName, false, false, completeRebuild, offlineView);
			if (!result.Diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
			{
				var presentation = result.Value as Presentation;
				PresentationWriter.Write(presentation, targetDirectory, true);
				statusCode = result.Diagnostics.Any(d => d.Level == DiagnosticLevel.Warning) ? STATUS_WARNINGS_FOUND : STATUS_ALL_OK;
			}
			if (silentMode) return statusCode;
			Logger.Flush();

			//BIG TODO: We loose a lot of time with our libraries.
			//Soooo.... I think we should introduce binaries
			//and not always compute the libraries again and again.
			//Thats probably going to be big, hard and long. hehe.
			foreach (var time in result.Timewatch.GetEntries())
			{
				if (time.Value <= 0)
					continue;
				Console.WriteLine($"It took {time.Value,4}ms to {time.Key}.");
			}
			Console.WriteLine($"It took {result.Timewatch.GetTotal()}ms complete.");
			return statusCode;
		}
	}
}
