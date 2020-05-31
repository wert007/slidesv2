using Slides.Code;
using System;

namespace Slides
{
	public struct PresentationFlags
	{
		public bool IsLibrarySymbol { get; private set; }
		public string LibraryName { get; private set; }
		public bool StyleAllowed { get; set; }
		public bool AnimationsAllowed { get; set; }
		public bool GroupsAllowed { get; set; }
		public bool StructsAllowed { get; set; }
		public bool TemplatesAllowed { get; set; }
		public CodeHighlighter CodeHighlighter { get; set; }
		public bool UsesYouTube { get; set; }

		public void IsLibrary(string name)
		{
			IsLibrarySymbol = true;
			LibraryName = name;
		}

		public void SetPresentation()
		{
			StyleAllowed = true;
			AnimationsAllowed = true;
			GroupsAllowed = true;
			StructsAllowed = true;
			TemplatesAllowed = true;
		}
	}
}