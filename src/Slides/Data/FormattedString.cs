using Slides.Styling;
using System;
using System.Text;

namespace Slides
{
	public class FormattedString
	{


		public string value { get; }
		public Font font { get; set; }
		public Unit fontsize { get; set; }
		public Color borderColor { get; set; }
		public Thickness borderThickness { get; set; }
		public BorderStyle borderStyle { get; set; }
		public Brush background { get; set; }
		public Color color { get; set; }
		public Filter filter { get; set; }
		public CustomStyle hover { get; set; }

		public FormattedString(string str)
		{
			value = str;
		}

		public static FormattedString FromString(string str)
		{
			return new FormattedString(str);
		}

		public static string Convert(string str)
		{

			var span = new StringBuilder();
			for (int i = 0; i < str.Length; i++)
			{
				var character = str[i];
				var next = (char)0;
				if (i + 1 < str.Length)
					next = str[i + 1];
				var veryNext = i + 2 < str.Length ? str[i + 2] : '\0';
				switch (character)
				{
					case '(':
						var veryVeryNext = (char)0;
						if (i + 3 < str.Length)
							veryVeryNext = str[i + 3];
						if (next == 'c' && veryNext == ')')
						{
							span.Append("©");
							i += 2;
						}
						else if (next == 'r' && veryNext == ')')
						{
							span.Append("®");
							i += 2;
						}
						else if (next == 't' && veryNext == 'm' && veryVeryNext == ')')
						{
							span.Append("™");
							i += 3;
						}
						else
							span.Append(character);
						break;
					case '\\':
						i++;
						switch (next)
						{
							case '\\':
								span.Append('\\');
								break;
							case '\'':
								span.Append('\'');
								break;
							default:
								span.Append(character);
								span.Append(next);
								i++;
								break;
						}
						break;
					case '=':
						{
							if (next == '=' && veryNext == '>')
							{
								i += 3;
								span.Append("⇒");
							}
							else
								span.Append(character);
							break;
						}
					case '-':
						{
							if (next == '-' && veryNext == '>')
							{
								i += 3;
								span.Append("→");
							}
							else
								span.Append(character);
						}
						break;
					case '<':
						{
							if (next == '-' && veryNext == '-')
							{
								i += 3;
								span.Append("←");
							}
							else if(next == '=' && veryNext == '=')
							{
								i += 3;
								span.Append("⇐");
							}
							else
								span.Append(character);
						}
						break;
					default:
						span.Append(character);
						break;
				}
			}


			return span.ToString();
		}

		//public FormattedString[] n_getSubstrings()
		//{
		//	var start = 0;
		//	var builder = new List<FormattedString>();
		//	for (int i = 0; i < value.Length; i++)
		//	{
		//		var current = value[i];
		//		var next = '\0';
		//		if (i + 1 < value.Length) next = value[i + 1];
		//		if(current == '|' && next != '|')
		//		{
		//			builder.Add(FromString(value.Substring(start, i)));
		//			start = i + 1;
		//		}
		//	}
		//	return builder.ToArray();
		//}
	}
}
