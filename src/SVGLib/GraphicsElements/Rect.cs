using SVGLib.PathOperations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGLib.GraphicsElements
{
	public class Rect : BasicShape
	{
		public float rx { get; set; }
		public float ry { get; set; }

		public override SVGElementKind Kind => SVGElementKind.Rect;

		public Rect(int x, int y, int width, int height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
			rx = 0;
			ry = 0;
		}

		public override Path toPath()
		{
			var result = new Path(Width, Height);
			result.Fill = Fill;
			result.Stroke = Stroke;
			result.StrokeWidth = StrokeWidth;
			result.X = X;
			result.Y = Y;
			result.add(new CoordinatePairOperation(false, PathOperationKind.MoveTo, rx, 0));
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToHorizontal, Width - rx));
			if (rx > 0 && ry > 0)
			{

			}
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToVertical, Height - ry));
			if (rx > 0 && ry > 0)
			{

			}
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToHorizontal, rx));
			if (rx > 0 && ry > 0)
			{

			}
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToVertical, ry));
			if (rx > 0 && ry > 0)
			{

			}
			return result;
		}
	}
}
