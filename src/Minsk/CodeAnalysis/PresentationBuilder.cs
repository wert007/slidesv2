using System;
using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Elements;
using Slides.Styling;

namespace Minsk.CodeAnalysis
{
	internal class PresentationBuilder
	{
		private readonly LibrarySymbol[] _referenced;
		private readonly HashSet<string> _referencedFiles = new HashSet<string>();
		private readonly List<Transition> _transitions = new List<Transition>();
		private readonly List<CustomFilter> _filters = new List<CustomFilter>();
		//TODO: Could probably be a list!
		private readonly Dictionary<VariableSymbol, Slide> _slides = new Dictionary<VariableSymbol, Slide>();
		private readonly Dictionary<TypeSymbol, BodySymbol> _customTypes = new Dictionary<TypeSymbol, BodySymbol>();
		private readonly Dictionary<VariableSymbol, Style> _styles = new Dictionary<VariableSymbol, Style>();
		//TODO: Could probably be a list!
		private readonly Dictionary<Element, AnimationCall> _animations = new Dictionary<Element, AnimationCall>();
		private readonly Dictionary<VariableSymbol, BoundTemplateStatement> _templates = new Dictionary<VariableSymbol, BoundTemplateStatement>();
		private readonly List<string> _imports = new List<string>();
		private readonly List<JSInsertionBlock> _jsInsertions = new List<JSInsertionBlock>();
		private int _invisibleSlideCount = 0;
		private bool _useDarktheme;

		public PresentationBuilder(LibrarySymbol[] referenced)
		{
			_referenced = referenced;
			_useDarktheme = false;
		}

		internal LibrarySymbol[] GetReferenced() => _referenced;

		internal void AddReferencedFile(string fileName) => _referencedFiles.Add(fileName);
		internal string[] GetReferencedFiles() => _referencedFiles.ToArray();

		internal void AddTransition(Transition result) => _transitions.Add(result);
		internal Transition[] GetTransitions() => _transitions.ToArray();

		internal void AddFilter(CustomFilter result) => _filters.Add(result);
		internal CustomFilter[] GetFilters() => _filters.ToArray();

		internal void AddSlide(VariableSymbol variable, Slide slide)
		{
			if (!variable.IsVisible)
				_invisibleSlideCount++;
			_slides.Add(variable, slide);
		}
		internal int GetSlideIndex() => _slides.Count - _invisibleSlideCount;
		internal Slide[] GetSlides() => _slides.Values.ToArray();

		internal void AddCustomType(BodySymbol data) => _customTypes.Add(data.Symbol.Type, data);
		internal bool TryGetCustomType(TypeSymbol type, out BodySymbol group) => _customTypes.TryGetValue(type, out group);
		internal BodySymbol[] GetCustomTypes() => _customTypes.Values.ToArray();

		internal void AddStyle(VariableSymbol variable, Style style) => _styles.Add(variable, style);
		internal bool HasStyle(VariableSymbol variable) => _styles.ContainsKey(variable);
		internal Style GetStyle(VariableSymbol variable) => _styles[variable];
		internal Style[] GetStyles() => _styles.Values.ToArray();

		internal void AddAnimation(Element element, AnimationCall result) => _animations.Add(element, result);
		internal AnimationCall[] TakeAnimations()
		{
			var result = _animations.Values.ToArray();
			_animations.Clear();
			return result;
		}

		internal void AddTemplate(VariableSymbol variable, BoundTemplateStatement node) => _templates.Add(variable, node);
		internal BoundTemplateStatement GetTemplate(VariableSymbol variable) => _templates[variable];
		
		internal void AddImport(string import) => _imports.Add(import);
		internal void AddImportRange(string[] imports) => _imports.AddRange(imports);
		internal string[] GetImports() => _imports.ToArray();

		internal void AddJSInsertion(JSInsertionBlock insertion) => _jsInsertions.Add(insertion);
		internal JSInsertionBlock[] GetJSInsertions() => _jsInsertions.ToArray();

		internal void SetDarktheme(bool useDarktheme) => _useDarktheme = useDarktheme;

		internal bool GetDarktheme() => _useDarktheme;
	}
}