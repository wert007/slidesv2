using System;
using System.Collections.Generic;

namespace Slides.Data
{
	[Serializable]
	public class CSVFile
	{
		public CSVFile(string text)
		{
			Text = text;
			List<List<string>> values = new List<List<string>>();
			var start = 0;
			var position = 0;
			var maxCells = 0;
			List<string> currentRow = null;
			for (; position < text.Length; position++)
			{
				var c = text[position];
				if (c == ',')
				{
					if (currentRow == null)
					{
						currentRow = new List<string>();
						values.Add(currentRow);
					}
					currentRow.Add(text.Substring(start, position - start).Trim());
					start = position + 1;
				}
				else if (c == '\n')
				{
					if (currentRow == null)
						throw new Exception();
					currentRow.Add(text.Substring(start, position - start).Trim());
					maxCells = Math.Max(maxCells, currentRow.Count);
					currentRow = null;
					start = position;
				}
			}
			if(start != position)
			{
				if (currentRow == null)
				{
					currentRow = new List<string>();
					values.Add(currentRow);
				}
				currentRow.Add(text.Substring(start, position - start).Trim());
				maxCells = Math.Max(maxCells, currentRow.Count);
				currentRow = null;
				start = position;
			}
			Width = maxCells;
			Height = values.Count;
			_values = new string[maxCells, values.Count];
			for (int x = 0; x < maxCells; x++)
			{
				for (int y = 0; y < values.Count; y++)
				{
					string value = null;
					if (values[y].Count > x)
						value = values[y][x];
					_values[x, y] = value;
				}
			}
		}

		public string Text { get; }
		public int Width { get; }
		public int Height { get; }
		string[,] _values;

		public string GetValue(int x, int y)
		{
			return _values[x, y];
		}
	}
}
