using System.Collections.Generic;

namespace Slides
{
	public class List : Element
	{
		private List _parent;
		private List<Element> _children;

		public override ElementType type => ElementType.List;
		public Unit fontsize { get; set; }
		public Font font { get; set; }
		public bool isOrdered { get; set; } = false;
		public Element[] children => _children.ToArray();

		public List()
		{
			_children = new List<Element>();
		}
		
		public List(string[] contents)
		{
			_children = new List<Element>();
			foreach (var listItem in contents)
			{
				add((string)listItem);
			}
		}

		public List(string[][] contents)
		{
			_children = new List<Element>();
			foreach (var subList in contents)
			{
				if (subList.Length == 1)
					add((string)subList[0]);
				else
					add((string[])subList);
			}
		}

		private List(List parent)
		{
			_parent = parent;
			_children = new List<Element>();
		}

		public void add(string listItem)
		{
			var lbl = new Label(listItem);
			_children.Add(lbl);
		}

		public void add(string[] subList)
		{
			var list = new List(this);
			foreach (var listItem in subList)
				list.add(listItem);

			_children.Add(list);
		}

		protected override Unit get_InitialWidth()
		{
			var max = _children[0].width;
			for (int i = 1; i < _children.Count; i++)
			{
				var current = _children[i].width;
				if (current.Value > max.Value) //TODO(Essential): Add a comparison for units
				{
					max = current;
				}
			}
			return max;
		}

		protected override Unit get_InitialHeight()
		{
			var result = _children[0].height;
			for (int i = 1; i < _children.Count; i++)
			{
				result += _children[i].height;
			}
			return result;
		}
	}
}
