using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Debug
{
	[Flags]
	public enum LogLevel
	{
		UnusedGroupField = 1,
		EmptyStyle = 2,
		UnmatchedStyleFunction = 4,
		UnexpectedBoundNodeKind = 8,
		AlreadyRegisteredTypeSymbol = 16,
		UnmatchedCSSField = 32,
		UnknownFilter = 64,
		All = 127,
	}
	public class LogMessage
	{
		public LogMessage(string message, LogLevel level)
		{
			Message = message;
			Level = level;
		}

		public string Message { get; set; }
		public LogLevel Level { get; set; }

		public override string ToString()
		{
			return $"{Level}:: {Message}";
		}
	}
    public static class Logger
    {
		private static HashSet<LogMessage> _messages = new HashSet<LogMessage>();
		
		private static void Log(string message, LogLevel level)
		{
			_messages.Add(new LogMessage(message, level));
		}

		public static void Flush(LogLevel level, ConsoleColor warningColor = ConsoleColor.Yellow)
		{
			Console.ForegroundColor = warningColor;
			foreach (var message in _messages)
			{
				if (level.HasFlag(message.Level))
					Console.WriteLine($"WARNING: {message.Message}");
			}
			Console.ResetColor();
		}

		public static void LogUnmatchedCSSField(string field)
		{
			var message = $"Could not match field '{field}' to a css attribute.";
			Log(message, LogLevel.UnmatchedCSSField);
		}

		public static void LogAlreadyRegisteredTypeSymbol(string name, string newType, string registeredType)
		{
			var message = "";
			if (newType == registeredType)
				message = $"TypeSymbol '{name}' of Type '{newType}' already registered.";
			else
				message = $"TypeSymbol '{name}' of Type '{newType}' already registered as '{registeredType}'.";
			Log(message, LogLevel.AlreadyRegisteredTypeSymbol);
		}

		public static void LogEmptyStyle(string name)
		{
			var message = $"Style '{name}' is empty.";
			Log(message, LogLevel.EmptyStyle);
		}


		public static void LogUnusedVariableInGroup(string variable, Type type, string groupName)
		{
			var typePostfix = "";
			if (type != null)
				typePostfix = $" : {type}";
			var message = $"Unused variable {variable}{typePostfix} in group {groupName}.";
			Log(message, LogLevel.UnusedGroupField);
		}

		public static void LogUnmatchedStyleFunction(string name, string style)
		{
			var message = $"Couldn't match function '{name}' to anything in CollectStyleFields for style '{style}'.";
			Log(message, LogLevel.UnmatchedStyleFunction);
		}

		public static void LogCannotTestImageFunction(string kind)
		{
			var message = $"Can't test image function! ParameterExpression is of kind '{kind}'.";
			Log(message, LogLevel.UnexpectedBoundNodeKind);
		}

		public static void LogUnknownFilter(Type type, string name)
		{
			var message = $"Unknown filter '{type}' found. Could not match name '{name}'.";
			Log(message, LogLevel.UnknownFilter);
		}

		public static void LogUnmatchedIfConditionVariableExtractment()
		{
			throw new NotImplementedException();
		}

		public static void LogUnmatchedIfConditionVariableExtractment(string v)
		{
			throw new NotImplementedException();
		}
	}
}
