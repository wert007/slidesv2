using Slides;
using Slides.SVG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTMLWriter
{
	internal static class SVGWriter
	{
		internal static void Write(HTMLWriter writer, SVGElement element)
		{

			writer.PushAttribute("width", element.width.ToString());
			writer.PushAttribute("height", element.height.ToString());
			if(element.fill != Color.Transparent)
				writer.PushAttribute("fill", CSSWriter.GetValue(element.fill));
			if(element.stroke != Color.Transparent)
			writer.PushAttribute("stroke", CSSWriter.GetValue(element.stroke));
			if(element.strokeWidth != 0)
				writer.PushAttribute("stroke-width", element.strokeWidth.ToString());
			switch (element.kind)
			{
				case SVGElementKind.Group:
					WriteGroup(writer, (SVGGroup)element);
					break;
				case SVGElementKind.Rect:
					WriteRect(writer, (Rect)element);
					break;
				case SVGElementKind.Circle:
					WriteCircle(writer, (Circle)element);
					break;
				case SVGElementKind.Ellipse:
					WriteEllipse(writer, (Ellipse)element);
					break;
				case SVGElementKind.Line:
					WriteLine(writer, (Line)element);
					break;
				case SVGElementKind.Path:
					WritePath(writer, (SVGPath)element);
					break;
				case SVGElementKind.Polygon:
					WritePolygon(writer, (Polygon)element);
					break;
				case SVGElementKind.Polyline:
					WritePolyline(writer, (Polyline)element);
					break;
				case SVGElementKind.Text:
					WriteSVGText(writer, (SVGText)element);
					break;
				default:
					throw new Exception();
			}
		}

		private static void WriteGroup(HTMLWriter writer, SVGGroup element)
		{
			writer.PushAttribute("transform", $"translate({element.x}, {element.y})");
			writer.StartTag("g");
			foreach (var child in element.Children)
			{
				Write(writer, child);
			}
			writer.EndTag();
		}

		private static void WriteRect(HTMLWriter writer, Rect element)
		{
			writer.PushAttribute("x", element.x.ToString());
			writer.PushAttribute("y", element.y.ToString());
			writer.StartTag("rect");
			writer.EndTag();
		}

		private static void WriteCircle(HTMLWriter writer, Circle element)
		{
			writer.PushAttribute("cx", (element.cx + element.x).ToString());
			writer.PushAttribute("cy", (element.cy + element.y).ToString());
			writer.PushAttribute("r", element.r.ToString());
			writer.StartTag("circle");
			writer.EndTag();
		}

		private static void WriteEllipse(HTMLWriter writer, Ellipse element)
		{
			throw new Exception();
			writer.StartTag("rect");
			writer.EndTag();
		}

		private static void WriteLine(HTMLWriter writer, Line element)
		{
			writer.PushAttribute("x1", element.x.ToString());
			writer.PushAttribute("y1", element.y.ToString());
			writer.PushAttribute("x2", (element.x + element.width).ToString());
			writer.PushAttribute("y2", (element.y + element.height).ToString());

			writer.StartTag("line");
			writer.EndTag();
		}

		private static void WritePath(HTMLWriter writer, SVGPath element)
		{
			var builder = new StringBuilder();
			var operations = PathOperationHelper.Translate(element.Operations, element.x, element.y);

			foreach (var op in operations)
			{
				builder.Append($"{op} ");
			}
			writer.PushAttribute("d", builder.ToString());
			writer.StartTag("path");
			writer.EndTag();
		}

		private static void WritePolygon(HTMLWriter writer, Polygon element)
		{
			writer.StartTag("rect");
			writer.EndTag();
		}

		private static void WritePolyline(HTMLWriter writer, Polyline element)
		{
			writer.StartTag("rect");
			writer.EndTag();
		}

		private static void WriteSVGText(HTMLWriter writer, SVGText element)
		{
			writer.PushAttribute("x", element.x.ToString());
			writer.PushAttribute("y", element.y.ToString());
			writer.StartTag("text");
			writer.Write(element.Content);
			writer.EndTag();
		}
	}
}