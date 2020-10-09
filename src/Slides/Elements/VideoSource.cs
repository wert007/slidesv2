namespace Slides.Elements
{
	public class VideoSource
	{
		public int Height { get; set; }
		public int Width { get; set; }
		public string h_Path { get; }

		public VideoSource(string path)
		{
			h_Path = path;
		}
	}
}
