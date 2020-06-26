using Slides.Styling;
using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public class List : ParentElement
	{
		public enum ListMarkerType
		{
			Disk,
			Circle,
			Square,
			None,
		}
		private List<Element> _children;

		public override ElementKind kind => ElementKind.List;
		public bool isOrdered { get; set; } = false;
		public Element[] children => _children.ToArray();
		public ListMarkerType markerType { get; set; }
		private string _markerText = null;
		private readonly List<List<Style>> _appliedStyles = new List<List<Style>>();

		public List()
		{
			_children = new List<Element>();
			//TODO: Introduce a place for such constants maybe??
			addApplyStyleHandler("non-css-custom-text-marker", v => setTextmarker((string)v));
		}

		public List(string[] contents)
		{
			_children = new List<Element>();
			foreach (var listItem in contents)
				add(listItem);
			//TODO: Introduce a place for such constants maybe??
			addApplyStyleHandler("non-css-custom-text-marker", v => setTextmarker((string)v));
		}

		public List(string[][] contents)
		{
			_children = new List<Element>();
			foreach (var subList in contents)
				if (subList.Length == 1)
					add(subList[0]);
				else
					add(subList);
			//TODO: Introduce a place for such constants maybe??
			addApplyStyleHandler("non-css-custom-text-marker", v => setTextmarker((string)v)); 
		}

		private void addChild(int level, Element e)
		{
			e.h_parent = this;
			_children.Add(e);
			foreach (var style in get_AppliedStyles())
				e.applyStyle(style);

			for (int i = 0; i < Math.Min(level, _appliedStyles.Count); i++)
				foreach (var style in _appliedStyles[i])
					e.applyStyle(style);
		}

		public void add(string listItem)
		{
			add(0, listItem);
		}

		public void add(int level, string listItem)
		{
			if (level != 0)
			{
				var subList = new List();
				subList.add(level - 1, listItem);
				addChild(level, subList);
			}
			else
				addChild(level, new Label(listItem));
		}


		public void add(string[] subList)
		{
			var list = new List();
			foreach (var listItem in subList)
				list.add(listItem);
			//TODO: How should add work?
			// - a
			//   - b
			// - c
			//
			//   vs
			//
			// - a
			//   - b
			//   - c
			//
			//Right now the first is favored and used.
			addChild(1, list);	
		}

		public void setTextmarker(string marker)
		{
			markerType = ListMarkerType.None;
			_markerText = marker;
			foreach (var child in children)
			{
				if (child is List l) l.setTextmarker(marker);
			}
		}

		public void applyStyle(int level, Style style)
		{
			if (level == 0)
			{
				applyStyle(style);
				return;
			}
			while (_appliedStyles.Count < level) _appliedStyles.Add(new List<Style>());
			_appliedStyles[level-1].Add(style);
			foreach (var child in children)
			{
				if (child is List l)
					l.applyStyle(level - 1, style);
			}
		}

		public string get_TextMarker() => _markerText;

		internal override Unit get_InitialWidth()
		{
			if (children.Length == 0) return new Unit();
			var max = _children[0].width;
			for (int i = 1; i < _children.Count; i++)
			{
				var current = _children[i].width;
				if (current.Value > max.Value) //TODO(Essential): Add a comparison for units
					max = current;
			}
			return max;
		}

		internal override Unit get_InitialHeight()
		{
			var result = new Unit();
			for (int i = 0; i < _children.Count; i++)
				result += _children[i].height;
			return result * 1.25f; //TODO: Absolutely Hacky! 1.25f is for lineheight. but there should be better ways then this!
		}

		protected override IEnumerable<Element> get_Children() => _children;
	}
}
