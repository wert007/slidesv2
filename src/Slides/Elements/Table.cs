using Slides.Data;
using Slides.Styling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides.Elements
{
	public class Table : ParentElement
	{
		public override ElementKind kind => ElementKind.Table;
		public TableChild this[int column, int row]
		{
			get
			{
				return cells[column][row];
			}
		}
		public int columns { get; }
		public int rows { get; }
		public Alignment align { get; set; }
		public TableChild[][] cells { get; set; }
		public Table(int columns, int rows) : this()
		{
			this.rows = rows;
			this.columns = columns;
			cells = new TableChild[columns][];
			for (int c = 0; c < columns; c++)
			{
				cells[c] = new TableChild[rows];
				for (int r = 0; r < rows; r++)
				{
					cells[c][r] = new TableChild("");
					cells[c][r].ContentUpdated += UpdateLayout;
					cells[c][r].h_Parent = this;
				}
			}
			UpdateLayout();
		}
		public Table(string[][] contents) : this()
		{
			rows = contents.Length;
			for (int i = 0; i < rows; i++)
				columns = Math.Max(contents[i].Length, columns);
			cells = new TableChild[columns][];

			for (int c = 0; c < columns; c++)
			{
				cells[c] = new TableChild[rows];
				for (int r = 0; r < rows; r++)
				{
					var content = "";
					if (c < contents[r].Length)
						content = contents[r][c];
					cells[c][r] = new TableChild(content);
					cells[c][r].ContentUpdated += UpdateLayout;
					cells[c][r].h_Parent = this;
				}
			}
			UpdateLayout();
		}

		private Table()
		{
			font = null;
			fontsize = new Unit(14, Unit.UnitKind.Point);
			align = Alignment.Unset;
		}

		//TODO: 
		// And let's not forget how shitty Unit.Max is... 
		//    if you have w = 80% and 10 columns you have a
		//    columnWidth of 8%. Which is, if your screen is 2000px wide, 360px
		//    so if your text is longer than that we would never know. because we say
		//    relative is always bigger than absolute. even if your text is 500px long..
		public override void UpdateLayout()
		{
			var t = top;
			var l = left;
			var w = get_ActualWidth();
			var availableFairColumnCount = columns;
			for (int c = 0; c < columns; c++)
			{
				var availableWidth = availableFairColumnCount == 0 ? new Unit() : w / availableFairColumnCount;
				var columnWidth = Unit.Max(getColumnWidth(c), availableWidth);
				if (getUserDefinedColumnWidth(c) != null) columnWidth = getUserDefinedColumnWidth(c);
				if (columnWidth != availableWidth)
				{
					w -= columnWidth;
					availableFairColumnCount--;
				}
				var h = get_ActualHeight();
				var availableFairRowCount = rows;
				for (int r = 0; r < rows; r++)
				{
					cells[c][r].set_Top(t);
					cells[c][r].set_Left(l);

					var availableHeight = availableFairRowCount == 0 ? new Unit() : h / availableFairRowCount;
					var rowHeight = Unit.Max(getRowHeight(r), availableHeight);
					if (getUserDefinedRowHeight(r) != null) rowHeight = getUserDefinedRowHeight(r);
					if (rowHeight != availableHeight)
					{
						h -= rowHeight;
						availableFairRowCount--;
					}
					cells[c][r].set_WidthFromTable(columnWidth);
					cells[c][r].set_HeightFromTable(rowHeight);
					t = cells[c][r].bottomSide;
				}
				l = cells[c][0].rightSide;
				t = top;
				//l = left;
			}
		}

		private Unit getRowHeight(int r)
		{
			var row = getRow(r);
			var maxUnit = new Unit();
			foreach (var child in row)
			{
				var u = child.get_ActualTableChildHeight();
				maxUnit = Unit.Max(maxUnit, u);
			}
			return maxUnit;
		}

		private Unit getUserDefinedRowHeight(int r)
		{
			var row = getRow(r);
			Unit result = null;
			foreach (var child in row)
			{
				var height = child.get_UserDefinedHeight();
				if (result == null) result = height;
				if (height != null)
					result = Unit.Max(result, height);
			}
			return result;
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

		private Unit getUserDefinedColumnWidth(int c)
		{
			var column = getColumn(c);
			Unit result = null;
			foreach (var child in column)
			{
				var width = child.get_UserDefinedWidth();
				if (result == null) result = width;
				if (width != null)
					result = Unit.Max(result, width);
			}
			return result;
		}

		public TableChild[] getRow(int r)
		{
			if (r < 0 || r >= rows) return new TableChild[0];
			var result = new TableChild[columns];
			for (int c = 0; c < columns; c++)
				result[c] = this[c, r];
			return result;
		}

		public TableChild[] getColumn(int c)
		{
			if (c < 0 || c >= columns) return new TableChild[0];
			var result = new TableChild[rows];
			for (int r = 0; r < rows; r++)
				result[r] = this[c, r];
			return result;
		}

		//TODO: We exposed the cells[][] so do we need an extra getAt()?
		public TableChild getAt(int c, int r)
		{
			return this[c, r];
		}

		public void setRow(string[] contents, int row) => setRow(contents, row, 0);
		public void setRow(string[] contents, int row, int offset)
		{
			for (int c = offset; c < Math.Min(contents.Length + offset, columns); c++)
				cells[c][row].content = contents[c - offset];
		}

		public void setColumn(string[] contents, int column) => setColumn(contents, column, 0);
		public void setColumn(string[] contents, int column, int offset)
		{
			for (int r = offset; r < Math.Min(contents.Length + offset, rows); r++)
				cells[column][r].content = contents[r - offset];
		}

		//TODO: What if we set the fontsize in a style?
		//      Is this still valid? I thought we fixed that?
		internal override Unit get_InitialHeight()
		{
			var maxHeight = new Unit(0, Unit.UnitKind.Pixel);
			for (int c = 0; c < columns; c++)
			{
				Unit sum = null;
				for (int r = 0; r < rows; r++)
					if (sum == null)
						sum = this[c, r].get_InitialHeight();
					else
						sum += this[c, r].get_InitialHeight();
				if (sum.Value > maxHeight.Value)
					maxHeight = sum;
			}
			return maxHeight;
		}

		//TODO: What if we set the fontsize in a style?
		internal override Unit get_InitialWidth()
		{
			var maxWidth = new Unit(0, Unit.UnitKind.Pixel);
			for (int r = 0; r < rows; r++)
			{
				Unit sum = null;
				for (int c = 0; c < columns; c++)
					if (sum == null)
						sum = this[c, r].get_InitialWidth();
					else
						sum += this[c, r].get_InitialWidth();

				if (sum.Value > maxWidth.Value)
					maxWidth = sum;
			}
			return maxWidth;
		}

		protected override IEnumerable<Element> get_Children()
		{
			foreach (var col in cells)
				foreach (var c in col)
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
