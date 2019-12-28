using System;

namespace Slides
{
	[Serializable]
	public class ImageSource
	{
		public ImageSource(string path)
		{
			Path = path;
			isSvg = path.EndsWith(".svg");
		}
		public ImageSource(string path, bool isSvg)
		{
			Path = path;
			this.isSvg = isSvg;
		}
		public bool isSvg { get; }
		public string Path { get; }
		public float width { get; set; }
		public float height { get; set; }
		public float aspectRatio => width / height;

	}
}
