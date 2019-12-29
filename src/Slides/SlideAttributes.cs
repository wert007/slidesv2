using Slides.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides
{
	//TODO: They can all be none anyway!
	//You have no guarantee, that they have
	//a value. So do we need this  ???????????
	public class StyleSlideAttributes
	{
		public Brush background { get; set; }
		public Color color { get; set; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Thickness padding { get; set; }
		public Transition transition { get; set; }
		public Filter filter { get; set; }
	}
	public class SlideAttributes
	{
		public string name { get; }
		public int index { get; }
		public Brush background { get; set; }
		public Color color { get; set; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Thickness padding { get; set; }
		public Transition transition { get; set; }
		public Filter filter { get; set; }
		private HashSet<CustomStyle> appliedStyles = new HashSet<CustomStyle>();

		public SlideAttributes(string name, int index)
		{
			this.name = name;
			this.index = index;
		}

		public void hide(Time delay)
		{
			throw new NotImplementedException();
		}

		public void fadeIn(Time delay, Time duration)
		{
			throw new NotImplementedException();
		}

		public void applyStyle(CustomStyle style)
		{
			appliedStyles.Add(style);
		}

		public CustomStyle[] get_AppliedStyles()
		{
			return appliedStyles.ToArray();
		}
	}
}
