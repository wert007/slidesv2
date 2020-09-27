using Slides.Data;
using Slides.Styling;
using System;
using System.Collections.Generic;

namespace Slides.Elements
{
	public enum ListMarkerType
	{
		Disk,
		Circle,
		Square,
		None,
	}
	public class ListElementStyling : TextElementStyling
	{
		private string _markerText = null;
		public ListMarkerType? h_MarkerType { get; set; }
		public ListElementStyling h_Parent { get; }
		public ListElementStyling h_Child { get; set; } = null;
		public bool isOrdered { get; set; } = false;
		public ListMarkerType markerType { get => h_MarkerType ?? h_Parent?.h_MarkerType ?? ListMarkerType.Disk;
			set => h_MarkerType = value; }
		public Alignment align { get; set; } = Alignment.Unset;
		private ListElementStyling(List _element) : base(_element)
		{

		}
		private ListElementStyling(ListElementStyling parent)
		{
			h_Parent = parent;
		}

		public static ListElementStyling h_CreateElementStyling(List _element) => new ListElementStyling(_element);

		public void setTextmarker(string marker)
		{
			markerType = ListMarkerType.None;
			_markerText = marker;
			if (h_Child != null) h_Child.setTextmarker(marker);
		}

		public string get_Textmarker() => _markerText;

		public ListElementStyling h_AssureNext()
		{
			if (h_Child == null)
				h_Child = new ListElementStyling(this);
			return h_Child;
		}

		public ListElementStyling[] h_ToArray()
		{
			var current = this;
			var result = new List<ListElementStyling>();
			while(current != null)
			{
				result.Add(current);
				current = current.h_Child;
			}
			return result.ToArray();
		}
	}
	public class List : ParentElement
	{
		private List<Element> _children;
		public bool isOrdered { get => ((ListElementStyling)h_Styling).isOrdered; set => ((ListElementStyling)h_Styling).isOrdered = value; }
		public Alignment align { get => ((ListElementStyling)h_Styling).align; set => ((ListElementStyling)h_Styling).align = value; }
		public ListMarkerType markerType { get => ((ListElementStyling)h_Styling).markerType;
			set => ((ListElementStyling)h_Styling).markerType = value; }
		public override ElementKind kind => ElementKind.List;
		public Element[] children => _children.ToArray();
		private readonly ListElementStyling _styling;
		public override ElementStyling h_Styling => _styling;

		public List() : base(null)
		{
			_styling = ListElementStyling.h_CreateElementStyling(this);
			_children = new List<Element>();
			//TODO: Introduce a place for such constants maybe??
			addApplyStyleHandler("non-css-custom-text-marker", v => setTextmarker((string)v));
			Init();
		}

		public List(string[] contents) : base(null)
		{
			_styling = ListElementStyling.h_CreateElementStyling(this);
			_children = new List<Element>();
			foreach (var listItem in contents)
				addParsing(listItem);
			//TODO: Introduce a place for such constants maybe??
			addApplyStyleHandler("non-css-custom-text-marker", v => setTextmarker((string)v));
			Init();
		}

		public List(string[][] contents) : base(null)
		{
			_styling = ListElementStyling.h_CreateElementStyling(this);
			_children = new List<Element>();
			foreach (var subList in contents)
				if (subList.Length == 1)
					add(subList[0]);
				else
					add(subList);
			//TODO: Introduce a place for such constants maybe??
			addApplyStyleHandler("non-css-custom-text-marker", v => setTextmarker((string)v)); 
			Init();
		}
		public new ListElementStyling styling(int index)
		{
			var current = ((ListElementStyling)h_Styling);
			while (index > 0)
			{
				current = current.h_AssureNext();
				index--;
			}
			return current;
		}

		public ListElementStyling[] get_Stylings() => ((ListElementStyling)h_Styling).h_ToArray();

		public void setTextmarker(string marker) => ((ListElementStyling)h_Styling).setTextmarker(marker);
		public string get_Textmarker() => ((ListElementStyling)h_Styling).get_Textmarker();
		private void addChild(int level, Element e)
		{
			e.h_Parent = this;
			_children.Add(e);
			foreach (var style in get_AppliedStyles())
				e.applyStyle(style);

			//for (int i = 0; i < Math.Min(level, _appliedStyles.Count); i++)
			//	foreach (var style in _appliedStyles[i])
			//		e.applyStyle(style);
		}

		public void addParsing(string listItem)
		{
			int i = 0;
			while (listItem[i] == ' ') i++;
			add(i, listItem);
		}

		public void add(string listItem)
		{
			add(0, listItem);
		}

		public void add(List subList)
		{
			add(0, subList);
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

		public void add(int level, List subList)
		{
			if (level != 0)
			{
				var newSubList = new List();
				newSubList.add(level - 1, subList);
				addChild(level, newSubList);
			}
			else
				addChild(level, subList);
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


		//public void applyStyle(int level, Style style)
		//{
		//	if (level == 0)
		//	{
		//		applyStyle(style);
		//		return;
		//	}
		//	styling(level).applyStyle(style);
		//	foreach (var child in children)
		//	{
		//		if (child is List l)
		//			l.applyStyle(level - 1, style);
		//	}
		//}

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
			return result * lineHeight; //TODO: Check if this works out!
		}

		protected override IEnumerable<Element> get_Children() => _children;
	}
}
