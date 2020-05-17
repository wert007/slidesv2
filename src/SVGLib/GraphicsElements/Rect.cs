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
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
			rx = 0;
			ry = 0;
		}

		public override Path toPath()
		{
			var result = new Path(width, height);
			result.Fill = Fill;
			result.Stroke = Stroke;
			result.StrokeWidth = StrokeWidth;
			result.x = x;
			result.y = y;
			result.add(new CoordinatePairOperation(false, PathOperationKind.MoveTo, rx, 0));
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToHorizontal, width - rx));
			if (rx > 0 && ry > 0)
			{

			}
			result.add(new SingleCoordinateOperation(false, PathOperationKind.LineToVertical, height - ry));
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
