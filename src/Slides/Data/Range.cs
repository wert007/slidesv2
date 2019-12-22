using System;

namespace Slides
{
	[Serializable]
	public class Range
	{
		public int From { get; set; }
		public int To { get; set; }
		public int Step { get; set; }

		public Range(int from, int to, int step)
		{
			From = from;
			To = to;
			Step = step;
		}

		public override string ToString()
		{
			if(Step == 1 || Step == -1)
				return $"{From}..{To}";
			return $"{From}..{To} by {Step}";
		}
	}
}
