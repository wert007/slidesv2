using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Slides;
using Slides.Helpers;

namespace HTMLWriter
{
	public class JavaScriptWriter : IDisposable
	{
		private IndentedTextWriter _writer;
		private IndentedTextWriter _onLoadWriter;

		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");

		private IndentedTextWriter CurrentWriter
		{
			get
			{
				if (writeInOnLoad)
					return _onLoadWriter;
				return _writer;
			}
		}

		private bool writeInOnLoad = false;

		internal void WriteFunctionCall(string name, params object[] args)
		{
			CurrentWriter.Write($"{name}(");
			var isFirst = true;
			foreach (var arg in args)
			{
				if (!isFirst)
					CurrentWriter.Write(", ");
				isFirst = false;
				WriteValue(arg);
			}
			CurrentWriter.WriteLine(");");
		}

		private Stack<string> _startedFunctions;

		public int Indent => _writer.Indent;

		public JavaScriptWriter(FileStream stream)
		{
			_writer = new IndentedTextWriter(new StreamWriter(stream));
			_onLoadWriter = new IndentedTextWriter(new StringWriter());
			_startedFunctions = new Stack<string>();
		}

		internal void Write(string s)
		{
			_writer.Write(s);
		}

		internal void WriteLine(string s)
		{
			_writer.WriteLine(s);
		}

		public void Dispose()
		{
			WriteOnLoad();
			_onLoadWriter.Flush();
			_onLoadWriter.Dispose();
			_writer.Flush();
			_writer.Dispose();
		}

		public void ToggleOnload()
		{
			writeInOnLoad = !writeInOnLoad;
		}

		private void WriteOnLoad()
		{
			writeInOnLoad = false;
			StartFunction("loadInner");
			_writer.WriteLine(_onLoadWriter.InnerWriter.ToString());
			EndFunction();
		}

		public void StartFunction(string name, params string[] parameters)
		{
			CurrentWriter.WriteLine($"function {name}({string.Join(", ", parameters)}){{");
			_startedFunctions.Push(name);
			CurrentWriter.Indent++;
		}

		public void EndFunction()
		{
			_startedFunctions.Pop();
			End();
		}

		private void End()
		{
			CurrentWriter.Indent--;
			CurrentWriter.WriteLine("}");
			CurrentWriter.WriteLine();
		}

		public void WriteVariableDeclarationInline(string name, string initializer, bool isConst = false)
		{
			if (isConst)
				CurrentWriter.Write("const ");
			else
				CurrentWriter.Write("let ");
			CurrentWriter.WriteLine($"{name} = {initializer};");
		}
		public void StartVariableDeclaration(string name, bool isConst = false)
		{
			if (isConst)
				CurrentWriter.Write("const ");
			else
				CurrentWriter.Write("let ");
			CurrentWriter.Write($"{name} = ");
		}

		public void EndVariableDeclaration()
		{
			CurrentWriter.WriteLine(";");
		}

		public void WriteField(string name, object value)
		{
			CurrentWriter.Write($"{name}: ");
			WriteValue(value);
			CurrentWriter.WriteLine(",");
		}
		public void StartField(string name)
		{
			CurrentWriter.WriteLine($"{name}: ");
		}
		public void EndField()
		{
			CurrentWriter.WriteLine(",");
		}

		public void WriteArray(params object[] values)
		{
			WriteArraySingleArgument(values);
		}
		public void WriteArraySingleArgument(object[] values)
		{
			CurrentWriter.Write("[");
			bool isFirst = true;
			foreach (var v in values)
			{
				if (!isFirst)
					CurrentWriter.Write(", ");
				isFirst = false;
				WriteValue(v);
			}
			CurrentWriter.Write("]");
		}
		public void StartArray()
		{
			CurrentWriter.Write("[");
		}

		public void WriteArraySeperator()
		{
			CurrentWriter.Write(", ");
		}
		public void EndArray()
		{
			CurrentWriter.WriteLine("]");
		}

		public void StartObject() => WriteOpenBrace(true);
		public void EndObject() => WriteCloseBrace(false, true);

		public void WriteOpenBrace(bool newLine = true)
		{
			if (newLine)
			{
				CurrentWriter.WriteLine("{");
				CurrentWriter.Indent++;
			}
			else
				CurrentWriter.Write("{");
		}

		public void WriteCloseBrace(bool newLine = false, bool wasMultiLine = true)
		{
			if (wasMultiLine)
				CurrentWriter.Indent--;
			if (newLine)
				CurrentWriter.WriteLine("}");
			else
				CurrentWriter.Write("}");
		}

		public void WriteSemicolon()
		{
			CurrentWriter.WriteLine(";");
		}

		public void WriteValue(object value)
		{
			CurrentWriter.EmitObject(value);
		}

		public void StartForLoop(string variable, Range range)
		{
			CurrentWriter.WriteLine($"for(let {variable} = {range.From}; {variable} < {range.To}; {variable} += {range.Step}) {{");
			CurrentWriter.Indent++;
		}



		internal void StartForLoop(string variable, string from, string to, string inc)
		{
			CurrentWriter.WriteLine($"for(let {variable} = {from}; {variable} < {to}; {variable} += {inc}) {{");
			CurrentWriter.Indent++;
		}

		public void StartIfStatement(string condition)
		{
			CurrentWriter.WriteLine($"if({condition}) {{");
			CurrentWriter.Indent++;
		}


		public void StartSwitch(string switchOn)
		{
			CurrentWriter.WriteLine($"switch({switchOn}) {{");
			CurrentWriter.Indent++;
		}

		public void EndSwitch()
		{
			End();
		}

		public void StartCase(object value)
		{
			CurrentWriter.Write("case ");
			WriteValue(value);
			CurrentWriter.WriteLine(":");
			CurrentWriter.Indent++;
		}

		public void EndCase()
		{
			CurrentWriter.WriteLine("break;");
			CurrentWriter.Indent--;
		}

		public void WriteAssignment(string name, string value)
		{
			CurrentWriter.WriteLine($"{name} = {value};");
		}

		public void WriteReturnStatement(string value)
		{
			if (value == null)
				CurrentWriter.WriteLine("return;");
			else
				CurrentWriter.WriteLine($"return {value};");
		}

		public void EndIf()
		{
			End();
		}

		public void EndFor()
		{
			End();
		}

		public static string ToJSAttribute(string cssAttribute)
		{
			switch (cssAttribute)
			{
				case "font-size":
					return "style.fontSize";
				case "margin":
					return "style.margin";
				case "padding":
					return "style.padding";
				case "color":
					return "style.color";
				case "background":
					return "style.backgroundColor";
				case "fill":
					return "style.fill";
				case "innerHTML":
					return cssAttribute;
				default:
					throw new Exception();
			}
		}
	}
}