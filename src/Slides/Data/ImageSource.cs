namespace Slides
{
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
