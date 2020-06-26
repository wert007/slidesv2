using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using SVGLib.GraphicsElements;
using Slides.Elements;
using Slides.Styling;

namespace Minsk.CodeAnalysis
{
	internal class GroupBuilder
	{
		private readonly Dictionary<VariableSymbol, SVGGraphicsElement> _svgChildren;
		private readonly Dictionary<VariableSymbol, Element> _children;
		private readonly List<CustomStyle> _groupAppliedStyles = new List<CustomStyle>();
		private bool IsSVG => _children == null;
		private GroupBuilder(bool isSVG) 
		{
			if (!isSVG) _children = new Dictionary<VariableSymbol, Element>();
			else _svgChildren = new Dictionary<VariableSymbol, SVGGraphicsElement>();
		}

		public static GroupBuilder CreateSVGGroupBuilder()
		{
			return new GroupBuilder(true);
		}
		public static GroupBuilder CreateGroupBuilder()
		{
			return new GroupBuilder(false);
		}

		internal void TryAddChildren(VariableSymbol variable, object value)
		{
			if (IsSVG)
				TryAddSVGGroupChildren(value, variable);
			else
				TryAddGroupChildren(value, variable);
		}


		private void TryAddGroupChildren(object value, VariableSymbol variable)
		{
			if (!(value is Element))
				return;
			if(value is object[] array)
			{
				var type = ((ArrayTypeSymbol)variable.Type).Child;
				for (int i = 0; i < array.Length; i++)
				{
					TryAddGroupChildren(array[i], new VariableSymbol($"{variable.Name}#{i}", variable.IsReadOnly, type, variable.NeedsDataFlag));
				}
			}
			if (variable.IsVisible && !variable.Type.IsData)
				if (value is Element element && element.isVisible)
					_children[variable] = element;
		}

		private void TryAddSVGGroupChildren(object value, VariableSymbol variable)
		{
			if (!(value is SVGGraphicsElement || value is object[]))
				return;
			if (variable.IsVisible && !variable.Type.IsData)
			{
				if (variable.Type.Type != TypeType.Array && value is SVGGraphicsElement element && element.IsVisible)
					_svgChildren[variable] = element;
				else
					for (int i = 0; i < ((object[])value).Length; i++)
					{
						var e = ((object[])value)[i];
						if (e is SVGGraphicsElement elementArr && elementArr.IsVisible)
						{
							var t = ((ArrayTypeSymbol)variable.Type).Child;
							var variableArray = new VariableSymbol($"{variable.Name}#{i}", variable.IsReadOnly, t, variable.NeedsDataFlag);
							_svgChildren.Add(variableArray, elementArr);
						}
					}
			}
		}

		internal SVGGraphicsElement[] GetSVGValues()
		{
			return _svgChildren.Select(c => c.Value).Where(c => c.IsVisible).ToArray();
		}

		internal Element[] GetGroupValues()
		{
			return _children.Select(c => c.Value).Where(c => c.isVisible).ToArray();
		}

		internal IEnumerable<CustomStyle> GetAppliedStyles()
		{
			return _groupAppliedStyles;
		}

		internal void ApplyStyle(CustomStyle style)
		{
			_groupAppliedStyles.Add(style);
		}
	}
}