using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Slides;
using Slides.Elements;
using Slides.Helpers;
using Slides.MathExpressions;

namespace HTMLWriter
{
	internal class ChartWriter
	{
		private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");
		private static Dictionary<string, MathPlot> _plots = new Dictionary<string, MathPlot>();

		public static void WritePlot(JavaScriptWriter writer, string parentName, MathPlot plot)
		{
			var id = $"{parentName}-{plot.name}";
			writer.SwitchInto("loadInner");
			WritePlotOptions(writer, plot);
			WriteApexChart(writer, id, plot, true);
			writer.ResetWriter();
			_plots.Add(id, plot);
			WriteUpdatePlot(writer, parentName, plot);
		}


		public static void WriteChart(JavaScriptWriter writer, string id, Chart chart)
		{
			writer.SwitchInto("loadInner");
			WriteOptions(writer, chart);

			WriteApexChart(writer, id, chart, false);
			writer.ResetWriter();
		}

		private static void WriteApexChart(JavaScriptWriter writer, string id, Chart chart, bool isPlot)
		{
			writer.WriteVariableDeclarationInline($"chart_{chart.name}", $"new ApexCharts(document.getElementById('{id}'), options_{chart.name})");
			writer.WriteFunctionCall($"chart_{chart.name}.render");

			if(isPlot)
				writer.WriteFunctionCall("plots.push", new JavaScriptEmitter.JavaScriptObject($"{{ id: '{id}', value: chart_{chart.name} }}"));

		}

		private static void WritePlotOptions(JavaScriptWriter writer, MathPlot plot)
		{
			var showXAxis = plot.showXAxis.ToString().ToLower();
			var showYAxis = plot.showYAxis.ToString().ToLower();

			var a = $@"
let options_{plot.name} = {{
		chart: {{
			type: '{ToString(plot.chartType)}',
			toolbar: {{
				show: {ToString(plot.showDownload)},
			}},
		}},
		colors: ['{CSSWriter.GetValue(plot.color)}'],
		stroke: {{
			curve: 'straight',
		}},
		xaxis: {{
			tickAmount: {plot.xTickAmount},
			labels: {{
				show: {showXAxis},
				hideOverlappingLabels: true,
			}},
			axisBorder: {{
				show: {showXAxis},
			}},
			axisTicks: {{
				show: {showXAxis},
			}},
			crosshairs: {{
				show: false,
			}},
			tooltip: {{
				enabled: {ToString(plot.showTooltip)},
			}},
		}},
		yaxis: {{
			tickAmount: {plot.yTickAmount},
			labels: {{
				show: {showYAxis},
			}},
			axisBorder: {{
				show: {showYAxis},
			}},
			axisTicks: {{
				show: {showYAxis},
			}},
			crosshairs: {{
				show: false,
			}},
			tooltip: {{
				enabled: {ToString(plot.showTooltip)},
			}},
		}},
		grid: {{
			show: {ToString(plot.showGrid)},
		}},
		tooltip: {{
			enabled: {ToString(plot.showTooltip)},
		}},
		series: [],
		noData: {{
			text: 'Loading...'
		}},
	}}
";
			writer.WriteLine(a);
		}

		private static void WriteUpdatePlot(JavaScriptWriter writer, string parentName, MathPlot plot)
		{
			var scope = $"{parentName}_{plot.Graph.Name}_scope";
			writer.SwitchInto("loadInner");
			writer.WriteFunctionCall($"recalculate_{scope}");
			writer.ResetWriter();
			writer.StartFunction($"recalculate_{scope}");
			writer.StartVariableDeclaration("data");
			writer.StartArray();
			writer.EndArray();
			writer.EndVariableDeclaration();

			writer.WriteVariableDeclarationInline("i", "0");
			writer.StartForLoop("x", plot.Range.From.ToString(), plot.Range.To.ToString(), JavaScriptEmitter.ObjectToString(plot.Step));
			writer.WriteAssignment($"{scope}.{plot.InputVariable}", $"x");
			writer.WriteAssignment($"data[i]", "{}");
			writer.WriteAssignment($"data[i].x", $"x");
			writer.WriteAssignment($"data[i].y", $"math.evaluate('{plot.Graph.Expression}', {scope})");
			writer.WriteLine("i++;");
			writer.EndFor();
			writer.WriteAssignment($"{scope}.{plot.InputVariable}", "NaN");

			writer.WriteVariableDeclarationInline("plot", $"getPlot('{parentName}-{plot.name}')");
			//let z = [{
			//name: 'Sales',
			//data: data
			// }];
			writer.StartVariableDeclaration("tmp");
			writer.StartArray();
			writer.StartObject();
			writer.WriteField("name", "x");
			writer.WriteField("data", new JavaScriptEmitter.JavaScriptObject("data"));
			writer.EndObject();
			writer.EndArray();
			writer.WriteFunctionCall("plot.updateSeries", new JavaScriptEmitter.JavaScriptObject("tmp"));
			writer.EndFunction();
		}

		private static void WriteOptions(JavaScriptWriter writer, Chart chart)
		{
			var showXAxis = chart.showXAxis.ToString().ToLower();
			var showYAxis = chart.showYAxis.ToString().ToLower();
			LineChart c = (LineChart)chart;
			var a = $@"
let options_{chart.name} = {{
		chart: {{
			type: '{ToString(chart.chartType)}',
			toolbar: {{
				show: {ToString(chart.showDownload)},
			}},
		}},
		colors: ['{CSSWriter.GetValue(c.color)}'],
		series: [{{
			name: '{c.xName}',
			data: [
				{string.Join(", ", c.values[1].Select(f => f.ToString("0.00", _usCulture)))}
			],
		}}],
		stroke: {{
			curve: 'smooth',
		}},
		xaxis: {{
			tickAmount: {chart.xTickAmount},
			categories: [{string.Join(", ", c.values[0].Select(f => f.ToString("0.00", _usCulture)))}],
			labels: {{
				show: {showXAxis},
				hideOverlappingLabels: true,
			}},
			axisBorder: {{
				show: {showXAxis},
			}},
			axisTicks: {{
				show: {showXAxis},
			}},
			crosshairs: {{
				show: false,
			}},
			tooltip: {{
				enabled: {ToString(chart.showTooltip)},
			}},
		}},
		yaxis: {{
			tickAmount: {chart.yTickAmount},
			labels: {{
				show: {showYAxis},
			}},
			axisBorder: {{
				show: {showYAxis},
			}},
			axisTicks: {{
				show: {showYAxis},
			}},
			crosshairs: {{
				show: false,
			}},
			tooltip: {{
				enabled: {ToString(chart.showTooltip)},
			}},
		}},
		grid: {{
			show: {ToString(chart.showGrid)},
		}},
		tooltip: {{
			enabled: {ToString(chart.showTooltip)},
		}},
	}}
";

			writer.WriteLine(a);
		}

		private static string ToString(bool b)
		{
			return b.ToString().ToLower();
		}

		private static string ToString(ChartType type)
		{
			switch (type)
			{
				case ChartType.LineChart:
					return "line";
				default:
					throw new Exception();		
	}
		}

	}
}