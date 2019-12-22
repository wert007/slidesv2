using System.Runtime.Serialization;

namespace Github
{
	[DataContract]
	public class Repository
	{
		[DataMember(Name = "id")]
		public long Id { get; internal set; }

		[DataMember(Name = "node_id")]
		public string NodeId { get; internal set; }

		[DataMember(Name = "name")]
		public string Name { get; internal set; }

		[DataMember(Name = "full_name")]
		public string FullName { get; internal set; }

		[DataMember(Name = "private")]
		public bool Private { get; internal set; }

		[DataMember(Name = "owner")]
		public User Owner { get; internal set; }

		[DataMember(Name = "description")]
		public string Description { get; internal set; }

		[DataMember(Name = "fork")]
		public bool Fork { get; internal set; }

		[DataMember(Name = "language")]
		public string Language { get; internal set; }

		public File[] Contents { get; internal set; }
	}
}
