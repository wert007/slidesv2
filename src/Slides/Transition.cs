using Slides.Data;
using Slides.Elements;
using Slides.Styling;
using System;
using System.Collections.Generic;

namespace Slides
{
	/// <summary>
	/// Hull for the actual Slide. Will be replaced by Slide in the future!
	/// </summary>
	public class TransitionSlide
	{
		private TransitionCall _lastTransitionCall;
		private static readonly Time _instant = new Time(1, Time.TimeUnit.Milliseconds);

		public void hide(Time delay)
		{
			_lastTransitionCall = new TransitionCall("hide", _instant, delay);
		}

		public void fadeIn(Time delay, Time duration)
		{
			_lastTransitionCall = new TransitionCall("fadeIn", duration, delay);
		}

		public TransitionCall CreateTransitionCall()
		{
			return _lastTransitionCall;
		}
	}
	public class Transition
	{
		public string name { get; }
		public Brush background { get; set; }
		public Color color { get; set; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Thickness padding { get; set; }
		public Time duration { get; set; }
		public TransitionCall from { get; }
		public TransitionCall to { get; }
		private Element[] _children;

		//TODO: Why is this here? it is not used and the StdStyle is strange. shouldn't it be Customstyle?
//		private HashSet<StdStyle> appliedStyles = new HashSet<StdStyle>();

		public Transition(string name, TransitionCall from, TransitionCall to, Element[] children)
		{
			this.name = name;
			this.from = from;
			this.to = to;
			_children = children;
		}

		public Element[] get_Children() => _children;
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
