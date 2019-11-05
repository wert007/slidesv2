using System.Linq;

namespace Slides
{
	public class Label : Element
	{
		public string text { get; set; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Alignment align { get; set; }

		private static Font stdFont = new Font("Arial");

		public Label(string text)
		{
			this.text = text;
			align = Alignment.Left;
		}

		public override ElementType type => ElementType.Label;
		public override string ToString() => text;

		private Font GetFont()
		{
			if (font != null)
				return font;
			foreach (var style in get_AppliedStyles().Reverse())
			{
				if(style.ModifiedFields.ContainsKey("font"))
				{
					return (Font)style.ModifiedFields["font"];
				}
			}
			//TODO: std stylemb.,jhjbmbgqwertzuiopü+asdfghjklöäyxcvbm,
			return stdFont;
		}

		protected override Unit get_InitialWidth()
		{
			return new Unit(GetFont().Measure(text, fontsize).X, Unit.UnitKind.Pixel);
		}

		protected override Unit get_InitialHeight()
		{
			return new Unit(GetFont().Measure(text, fontsize).Y, Unit.UnitKind.Pixel);
		}
	}
}
