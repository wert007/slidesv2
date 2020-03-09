using ImageMagick;
using Slides;
using Slides.SVG;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Minsk.CodeAnalysis.SlidesTypes
{

	public static class GlobalSVGFunctions
	{
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
			public static SVGPathParser Create(string source)
			{
				var tokens = new List<Token>();
				var m = source;
				int i = 0;
				while (i < m.Length)
				{
					while (char.IsWhiteSpace(m[i]))
						i++;
					if (m[i] == ',') i++; //Not 100% correct. you can't have a ',' everywhere
					while (char.IsWhiteSpace(m[i]))
						i++;
					if (char.IsDigit(m[i]) || m[i] == '-' || m[i] == '.')
					{
						int start = i;
						if (m[i] == '-') i++;
						while (char.IsDigit(m[i])) i++;
						if (m[i] == '.') i++;
						while (char.IsDigit(m[i])) i++;
						tokens.Add(new Token(m.Substring(start, i - start), true));
					}
					else if (char.IsLetter(m[i]))
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
		public static SVGPath path(string m, int width, int height)
		{
			var p = SVGPathParser.Create(m);
			return new SVGPath(p.Parse(), width, height);
		}

		public static SVGPath arrow(Direction direction, int arrowLength, int arrowWidth, float baseWidthPercent, float arrowHeadLengthPercent)
		{
			var initWidth = arrowWidth;
			var initHeight = arrowLength;
			if (direction == Direction.East || direction == Direction.West)
			{
				initWidth = arrowLength;
				initHeight = arrowWidth;
			}
			var result = new SVGPath(initWidth, initHeight);
			var width = initWidth;
			var height = initHeight;
			var baseWidth = baseWidthPercent * arrowWidth;
			var arrowHeadLength = arrowHeadLengthPercent * arrowLength;
			float ax, ay, bx, by, cx, cy, dx, dy, ex, ey, fx, fy, gx, gy;

			switch (direction)
			{
				case Direction.North:
					ax = (width - baseWidth) / 2;
					ay = height;
					bx = ax;
					by = arrowHeadLength;
					cx = 0;
					cy = by;
					dx = width / 2;
					dy = 0;
					ex = width;
					ey = by;
					fx = width - ax;
					fy = by;
					gx = fx;
					gy = height;
					break;
				case Direction.South:
					ax = width - (width - baseWidth) / 2;
					ay = 0;
					bx = ax;
					by = height - arrowHeadLength;
					cx = width;
					cy = by;
					dx = width / 2;
					dy = height;
					ex = 0;
					ey = by;
					fx = (width - baseWidth) / 2;
					fy = by;
					gx = fx;
					gy = 0;
					break;
				case Direction.West:
					ax = width;
					ay = height - (height - baseWidth) / 2;
					bx = arrowHeadLength;
					by = ay;
					cx = bx;
					cy = height;
					dx = 0;
					dy = height / 2;
					ex = bx;
					ey = 0;
					fx = ex;
					fy = (height - baseWidth) / 2;
					gx = width;
					gy = fy;
					break;
				case Direction.East:
					ax = 0;
					ay = (height - baseWidth) / 2;
					bx = width - arrowHeadLength;
					by = ay;
					cx = bx;
					cy = 0;
					dx = width;
					dy = height / 2;
					ex = bx;
					ey = height;
					fx = bx;
					fy = height - (height - baseWidth) / 2;
					gx = 0;
					gy = fy;
					break;
				default:
					throw new Exception();
			}

			result.add(new CoordinatePairOperation(false, PathOperationKind.MoveTo, ax, ay));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, bx, by));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, cx, cy));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, dx, dy));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, ex, ey));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, fx, fy));
			result.add(new CoordinatePairOperation(false, PathOperationKind.LineTo, gx, gy));
			result.add(new ClosePathOperation());
			return result;
		}

		public static SVGPath combinePaths(SVGPath a, SVGPath b, int width, int height)
		{
			//TODO: Find intersection points!
			var intersections = PathOperationIntersectionHandler.FindIntersections(a, b);
			return new SVGPath(a.Operations.Concat(b.Operations).ToArray(), width, height);
		}
		class Foo
		{
			Vertex A { get; }
			Vertex B { get; }
			Vector2 Position { get; }
		}

		public static SVGElement[] highlightIntersectionOp(SVGPath a, SVGPath b)
		{
			var aVertices = PathOperationIntersectionHandler.CollectVertices(a, b);
			var bVertices = PathOperationIntersectionHandler.CollectVertices(b, a);
			for (int i = 0; i < bVertices.Length; i++)
				bVertices[i].Id += aVertices.Length;
			var totalVertices = aVertices.Concat(bVertices).ToList();
			var removed = 0;
			for (int i = 0; i < totalVertices.Count;)
			{
				var current = totalVertices[i];
				if (current.Kind == IntersectionKind.None)
				{
					totalVertices.RemoveAt(i);
					removed++;
				}
				else
				{
					current.Id -= removed;
					totalVertices[i] = current;
					i++;
				}
			}
			totalVertices = ClearVertices(totalVertices);
			var v = FindVVVV(totalVertices.ToArray()); // IntersectVerticies(totalVertices);
																	 //FindUniqueVertices(v);
			var result = new List<SVGElement>();
			for (int i = 0; i < v.Length; i++)
			{
				var x = (int)v[i].Position.X;
				var y = (int)v[i].Position.Y;
				var c = new Circle(x, y, 5);
				c.fill = new Color(255, 0, 127, 255);
				result.Add(c);
				var t = new SVGText($"{v[i].Id}");
				t.x = x;
				t.y = y;
				t.fill = new Color(0, 0, 0, 255);
				result.Add(t);
			}
			return result.ToArray();
		}
		public static SVGElement[] highlightIntersectionOp2(SVGPath a, SVGPath b)
		{
			var aVertices = PathOperationIntersectionHandler.CollectVertices(a, b);
			var bVertices = PathOperationIntersectionHandler.CollectVertices(b, a);
			for (int i = 0; i < bVertices.Length; i++)
				bVertices[i].Id += aVertices.Length;
			var totalVertices = aVertices.Concat(bVertices).ToList();
			var removed = 0;
			for (int i = 0; i < totalVertices.Count;)
			{
				var current = totalVertices[i];
				if (current.Kind == IntersectionKind.None)
				{
					totalVertices.RemoveAt(i);
					removed++;
				}
				else
				{
					current.Id -= removed;
					totalVertices[i] = current;
					i++;
				}
			}
			totalVertices = ClearVertices(totalVertices);
			var v = totalVertices.ToArray();
			var result = new List<SVGElement>();
			for (int i = 0; i < v.Length; i++)
			{
				var x = (int)v[i].Position.X;
				var y = (int)v[i].Position.Y;
				var c = new Circle(x, y, 5);
				c.fill = new Color(255, 0, 127, 255);
				result.Add(c);
				var t = new SVGText($"{v[i].Id}");
				t.x = x;
				t.y = y;
				t.fill = new Color(0, 0, 0, 255);
				result.Add(t);
			}
			return result.ToArray();
		}

		public static SVGPath intersectPaths(SVGPath a, SVGPath b)
		{
			var aVertices = PathOperationIntersectionHandler.CollectVertices(a, b);
			var bVertices = PathOperationIntersectionHandler.CollectVertices(b, a);
			//return PathFromVertices(bVertices);
			for (int i = 0; i < bVertices.Length; i++)
			{
				bVertices[i].Id += aVertices.Length;
			}
			var totalVertices = aVertices.Concat(bVertices).ToList();
			for (int i = totalVertices.Count - 1; i >= 0; i--)
			{
				if (totalVertices[i].Kind == IntersectionKind.None)
					totalVertices.RemoveAt(i);
			}
			totalVertices = ClearVertices(totalVertices);

			var v = FindVVVV(totalVertices.ToArray()); // IntersectVerticies(totalVertices);
			return PathFromVertices(v);
		}

		private static int FindPartner(int i, Vertex[] vertices)
		{
			i = (i + 100 * vertices.Length) % vertices.Length;
			for (int j = 0; j < vertices.Length; j++)
			{
				if (i == j) continue;
				if (vertices[i].Position == vertices[j].Position)
					return j;
			}
			return -1;
		}

		class UniqueVertexLine
		{
			public UniqueVertexLine(Vertex[] source, Vertex prev, Vertex next, Vertex altPrev, Vertex altNext)
			{
				Source = source;
				Prev = prev;
				Next = next;
				AltPrev = altPrev;
				AltNext = altNext;
			}
			public UniqueVertexLine(Vertex source, Vertex prev, Vertex next, Vertex altPrev, Vertex altNext)
			{
				Source = new Vertex[] { source };
				Prev = prev;
				Next = next;
				AltPrev = altPrev;
				AltNext = altNext;
			}

			public Vertex[] Source { get; }
			public Vertex Prev { get; }
			public Vertex Next { get; }
			public Vertex AltPrev { get; }
			public Vertex AltNext { get; }

		}

		private static Vertex[] FindVVVV(Vertex[] vertices)
		{
			var uniqueVertices = FindUniqueVertices(vertices).ToList();
			var neededVertices = new List<Vertex>();
			for (int i = 0; i < uniqueVertices.Count; i++)
			{
				var uniqueVertex = uniqueVertices[i];
				foreach (var otherUniqueVertex in uniqueVertices)
				{
					if (uniqueVertex == otherUniqueVertex)
						continue;
					if (uniqueVertex.Prev == otherUniqueVertex.Next ||
						uniqueVertex.Prev == otherUniqueVertex.AltNext)
					{
						neededVertices.Add(uniqueVertex.Prev);
						foreach (var s in uniqueVertex.Source)
							neededVertices.Add(s);
						neededVertices.Add(uniqueVertex.Next);
						uniqueVertices.RemoveAt(i);
						i--;
						break;
					}
					else if (uniqueVertex.AltPrev == otherUniqueVertex.Next ||
						uniqueVertex.AltPrev == otherUniqueVertex.AltNext)
					{
						neededVertices.Add(uniqueVertex.AltPrev);
						foreach (var s in uniqueVertex.Source)
							neededVertices.Add(s);
						neededVertices.Add(uniqueVertex.AltNext);
						uniqueVertices.RemoveAt(i);
						i--;
						break;
					}

				}
			}
			foreach (var uniqueVertex in uniqueVertices)
			{
				if (!neededVertices.Contains(uniqueVertex.Prev))
					neededVertices.Add(uniqueVertex.Prev);
				foreach (var s in uniqueVertex.Source)
					if (!neededVertices.Contains(s))
						neededVertices.Add(s);
				if (!neededVertices.Contains(uniqueVertex.Next))
					neededVertices.Add(uniqueVertex.Next);
			}
			neededVertices = neededVertices.OrderBy(nv => nv.Id).ToList();
			for (int i = 0; i < neededVertices.Count; i++)
			{
				neededVertices[i].Id = i + 1;
			}
			return neededVertices.ToArray();
		}

		private static UniqueVertexLine[] FindUniqueVertices(Vertex[] vertices)
		{
			var result = new List<UniqueVertexLine>();
			for (int i = 0; i < vertices.Length; i++)
			{

				var isUnique = true;
				for (int j = 0; j < vertices.Length; j++)
				{
					if (i == j)
						continue;
					if (vertices[i].Position == vertices[j].Position)
					{
						isUnique = false;
						break;
					}
				}
				if (isUnique)
					result.Add(new UniqueVertexLine(vertices[i], vertices[i - 1], vertices[i + 1], GetAlternate(i - 1, vertices), GetAlternate(i + 1, vertices)));
			}
			return result.ToArray();
		}

		private static Vertex GetAlternate(int index, Vertex[] source)
		{
			if (index < 0 || index >= source.Length)
				return null;
			for (int i = 0; i < source.Length; i++)
			{
				if (i == index)
					continue;
				if (source[i].Position == source[index].Position)
					return source[i];
			}
			return source[index];
		}

		private static Vertex[] IntersectVerticies(List<Vertex> vertices)
		{
			var result = new List<Vertex>();
			var idInc = 0;
			int id;
			vertices = vertices.OrderBy(v => v.Id).ToList();
			for (int i = 0; i < vertices.Count; i++)
			{
				var isUnique = true;
				for (int j = 0; j < vertices.Count; j++)
				{
					if (i == j)
						break;
					//This is double
					if (vertices[i].Position == vertices[j].Position)
					{
						isUnique = false;
						break;
					}
				}
				if (!isUnique)
					continue;
				var prev = -1;
				var otherIndex = i - 1;
				while (prev == -1)
				{
					prev = FindPartner(otherIndex, vertices.ToArray());
					otherIndex = (otherIndex - 1 + vertices.Count) % vertices.Count;
				}
				otherIndex = (otherIndex + 1 + vertices.Count) % vertices.Count;
				id = Math.Min(vertices[prev].Id, vertices[otherIndex].Id) + idInc;
				var toRemove = new List<int>();
				toRemove.Add(otherIndex);
				var next = -1;
				var offset = 1;
				while (next == -1)
				{
					next = FindPartner(i + offset, vertices.ToArray());
					offset++;
				}
				offset--;
				toRemove.Add(i + offset);
				vertices[prev].Id = id++;
				vertices[i].Id = id++;
				var minEndId = Math.Min(vertices[next].Id, vertices[i + offset].Id);
				vertices[next].Id = id++;
				idInc += Math.Max(0, id - minEndId);
				result.Add(vertices[prev]);
				result.Add(vertices[i]);
				result.Add(vertices[next]);
				toRemove.Add(prev);
				toRemove.Add(i);
				toRemove.Add(next);
				toRemove.Sort();
				toRemove.Reverse();
				for (int u = 0; u < toRemove.Count; u++)
				{
					for (int t = toRemove.Count - 1; t > 0; t--)
					{
						if (t == u)
							continue;
						if (toRemove[t] == toRemove[u])
							toRemove.RemoveAt(t);
					}
				}
				foreach (var toR in toRemove)
				{
					vertices.RemoveAt(toR);
				}
			}
			foreach (var _v in vertices)
			{
				result.Add(_v);
			}
			return result.ToArray();
		}

		private static List<Vertex> ClearVertices(List<Vertex> totalVertices)
		{
			var v = totalVertices.OrderBy(vertex => vertex.Id).ToList();
			var oldCount = totalVertices.Count;
			var removed = 0;
			for (int i = 0; i < v.Count - 1;)
			{
				var current = v[i];
				if (current.Position == v[i + 1].Position)
				{
					v.RemoveAt(i);
					removed++;
				}
				else
				{
					current.Id -= removed;
					v[i] = current;
					i++;
				}
			}
			if (oldCount != v.Count)
				return ClearVertices(v);
			return v;
		}

		private static SVGPath PathFromVertices(Vertex[] vertieces)
		{
			var operations = new List<PathOperation>();
			var width = 0;
			var height = 0;
			foreach (var vertex in vertieces.OrderBy(v => v.Id))
			{
				var opKind = vertex.OpKind;
				if (opKind == PathOperationKind.ClosePath)
					opKind = PathOperationKind.LineTo;
				operations.Add(new CoordinatePairOperation(false, opKind, vertex.Position.X, vertex.Position.Y));
				width = (int)Math.Max(vertex.Position.X, width);
				height = (int)Math.Max(vertex.Position.Y, height);
			}
			operations.Add(new ClosePathOperation());
			return new SVGPath(operations.ToArray(), width, height);
		}

		public static SVGPath badCut(SVGPath a, SVGPath b)
		{
			var intersections = PathOperationIntersectionHandler.FindIntersections(a, b);

			var operations = new List<PathOperation>();
			foreach (var i in intersections)
			{
				if (operations.Count == 0)
					operations.Add(new CoordinatePairOperation(false, PathOperationKind.MoveTo, i.End.X, i.End.Y));
				else
					operations.Add(new CoordinatePairOperation(false, PathOperationKind.LineTo, i.End.X, i.End.Y));
			}
			operations.Add(new ClosePathOperation());
			//TODO!
			return new SVGPath(operations.ToArray(), a.width, a.height);
		}

		public static SVGShape[] highlightIntersections(SVGPath a, SVGPath b, int x, int y)
		{
			var intersections = PathOperationIntersectionHandler.FindIntersections(a, b).ToList();

			var resultIntersections = new List<SVGShape>(); // new SVGShape[intersections.Length];
			for (int i = 0; i < intersections.Count; i++)
			{
				var current = intersections.ElementAt(i);
				SVGShape result = null;
				switch (current.Kind)
				{
					case IntersectionKind.Inside:
					case IntersectionKind.Point:
						result = new Circle((int)current.Start.X, (int)current.Start.Y, 5);
						break;
					case IntersectionKind.Line:
						result = new Line((int)current.Start.X, (int)current.Start.Y, (int)current.End.X, (int)current.End.Y);
						result.strokeWidth = 5;
						break;
				}

				result.fill = new Color(255, 0, 0, 255);
				result.stroke = new Color(255, 0, 0, 255);
				if (current.Kind == IntersectionKind.Inside)
				{
					result.fill = new Color(0, 0, 255, 255);
					result.stroke = new Color(0, 0, 255, 255);
				}
				result.x += x;
				result.y += y;

				resultIntersections.Add(result);
			}
			return resultIntersections.ToArray();
		}

		public static ImageSource svg(string fileName)
		{
			var path = Path.Combine(CompilationFlags.Directory, fileName);
			var result = new ImageSource(path, true);
			using (var image = new MagickImage(path))
			{
				result.width = image.BaseWidth;
				result.height = image.BaseHeight;
			}
			return result;

		}

	}
}
