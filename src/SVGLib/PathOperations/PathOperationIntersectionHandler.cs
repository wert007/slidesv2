using SVGLib.Datatypes;
using SVGLib.GraphicsElements;
using SVGLib.PathOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides.SVG
{
	public enum IntersectionKind
	{
		Point,
		Line,
		None,
		Inside,
	}
	public struct Intersection
	{
		public IntersectionKind Kind { get; private set; }
		public Vector2 Start { get; private set; }
		public Vector2 End { get; private set; }

		public Intersection(Vector2 point)
		{
			Kind = IntersectionKind.Point;
			Start = point;
			End = point;
		}
		public Intersection(Vector2 start, Vector2 end)
		{
			Kind = IntersectionKind.Line;
			if (start == end)
				Kind = IntersectionKind.Point;
			Start = start;
			End = end;
		}

		public static Intersection CreateNoIntersection()
		{
			var result = new Intersection();
			result.Kind = IntersectionKind.None;
			result.Start = Vector2.NaN;
			result.End = Vector2.NaN;
			return result;
		}

		public static Intersection CreateInside(Vector2 point)
		{
			var result = new Intersection(point);
			result.Kind = IntersectionKind.Inside;
			return result;
		}

		public override bool Equals(object obj)
		{
			return obj is Intersection intersection &&
					 Kind == intersection.Kind &&
					 (Kind == IntersectionKind.None || (
					 EqualityComparer<Vector2>.Default.Equals(Start, intersection.Start) &&
					 EqualityComparer<Vector2>.Default.Equals(End, intersection.End)));
		}

		public override int GetHashCode()
		{
			var hashCode = -1596069196;
			hashCode = hashCode * -1521134295 + Kind.GetHashCode();
			hashCode = hashCode * -1521134295 + Start.GetHashCode();
			hashCode = hashCode * -1521134295 + End.GetHashCode();
			return hashCode;
		}

		public override string ToString()
		{
			switch (Kind)
			{
				case IntersectionKind.Point:
					return $"Pt: {Start}";
				case IntersectionKind.Inside:
					return $"In: {Start}";
				case IntersectionKind.Line:
					return $"Line: {Start}, {End}";
				case IntersectionKind.None:
					return "None";
				default:
					return base.ToString();
			}
		}

		public static bool operator ==(Intersection left, Intersection right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Intersection left, Intersection right)
		{
			return !(left == right);
		}
	}
	public struct PathOperationSegment
	{
		public PathOperationSegment(PathOperationKind kind, Vector2 start, Vector2 end)
		{
			Kind = kind;
			Start = start;
			End = end;
		}

		public PathOperationKind Kind { get; }
		public Vector2 Start { get; }
		public Vector2 End { get; }

		public override string ToString()
		{
			return $"{Kind} {Start} {End}";
		}
	}


	public class IntersectionVertex
	{
		public Vector2 Position { get; private set; }
		public IntersectionKind Kind { get; }
		public double Index { get; private set; }
		public IntersectionVertex Next { get; private set; }

		public IntersectionVertex(double index, Vector2 position, IntersectionKind kind)
		{
			Index = index;
			Position = position;
			Kind = kind;
			Next = null;
		}

		public bool SetNext(IntersectionVertex next)
		{
			var result = Next == null;
			if (result) Next = next;
			return result;
		}

		public IntersectionVertex GetPrevious()
		{
			var current = Next;
			while (current.Next != this)
				current = current.Next;
			return current;
		}

		public override string ToString()
		{
			return $"{Index}: {Position}";
		}
	}

	public class Vertex
	{

		public int Id { get; set; }
		public Vector2 Position { get; set; }
		public IntersectionKind Kind { get; set; }
		public PathOperationKind OpKind { get; set; }

		public Vertex(int id, Vector2 pos, IntersectionKind kind, PathOperationKind opKind)
		{
			this.Id = id;
			this.Position = pos;
			this.Kind = kind;
			this.OpKind = opKind;
		}

		public override string ToString()
		{
			return $"{Id}: {Kind}::({OpKind}){Position}";
		}
	}

	public enum PathShapeOperation
	{
		Intersect,
		Unite,
		Difference,
	}

	public static class PathOperationIntersectionHandler
	{
		private static PathOperationSegment[] PathToPathSegments(Path p)
		{
			var op = PathOperationHelper.Translate(PathOperationHelper.ToAbsolute(p.Operations), p.X, p.Y);
			return ToSegment(op);
		}


		public static PathOperation[] PathOperationShape(Path a, Path b, PathShapeOperation kind)
		{
			var result = new List<PathOperation>();
			var aSeg = PathToPathSegments(a);
			var bSeg = PathToPathSegments(b);
			var aVert = FindVertices(aSeg, bSeg);
			var bVert = FindVertices(bSeg, aSeg);
			var aChain = ToIntersectionVertex(aVert);
			var bChain = ToIntersectionVertex(bVert);
			var current = aChain.Next;
			var currentIsA = true;
			IntersectionVertex start = null;
			while (current.Kind != IntersectionKind.Point && current != aChain) current = current.Next;
			
			
			IntersectionVertex Switch(bool isA)
			{
				if (isA)
					return FindEquivalent(current, aChain);
				else
					return FindEquivalent(current, bChain);
			}

			while (true)
			{
				var other = Switch(!currentIsA);
				if (current == start ||
					other == start)
					break;
				switch (kind)
				{
					case PathShapeOperation.Intersect:
						if(current.Kind == IntersectionKind.Point)
						{
							if(current.Next.Kind == IntersectionKind.None ||
								other.Next.Kind == IntersectionKind.Inside)
							{
								currentIsA = !currentIsA;
								current = Switch(currentIsA);
							}
						}
						if (current.Kind == IntersectionKind.None) throw new Exception();
						start = AddIntersectionVertex(current, result, start);
						current = current.Next;
						break;
					case PathShapeOperation.Unite:
						if (current.Kind == IntersectionKind.Point)
						{
							if (current.Next.Kind == IntersectionKind.Inside
								|| other.Next.Kind == IntersectionKind.None)
							{
								currentIsA = !currentIsA;
								current = Switch(currentIsA);
							}
						}
						if (current.Kind == IntersectionKind.Inside) throw new Exception();
						start = AddIntersectionVertex(current, result, start);
						current = current.Next;
						break;
					case PathShapeOperation.Difference:
						if (current.Kind == IntersectionKind.Point &&
							current.Next.Kind == IntersectionKind.Inside)
						{
							currentIsA = !currentIsA;
							current = Switch(currentIsA);
						}
						start = AddIntersectionVertex(current, result, start);
						if (currentIsA)
							current = current.Next;
						else
							current = current.GetPrevious();

						break;
					default:
						throw new Exception();
				}
			}

			result.Add(new ClosePathOperation());
			return result.ToArray();
		}

		private static IntersectionVertex AddIntersectionVertex(IntersectionVertex toAdd, List<PathOperation> result, IntersectionVertex start)
		{
			var pathOperation = PathOperationKind.LineTo;
			if (start == null) 
				pathOperation = PathOperationKind.MoveTo;
			result.Add(new CoordinatePairOperation(false, pathOperation, toAdd.Position.X, toAdd.Position.Y));
			return start ?? toAdd;
		}
		public static PathOperation[] IntersectionShape(Path a, Path b)
		{
			var result = new List<PathOperation>();
			void AddIntersectionVertex(IntersectionVertex v)
			{
				result.Add(new CoordinatePairOperation(false, PathOperationKind.LineTo, v.Position.X, v.Position.Y));
			}
			var aSeg = PathToPathSegments(a);
			var bSeg = PathToPathSegments(b);
			var aVert = FindVertices(aSeg, bSeg).Where(v => v.Kind != IntersectionKind.None).ToArray();
			var bVert = FindVertices(bSeg, aSeg).Where(v => v.Kind != IntersectionKind.None).ToArray();
			var aStart = ToIntersectionVertex(aVert);
			var bStart = ToIntersectionVertex(bVert);
			var aCurrent = aStart.Next;
			var bCurrent = bStart.Next;
			bCurrent = FindEquivalent(aCurrent, bStart);
			while (bCurrent == null && aCurrent != aStart)
			{
				aCurrent = aCurrent.Next;
				bCurrent = FindEquivalent(aCurrent, bStart);
			}
			if (aCurrent == aStart) return a.Operations;
			aStart = aCurrent.GetPrevious();
			result.Add(new CoordinatePairOperation(false, PathOperationKind.MoveTo, aStart.Position.X, aStart.Position.Y));
			while (aStart != aCurrent)
			{
				if (aCurrent.Position == bCurrent.Position)
					AddIntersectionVertex(aCurrent);
				else
				{
					while (FindEquivalent(aCurrent, bCurrent) == null) //aCurrent not in b -> aCurrent is essential
					{
						AddIntersectionVertex(aCurrent);
						aCurrent = aCurrent.Next;
					}
					while (FindEquivalent(bCurrent, aCurrent) == null)//bCurrent not in a -> bCurrent is essential
					{
						AddIntersectionVertex(bCurrent);
						bCurrent = bCurrent.Next;
					}
					AddIntersectionVertex(aCurrent);
				}
				aCurrent = aCurrent.Next;
				bCurrent = bCurrent.Next;
			}
			result.Add(new ClosePathOperation());
			return result.ToArray();
		}
		public static Vertex[] CollectVertices(Path a, Path b)
		{
			var aOp = PathOperationHelper.Translate(PathOperationHelper.ToAbsolute(a.Operations), a.X, a.Y);
			var bOp = PathOperationHelper.Translate(PathOperationHelper.ToAbsolute(b.Operations), b.X, b.Y);
			var aSeg = ToSegment(aOp);
			var bSeg = ToSegment(bOp);
			var vertices = FindVertices(aSeg, bSeg);

			return vertices;
		}

		private static Vertex[] FindVertices(PathOperationSegment[] aSeg, PathOperationSegment[] bSeg)
		{
			var vertices = new List<Vertex>();
			var index = 0;
			void AddVertex(Vertex v)
			{
				var similiar = vertices.Count > 0 && vertices.Last().Position == v.Position ? vertices.Last() : null;
				var shouldAdd = true;
				if (similiar != null)
				{
					if (similiar.Kind == IntersectionKind.None)
					{
						similiar.Kind = v.Kind;
						shouldAdd = false;
					}
					else if (v.Kind == IntersectionKind.None)
					{
						vertices.Remove(similiar);
						v.Kind = similiar.Kind;

					}
					else if (similiar.Kind == IntersectionKind.Inside && v.Kind == IntersectionKind.Point)
					{
						similiar.Kind = v.Kind;
						shouldAdd = false;
					}
					else if (v.Kind == IntersectionKind.Inside && similiar.Kind == IntersectionKind.Point)
					{
						vertices.Remove(similiar);
						v.Kind = similiar.Kind;
					}
					else if (similiar.Kind == v.Kind)
					{
						shouldAdd = false;
					}
					else
						throw new Exception();
				}
				if (shouldAdd)
				{
					vertices.Add(v);
					index++;
				}
			}
			foreach (var aChild in aSeg)
			{
				var inline = PointLiesWithinShape(aChild.Start, bSeg);
				AddVertex(new Vertex(index, aChild.Start, inline.Kind, aChild.Kind));
				var intersections = new HashSet<Intersection>();
				foreach (var bChild in bSeg)
				{

					var children = FindOperationOperationIntersections(aChild, bChild);
					if (children != null)
						foreach (var c in children)
							switch (c.Kind)
							{
								case IntersectionKind.Point:
									if (!intersections.Contains(c))
										intersections.Add(c);
									break;
								case IntersectionKind.Line:
									{
										var start = new Intersection(c.Start);
										var end = new Intersection(c.End);
										if (!intersections.Contains(start))
											intersections.Add(start);
										if (!intersections.Contains(end))
											intersections.Add(end);
									}
									break;
								case IntersectionKind.None:
								case IntersectionKind.Inside:
									break;
								default:
									throw new Exception();
							}
				}
				foreach (var inter in intersections.OrderBy(i => (i.Start - aChild.Start).Length()))
					AddVertex(new Vertex(index, inter.Start, inter.Kind, PathOperationKind.LineTo));
				inline = PointLiesWithinShape(aChild.End, bSeg);
				AddVertex(new Vertex(index, aChild.End, inline.Kind, aChild.Kind));
			}

			return vertices.ToArray();
		}

		public static Intersection[] FindIntersections(Path a, Path b)
		{
			var aOp = PathOperationHelper.Translate(a.Operations, a.X, a.Y);
			var bOp = PathOperationHelper.Translate(b.Operations, b.X, b.Y);
			var aSeg = ToSegment(aOp);
			var bSeg = ToSegment(bOp);
			var result = new HashSet<Intersection>();

			foreach (var aChild in aSeg)
			{
				foreach (var bChild in bSeg)
				{
					var inside = PointLiesWithinShape(aChild.Start, bSeg);
					if (!result.Contains(inside) && inside.Kind != IntersectionKind.None)
						result.Add(inside);
					inside = PointLiesWithinShape(bChild.Start, aSeg);
					if (!result.Contains(inside) && inside.Kind != IntersectionKind.None)
						result.Add(inside);
					var children = FindOperationOperationIntersections(aChild, bChild);
					if (children != null)
						foreach (var c in children)
							if (!result.Contains(c) && c.Kind != IntersectionKind.None)
								result.Add(c);

					inside = PointLiesWithinShape(aChild.End, bSeg);
					if (!result.Contains(inside) && inside.Kind != IntersectionKind.None)
						result.Add(inside);
					inside = PointLiesWithinShape(bChild.End, aSeg);
					if (!result.Contains(inside) && inside.Kind != IntersectionKind.None)
						result.Add(inside);
				}
			}

			return result.ToArray();
		}


		private static IntersectionVertex ToIntersectionVertex(Vertex[] vertices)
		{
			var start = new IntersectionVertex(0, vertices[0].Position, vertices[0].Kind);
			var current = start;
			for (int i = 1; i < vertices.Length; i++)
			{
				var newIV = new IntersectionVertex((double)i, vertices[i].Position, vertices[i].Kind);
				current.SetNext(newIV);
				current = newIV;
			}
			current.SetNext(start);
			return start;
		}

		private static IntersectionVertex FindEquivalent(IntersectionVertex target, IntersectionVertex source)
		{
			var sourceStart = source;
			if (sourceStart.Position == target.Position)
				return sourceStart;
			var sourceCurrent = sourceStart.Next;
			while (sourceStart != sourceCurrent)
			{
				if (sourceCurrent.Position == target.Position)
					return sourceCurrent;
				sourceCurrent = sourceCurrent.Next;
			}

			return null;
		}

		private static PathOperationSegment[] ToSegment(PathOperation[] operations)
		{
			var result = new List<PathOperationSegment>();
			var coord = new Vector2();
			var init = Vector2.NaN;
			for (int i = 0; i < operations.Length; i++)
			{
				var segment = ToSegment(operations[i], coord, init);
				if (operations[i].Kind != PathOperationKind.MoveTo)
				{
					if (Vector2.IsNaN(init))
						init = coord;
					result.Add(segment);
				}
				coord = segment.End;
			}
			return result.ToArray();
		}

		private static PathOperationSegment ToSegment(PathOperation operation, Vector2 coord, Vector2 init)
		{
			var end = PathOperationHelper.GetCoordinate(operation, coord);
			if (Vector2.IsNaN(end))
				end = init;
			return new PathOperationSegment(operation.Kind, coord, end);
		}

		private static double RelativePositionTo(Vector2 point, PathOperationSegment line)
		{
			return ((line.End.X - line.Start.X) * (point.Y - line.Start.Y)
						- (point.X - line.Start.X) * (line.End.Y - line.Start.Y));
		}

		public static Intersection PointLiesWithinShape(Vector2 point, PathOperationSegment[] shape)
		{
			int wn = 0;    // the  winding number counter

			for (int i = 0; i < shape.Length; i++)
			{
				if (shape[i].Start.Y <= point.Y)
				{
					if (shape[i].End.Y > point.Y)
						if (RelativePositionTo(point, shape[i]) > 0)
							wn++;
				}
				else
				{
					if (shape[i].End.Y <= point.Y)
						if (RelativePositionTo(point, shape[i]) < 0)
							wn--;
				}
			}

			if (wn == 0)
				return Intersection.CreateNoIntersection();
			return Intersection.CreateInside(point);
		}

		public static Intersection[] FindOperationOperationIntersections(PathOperationSegment a, PathOperationSegment b)
		{
			switch (a.Kind)
			{
				case PathOperationKind.MoveTo:
					return new Intersection[] {
							Intersection.CreateNoIntersection()
						};
				case PathOperationKind.ClosePath:
				case PathOperationKind.LineTo:
				case PathOperationKind.LineToHorizontal:
				case PathOperationKind.LineToVertical:
					return FindLineOperationIntersection(a, b);
				case PathOperationKind.CurveTo:
				case PathOperationKind.SmoothCurveTo:
				case PathOperationKind.QuadraticBezierCurveTo:
				case PathOperationKind.SmoothQuadraticBezierCurveTo:
				case PathOperationKind.EllipticalArc:
				case PathOperationKind.Error:
				default:
					throw new Exception();
			}
		}

		private static Intersection[] FindLineOperationIntersection(PathOperationSegment a, PathOperationSegment b)
		{
			switch (b.Kind)
			{
				case PathOperationKind.MoveTo:
					return new Intersection[] {
							Intersection.CreateNoIntersection()
						};
				case PathOperationKind.ClosePath:
				case PathOperationKind.LineTo:
				case PathOperationKind.LineToHorizontal:
				case PathOperationKind.LineToVertical:
					return FindLineLineIntersection(a, b);
				case PathOperationKind.CurveTo:
				case PathOperationKind.SmoothCurveTo:
				case PathOperationKind.QuadraticBezierCurveTo:
				case PathOperationKind.SmoothQuadraticBezierCurveTo:
				case PathOperationKind.EllipticalArc:
				case PathOperationKind.Error:
				default:
					throw new Exception();
			}
		}
		struct thingything
		{
			public double Length { get; }
			public Vector2 Start { get; }
			public Vector2 End { get; }
			public thingything(Vector2 start, Vector2 end)
			{
				Start = start;
				End = end;
				Length = (end - start).Length();
			}
		}
		private static Intersection[] FindLineLineIntersection(PathOperationSegment a, PathOperationSegment b)
		{
			var aMin = Vec2XMin(a.Start, a.End);
			var bMin = Vec2XMin(b.Start, b.End);
			var aMax = a.End;
			if (aMin == a.End)
				aMax = a.Start;
			var bMax = b.End;
			if (bMin == b.End)
				bMax = b.Start;
			if (aMin == bMin && aMax == bMax)
				return new Intersection[]
					{
							new Intersection(aMin, aMax),
					};
			if (aMin.X > bMax.X || aMax.X < bMin.X)
				return ToArray(Intersection.CreateNoIntersection());
			if (Math.Max(aMin.Y, aMax.Y) < Math.Min(bMin.Y, bMax.Y))
				return ToArray(Intersection.CreateNoIntersection());
			if (Math.Min(aMin.Y, aMax.Y) > Math.Max(bMin.Y, bMax.Y))
				return ToArray(Intersection.CreateNoIntersection());

			var denom = ((aMin.X - aMax.X) * (bMin.Y - bMax.Y) - (aMin.Y - aMax.Y) * (bMin.X - bMax.X));

			if (denom == 0)
			{
				var d = DistanceBetween(aMin, b);
				if (d == 0)
				{
					//TODO! Could it happen, that we need aMin and bMax
					//or bMin and aMax? Yeah it could.
					var options = new thingything[4];
					options[0] = new thingything(aMin, aMax);
					options[1] = new thingything(aMin, bMax);
					options[2] = new thingything(bMin, bMax);
					options[3] = new thingything(bMin, aMax);
					var shortest = options.OrderBy(o => o.Length).First();
					return ToArray(new Intersection(shortest.Start, shortest.End));
				}
				return ToArray(Intersection.CreateNoIntersection());
			}

			var t = ((aMin.X - bMin.X) * (bMin.Y - bMax.Y) - (aMin.Y - bMin.Y) * (bMin.X - bMax.X)) / denom;

			var u = -((aMin.X - aMax.X) * (aMin.Y - bMin.Y) - (aMin.Y - aMax.Y) * (aMin.X - bMin.X)) / denom;
			//In an ideal world!
			if (t < 0 || t > 1 || u < 0 || u > 1)
				//if(t < -0.1 || t > 1.1 || u < -0.1 || u > 1.1)
				return ToArray(Intersection.CreateNoIntersection());
			return ToArray(new Intersection(
				new Vector2(aMin.X + t * (aMax.X - aMin.X), aMin.Y + t * (aMax.Y - aMin.Y))
			));
		}

		private static Intersection[] ToArray(Intersection intersection) => new Intersection[]
			{
				intersection
			};

		private static float DistanceBetween(Vector2 p, PathOperationSegment g)
		{
			var dir = g.End - g.Start;
			var denom = Math.Sqrt(dir.Length());
			if (denom == 0)
				return float.NaN;
			var top = Math.Abs(dir.Y * p.X - dir.X * p.Y + g.End.X * g.Start.Y - g.End.Y * g.Start.X);
			return (float)(top / denom);
		}

		private static Vector2 Vec2XMin(Vector2 a, Vector2 b)
		{
			if (a.X < b.X)
				return a;
			if (a.X == b.X)
			{
				if (a.Y < b.Y)
					return a;
				return b;
			}
			return b;
		}
		private static Vector2 Vec2XMax(Vector2 a, Vector2 b)
		{
			if (a.X > b.X)
				return a;
			if (a.X == b.X)
			{
				if (a.Y > b.Y)
					return a;
				return b;
			}
			return b;
		}
	}
}
