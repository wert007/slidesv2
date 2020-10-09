using Slides.Data;
using System.Linq;

namespace Slides.Elements
{
	public class Video : Element
	{
		public Video(VideoSource source)
		{
			sources = new[] { source };
		}

		public Video(VideoSource[] sources)
		{
			this.sources = sources;
		}

		public override ElementKind kind => ElementKind.Video;

		public VideoSource[] sources { get; }
		public Stretching stretching { get; set; }
		public bool? h_Autoplay { get; set; }
		public bool autoplay { get => h_Autoplay ?? (bool?)get_ActualValue(nameof(autoplay)) ?? (bool?)StdStyle.GetValue("autoplay", "video") ?? false; set
			{
				muted = muted || value;
				h_Autoplay = value;
			}
			}
		public bool keepPlaying { get; set; }
		public bool soundOnly { get; set; }
		public bool controls { get; set; }
		public bool? h_Muted { get; set; }
		public bool muted { get => h_Muted ?? autoplay; set => h_Muted = value; }


		internal override Unit get_InitialHeight()
		{
			return new Unit(sources.First().Height, Unit.UnitKind.Pixel);
		}

		internal override Unit get_InitialWidth()
		{
			return new Unit(sources.First().Width, Unit.UnitKind.Pixel);
		}
	}
}
