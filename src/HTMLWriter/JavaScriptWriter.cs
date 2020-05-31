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
		private IndentedTextWriter _mainWriter;
		private IndentedTextWriter _currentWriter = null;

		private Dictionary<string, IndentedTextWriter> _functions = new Dictionary<string, IndentedTextWriter>();

		private IndentedTextWriter CurrentWriter => _currentWriter ?? _mainWriter;


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

		public int Indent => _mainWriter.Indent;

		public JavaScriptWriter(FileStream stream)
		{
			_mainWriter = new IndentedTextWriter(new StreamWriter(stream));
			StartFunctionCollector("loadInner");
			_startedFunctions = new Stack<string>();
		}

		internal void Write(string s)
		{
			_mainWriter.Write(s);
		}

		internal void WriteLine(string s)
		{
			_mainWriter.WriteLine(s);
		}

		public void Dispose()
		{
			foreach (var f in _functions)
			{
				var w = f.Value.InnerWriter;
				EndFunctionCollector(f.Key, true);
				_mainWriter.WriteLine(w.ToString());
				w.Flush();
				w.Dispose();
			}
			_mainWriter.Flush();
			_mainWriter.Dispose();
		}

		public void StartFunctionCollector(string name, params string[] parameters)
		{
			var writer = new IndentedTextWriter(new StringWriter());
			writer.WriteLine($"function {name}({string.Join(", ", parameters)}){{");
			writer.Indent++;
			_functions.Add(name, writer);
		}

		public void SwitchInto(string name)
		{
			_currentWriter = _functions[name];
		}

		public void ResetWriter()
		{
			_currentWriter = null;
		}

		public void EndFunctionCollector(string name, bool keepEntry = false)
		{
			var writer = _functions[name];
			writer.Indent--;
			writer.WriteLine("}");
			writer.Flush();
			if (!keepEntry)
			{
				writer.Dispose();
				_functions.Remove(name);
			}
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