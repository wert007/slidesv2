using System.Runtime.Serialization;

namespace Github
{
	[DataContract]
	public class User
	{
		[DataMember(Name = "login")]
		public string Login { get; internal set; }

		[DataMember(Name = "id")]
		public long Id { get; internal set; }

		[DataMember(Name = "node_id")]
		public string NodeId { get; internal set; }
		//... TODO
	}
}
