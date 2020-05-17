using Slides.Code;

namespace Minsk.CodeAnalysis
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

		public void IsLibrary(string name)
		{
			IsLibrarySymbol = true;
			LibraryName = name;
		}
	}
}