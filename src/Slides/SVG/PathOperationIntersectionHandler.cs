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

	public static class PathOperationIntersectionHandler
	{
		public static Vertex[] CollectVertices(SVGPath a, SVGPath b)
		{
			var aOp = PathOperationHelper.Translate(PathOperationHelper.ToAbsolute(a.Operations), a.x, a.y);
			var bOp = PathOperationHelper.Translate(PathOperationHelper.ToAbsolute(b.Operations), b.x, b.y);
			var aSeg = ToSegment(aOp);
			var bSeg = ToSegment(bOp);
			var vertices = new List<Vertex>();
			var index = 0;
			foreach (var aChild in aSeg)
			{
				var inline = PointLiesWithinShape(aChild.Start, bSeg);
				vertices.Add(new Vertex(index++, aChild.Start, inline.Kind, aChild.Kind));
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
				foreach(var inter in intersections.OrderBy(i => (i.Start - aChild.Start).Length()))
					vertices.Add(new Vertex(index++, inter.Start, inter.Kind, PathOperationKind.LineTo));
				inline = PointLiesWithinShape(aChild.End, bSeg);
				vertices.Add(new Vertex(index++, aChild.End, inline.Kind, aChild.Kind));
			}

			return vertices.ToArray();
		}
		public static Intersection[] FindIntersections(SVGPath a, SVGPath b)
		{
			var aOp = PathOperationHelper.Translate(a.Operations, a.x, a.y);
			var bOp = PathOperationHelper.Translate(b.Operations, b.x, b.y);
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
		/*
		public static Intersection[] FindPointsWithin(SVGPath a, SVGPath b)
		{
			var aOp = PathOperationHelper.Translate(a.Operations, a.x, a.y);
			var bOp = PathOperationHelper.Translate(b.Operations, b.x, b.y);
			var aSeg = ToSegment(aOp);
			var bSeg = ToSegment(bOp);
			var result = new HashSet<Intersection>();
			foreach (var aChild in aSeg)
			{
				foreach (var bChild in bSeg)
				{
					var intersections = FindSegmentWithin(aChild, bSeg);
					foreach (var i in intersections)
					{
						if (!result.Contains(i) && i.Kind != IntersectionKind.None)
							result.Add(i);
					}
				}
			}
			return result.ToArray();
		}
		*/

		private static Intersection[] FindSegmentWithin(PathOperationSegment segment, PathOperationSegment[] shape)
		{
			if (segment.Kind == PathOperationKind.MoveTo)
				return ToArray(Intersection.CreateNoIntersection());
			var start = PointLiesWithinShape(segment.Start, shape);
			var end = PointLiesWithinShape(segment.End, shape);
			if (start.Kind == IntersectionKind.None)
			{
				if (end.Kind == IntersectionKind.None)
					return ToArray(Intersection.CreateNoIntersection());
				else
					return ToArray(end);
			}
			else
			{
				if (end.Kind == IntersectionKind.None)
					return ToArray(end);
				else
					return new Intersection[] { start, end };
			}
		}

		private static PathOperationSegment[] ToSegment(PathOperation[] operations)
		{
			var result = new PathOperationSegment[operations.Length];
			var coord = new Vector2();
			var init = Vector2.NaN;
			for (int i = 0; i < result.Length; i++)
			{
				if (Vector2.IsNaN(init) && operations[i].Kind != PathOperationKind.MoveTo)
					init = coord;
				result[i] = ToSegment(operations[i], coord, init);
				coord = result[i].End;
			}
			return result;
		}

		private static PathOperationSegment ToSegment(PathOperation operation, Vector2 coord, Vector2 init)
		{
			var end = PathOperationHelper.GetCoordinate(operation, coord);
			if (Vector2.IsNaN(end))
				end = init;
			return new PathOperationSegment(operation.Kind, coord, end);
		}

		private static float RelativePositionTo(Vector2 point, PathOperationSegment line)
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
