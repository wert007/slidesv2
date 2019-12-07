using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLWriter
{
	public class HTMLWriter : IDisposable
	{
		private IndentedTextWriter _writer;
		private Stack<string> _openedTags;
		private Stack<KeyValuePair<string, string>> _attributes;

		public HTMLWriter(Stream stream)
		{
			_writer = new IndentedTextWriter(new StreamWriter(stream));
			_openedTags = new Stack<string>();
			_attributes = new Stack<KeyValuePair<string, string>>();
		}

		public void Dispose()
		{
			End();
			_writer.Flush();
			_writer.Dispose();
		}

		public void Start()
		{
			//<!DOCTYPE html>
			_writer.WriteLine("<!DOCTYPE html>");
			StartTag("html");
		}

		public void End()
		{
			while (_openedTags.Any())
				EndTag();
		}

		public void StartHead()
		{
			StartTag("head");
		}

		public void StartBody()
		{
			EndTag();
			StartTag("body");
		}

		public void UseCSS(string file)
		{
			//<link rel="stylesheet" type="text/css" href="theme.css">
			PushAttribute("rel", "stylesheet");
			PushAttribute("type", "text/css");
			PushAttribute("href", file);
			WriteTag("link", needsEnding: false);
		}

		public void UseJS(string file)
		{
			PushAttribute("type", "text/javascript");
			PushAttribute("src", file);
			WriteInlineTag("script", "");
		}

		public void PushAttribute(string name, string value)
		{
			_attributes.Push(new KeyValuePair<string, string>(name, value));
		}

		public void WriteTag(string name, string id = null, string classes = null, bool needsEnding = true, bool useNewLine = true)
		{
			_writer.Write($"<{name}");
			if(id != null)
				_writer.Write($" id=\"{id}\"");
			if (classes != null)
				_writer.Write($" class=\"{classes}\"");
			while(_attributes.Count != 0)
			{
				var attribute = _attributes.Pop();
				_writer.Write($" {attribute.Key}=\"{attribute.Value}\"");
			}
			if (needsEnding)
				_writer.Write("/");
			if (useNewLine)
				_writer.WriteLine(">");
			else
				_writer.Write(">");
		}

		public void StartTag(string name, string id = null, string classes = null, bool useNewLine = true)
		{
			WriteTag(name, id, classes, needsEnding: false, useNewLine: useNewLine);
			_writer.Indent++;
			_openedTags.Push(name);
		}

		public void WriteInlineTag(string name, string content, string id = null, string classes = null, bool useNewLine = false)
		{
			WriteTag(name, id, classes, needsEnding: false, useNewLine: useNewLine);
			_writer.Write(content);
			_writer.Indent++;
			_openedTags.Push(name);
			EndTag();
		}

		public void EndTag(bool useNewLine = true)
		{
			_writer.Indent--;
			var tag = _openedTags.Pop();
			_writer.Write($"</{tag}>");
			if (useNewLine)
				_writer.WriteLine();
			_writer.Flush();
		}

		public void Write(string text)
		{
			_writer.Write(text);
		}

		public void WriteComment(string text)
		{
			_writer.WriteLine($"<!--{text}-->");
		}
	}
}
