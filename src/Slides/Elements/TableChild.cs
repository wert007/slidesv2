namespace Slides.Elements
{
	public class TableChild : TextElement
	{
		internal delegate void ContentUpdatedHandler();
		internal event ContentUpdatedHandler ContentUpdated;

		private string _content;
		public string content { get => _content; set
			{
				_content = value;
				ContentUpdated?.Invoke();
			}
		}
		public Alignment? align { get; set; }

		public override ElementKind kind => ElementKind.TableChild;

		private Unit _top;
		private Unit _left;

		public override Unit top => _top;
		public override Unit left => _left;

		public void set_Top(Unit t) => _top = t;
		public void set_Left(Unit l) => _left = l;

		public TableChild(string content)
		{
			_content = content;
			font = null;
			fontsize = null;
			align = null;
			//TODO: Remove this. There should be a solution in core.css
			position = "relative";
		}

		public override string ToString() => content;

		public Unit get_ActualTableChildHeight() => get_ActualHeight();
		public Unit get_ActualTableChildWidth() => get_ActualWidth();

	

		internal override Unit get_InitialWidth() => new Unit(MeasureText(_content).X, Unit.UnitKind.Pixel);
		internal override Unit get_InitialHeight() => new Unit(MeasureText(_content).Y, Unit.UnitKind.Pixel);
	}
}
