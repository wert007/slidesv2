using SVGLib;
using SVGLib.PathOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SVGLib.GraphicsElements
{

	public class Path : BasicShape
	{
		public override SVGElementKind Kind => SVGElementKind.Path;

		private List<PathOperation> _operations = new List<PathOperation>();
		public PathOperation[] Operations => _operations.ToArray();

		public Path(PathOperation[] operations, int width, int height)
		{
			_operations.AddRange(operations);
			this.width = width;
			this.height = height;
		}

		public Path(int width, int height)
		{
			this.width = width;
			this.height = height;
		}

		public void add(PathOperation operation)
		{
			_operations.Add(operation);
		}

		public void rotate(int degree)
		{
			rotate(degree, 0.5f, 0.5f);
		}
		public void rotate(int degree, float xOrigin, float yOrigin)
		{
			_operations = PathOperationHelper.ToAbsolute(_operations.ToArray()).ToList();
			var rad = (float)(degree * Math.PI / 180f);
			_operations = PathOperationHelper.Rotate(_operations.ToArray(), rad, width * xOrigin, height * yOrigin).ToList();
		}

		public override Path toPath()
		{
			return new Path(Operations, width, height);
		}
	}
}
