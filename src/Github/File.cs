using System.Net;
using System.Runtime.Serialization;

namespace Github
{
	[DataContract]
	public class File
	{
		[DataMember(Name = "name")]
		public string Name { get; internal set; }

		[DataMember(Name = "path")]
		public string Path { get; internal set; }

		[DataMember(Name = "sha")]
		public string SHA { get; internal set; }

		[DataMember(Name = "size")]
		public long Size { get; internal set; }

		[DataMember(Name = "type")]
		public string Type { get; internal set; }

		//TODO
		[DataMember(Name = "download_url")]
		public string ContentURL { get; internal set; }

		public string RequestContent()
		{
			var client = new WebClient();
			client.Headers.Add("User-Agent", "Nothing");

			return client.DownloadString(ContentURL);
		}
	}
}
