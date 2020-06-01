using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides.Elements
{
	public class Table : Element
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
		public Font font { get; set; }
		public Unit fontsize { get; set; }
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
			var px = new Unit(1, Unit.UnitKind.Pixel);
			//borderThickness = new Thickness(px, px, px, px);
			//borderStyle = BorderStyle.Solid;
		}

		//TODO: 
		//			l += Unit.Max(getColumnWidth(c), w / columns);
		//gives the wrong result, when getColumnWidth is bigger than w.
		//in that case you would need to subtract the columnWidth and 
		//divide w by columns - 1 in the future. 
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
					cells[r][c].initWidth = Unit.Max(getColumnWidth(c), w / columns);
					cells[r][c].initHeight = rowHeight;
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

		protected override Unit get_InitialHeight()
		{
			var maxHeight = new Unit(0, Unit.UnitKind.Pixel);
			for (int i = 0; i < columns; i++)
			{
				Unit sum = null;
				for (int r = 0; r < rows; r++)
					if (sum == null)
						sum = this[r, i].get_Height(font, fontsize);
					else
						sum += this[r, i].get_Height(font, fontsize);
				if (sum.Value > maxHeight.Value)
					maxHeight = sum;
			}
			return maxHeight;
		}

		protected override Unit get_InitialWidth()
		{
			var maxWidth = new Unit(0, Unit.UnitKind.Pixel);
			for (int i = 0; i < rows; i++)
			{
				Unit sum = null;
				for (int c = 0; c < columns; c++)
					if (sum == null)
						sum = this[i, c].get_Width(font, fontsize);
					else
						sum += this[i, c].get_Width(font, fontsize);
				if (sum.Value > maxWidth.Value)
					maxWidth = sum;
			}
			return maxWidth;
		}

		private void AddToModifiedFields(Dictionary<string, object> modifiedFields, string fieldName, object field, object def = null)
		{
			var val = field;
			if (val == null || val.Equals(def))
				val = get_AppliedStyles().FirstOrDefault(s => s.ModifiedFields.ContainsKey(fieldName))?.ModifiedFields[fieldName] ?? def;
			if (val != null && !val.Equals(def))
				modifiedFields.Add(fieldName, val);
		}

		public CustomStyle get_TableChildStyle(string parentName)
		{
			Dictionary<string, object> modifiedFields = new Dictionary<string, object>();
			AddToModifiedFields(modifiedFields, "font", font);
			AddToModifiedFields(modifiedFields, "fontsize", fontsize);
			AddToModifiedFields(modifiedFields, "align", align, Alignment.Unset);
			AddToModifiedFields(modifiedFields, "borderStyle", borderStyle, BorderStyle.Unset);
			AddToModifiedFields(modifiedFields, "borderColor", borderColor);
			AddToModifiedFields(modifiedFields, "borderThickness", borderThickness, new Thickness());
			return new CustomStyle($"{parentName}_{name}_tablechild_style", modifiedFields);
		}
	}
}
