﻿using Slides.Data;
using Slides.Elements;
using Slides.Styling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides
{
	public class SlideAttributes
	{
		public string name { get; }
		public int index { get; }
		public bool isVisible { get; }
		//TODO: Can this be deleted?
		public int startTime { get; }
		public Brush background { get; set; }
		public Color color { get; set; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Thickness padding { get; set; }
		public Transition transition { get; set; }
		public Border border { get; set; }
		public Filter n_filter { get; set; }

		private HashSet<CustomStyle> appliedStyles = new HashSet<CustomStyle>();
		private Dictionary<string, string> _data = new Dictionary<string, string>();

		public SlideAttributes(string name, int index, bool isVisible)
		{
			this.name = name;
			this.index = index;
			this.isVisible = isVisible;
			padding = new Thickness();
			border = new Border();

		}

		public void applyStyle(CustomStyle style)
		{
			appliedStyles.Add(style);
		}

		public string getData(string name)
		{
			if (_data.ContainsKey(name))
				return _data[name];
			return null;
		}

		public void setData(string name, string data)
		{
			_data[name] = data;
		}

		public CustomStyle[] get_AppliedStyles()
		{
			return appliedStyles.ToArray();
		}

		public Thickness get_ActualPadding()
		{
			if (padding != new Thickness()) return padding;
			var style = Element.StdStyle?.GetStyle("slide");
			if(style != null && style.HasProperty("padding"))
				return (Thickness)style.GetValue("padding");
			return new Thickness();
		}
	}
}
