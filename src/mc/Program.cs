using HTMLWriter;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Syntax;
using Minsk.CodeAnalysis.Text;
using Slides;
using Slides.Debug;
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
			foreach (var arg in args)
			{
				Console.WriteLine($">> arg '{arg}' received");
			}
			if(args.Length >= 2)
			{
				if (args.Contains("-build-fresh"))
					completeRebuild = true;
				LoadFromFile(args[0], args[1], completeRebuild);
			}
			else
			{
				Console.WriteLine("Usage: 'slides <SlidesFile> <targetDirectory>");
			}
		}


		private static void LoadFromFile(string fileName, string targetDirectory, bool completeRebuild)
		{
			var result = Loader.LoadFromFile(fileName, false, false, completeRebuild);
			if(!result.Diagnostics.Any(d => d.Level == DiagnosticLevel.Error))
			{
				var presentation = result.Value as Presentation;
				PresentationWriter.Write(presentation, targetDirectory, true);
			}

			var level = LogLevel.All;
			Logger.Flush(level);

			//BIG TODO: We loose a lot of time with our libraries.
			//Soooo.... I think we should introduce binaries
			//and not always compute the libraries again and again.
			//Thats probably going to be big, hard and long. hehe.
			foreach (var time in result.Timewatch.GetEntries())
			{
				if (time.Value <= 0)
					continue;
				Console.WriteLine($"It took {time.Value.ToString().PadLeft(4, ' ')}ms to {time.Key}.");
			}
			Console.WriteLine($"It took {result.Timewatch.GetTotal()}ms complete.");
			Console.WriteLine("");
			Console.WriteLine("Press enter to exit.");
			Console.ReadLine();
		}
	}
}
