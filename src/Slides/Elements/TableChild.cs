using Slides.Data;
using System;

namespace Slides.Elements
{
	public class TableChild : TextElement
	{
		internal delegate void ContentUpdatedHandler();
		internal event ContentUpdatedHandler ContentUpdated;

		private string _content;

		private Unit _tableHeight;
		private Unit _tableWidth;

		public string content { get => _content; set
			{
				_content = value;
				UpdateLayout();
			}
		}
		public Alignment? align { get; set; }

		public override ElementKind kind => ElementKind.TableChild;

		private Unit _top;
		private Unit _left;

		public override Unit top => _top;
		public override Unit left => _left;

		public void set_Top(Unit t) => _top = t;
		public void set_Left(Unit l) => _left = l;

		public TableChild(string content)
		{
			_content = content;
			font = null;
			fontsize = null;
			align = null;
			//TODO: Remove this. There should be a solution in core.css
			styling.position = "relative";
		}

		public override string ToString() => content;

		internal Unit get_ActualTableChildHeight() => get_ActualHeight();
		internal Unit get_ActualTableChildWidth() => get_ActualWidth();
		internal Unit get_UserDefinedHeight() => styling.get_UserDefinedHeight();
		internal Unit get_UserDefinedWidth() => styling.get_UserDefinedWidth();

		public override Unit get_UninitializedStyleHeight() => get_InitialHeight();
		public override Unit get_UninitializedStyleWidth() => get_InitialWidth();

		public override void UpdateLayout()
		{
			ContentUpdated?.Invoke();
		}
		internal override Unit get_InitialWidth() => _tableWidth ?? new Unit(MeasureText(_content).X, Unit.UnitKind.Pixel);
		internal override Unit get_InitialHeight() => _tableHeight ?? new Unit(MeasureText(_content).Y, Unit.UnitKind.Pixel);

		internal void set_HeightFromTable(Unit height) => _tableHeight = height;
		internal void set_WidthFromTable(Unit width) => _tableWidth = width;
	}
}
