using Slides.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides.MathExpressions
{
	public class MathPlot : Chart
	{
		public MathPlot(MathFormula graph, Range range)
		{
			Graph = graph;
			InputVariable = "x";
			for (int i = 0; i < graph.Variables.Length; i++)
			{
				if (graph.IsUnknown(graph.Variables[i]))
				{
					InputVariable = graph.Variables[i];
					break;
				}
			}
			Range = range;
			color = new Color(255, 0, 0, 255);
			showGrid = true;
			showTooltip = true;
			showXAxis = true;
			showYAxis = true;
		}

		public MathFormula Graph { get; }
		public Range Range { get; }
		public string InputVariable { get; }

		public override ElementType type => ElementType.MathPlot;

		public override ChartType chartType => ChartType.LineChart;

		protected override Unit get_InitialHeight()
		{
			return new Unit(480, Unit.UnitKind.Pixel);
		}

		protected override Unit get_InitialWidth()
		{
			return new Unit(640, Unit.UnitKind.Pixel);
		}
	}
	public class MathFormula
	{
		public string[] Variables { get; }
		public float[] Values { get; }

		public string Name { get; set; }
		public string Expression { get; }

		public MathFormula(string expression)
		{
			Variables = ParseVariables(expression);
			Values = new float[Variables.Length];
			for (int i = 0; i < Values.Length; i++)
			{
				Values[i] = float.NaN;
			}

			Expression = expression;
		}

		private string[] ParseVariables(string expression)
		{
			var result = new List<string>();
			string currentVariable = null;
			for (int i = 0; i < expression.Length; i++)
			{
				char cur = expression[i];
				if (currentVariable != null)
				{
					if (char.IsLetter(cur))
						currentVariable += cur.ToString();
					else
					{
						result.Add(currentVariable);
						currentVariable = null;
					}
				}
				else if (char.IsLetter(cur))
					currentVariable = cur.ToString();
			}
			if(currentVariable != null)
			{
				result.Add(currentVariable);
			}
			return result.ToArray();
		}


		public bool TrySet(string field, float value)
		{
			int i = Array.IndexOf(Variables, field);
			if (i >= 0)
				Values[i] = value;
			return i >= 0;
		}

		public bool IsUnknown(string variable)
		{
			int i = Array.IndexOf(Variables, variable);
			if (i >= 0)
				return float.IsNaN(Values[i]);
			return false;
		}
	}
}
