using Slides.Filters;

namespace Slides
{
	public class Presentation
	{
		public Slide[] Slides { get; }
		public Style[] Styles { get; }
		public CustomFilter[] CustomFilter { get; }
		public Transition[] Transitions { get; }
		public Library[] Libraries { get; }
		public string[] Imports { get; }
		public string[] ReferencedFiles { get; }

		public Presentation(Slide[] slides, Style[] styles, CustomFilter[] customFilter, Transition[] transitions, Library[] libraries, string[] imports, string[] referencedFiles)
		{
			Slides = slides;
			Styles = styles;
			CustomFilter = customFilter;
			Transitions = transitions;
			Libraries = libraries;
			Imports = imports;
			ReferencedFiles = referencedFiles;
		}
	}

	public class Library
	{
		public string Name { get; }
		public Library[] Libraries { get; }
		public Style[] Styles { get; }
		public static Library Seperator => new Library("seperator", new Library[0], new Style[0]);

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
