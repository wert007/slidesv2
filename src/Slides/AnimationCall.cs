using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Slides
{
	public class Attribute
	{
		public string Name { get; }
		public object Value { get; }

		public Attribute(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}
	public class AnimationCall
	{
		public AnimationCall(string name, string[] changedFields, Time time, CaseCall[] cases, Element element)
		{
			Name = name;
			ChangedFields = changedFields;
			Time = time;
			Cases = cases;
			Element = element;
		}

		public string Name { get; }
		public string[] ChangedFields { get; }
		public Time Time { get; }
		public CaseCall[] Cases { get; }
		public Element Element { get; }
	}
	//class Animation
 //   {
		
	//	public Interpolation interpolation { get; set; }

	//	public void play(Element element, Time time)
	//	{
	//		Console.WriteLine($"Playing Animation {Variable.Name} with {element} and {time}.");
	//	}
 //   }
}
