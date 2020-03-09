using System;
using System.Collections.Generic;
using System.Linq;

namespace Slides.SVG
{
	public static class PathOperationHelper
	{
		public static PathOperation[] ToAbsolute(PathOperation[] operations)
		{
			var result = new PathOperation[operations.Length];
			var coord = new Vector2();
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = ToAbsolute(operations[i], coord.X, coord.Y);
				coord = GetCoordinate(result[i], coord);
			}
			return result;
		}

		public static PathOperation[] Translate(PathOperation[] operations, float xOffset, float yOffset)
		{
			var result = new List<PathOperation>();
			result.Add(new CoordinatePairOperation(false, PathOperationKind.MoveTo, xOffset, yOffset));
			result.AddRange(ToRelative(operations));
			return result.ToArray();
		}

		public static PathOperation[] ToRelative(PathOperation[] operations)
		{
			var result = new PathOperation[operations.Length];
			var coord = new Vector2();
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = ToRelative(operations[i], coord);
				coord = GetCoordinate(result[i], coord);
			}
			return result.ToArray();
		}
		
		public static PathOperation[] Rotate(PathOperation[] operations, float rad, float xOrigin, float yOrigin)
		{
			var result = new PathOperation[operations.Length];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = Rotate(operations[i], rad, xOrigin, yOrigin);
			}
			return result;
		}

		private static PathOperation ToRelative(PathOperation operation, Vector2 coord)
		{
			if (operation.IsRelative)
				return operation;
			switch (operation.Kind)
			{
				case PathOperationKind.ClosePath:
					return operation;
				case PathOperationKind.MoveTo:
				case PathOperationKind.LineTo:
					return CoordinatePairToRelative((CoordinatePairOperation)operation, coord);
				case PathOperationKind.LineToHorizontal:
				case PathOperationKind.LineToVertical:
					return SingleCoordinateToRelative((SingleCoordinateOperation)operation, coord);
				case PathOperationKind.CurveTo:
				case PathOperationKind.SmoothCurveTo:
				case PathOperationKind.QuadraticBezierCurveTo:
				case PathOperationKind.SmoothQuadraticBezierCurveTo:
				case PathOperationKind.EllipticalArc:
				default:
					throw new Exception();
			}
		}

		private static PathOperation SingleCoordinateToRelative(SingleCoordinateOperation operation, Vector2 coord)
		{
			if (operation.Kind == PathOperationKind.LineToHorizontal)
				return new SingleCoordinateOperation(true, operation.Kind, coord.X - operation.Coordinate);
			return new SingleCoordinateOperation(true, operation.Kind, coord.Y - operation.Coordinate);
		}

		private static PathOperation CoordinatePairToRelative(CoordinatePairOperation operation, Vector2 coord)
			=> new CoordinatePairOperation(true, operation.Kind, operation.X - coord.X, operation.Y - coord.Y);

		private static PathOperation Rotate(PathOperation operation, float rad, float xOrigin, float yOrigin)
		{
			switch (operation.Kind)
			{
				case PathOperationKind.ClosePath:
					return new ClosePathOperation();
				case PathOperationKind.LineTo:
				case PathOperationKind.MoveTo:
					return RotateCoordinatePair((CoordinatePairOperation)operation, rad, xOrigin, yOrigin);
				case PathOperationKind.LineToHorizontal:
				case PathOperationKind.LineToVertical:
					return RotateSingleCoordinate((SingleCoordinateOperation)operation, rad, xOrigin, yOrigin);
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

		private static PathOperation RotateCoordinatePair(CoordinatePairOperation operation, float rad, float xOrigin, float yOrigin)
		{
			float x = operation.X;
			float y = operation.Y;
			var xDif = x - xOrigin;
			var yDif = y - yOrigin;
			var dist = (float)Math.Sqrt(xDif * xDif + yDif * yDif);
			var resRad = Math.Atan2(yDif, xDif) + rad;
			x = (float)Math.Cos(resRad) * dist + xOrigin;
			y = (float)Math.Sin(resRad) * dist + yOrigin;
			return new CoordinatePairOperation(operation.IsRelative, operation.Kind, x, y);
		}

		private static PathOperation RotateSingleCoordinate(SingleCoordinateOperation operation, float rad, float xOrigin, float yOrigin)
		{
			float x = 0;
			if (operation.Kind == PathOperationKind.LineToHorizontal)
				x = operation.Coordinate;
			float y = 0;
			if (operation.Kind == PathOperationKind.LineToVertical)
				y = operation.Coordinate;
			var xDif = x - xOrigin;
			var yDif = y - yOrigin;
			var dist = (float)Math.Sqrt(xDif * xDif + yDif * yDif);
			var resRad = Math.Atan2(yDif, xDif) + rad;
			x = (float)Math.Cos(resRad) * dist;
			y = (float)Math.Sin(resRad) * dist;
			return new CoordinatePairOperation(operation.IsRelative, PathOperationKind.LineTo, x, y);
		}

		public static Vector2 GetCoordinate(PathOperation operation, Vector2 coord)
		{
			switch (operation.Kind)
			{
				case PathOperationKind.ClosePath:
					return GetCloseOperationCoordinate((ClosePathOperation)operation, coord);
				case PathOperationKind.LineTo:
				case PathOperationKind.MoveTo:
					return GetCoordinatePairOperationCoordinate((CoordinatePairOperation)operation, coord);
				case PathOperationKind.LineToHorizontal:
				case PathOperationKind.LineToVertical:
					return GetSingleCoordinateOperationCoordinate((SingleCoordinateOperation)operation, coord);
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

		private static Vector2 GetCoordinatePairOperationCoordinate(CoordinatePairOperation operation, Vector2 coord)
		{
			if (operation.IsRelative)
				return new Vector2(coord.X + operation.X, coord.Y + operation.Y);
			return new Vector2(operation.X, operation.Y);
		}

		private static Vector2 GetSingleCoordinateOperationCoordinate(SingleCoordinateOperation operation, Vector2 coord)
		{
			if (operation.IsRelative)
			{
				if (operation.Kind == PathOperationKind.LineToHorizontal)
					return new Vector2(coord.X + operation.Coordinate, coord.Y);
				else
					return new Vector2(coord.X, coord.Y + operation.Coordinate);
			}
			else
			{
				if (operation.Kind == PathOperationKind.LineToHorizontal)
					return new Vector2(operation.Coordinate, coord.Y);
				else
					return new Vector2(coord.X, operation.Coordinate);
			}
		}

		private static Vector2 GetCloseOperationCoordinate(ClosePathOperation operation, Vector2 coord) => new Vector2(float.NaN, float.NaN);

		private static PathOperation ToAbsolute(PathOperation operation, float lastX, float lastY)
		{
			if (!operation.IsRelative)
			{
				if (operation.Kind == PathOperationKind.LineToHorizontal ||
					operation.Kind == PathOperationKind.LineToVertical)
					return SingleCoordinateOperationToAbsolute((SingleCoordinateOperation)operation, lastX, lastY);
				return operation;
			}
			switch (operation.Kind)
			{
				case PathOperationKind.ClosePath:
					return operation;
				case PathOperationKind.MoveTo:
				case PathOperationKind.LineTo:
					return CoordinatePairOperationToAbsolute((CoordinatePairOperation)operation, lastX, lastY);
				case PathOperationKind.LineToHorizontal:
				case PathOperationKind.LineToVertical:
					return SingleCoordinateOperationToAbsolute((SingleCoordinateOperation)operation, lastX, lastY);
				case PathOperationKind.CurveTo:
				case PathOperationKind.SmoothCurveTo:
				case PathOperationKind.QuadraticBezierCurveTo:
				case PathOperationKind.SmoothQuadraticBezierCurveTo:
				case PathOperationKind.EllipticalArc:
				case PathOperationKind.Error:
					return operation;
				default:
					throw new Exception();
			}
		}

		private static PathOperation CoordinatePairOperationToAbsolute(CoordinatePairOperation operation, float lastX, float lastY)
		{
			if (operation.IsRelative)
			{
				return new CoordinatePairOperation(false, operation.Kind, lastX, lastY);
			}
			return operation;
		}

		private static PathOperation SingleCoordinateOperationToAbsolute(SingleCoordinateOperation operation, float lastX, float lastY)
		{
			if (!operation.IsRelative)
			{
				var x = lastX;
				var y = lastY;
				if (operation.Kind == PathOperationKind.LineToHorizontal)
					x = operation.Coordinate;
				else
					y = operation.Coordinate;

				return new CoordinatePairOperation(false, PathOperationKind.LineTo, x, y);
			}
			var xOffset = 0f;
			var yOffset = 0f;
			if (operation.Kind == PathOperationKind.LineToHorizontal)
				xOffset = operation.Coordinate;
			else
				yOffset = operation.Coordinate;
			return new CoordinatePairOperation(false, PathOperationKind.LineTo, lastX + xOffset, lastY + yOffset);
		}
	}
}
