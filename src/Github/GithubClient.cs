using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Github
{
	public static class GithubClient
	{
		private static File[] GetFiles(string owner, string name)
		{
			File[] result;

			var client = new WebClient();
			client.Headers.Add("User-Agent", "Nothing");

			var url = $"https://api.github.com/repos/{owner}/{name}/contents";
			var content = client.DownloadString(url);

			var serializer = new DataContractJsonSerializer(typeof(File[]));
			using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(content)))
			{
				result = (File[])serializer.ReadObject(ms);
			}

			return result;
		}
		public static Repository GetRepository(string owner, string name)
		{
			Repository result = null;

			try
			{
				var client = new WebClient();
				client.Headers.Add("User-Agent", "Nothing");

				var url = $"https://api.github.com/repos/{owner}/{name}";
				var content = client.DownloadString(url);

				var serializer = new DataContractJsonSerializer(typeof(Repository));
				using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(content)))
				{
					result = (Repository)serializer.ReadObject(ms);
				}
				result.Contents = GetFiles(owner, name);
			}
			catch { }
			if(result == null)
			{
				result = new Repository();
			}
			return result;
		}
	}
}
