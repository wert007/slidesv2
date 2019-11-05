using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Slides;

namespace HTMLWriter
{
	public class JavaScriptWriter : IDisposable
	{
		private IndentedTextWriter _writer;

		private Stack<string> _startedFunctions;

		public int Indent => _writer.Indent;

		public JavaScriptWriter(FileStream stream)
		{
			_writer = new IndentedTextWriter(new StreamWriter(stream));
			_startedFunctions = new Stack<string>();
		}

		public void Dispose()
		{
			_writer.Flush();
			_writer.Dispose();
		}

		public void StartFunction(string name, params string[] parameters)
		{
			_writer.WriteLine($"function {name}({string.Join(", ", parameters)}){{");
			_startedFunctions.Push(name);
			_writer.Indent++;
		}

		public void EndFunction()
		{
			_startedFunctions.Pop();
			End();
		}

		private void End()
		{
			_writer.Indent--;
			_writer.WriteLine("}");
			_writer.WriteLine();
		}

		public void WriteVariableDeclaration(string name, string initializer, bool isConst = false)
		{
			if (isConst)
				_writer.Write("const ");
			else
				_writer.Write("let ");
			_writer.WriteLine($"{name} = {initializer};");
		}

		public void WriteValue(object value)
		{
			switch (value)
			{
				default:
					_writer.Write(value.ToString());
					break;
			}
		}

		public void StartForLoop(string variable, string maxValue, string addition = "++")
		{
			_writer.WriteLine($"for(let {variable} = 0; {variable} < {maxValue}; {variable}{addition}) {{");
			_writer.Indent++;
		}

		public void StartIfStatement(string condition)
		{
			_writer.WriteLine($"if({condition}) {{");
			_writer.Indent++;
		}

		public void WriteAssignment(string name, string value)
		{
			_writer.WriteLine($"{name} = {value};");
		}

		public void WriteReturnStatement(string value)
		{
			if (value == null)
				_writer.WriteLine("return;");
			else
				_writer.WriteLine($"return {value};");
		}

		public void EndIf()
		{
			End();
		}

		public void EndFor()
		{
			End();
		}
	}
}