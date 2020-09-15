using SimpleLogger;
using Slides.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slides
{
	//TODO: What do we do with inline text-align?! 
	//	     as of now, [left][right[center][justify] are not supported!!

	//TODO: SecurityCheck() should we allow javascript: links?
	//      Maybe with a compiler-flag?
	public class FormattedString
	{
		private Tag[] Tags { get; }
		public string Text { get; }

		private class Tag
		{
			public int OriginalPosition { get; }
			public int TextPosition { get; set; }
			public int Length { get; set; }
			public string Name { get; }
			public string Attribute { get; }

			public Tag(int originalPosition, int textPosition, string name, string attribute)
			{
				OriginalPosition = originalPosition;
				TextPosition = textPosition;
				Name = name;
				Attribute = attribute;
				Length = 0;
			}
		}


		private FormattedString(Tag[] tags, string text)
		{
			Tags = tags;
			Text = text;
		}


		public static FormattedString Convert(string text)
		{
			text = SimpleTextReplacement(text);
			var textBuilder = new StringBuilder();
			var openTags = new List<Tag>();
			var foundTags = new List<Tag>();
			var tagParsingActive = true;

			for (int i = 0; i < text.Length; i++)
			{
				var cur = text[i];
				var lookahead = i + 1 < text.Length ? text[i + 1] : '\0';
				if (cur == '[' && lookahead != '/')
				{
					ParseOpenTag(text, ref i, textBuilder, openTags, foundTags, ref tagParsingActive);
				}
				else if (cur == '[' && lookahead == '/')
				{
					i = ParseCloseTag(text, i, textBuilder, openTags, foundTags, ref tagParsingActive);
				}
				else if (cur == '_' && lookahead == '_')
				{
					var looktwoahead = i + 2 < text.Length ? text[i + 2] : '\0';
					if (looktwoahead == '_')
					{
						if (tagParsingActive)
						{
							if (openTags.Any(t => t.Name.ToLower() == "u"))
								CloseTag(textBuilder, openTags, foundTags, openTags.First(t => t.Name.ToLower() == "u"));
							else
								openTags.Add(new Tag(i, textBuilder.Length, "u", null));
						}
						else
							textBuilder.Append("___");
						i++;
					}
					else
					{
						if (tagParsingActive)
						{
							if (openTags.Any(t => t.Name.ToLower() == "i"))
								CloseTag(textBuilder, openTags, foundTags, openTags.First(t => t.Name.ToLower() == "i"));
							else
								openTags.Add(new Tag(i, textBuilder.Length, "i", null));
						}
						else
							textBuilder.Append("__");
					}

					i++;
				}
				else if (cur == '*' && lookahead == '*')
				{
					if (tagParsingActive)
					{
						if (openTags.Any(t => t.Name.ToLower() == "b"))
							CloseTag(textBuilder, openTags, foundTags, openTags.First(t => t.Name.ToLower() == "b"));
						else
							openTags.Add(new Tag(i, textBuilder.Length, "b", null));
					}
					else
						textBuilder.Append("**");
					i++;
				}
				else if (cur == '~' && lookahead == '~')
				{
					if (tagParsingActive)
					{
						if (openTags.Any(t => t.Name.ToLower() == "s"))
							CloseTag(textBuilder, openTags, foundTags, openTags.First(t => t.Name.ToLower() == "s"));
						else
							openTags.Add(new Tag(i, textBuilder.Length, "s", null));
					}
					else
						textBuilder.Append("~~");
					i++;
				}
				else if (cur == '{')
				{
					if (tagParsingActive)
						ParseMarkdownLink(text, ref i, textBuilder, foundTags);
					else textBuilder.Append("{");
				}
				else
				{
					textBuilder.Append(cur);

				}
			}
			//TODO: Actual diagnostics!
			if (openTags.Any())
				Logger.Log($"Unclosed tag(s) found in {text}");
			foreach (var remainingTag in openTags.OrderBy(t => t.OriginalPosition))
			{
				RevertOpenTag(textBuilder, foundTags, remainingTag);
			}
			return new FormattedString(foundTags.OrderBy(t => t.OriginalPosition).ToArray(), textBuilder.ToString());
		}

		private static bool SecurityCheck(Tag tag)
		{
			switch (tag.Name)
			{
				case "color":
				case "bgcolor":
				case "size":
					return !tag.Attribute.Contains(";");
				case "url":
					return !tag.Attribute?.StartsWith("javascript:") ?? true;
				default:
					return true;
			}
		}

		private static void ParseMarkdownLink(string text, ref int i, StringBuilder textBuilder, List<Tag> foundTags)
		{
			var linkArgumentBuilder = new StringBuilder();
			if (text[i] != '{')
			{
				textBuilder.Append("{");
				return;
			}
			i++;
			int j;
			for (j = i; j < text.Length; j++)
			{
				if (text[j] == '}') break;
				linkArgumentBuilder.Append(text[j]);
			}
			j++;
			if (j >= text.Length)
			{
				textBuilder.Append("{");
				i--;
				return;
			}
			var linkText = linkArgumentBuilder.ToString();
			linkArgumentBuilder.Clear();
			if (text[j] != '(')
			{
				textBuilder.Append("{");
				i--;
				return;
			}
			j++;
			for (; j < text.Length; j++)
			{
				if (text[j] == ')') break;
				linkArgumentBuilder.Append(text[j]);
			}
			if (j == text.Length)
			{
				textBuilder.Append("{");
				i--;
				return;
			}
			var linkTarget = linkArgumentBuilder.ToString();
			textBuilder.Append(linkText);
			var tag = new Tag(i - 1, textBuilder.Length - linkText.Length, "url", linkTarget);
			tag.Length = linkText.Length;
			if (tag.Length > 0)
				foundTags.Add(tag);
			i = j;
		}

		private static bool ParseOpenTag(string text, ref int i, StringBuilder textBuilder, List<Tag> openTags, List<Tag> foundTags, ref bool tagParsingActive)
		{
			var tagnameBuilder = new StringBuilder();
			var failTextBuilder = new StringBuilder();
			var originalPosition = i;
			failTextBuilder.Append(text[i]);
			i++;
			if (i >= text.Length)
			{
				textBuilder.Append(failTextBuilder.ToString());
				return false;
			}
			for (; i < text.Length; i++)
			{
				failTextBuilder.Append(text[i]);
				if (!char.IsLetter(text[i]) && text[i] != '*')
					break;
				tagnameBuilder.Append(text[i]);
			}
			var tagname = tagnameBuilder.ToString();
			if (!IsValidTagname(tagname) || text[i] != ']' && text[i] != '=')
			{
				textBuilder.Append(failTextBuilder.ToString());
				return false;
			}
			string attribute = null;
			bool isInsideQuotes = false;
			if (text[i] == '=')
			{
				//failTextBuilder.Append(text[i]);
				i++;
				tagnameBuilder.Clear();
				isInsideQuotes = text[i] == '"';
				if (isInsideQuotes)
				{
					failTextBuilder.Append(text[i]);
					i++;
				}
				for (; i < text.Length; i++)
				{
					failTextBuilder.Append(text[i]);
					if (!isInsideQuotes && (text[i] == ' ' || text[i] == ']' || text[i] == '"'))
						break;
					if (isInsideQuotes && text[i] == '"') break;
					tagnameBuilder.Append(text[i]);
				}
				if (isInsideQuotes && text[i] == '"')
				{
					i++;
					failTextBuilder.Append(text[i]);
					if (text[i] == ']')
					{
						attribute = tagnameBuilder.ToString();
					}
					else
					{
						textBuilder.Append(failTextBuilder);
						return false;
					}
				}
				else if (!isInsideQuotes && text[i] == ']')
				{
					attribute = tagnameBuilder.ToString();
				}
				else
				{
					textBuilder.Append(failTextBuilder);
					return false;
				}
			}
			if (tagParsingActive)
			{
				if (tagname.ToLower() == "list" && openTags.Any(t => t.Name == "*") && !openTags.Any(t => t.Name.ToLower() == "list"))
				{
					var remainingTag = openTags.First(t => t.Name == "*");
					openTags.Remove(remainingTag);
					RevertOpenTag(textBuilder, foundTags, remainingTag);
				}
				else if (tagname == "*" && openTags.Any(t => t.Name == "*"))
				{
					var lastStarTag = openTags.First(t => t.Name == "*");
					CloseTag(textBuilder, openTags, foundTags, lastStarTag);
				}
				var tag = new Tag(originalPosition, textBuilder.Length, tagname, attribute);
				if (SecurityCheck(tag))
				{
					openTags.Add(tag);
					if (tagname.ToLower() == "code")
						tagParsingActive = !tagParsingActive;
					return true;
				}
				else
				{
					textBuilder.Append(failTextBuilder);
					return false;
				}
			}
			else
			{
				textBuilder.Append(failTextBuilder);
				return false;
			}
		}

		private static void RevertOpenTag(StringBuilder textBuilder, List<Tag> foundTags, Tag remainingTag)
		{
			textBuilder.Insert(remainingTag.TextPosition, $"[{remainingTag.Name}]");
			foreach (var foundTag in foundTags)
			{
				if (foundTag.TextPosition > remainingTag.TextPosition || foundTag.TextPosition + foundTag.Length < remainingTag.TextPosition)
					continue;
				if (foundTag.TextPosition == remainingTag.TextPosition && foundTag.OriginalPosition > remainingTag.OriginalPosition)
				{
					foundTag.TextPosition += remainingTag.Name.Length + 2;
					if (remainingTag.Attribute != null)
						foundTag.TextPosition += 1 + remainingTag.Attribute.Length;
				}
				else
				{
					foundTag.Length += remainingTag.Name.Length + 2;
					if (remainingTag.Attribute != null)
						foundTag.Length += 1 + remainingTag.Attribute.Length;
				}
			}
		}

		private static int ParseCloseTag(string text, int i, StringBuilder textBuilder, List<Tag> openTags, List<Tag> foundTags, ref bool tagParsingActive)
		{
			var failTextBuilder = new StringBuilder();
			var tagnameBuilder = new StringBuilder();
			failTextBuilder.Append("[/");
			i += 2;
			for (; i < text.Length; i++)
			{
				failTextBuilder.Append(text[i]);
				if (!char.IsLetter(text[i]))
					break;
				tagnameBuilder.Append(text[i]);
			}
			if (i >= text.Length || text[i] != ']')
			{
				textBuilder.Append(failTextBuilder.ToString());
				return i;
			}
			var tagname = tagnameBuilder.ToString();
			if (tagname.ToLower() == "code")
				tagParsingActive = !tagParsingActive;
			var matchingTag = openTags.FirstOrDefault(t => t.Name.ToLower() == tagname.ToLower());
			if (matchingTag == null || !tagParsingActive)
			{
				textBuilder.Append(failTextBuilder.ToString());
				return i;
			}
			else
			{
				if (tagname.ToLower() == "list" && openTags.Any(t => t.Name == "*"))
				{
					var lastStarTag = openTags.First(t => t.Name == "*");
					CloseTag(textBuilder, openTags, foundTags, lastStarTag);
				}
				CloseTag(textBuilder, openTags, foundTags, matchingTag);
			}
			return i;
		}

		private static void CloseTag(StringBuilder textBuilder, List<Tag> openTags, List<Tag> foundTags, Tag tag)
		{
			if (!SecurityCheck(tag, textBuilder.ToString()))
			{
				textBuilder.Append("[/");
				textBuilder.Append(tag.Name);
				textBuilder.Append("]");
				return;
			}
				openTags.Remove(tag);
			tag.Length = textBuilder.Length - tag.TextPosition;
			if (tag.Length > 0)
				foundTags.Add(tag);
		}

		private static bool SecurityCheck(Tag tag, string text)
		{
			if(tag.Name.ToLower() =="url")
			{
				var link = text.Substring(tag.TextPosition);
				return !link.StartsWith("javascript:");
			}
			return true;
		}

		private static bool IsValidTagname(string tagname)
		{
			switch (tagname.ToLower())
			{
				case "b":
				case "i":
				case "s":
				case "u":
				//case "left":
				//case "center":
				//case "right":
				case "justify":
				case "size":
				case "color":
				case "bgcolor":
				case "quote":
				case "url":
				case "code":
				case "list":
				case "*":
				case "sup":
				case "sub":
					return true;
				default:
					return false;
			}
		}

		public string ToHTML()
		{
			var result = new StringBuilder();
			var activeStyles = new Dictionary<Tag, string>();

			//if (Tags.Any(t => t.Name == "list"))
			//	result.Append("<div class=\"labelContainer\">");
			for (int i = 0; i < Text.Length; i++)
			{
				foreach (var tag in Tags.Where(t => t.TextPosition == i))
				{
					if (HasStyle(tag))
						activeStyles.Add(tag, GetStyle(tag));
					result.Append(GetOpeningTag(tag, activeStyles.Values.AsEnumerable()));
				}
				result.Append(Text[i]);
				foreach (var tag in Tags.Where(t => t.TextPosition + t.Length - 1 == i).Reverse())
				{
					result.Append(GetClosingTag(tag));
					activeStyles.Remove(tag);
				}
			}

			//if (Tags.Any(t => t.Name == "list"))
			//	result.Append("</div>");
			return result.ToString();
		}

		private bool HasStyle(Tag tag)
		{
			switch (tag.Name.ToLower())
			{
				//case "left":
				//case "center":
				//case "right":
				case "justify":
				case "color":
				case "bgcolor":
				case "size":
					return true;
				default:
					return false;
			}
		}

		private string GetStyle(Tag tag)
		{
			switch (tag.Name.ToLower())
			{
				//case "left":
				//	return "text-align: left;";
				//case "center":
				//	return "text-align: center;";
				//case "right":
				//	return "text-align: right;";
				//case "justify":
				//	return "text-align: justify;";
				case "color":
					return $"color: {tag.Attribute};";
				case "bgcolor":
					return $"background-color: {tag.Attribute};";
				case "size":
					return $"font-size: {tag.Attribute};";
				default:
					return null;
			}
		}

		private string GetOpeningTag(Tag tag, IEnumerable<string> enumerable)
		{
			switch (tag.Name.ToLower())
			{
				case "url":
					var url = tag.Attribute ?? Text.Substring(tag.TextPosition, tag.Length);
					if (enumerable.Any())
						return $"<a href=\"{url}\" style=\"{string.Join("", enumerable)}\">";

					return $"<a href=\"{url}\">";
				case "quote":
					if (enumerable.Any())
						return $"<q style=\"{string.Join("", enumerable)}\">";
					return "<q>";
				case "i":
					if (enumerable.Any())
						return $"<i style=\"{string.Join("", enumerable)}\">";
					return "<i>";
				case "b":
					if (enumerable.Any())
						return $"<b style=\"{string.Join("", enumerable)}\">";
					return "<b>";
				case "u":
					if (enumerable.Any())
						return $"<ins style=\"{string.Join("", enumerable)}\">";
					return "<ins>";
				case "s":
					if (enumerable.Any())
						return $"<del style=\"{string.Join("", enumerable)}\">";
					return "<del>";
				case "code":
					if (enumerable.Any())
						return $"<pre style=\"display: inline-block;{string.Join("", enumerable)}\">";
					return "<pre style=\"display: inline-block;\">";
				case "list":
					switch (tag.Attribute)
					{
						case "1":
						case "i":
						case "I":
						case "a":
						case "A":
							if (enumerable.Any())
								return $"<ol type=\"{tag.Attribute}\" style=\"{string.Join("", enumerable)}\">";
							return $"<ol type=\"{tag.Attribute}\">";
						case null:
						default:
							if (enumerable.Any())
								return $"<ul style=\"{string.Join("", enumerable)}\">";
							return "<ul>";

					}
				case "*":
					if (enumerable.Any())
						return $"<li style=\"{string.Join("", enumerable)}\">";
					return "<li>";
				case "sup":
					if (enumerable.Any())
						return $"<sup style=\"{string.Join("", enumerable)}\">";
					return "<sup>";
				case "sub":
					if (enumerable.Any())
						return $"<sub style=\"{string.Join("", enumerable)}\">";
					return "<sub>";
				//case "left":
				//case "center":
				//case "right":
				//case "justify":
				case "color":
				case "bgcolor":
				case "size":
					if (enumerable.Any())
						return $"<span style=\"{string.Join("", enumerable)}\">";
					return "<span>";
				default:
					throw new Exception();
			}
		}

		private string GetClosingTag(Tag tag)
		{
			switch (tag.Name.ToLower())
			{
				case "url":
					return "</a>";
				case "quote":
					if (tag.Attribute == null)
						return "</q>";
					return $"</q> - <cite style=\"font-size:smaller;\">{tag.Attribute}</cite>";
				case "i":
					return "</i>";
				case "b":
					return "</b>";
				case "u":
					return "</ins>";
				case "s":
					return "</del>";
				case "code":
					return "</pre>";
				case "list":
					switch (tag.Attribute)
					{
						case "1":
						case "i":
						case "I":
						case "a":
						case "A":
							return "</ol>";
						case null:
						default:
							return "</ul>";
					}
				case "*":
					return "</li>";
				case "sup":
					return "</sup>";
				case "sub":
					return "</sub>";
				//case "left":
				//case "center":
				//case "right":
				case "color":
				case "bgcolor":
				case "size":
					return "</span>";
				default: throw new Exception();
			}

		}


		private static string SimpleTextReplacement(string text)
		{
			var result = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				var character = text[i];
				var next = i + 1 < text.Length ? text[i + 1] : '\0';
				var veryNext = i + 2 < text.Length ? text[i + 2] : '\0';
				var veryVeryNext = i + 3 < text.Length ? text[i + 3] : '\0';
				switch (character)
				{
					case '(':
						if (next == 'c' && veryNext == ')')
						{
							result.Append("©");
							i += 2;
						}
						else if (next == 'r' && veryNext == ')')
						{
							result.Append("®");
							i += 2;
						}
						else if (next == 't' && veryNext == 'm' && veryVeryNext == ')')
						{
							result.Append("™");
							i += 3;
						}
						else
							result.Append(character);
						break;
					case '\\':
						i++;
						switch (next)
						{
							case '\\':
								result.Append('\\');
								break;
							case '\'':
								result.Append('\'');
								break;
							default:
								result.Append(character);
								result.Append(next);
								//i++;
								break;
						}
						break;
					case '=':
						if (next == '=' && veryNext == '>')
						{
							i += 2;
							result.Append("⇒");
						}
						else
							result.Append(character);
						break;
					case '-':
						if (next == '-' && veryNext == '>')
						{
							i += 2;
							result.Append("→");
						}
						else
							result.Append(character);
						break;
					case '<':
						if (next == '-' && veryNext == '-')
						{
							i += 2;
							result.Append("←");
						}
						else if (next == '=' && veryNext == '=')
						{
							i += 2;
							result.Append("⇐");
						}
						else
							result.Append(character);
						break;
					default:
						result.Append(character);
						break;
				}
			}
			return result.ToString();
		}
	}
	/*
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
							else if (next == '=' && veryNext == '=')
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
	*/
}
