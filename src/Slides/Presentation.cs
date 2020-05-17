﻿using Slides.Code;
using Slides.Filters;
using System;
using System.Collections.Generic;

namespace Slides
{
	public enum JSInsertionKind
	{
		Time,
		Slider,
		None
	}
	public class JSInsertionBlock
	{
		public JSInsertionBlock(string functionName, string body, Dictionary<string, string> variables, JSInsertionKind kind)
		{
			FunctionName = functionName;
			Body = body;
			Variables = variables;
			Kind = kind;
		}

		public string FunctionName { get; }
		public string Body { get; }
		public Dictionary<string, string> Variables { get; }
		public JSInsertionKind Kind { get; }
	}
	public class Presentation
	{
		public Slide[] Slides { get; }
		public Style[] Styles { get; }
		public CustomFilter[] CustomFilter { get; }
		public Transition[] Transitions { get; }
		public Library[] Libraries { get; }
		public JSInsertionBlock[] JSInsertions { get; }
		public string[] Imports { get; }
		public string[] ReferencedFiles { get; }
		public CodeHighlighter CodeHighlighter { get; }

		public Presentation(Slide[] slides, Style[] styles, CustomFilter[] customFilter, Transition[] transitions, Library[] libraries, JSInsertionBlock[] jsInsertions, string[] imports, string[] referencedFiles, CodeHighlighter codeHighlighter)
		{
			Slides = slides;
			Styles = styles;
			CustomFilter = customFilter;
			Transitions = transitions;
			Libraries = libraries;
			JSInsertions = jsInsertions;
			Imports = imports;
			ReferencedFiles = referencedFiles;
			CodeHighlighter = codeHighlighter;
		}
	}

	//Why do we need this?!
	//I think because it can be a value sometimes..
	[Serializable]
	public class Library
	{
		public string Name { get; }
		public Library[] Libraries { get; }
		public Style[] Styles { get; }
		public static Library Seperator => new Library("seperator", new Library[0], new Style[0]);
		public static Library Code => new Library("code", new Library[0], new Style[0]);

		public Library(string name, Library[] libraries, Style[] styles)
		{
			Name = name;
			Libraries = libraries;
			Styles = styles;
		}

		public Library(string name)
		{
			Name = name;
		}

		public override string ToString() => Name;

	}
}
