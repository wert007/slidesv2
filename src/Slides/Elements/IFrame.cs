namespace Slides.Elements
{
	public class IFrame : Element
	{
		public IFrame(string src)
			: this(src, null, null, null, null)
		{ }
		public IFrame(string src, Unit width, Unit height)
			: this(src, null, null, width, height)
		{ }
		public IFrame(string src, string allow, string extra)
			: this(src, allow, extra, null, null)
		{ }

		public IFrame(string src, string allow, string extra, Unit width, Unit height)
		{
			this.src = src;
			this.allow = allow;
			Extra = extra;
			initWidth = width;
			initHeight = height;
		}

		public override ElementKind kind => ElementKind.IFrame;

		public string src { get; }
		public string allow { get; }
		public string Extra { get; }

		protected override Unit get_InitialHeight() => initHeight;

		protected override Unit get_InitialWidth() => initWidth;
	}
}
