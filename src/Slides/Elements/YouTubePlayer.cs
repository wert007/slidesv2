using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Elements
{
	public enum YouTubeQuality
	{
		Default,
		Small,
		Medium,
		Large,
		HD720,
		HD1080,
		Highres,
	}

	public class YouTubePlayerParameters
	{
		public enum ShowControls
		{
			Hidden,
			OldDefault,
			Default,
		}
		public enum ListType
		{
			Playlist,
			Search,
			UserUploads,
		}
		public bool autoplay { get; set; } = false;
		//Only 'red' and 'white' allowed
		public string color { get; set; } = "red";
		public ShowControls controls { get; set; } = ShowControls.OldDefault;
		public bool disablekb { get; set; } = false;
		public bool enablejsapi { get; set; } = false;
		public int? end { get; set; } = null;
		public bool fs { get; set; } = true;
		public string hl { get; set; } = null;
		//true == 1 and false == 3
		public bool iv_load_policy { get; set; } = true;
		public string list { get; set; } = null;
		public ListType? listType { get; set; } = null;
		public bool loop { get; set; } = false;
		public bool modestbranding { get; set; } = false;
		public string origin { get; set; } = null;
		public string[] playlist { get; set; } = null;
		public bool playsinline { get; set; } = false;
		public bool rel { get; set; } = true;
		public bool showinfo { get; set; } = true;
		public int? start { get; set; } = null;


		public YouTubePlayerParameters()
		{
			controls = ShowControls.Default;
			enablejsapi = true;
		}
	}

	public class YouTubePlayer : Element
	{

		public YouTubePlayer(string videoId, YouTubeQuality quality, YouTubePlayerParameters parameters = null)
		{
			this.videoId = videoId;
			this.quality = quality;
			this.parameters = parameters ?? new YouTubePlayerParameters();
		}


		public override ElementKind kind => ElementKind.YouTubePlayer;

		public bool isMuted { get; set; }
		public string videoId { get; }
		public YouTubeQuality quality { get; }
		public YouTubePlayerParameters parameters { get; }

		public static Vector2 GetDefaultPlayerSize(YouTubeQuality quality)
		{
			switch (quality)
			{
				case YouTubeQuality.Small:
					//TODO: this is 4:3 we actually want 16:9
					return new Vector2(320, 240);
				case YouTubeQuality.Medium: 
					return new Vector2(640, 360);
				case YouTubeQuality.Large:
					return new Vector2(853, 480);
				case YouTubeQuality.HD720:
					return new Vector2(1280, 720);
				case YouTubeQuality.HD1080:
					return new Vector2(1920, 1080);
				//Cannot determine a concrete size, so we return (0, 0) as default
				case YouTubeQuality.Highres:
				case YouTubeQuality.Default:
					return new Vector2(0, 0);
				default:
					throw new NotImplementedException();
			}
		}

		protected override Unit get_InitialHeight()
		{
			return new Unit(GetDefaultPlayerSize(quality).Y, Unit.UnitKind.Pixel);
		}

		protected override Unit get_InitialWidth()
		{
			return new Unit(GetDefaultPlayerSize(quality).X, Unit.UnitKind.Pixel);
		}
	}
}
