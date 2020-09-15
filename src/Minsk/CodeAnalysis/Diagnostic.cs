using Minsk.CodeAnalysis.Text;
using System;

namespace Minsk.CodeAnalysis
{
	public enum DiagnosticLevel
	{
		Error,
		Warning,
	}

	public sealed class Diagnostic
	{
		public Diagnostic(TextSpan span, string message, string file, DiagnosticLevel level)
		{
			Span = span;
			Message = message;
			File = file;
			Level = level;
		}

		public TextSpan Span { get; }
		public string Message { get; }
		public string File { get; }
		public DiagnosticLevel Level { get; }

		public override string ToString() => Message;

		public void OutputToConsole(SourceText text)
		{
			var color = Level == DiagnosticLevel.Error ? ConsoleColor.DarkRed : ConsoleColor.DarkYellow;

			var lineIndex = text.GetLineIndex(Span.Start);
			var line = text.Lines[lineIndex];
			var lineNumber = lineIndex + 1;
			var character = Span.Start - line.Start + 1;

			Console.Error.WriteLine();

			Console.ForegroundColor = color;
			Console.Error.Write($"{File} ({lineNumber}, {character}): ");
			Console.Error.WriteLine(Message);
			Console.ResetColor();

			var prefixSpan = TextSpan.FromBounds(line.Start, Span.Start);
			var suffixSpan = TextSpan.FromBounds(Span.End, Math.Max(line.End, Span.End));

			var prefix = text.ToString(prefixSpan);
			var error = text.ToString(Span);
			var suffix = text.ToString(suffixSpan);

			Console.Error.Write("    ");
			Console.Error.Write(prefix);

			Console.ForegroundColor = color;
			Console.Error.Write(error);
			Console.ResetColor();

			Console.Error.Write(suffix);

			Console.Error.WriteLine();
		}
	}
}