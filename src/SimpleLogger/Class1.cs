using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SimpleLogger
{
	public static class Logger
	{
		private struct LogMessage
		{
			public LogMessage(string message, string stackTrace)
			{
				Message = message;
				StackTrace = stackTrace;
			}

			public string Message { get; }
			public string StackTrace { get; }

			
		}
		private static HashSet<LogMessage> _messages = new HashSet<LogMessage>();

		private static void Store(string stacktrace, string message)
		{
			_messages.Add(new LogMessage(message, stacktrace));
		}

		public static void Flush(bool showStackTrace = true, ConsoleColor warningColor = ConsoleColor.Yellow)
		{
			foreach (var message in _messages)
			{
				Console.ForegroundColor = warningColor;
				Console.WriteLine($"WARNING: {message.Message}");
				Console.ResetColor();
				if (showStackTrace)
				{
					Console.WriteLine(message.StackTrace);
					Console.WriteLine();
				}
			}
		}

		public static void Log(string message)
		{
			var trace = new StackTrace(true);
			var stackTraceBuilder = new StringBuilder();
			foreach (var frame in trace.GetFrames().Skip(1))
			{
				stackTraceBuilder.AppendLine($"{frame.GetFileLineNumber()}:{frame.GetMethod()}");
			}
			Store(stackTraceBuilder.ToString(), message);
		}
	}
}
