using System;

namespace Slides
{
	[Serializable]
	public class ImageSource
	{
		public ImageSource(string path)
		{
			h_Path = path;
		}

		public string h_Path { get; }
		public int width { get; set; }
		public int height { get; set; }
		public float aspectRatio => (float)width / height;

	}
}
