using Slides.Code;

namespace Minsk.CodeAnalysis
{
	public struct PresentationFlags
	{
		public bool IsLibrarySymbol { get; set; }
		public bool StyleAllowed { get; set; }
		public bool AnimationsAllowed { get; set; }
		public bool GroupsAllowed { get; set; }
		public bool DatasAllowed { get; set; }
		public bool TemplatesAllowed { get; set; }
		public CodeHighlighter CodeHighlighter { get; set; }
	}
}