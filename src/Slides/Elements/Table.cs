using Slides.Styling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides.Elements
{
	public class Table : ParentElement
	{
		public override ElementKind kind => ElementKind.Table;
		public TableChild this[int row, int column]
		{
			get
			{
				return cells[row][column];
			}
		}
		public int columns { get; }
		public int rows { get; }
		public Alignment align { get; set; }
		public TableChild[][] cells { get; set; }
		public Table(int rows, int columns) : this()
		{
			this.rows = rows;
			this.columns = columns;
			cells = new TableChild[rows][];
			for (int r = 0; r < rows; r++)
			{
				cells[r] = new TableChild[columns];
				for (int c = 0; c < columns; c++)
				{
					cells[r][c] = new TableChild("");
					cells[r][c].ContentUpdated += UpdateLayout;
					cells[r][c].h_parent = this;
				}
			}
			UpdateLayout();
		}
		public Table(string[][] contents) : this()
		{
			rows = contents.Length;
			for (int i = 0; i < rows; i++)
				columns = Math.Max(contents[i].Length, columns);
			cells = new TableChild[rows][];

			for (int r = 0; r < rows; r++)
			{
				cells[r] = new TableChild[columns];
				for (int c = 0; c < columns; c++)
				{
					var content = "";
					if (c < contents[r].Length)
						content = contents[r][c];
					cells[r][c] = new TableChild(content);
					cells[r][c].ContentUpdated += UpdateLayout;
					cells[r][c].h_parent = this;
				}
			}
			UpdateLayout();
		}

		private Table()
		{
			font = null;
			fontsize = new Unit(14, Unit.UnitKind.Point);
			align = Alignment.Unset;
			//borderColor = new Color(0, 0, 0, 255);
			//var px = new Unit(1, Unit.UnitKind.Pixel);
			//borderThickness = new Thickness(px, px, px, px);
			//borderStyle = BorderStyle.Solid;
		}

		//TODO: 
		//			l += Unit.Max(getColumnWidth(c), w / columns);
		//gives the wrong result, when getColumnWidth is bigger than w.
		//in that case you would need to subtract the columnWidth and 
		//divide w by columns - 1 in the future. 
		// And let's not forget how shitty Unit.Max is... 
		//    if you have w = 80% and 10 columns you have a
		//    columnWidth of 8%. Which is, if your screen is 2000px wide, 360px
		//    so if your text is longer than that we would never know. because we say
		//    relative is always bigger than absolute. even if your text is 500px long..
		private void UpdateLayout()
		{
			var t = top;
			var l = left;
			var w = get_ActualWidth();
			var h = get_ActualHeight();
			for (int r = 0; r < rows; r++)
			{
				var rowHeight = Unit.Max(getRowHeight(r), h / rows);
				for (int c = 0; c < columns; c++)
				{
					cells[r][c].set_Top(t);
					cells[r][c].set_Left(l);
					cells[r][c].width = Unit.Max(getColumnWidth(c), w / columns);
					cells[r][c].height = rowHeight;
					l = cells[r][c].rightSide;
				}
				t = cells[r][0].bottomSide;
				l = left;
			}
		}

		private Unit getRowHeight(int r)
		{
			var row = getRow(r);
			var maxValue = float.MinValue;
			var maxUnit = new Unit();
			foreach (var child in row)
			{
				var u = child.get_ActualTableChildHeight();
				if(u.Value > maxValue) //TODO: Unit Comparison!
				{
					maxValue = u.Value;
					maxUnit = u;
				}
			}
			return maxUnit;
		}
		private Unit getColumnWidth(int c)
		{
			var column = getColumn(c);
			var maxValue = float.MinValue;
			var maxUnit = new Unit();
			foreach (var child in column)
			{
				var u = child.get_ActualTableChildWidth();
				if (u.Value > maxValue) //TODO: Unit Comparison!
				{
					maxValue = u.Value;
					maxUnit = u;
				}
			}
			return maxUnit;
		}

		public TableChild[] getRow(int r)
		{
			if (r < 0 || r >= rows) return new TableChild[0];
			var result = new TableChild[columns];
			for (int c = 0; c < columns; c++)
				result[c] = this[r, c];
			return result;
		}

		public TableChild[] getColumn(int c)
		{
			if (c < 0 || c >= columns) return new TableChild[0];
			var result = new TableChild[rows];
			for (int r = 0; r < rows; r++)
				result[r] = this[r, c];
			return result;
		}

		//TODO: We exposed the cells[][] so do we need an extra getAt()?
		public TableChild getAt(int r, int c)
		{
			return this[r, c];
		}

		public void setRow(string[] contents, int row) => setRow(contents, row, 0);
		public void setRow(string[] contents, int row, int offset)
		{
			for (int c = offset; c < Math.Min(contents.Length + offset, columns); c++)
				cells[row][c].content = contents[c - offset];
		}

		public void setColumn(string[] contents, int column) => setColumn(contents, column, 0);
		public void setColumn(string[] contents, int column, int offset)
		{
			for (int r = offset; r < Math.Min(contents.Length + offset, rows); r++)
				cells[r][column].content = contents[r - offset];
		}

		//TODO: What if we set the fontsize in a style?
		//      Is this still valid? I thought we fixed that?
		internal override Unit get_InitialHeight()
		{
			var maxHeight = new Unit(0, Unit.UnitKind.Pixel);
			for (int i = 0; i < columns; i++)
			{
				Unit sum = null;
				for (int r = 0; r < rows; r++)
					if (sum == null)
						sum = this[r, i].get_InitialHeight();
					else
						sum += this[r, i].get_InitialHeight();
				if (sum.Value > maxHeight.Value)
					maxHeight = sum;
			}
			return maxHeight;
		}

		//TODO: What if we set the fontsize in a style?
		internal override Unit get_InitialWidth()
		{
			var maxWidth = new Unit(0, Unit.UnitKind.Pixel);
			for (int i = 0; i < rows; i++)
			{
				Unit sum = null;
				for (int c = 0; c < columns; c++)
					if (sum == null)
						sum = this[i, c].get_InitialWidth();
					else
						sum += this[i, c].get_InitialWidth();

				if (sum.Value > maxWidth.Value)
					maxWidth = sum;
			}
			return maxWidth;
		}

		protected override IEnumerable<Element> get_Children()
		{
			foreach (var row in cells)
				foreach (var c in row)
					yield return c;
		}

		//private void AddToModifiedFields(Dictionary<string, object> modifiedFields, string fieldName, object field, object defaultValue = null)
		//{
		//	var val = field;
		//	if (val == null || val.Equals(defaultValue))
		//		val = get_AppliedStyles().FirstOrDefault(s => s.ModifiedFields.ContainsKey(fieldName))?.ModifiedFields[fieldName] ?? defaultValue;
		//	if (val != null && !val.Equals(defaultValue))
		//		modifiedFields.Add(fieldName, val);
		//}

		//public CustomStyle get_TableChildStyle(string parentName)
		//{
		//	Dictionary<string, object> modifiedFields = new Dictionary<string, object>();
		//	AddToModifiedFields(modifiedFields, "font", font);
		//	AddToModifiedFields(modifiedFields, "fontsize", fontsize);
		//	AddToModifiedFields(modifiedFields, "align", align, Alignment.Unset);
		//	AddToModifiedFields(modifiedFields, "borderStyle", borderStyle, BorderStyle.Unset);
		//	AddToModifiedFields(modifiedFields, "borderColor", borderColor);
		//	AddToModifiedFields(modifiedFields, "borderThickness", borderThickness, new Thickness());
		//	return new CustomStyle($"{parentName}_{name}_tablechild_style", modifiedFields);
		//}
	}
}
