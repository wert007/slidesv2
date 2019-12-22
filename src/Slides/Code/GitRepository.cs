using Github;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.Code
{
	public class GitRepository
	{
		public string owner { get; set; }
		public string name { get; set; }
		public string language { get; set; }
		private File[] gitFiles { get; set; }

		public GitRepository(string owner, string name, File[] gitFiles, string language)
		{
			this.owner = owner;
			this.name = name;
			this.gitFiles = gitFiles;
			this.language = language;
		}

		public GitFile file(string name)
		{
			foreach (var file in gitFiles)
			{
				if (file.Name == name)
				{
					return new GitFile(file.Name, language, file.RequestContent());
				}
			}
			return null;
		}
	}

	public class GitFile
	{
		public string name { get; set; }
		public string language { get; set; }
		public string content { get; set; }

		public GitFile(string name, string language, string content)
		{
			this.name = name;
			this.language = language;
			this.content = content;
		}
	}
}
