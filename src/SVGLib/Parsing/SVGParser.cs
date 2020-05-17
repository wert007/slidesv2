using SVGLib.ContainerElements;
using SVGLib.Datatypes;
using SVGLib.GraphicsElements;
using SVGLib.PathOperations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SVGLib.Parsing
{
	public class SVGParser
	{

		public SVGTag FromSource(string source)
		{
			var doc = new XmlDocument();
			doc.LoadXml(source);
			return LoadSVGTag(doc.DocumentElement);
		}

		private SVGElement LoadNode(XmlNode node)
		{
			switch (node.Name)
			{
				case "svg":
					return LoadSVGTag(node);
				case "g":
					return LoadGTag(node);
				case "path":
					return LoadPathTag(node);
				case "defs":
				case "metadata":
					return null;
				default:
					if(!node.Name.Contains(":"))
						Console.WriteLine($"(SVGLib) WARNING: Unexpected XmlNode {node.Name}.");
					return null;
			}
		}

		public static PathOperation[] ParsePathInstructions(string m)
		{
			var p = SVGPathParser.Create(m);
			return p.Parse();
		}

		private Path LoadPathTag(XmlNode node)
		{
			var width = 5;
			var height = 6;
			var d = node.Attributes["d"]?.Value;
			var operations = ParsePathInstructions(d);
			var result = new Path(operations, width, height);
			ParseStyle(result, node.Attributes["style"]?.Value);
			ParseTransform(result, node.Attributes["transform"]?.Value);
			return result;
		}


		private Group LoadGTag(XmlNode node)
		{
			var children = new List<SVGElement>();
			foreach (XmlNode childNode in node.ChildNodes)
			{
				var element = LoadNode(childNode);
				if (element != null) children.Add(element);
			}
			var result = new Group(children.ToArray());
			ParseStyle(result, node.Attributes["style"]?.Value);
			ParseTransform(result, node.Attributes["transform"]?.Value);
			return result;
		}

		private SVGTag LoadSVGTag(XmlNode node)
		{
			var viewBox = new ViewBox();
			var viewBoxAttribute = node.Attributes["viewBox"];
			if (viewBoxAttribute != null)
			{
				viewBox = ParseViewBox(viewBoxAttribute.Value);
			}
			var children = new List<SVGElement>();
			foreach (XmlNode childNode in node.ChildNodes)
			{
				var element = LoadNode(childNode);
				if (element != null)
					children.Add(element);
			}
			var result = new SVGTag(viewBox, children.ToArray());
			ParseStyle(result, node.Attributes["style"]?.Value);
			ParseTransform(result, node.Attributes["transform"]?.Value);
			return result;
		}



		private void ParseStyle(SVGElement element, string style)
		{
			if (style == null) return;
			var styles = style.Split(';');
			foreach (var s in styles)
			{
				var name = s.Split(':')[0];
				var value = s.Split(':')[1];
				switch (name)
				{
					case "fill":
						element.Fill = ParseColor(value);
						break;
					case "fill-opacity":
						//TODO: Hack
						if (double.TryParse(value, NumberStyles.Float, TextHelper.UsCulture, out var opacity))
							element.Fill = Color.FromAlphaColor(element.Fill, opacity);
						break;
						//TODO: FIXME
					case "fill-rule":  break;
					case "stroke":
						element.Stroke = ParseColor(value);
						break;
					case "stroke-width":
						if (double.TryParse(value, NumberStyles.Float, TextHelper.UsCulture, out var strokeWidth))
							element.StrokeWidth = strokeWidth;
						break;
					case "stroke-linecap":break;
					case "stroke-miterlimit": break;
					case "stroke-linejoin": break;
					default:
						//TODO: FIXME
						throw new NotImplementedException();
				}
			}
		}

		private void ParseTransform(SVGElement element, string transformSource)
		{
			if (transformSource == null) return;
			foreach (var transformOperation in transformSource.Split(';'))
			{
				var transformOpTrim = transformOperation.Trim();
				var name = transformOpTrim.Split('(')[0];
				var parameterStr = transformOpTrim.Remove(transformOpTrim.Length - 2).Substring(name.Length + 1);
				var parameters = parameterStr.Split(',').Select(p => p.Trim()).ToArray();

				switch (name)
				{
					case "translate":
						double y = 0;
						double x = 0;
						double.TryParse(parameters[0], NumberStyles.Float, TextHelper.UsCulture, out x);
						if (parameters.Length > 1)
							double.TryParse(parameters[1], NumberStyles.Float, TextHelper.UsCulture, out y);
						element.AddTransform(Transform.CreateTranslation(x, y));


						break;
					default:
						throw new NotImplementedException();
				}
			}
		}

		private Color ParseColor(string value)
		{
			if (value == "none") return Color.Transparent;
			value = value.Substring(1); //Consume #
			var singleValueLength = value.Length / 3;
			var rgb = new byte[3];
			for (int i = 0; i < rgb.Length; i++)
			{
				var singleValueStr = value;
				if (singleValueLength < value.Length)
				{
					singleValueStr = value.Remove(singleValueLength);
					value = value.Substring(singleValueLength);
				}
				rgb[i] = Convert.ToByte(singleValueStr, 16);
			}
			return Color.FromRGB(rgb[0], rgb[1], rgb[2]);
		}


		private ViewBox ParseViewBox(string value)
		{
			var values = value.Split(' ').Select(v => double.Parse(v, TextHelper.UsCulture)).ToArray();
			if (values.Length == 2) return new ViewBox(values[0], values[1]);
			return new ViewBox(values[0], values[1], values[2], values[3]);
		}




		private class SVGPathParser
		{
			private static CultureInfo _usCulture = CultureInfo.CreateSpecificCulture("US-us");

			private readonly Token[] _tokens;

			private SVGPathParser(Token[] tokens)
			{
				_tokens = tokens;
			}

			private class Token
			{
				public string Text { get; set; }
				public bool IsNumber { get; set; }

				public Token(string text, bool isNumber)
				{
					Text = text;
					IsNumber = isNumber;
				}

				public override string ToString() => Text;
			}

			private static char GetChar(string source, int index)
			{
				if (index >= source.Length) return '\0';
				return source[index];
			}

			public static SVGPathParser Create(string source)
			{
				var tokens = new List<Token>();
				var m = source;
				int i = 0;
				while (i < m.Length)
				{
					while (char.IsWhiteSpace(GetChar(m, i)))
						i++;
					if (GetChar(m,i) == ',') i++; //Not 100% correct. you can't have a ',' everywhere
					while (char.IsWhiteSpace(GetChar(m, i)))
						i++;
					if (char.IsDigit(GetChar(m, i)) || GetChar(m, i) == '-' || GetChar(m, i) == '.')
					{
						int start = i;
						if (GetChar(m, i) == '-') i++;
						while (char.IsDigit(GetChar(m, i))) i++;
						if (GetChar(m, i) == '.') i++;
						while (char.IsDigit(GetChar(m, i))) i++;
						tokens.Add(new Token(m.Substring(start, i - start), true));
					}
					else if (char.IsLetter(GetChar(m, i)))
					{
						i++;
						tokens.Add(new Token(m[i - 1].ToString(), false));
					}
				}
				return new SVGPathParser(tokens.ToArray());
			}

			public PathOperation[] Parse()
			{
				var operations = new List<PathOperation>();
				var lastOp = 'M';
				for (int i = 0; i < _tokens.Length; i++)
				{
					var cur = lastOp;
					if (!_tokens[i].IsNumber)
					{
						cur = _tokens[i].Text[0];
						lastOp = cur;
					}
					else i--;
					var isRelative = char.IsLower(cur);
					switch (cur)
					{
						case 'M':
						case 'm':
							{
								if (!float.TryParse(_tokens[++i].Text, NumberStyles.Float, _usCulture, out var x))
									continue;
								if (!float.TryParse(_tokens[++i].Text, NumberStyles.Float, _usCulture, out var y))
									continue;
								operations.Add(new CoordinatePairOperation(isRelative, PathOperationKind.MoveTo, x, y));
								lastOp = (char)(cur + ('L' - 'M'));
							}
							break;
						case 'H':
						case 'h':
							{
								if (!float.TryParse(_tokens[++i].Text, NumberStyles.Float, _usCulture, out var v))
									continue;
								operations.Add(new SingleCoordinateOperation(isRelative, PathOperationKind.LineToHorizontal, v));
							}
							break;
						case 'V':
						case 'v':
							{
								if (!float.TryParse(_tokens[++i].Text, NumberStyles.Float, _usCulture, out var v))
									continue;
								operations.Add(new SingleCoordinateOperation(isRelative, PathOperationKind.LineToVertical, v));
							}
							break;
						case 'L':
						case 'l':
							{
								if (!float.TryParse(_tokens[++i].Text, NumberStyles.Float, _usCulture, out var x))
									continue;
								if (!float.TryParse(_tokens[++i].Text, NumberStyles.Float, _usCulture, out var y))
									continue;
								operations.Add(new CoordinatePairOperation(isRelative, PathOperationKind.LineTo, x, y));
							}
							break;
						case 'Z':
						case 'z':
							{
								operations.Add(new ClosePathOperation());
							}
							break;
						default:
							throw new NotImplementedException();

					}
				}
				return operations.ToArray();
			}
		}
	}
}
