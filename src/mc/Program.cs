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
		private static void Main(string[] args)
		{
			bool completeRebuild = false;
			bool offlineView = false;
			foreach (var arg in args)
			{
				Console.WriteLine($">> arg '{arg}' received");
			}
			if(args.Length >= 2)
			{
				if (args.Contains("-build-fresh"))
					completeRebuild = true;
				if (args.Contains("-offline"))
					offlineView = true;
				LoadFromFile(args[0], args[1], completeRebuild, offlineView);
			}
			else
			{
				Console.WriteLine("Usage: 'slides <SlidesFile> <targetDirectory>");
			}
		}


		private static void LoadFromFile(string path, string targetDirectory, bool completeRebuild, bool offlineView)
		{
			var fileName = Path.GetFileName(path);
			var directory = Path.GetDirectoryName(path);
			CompilationFlags.Init(directory, fileName, completeRebuild);
			var result = Loader.LoadFromFile(directory, fileName, false, false, completeRebuild, offlineView);
			if(!result.Diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
			{
				var presentation = result.Value as Presentation;
				PresentationWriter.Write(presentation, targetDirectory, true);
			}

			Logger.Flush();

			//BIG TODO: We loose a lot of time with our libraries.
			//Soooo.... I think we should introduce binaries
			//and not always compute the libraries again and again.
			//Thats probably going to be big, hard and long. hehe.
			foreach (var time in result.Timewatch.GetEntries())
			{
				if (time.Value <= 0)
					continue;
				Console.WriteLine($"It took {time.Value.ToString().PadLeft(4)}ms to {time.Key}.");
			}
			Console.WriteLine($"It took {result.Timewatch.GetTotal()}ms complete.");
			//Console.WriteLine("");
			//Console.WriteLine("Press enter to exit.");
			//Console.ReadLine();
		}
	}
}
