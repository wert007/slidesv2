using System;

namespace Slides
{
	[Serializable]
	public class ImageSource
	{
		public ImageSource(string path)
		{
			Path = path;
		}

		public string Path { get; }
		public float width { get; set; }
		public float height { get; set; }

	}
}
