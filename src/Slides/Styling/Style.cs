using System;
using System.Collections.Generic;

namespace Slides.Styling
{

	[Serializable]
	public abstract class Style
	{
		public abstract string Name { get; }
		public SubstyleCollection Substyles { get; protected set; }
		public abstract Substyle GetMainStyle();
	}

	[Serializable]
	public class CustomStyle : Style
	{

		public CustomStyle(string name, SubstyleCollection substyles)
		{
			Name = name;
			Substyles = substyles;
		}

		public override string Name { get; }

		public override Substyle GetMainStyle() => Substyles.GetRootCustomStyle();
	}

	[Serializable]
	public class StdStyle : Style
	{
		public override string Name => "std";


		public StdStyle(SubstyleCollection substyles)
		{
			Substyles = substyles;
		}

		public object GetValue(string field, string type)
		{
			var substyle = GetStyle(type);
			if (substyle != null)
			{
				if (substyle.HasProperty(field))
					return substyle.GetValue(field);
			}
			return Substyles.GetAllStyle().GetValue(field);
		}

		public bool HasValue(string field, string type)
		{
			var substyle = GetStyle(type);
			if(substyle != null)
			{
				if (substyle.HasProperty(field))
					return true;
			}
			return Substyles.GetAllStyle().HasProperty(field);
		}

		public Substyle GetStyle(string type)
		{
			return Substyles.GetByType(type);
		}

		public override Substyle GetMainStyle() => Substyles.GetAllStyle();
	}
}
