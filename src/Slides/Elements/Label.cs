using Slides.Data;

namespace Slides.Elements
{

	public class Label : TextElement
	{
		public string text { get; set; }
		public Alignment align { get; set; }


		public Label(string text)
		{
			this.text = text;
			align = Alignment.Unset;
		}

		public override ElementKind kind => ElementKind.Label;
		public override string ToString() => text;


		internal override Unit get_InitialWidth() => new Unit(MeasureText(text).X, Unit.UnitKind.Pixel);
		internal override Unit get_InitialHeight() =>  new Unit(MeasureText(text).Y, Unit.UnitKind.Pixel);
	}
}
