using Slides.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides
{
	public class SlideAttributes
	{
		public string name { get; }
		public Brush background { get; set; }
		public Color color { get; set; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Thickness padding { get; set; }
		public Transition transition { get; set; }
		public Filter filter { get; set; }
		private HashSet<Style> appliedStyles = new HashSet<Style>();

		public SlideAttributes(string name)
		{
			this.name = name;
		}

		public void hide(Time delay)
		{
			throw new NotImplementedException();
		}

		public void fadeIn(Time delay, Time duration)
		{
			throw new NotImplementedException();
		}

		public void applyStyle(Style style)
		{
			appliedStyles.Add(style);
		}

		public Style[] get_AppliedStyles()
		{
			return appliedStyles.ToArray();
		}
	}
}
