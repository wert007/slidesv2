using System;
using System.Collections.Generic;

namespace Slides
{
	public class Transition
	{
		public string name { get; }
		public Brush background { get; set; }
		public Color color { get; set; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Thickness padding { get; set; }
		public Time duration { get; set; }
		private HashSet<StdStyle> appliedStyles = new HashSet<StdStyle>();
		public TransitionCall from { get; }
		public TransitionCall to { get; }

		public Transition(string name, TransitionCall from, TransitionCall to)
		{
			this.name = name;
			this.from = from;
			this.to = to;
		}
	}

	public class TransitionCall
	{
		public TransitionCall(string name, Time duration, Time delay)
		{
			Name = name;
			Duration = duration;
			Delay = delay;
		}

		public string Name { get; }
		public Time Duration { get; }
		public Time Delay { get; }
	}
}
