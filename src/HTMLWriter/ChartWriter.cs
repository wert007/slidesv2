using System;
using Slides;

namespace HTMLWriter
{
	internal class ChartWriter
	{
		public static void WriteChart(JavaScriptWriter writer, string parentName, Chart chart)
		{
			writer.ToggleOnload();
			writer.WriteVariableDeclarationInline("ctx", $"document.getElementById('{parentName}-{chart.name}').getContext('2d')");
			writer.WriteAssignment($"window.{chart.name}", $"new Chart(ctx, get{chart.name}Config())");
			writer.ToggleOnload();
			writer.StartFunction($"get{chart.name}Config");
			switch (chart.chartType)
			{
				case ChartType.LineChart:
					writer.StartVariableDeclaration("config");
					WriteLineChartConfig(writer, (LineChart)chart);
					break;
				default:
					throw new NotImplementedException();
			}
			writer.EndFunction();
		}

		private static void WriteLineChartConfig(JavaScriptWriter writer, LineChart chart)
		{
			writer.StartObject();
				writer.WriteField("type", "line");

				writer.StartField("data");
				writer.StartObject();

					writer.StartField("labels");
					writer.StartArray();
					for (int i = 0; i < chart.Data.Height - 1; i++)
					{
						if (i > 0)
							writer.WriteArraySeperator();
						writer.WriteValue(chart.values[0][i].ToString());
					}
					writer.EndArray();
					writer.EndField();

					writer.StartField("datasets");
					writer.StartArray();
						writer.StartObject();
						writer.WriteField("label", "hello World!");
						writer.WriteField("backgroundColor", chart.color);
						writer.WriteField("borderColor", chart.color);

						writer.StartField("data");
						writer.StartArray();
						for (int i = 0; i < chart.Data.Height - 1; i++)
						{
							if (i > 0)
								writer.WriteArraySeperator();
							 writer.WriteValue(chart.values[1][i]);
						}
						writer.EndArray();
						writer.EndField();

						writer.WriteField("fill", false);
					writer.EndObject();
					writer.EndArray();
				writer.EndObject();
				writer.EndField();

			writer.StartField("options");
				writer.StartObject();
			writer.WriteField("maintainAspectRatio", false);
			writer.WriteField("responsive", true);
			writer.StartField("legend");
			writer.StartObject();
			writer.WriteField("display", chart.showLegend);
			writer.EndObject();
			writer.EndField();
				writer.StartField("scales");
					writer.StartObject();
						writer.StartField("xAxes");
			writer.StartArray();
						writer.StartObject();
							writer.StartField("ticks");
							writer.StartObject();
							writer.WriteField("display", chart.showXAxis);
							writer.EndObject();
							writer.EndField();
							writer.StartField("gridLines");
							writer.StartObject();
							writer.WriteField("display", chart.showXAxis);
							writer.WriteField("drawBorder", chart.showXAxis);
							writer.EndObject();
							writer.EndField();
						writer.EndObject();
			writer.EndArray();
						writer.EndField();

						writer.StartField("yAxes");
			writer.StartArray();
						writer.StartObject();
						writer.StartField("ticks");
						writer.StartObject();
						writer.WriteField("display", chart.showYAxis);
						writer.EndObject();
						writer.EndField();

			writer.StartField("gridLines");
			writer.StartObject();
			writer.WriteField("display", chart.showXAxis);
			writer.WriteField("drawBorder", chart.showXAxis);
			writer.EndObject();
			writer.EndField();
			writer.EndObject();
			writer.EndArray();
						writer.EndField();
			writer.EndObject();
				writer.EndField();
				writer.EndObject();
				writer.EndField();
			writer.EndObject();
			writer.WriteSemicolon();

			writer.WriteReturnStatement("config");
		}
	}
}