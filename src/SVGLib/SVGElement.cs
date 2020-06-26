using SVGLib.Datatypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SVGLib
{
	public abstract class SVGElement
	{
		public Color Fill { get; set; } = Color.Black;
		public Color Stroke { get; set; } = Color.Transparent;
		public double StrokeWidth { get; set; }
		public LineCap StrokeLineCap { get; set; }
		//TODO: Do we keep this?
		public bool IsVisible { get; set; }

		private List<Transform> _transforms = new List<Transform>();
		public Transform[] Transform {
			get => _transforms.ToArray(); 
			set
			{
				_transforms.Clear();
				if (value != null) _transforms.AddRange(value);
			}
		}
		public abstract SVGElementKind Kind { get; }

		public void AddTransform(Transform transform)
		{
			_transforms.Add(transform);
		}
	}
}
