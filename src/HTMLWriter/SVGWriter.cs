using Slides;
using Slides.SVG;
using SVGLib;
using SVGLib.ContainerElements;
using SVGLib.GraphicsElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SVGGroup = SVGLib.ContainerElements.Group;
using SVGText = SVGLib.GraphicsElements.Text;
using SVGPath = SVGLib.GraphicsElements.Path;
using SVGColor = SVGLib.Datatypes.Color;
using SVGTransform = SVGLib.Datatypes.Transform;
using SVGLib.PathOperations;
using SVGLib.Datatypes;
using Slides.Elements.SVG;

namespace HTMLWriter
{
	internal static class SVGWriter
	{
		internal static void Write(HTMLWriter writer, UnitLine element)
		{
			WriteUnitLine(writer, element);
		}
	
		internal static void Write(HTMLWriter writer, SVGElement element)
		{
			var viewBox = GetViewBox(element);
			if (viewBox.HasValue)
			{
				writer.PushAttribute("width", viewBox.Value.Width.ToString());
				writer.PushAttribute("height", viewBox.Value.Height.ToString());
			}
			if(element.Transform.Any())
			{
				writer.PushAttribute("transform", string.Join<SVGTransform>(";", element.Transform));
			}
			if(element.Fill != SVGColor.Black)
				writer.PushAttribute("fill", CSSWriter.GetValue(element.Fill));
			if(element.Stroke != SVGColor.Transparent)
			writer.PushAttribute("stroke", CSSWriter.GetValue(element.Stroke));
			if(element.StrokeWidth != 0)
				writer.PushAttribute("stroke-width", element.StrokeWidth.ToString());
			switch (element.Kind)
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
				case SVGElementKind.SVGTag:
					WriteSVGTag(writer, (SVGTag)element);
					break;
				default:
					throw new Exception();
			}
		}

		private static void WriteGroup(HTMLWriter writer, SVGGroup element)
		{
			writer.StartTag("g");
			foreach (var child in element.Children)
			{
				Write(writer, (SVGGraphicsElement)child);
			}
			writer.EndTag();
		}

		private static void WriteRect(HTMLWriter writer, Rect element)
		{
			writer.PushAttribute("x", element.X.ToString());
			writer.PushAttribute("y", element.Y.ToString());
			writer.StartTag("rect");
			writer.EndTag();
		}

		private static void WriteCircle(HTMLWriter writer, Circle element)
		{
			writer.PushAttribute("cx", (element.cx + element.X).ToString());
			writer.PushAttribute("cy", (element.cy + element.Y).ToString());
			writer.PushAttribute("r", element.r.ToString());
			writer.StartTag("circle");
			writer.EndTag();
		}

		private static void WriteEllipse(HTMLWriter writer, Ellipse element)
		{
			writer.Equals(element);
			throw new NotImplementedException();
		}

		private static void WriteLine(HTMLWriter writer, Line element)
		{
			writer.PushAttribute("x1", element.X.ToString());
			writer.PushAttribute("y1", element.Y.ToString());
			writer.PushAttribute("x2", (element.X + element.Width).ToString());
			writer.PushAttribute("y2", (element.Y + element.Height).ToString());

			writer.StartTag("line");
			writer.EndTag();
		}


		private static void WriteUnitLine(HTMLWriter writer, UnitLine element)
		{
			writer.PushAttribute("x1", CSSWriter.GetValue(element.X1));
			writer.PushAttribute("y1", CSSWriter.GetValue(element.Y1));
			writer.PushAttribute("x2", CSSWriter.GetValue(element.X2));
			writer.PushAttribute("y2", CSSWriter.GetValue(element.Y2));
			writer.PushAttribute("stroke", CSSWriter.GetValue(element.stroke));
			writer.PushAttribute("stroke-width", CSSWriter.GetValue(element.strokeWidth));
			writer.PushAttribute("stroke-linecap", element.strokeLineCap.ToString().ToLower());

			writer.StartTag("line");
			writer.EndTag();
		}

		private static void WritePath(HTMLWriter writer, SVGPath element)
		{
			var builder = new StringBuilder();
			var operations = PathOperationHelper.Translate(element.Operations, element.X, element.Y);

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
			writer.Equals(element);
			throw new NotImplementedException();
		}

		private static void WritePolyline(HTMLWriter writer, Polyline element)
		{
			writer.Equals(element);
			throw new NotImplementedException();
		}

		private static void WriteSVGText(HTMLWriter writer, SVGText element)
		{
			writer.PushAttribute("x", element.X.ToString());
			writer.PushAttribute("y", element.Y.ToString());
			writer.StartTag("text");
			writer.Write(element.Content);
			writer.EndTag();
		}

		private static void WriteSVGTag(HTMLWriter writer, SVGTag element)
		{
			writer.PushAttribute("viewBox", element.ViewBox.ToString());
			writer.PushAttribute("preserveAspectRatio", $"{element.PreserveAspectRatioAlign} {element.PreserveAspectRatioMeetOrSlice}");
			writer.StartTag("svg");
			foreach (var child in element.Children)
			{
				Write(writer, child);
			}
			writer.EndTag();
		}

		public static ViewBox? GetViewBox(SVGElement element)
		{
			switch (element.Kind)
			{
				case SVGElementKind.Group:
					return null;
				case SVGElementKind.Rect:
				case SVGElementKind.Circle:
				case SVGElementKind.Ellipse:
				case SVGElementKind.Line:
				case SVGElementKind.Path:
				case SVGElementKind.Polygon:
				case SVGElementKind.Polyline:
				case SVGElementKind.Text:
					var graphicsElement = (SVGGraphicsElement)element;
					return new ViewBox(graphicsElement.X, graphicsElement.Y, graphicsElement.Width, graphicsElement.Height);
				case SVGElementKind.SVGTag:
					return ((SVGTag)element).ViewBox;
				default:
					throw new Exception();
			}
		}
	}
}