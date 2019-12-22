using Slides;
using Slides.Code;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace HTMLWriter
{
	public static class PresentationWriter
	{
		static HTMLWriter _htmlWriter;
		static JavaScriptWriter _jsWriter;
		static CSSWriter _cssWriter;
		static string _stdTransition = null;
		private static int _stepCounter;

		private static void CopyFile(string name, string targetDirectory, bool alwaysCopy)
		{
			var path = Path.Combine(targetDirectory, name);
			if (!File.Exists(path))
				File.Copy(name, path);
			else if (alwaysCopy)
			{
				File.Delete(path);
				File.Copy(name, path);
			}
		}

		public static void Write(Presentation presentation, string targetDirectory, bool alwaysCopyEverything = false)
		{
			_stepCounter = 0;
			_stdTransition = null;
			Directory.CreateDirectory(targetDirectory);
			foreach (var referencedFile in presentation.ReferencedFiles)
			{
				var targetFile = Path.Combine(targetDirectory, referencedFile);
				if (File.Exists(targetFile))
					continue;
				Directory.CreateDirectory(Path.Combine(targetDirectory, Directory.GetParent(referencedFile).Name));
				File.Copy(referencedFile, targetFile);
			}

			CopyFile("core.css", targetDirectory, alwaysCopyEverything);
			CopyFile("core.js", targetDirectory, alwaysCopyEverything);
			CopyFile("datatypes.js", targetDirectory, alwaysCopyEverything);
			CopyFile("prism.js", targetDirectory, alwaysCopyEverything);

			using (FileStream stream = new FileStream(Path.Combine(targetDirectory, "index.html"), FileMode.Create))
			using (_htmlWriter = new HTMLWriter(stream))
			{
				using (FileStream jsStream = new FileStream(Path.Combine(targetDirectory, "index.js"), FileMode.Create))
				using (_jsWriter = new JavaScriptWriter(jsStream))
				{

					using (FileStream cssStream = new FileStream(Path.Combine(targetDirectory, "index.css"), FileMode.Create))
					using (_cssWriter = new CSSWriter(cssStream))
					{

						foreach (var style in presentation.Styles)
						{
							StyleWriter.Write(_cssWriter, style, out var transition);
							if (_stdTransition == null)
								_stdTransition = transition;
						}
						_htmlWriter.Start();
						_htmlWriter.StartHead();
						_htmlWriter.UseCSS("index.css");
						_htmlWriter.UseJS("index.js");
						_htmlWriter.UseCSS("core.css");
						_htmlWriter.UseJS("core.js");
						_htmlWriter.UseJS("datatypes.js");
						_htmlWriter.UseJS("https://cdn.jsdelivr.net/npm/chart.js@2.9.3/dist/Chart.min.js");
						_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism-tomorrow.min.css");
						_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/plugins/line-numbers/prism-line-numbers.min.css");
						foreach (var library in presentation.Libraries)
						{
							_htmlWriter.UseCSS($"{library.Name}.css");
						}
						foreach (var import in presentation.Imports)
						{
							_htmlWriter.UseCSS($"{import}");
						}
						//onload="load()" onkeydown="keyDown(event);"
						_htmlWriter.PushAttribute("onload", "load()");
						_htmlWriter.PushAttribute("onkeydown", "keyDown(event);");
						_htmlWriter.StartBody();
						_htmlWriter.UseJS("prism.js");
						//_htmlWriter.UseJS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/components/prism-core.min.js");
					//	_htmlWriter.UseJS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/plugins/autoloader/prism-autoloader.js");
						//_htmlWriter.UseJS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/plugins/line-numbers/prism-line-numbers.min.js");

						FilterWriter.Write(_htmlWriter, presentation.CustomFilter);

						foreach (var transition in presentation.Transitions)
						{
							Write(transition);
						}

						foreach (var slide in presentation.Slides)
						{
							Write(slide);
						}

						AnimationWriter.EndFile(_jsWriter);
					}
				}
			}
			foreach (var library in presentation.Libraries)
			{

				using (FileStream stream = new FileStream(Path.Combine(targetDirectory, $"{library.Name}.css"), FileMode.Create))
				using (var writer = new CSSWriter(stream))
				{
					foreach (var style in library.Styles)
					{
						StyleWriter.Write(writer, style, out _);
					}
				}
			}
		}


		private static void Write(Transition transition)
		{
			StyleWriter.WriteTransition(_cssWriter, transition);
			//data-duration="1000"
			_htmlWriter.PushAttribute("data-duration", transition.duration.ToMilliseconds().ToString());
			_htmlWriter.StartTag("section", id: transition.name, classes: "transition");
			_htmlWriter.EndTag();
		}

		private static void Write(Slide slide)
		{
			StyleWriter.WriteSlide(_cssWriter, slide);
			//data-transition-id="stdTransition"
			_htmlWriter.PushAttribute("data-transition-id", _stdTransition);
			_htmlWriter.StartTag("section", id: slide.Name, classes: "slide");
			foreach (var step in slide.Steps)
			{
				WriteStep(slide, step);
			}
			if(slide.Parent != null)
				Write(slide, slide.Parent);
			_htmlWriter.EndTag();
		}

		private static void Write(Slide parent, Template template)
		{
			_htmlWriter.WriteComment($"Inserted by template '{template.Name}'.");
			foreach (var element in template.VisualChildren)
			{
				WriteElement(parent.Name, element);
			}
		}

		private static void WriteStep(Slide parent, Step step)
		{
			StyleWriter.WriteStep(_cssWriter, step, parent);
			_htmlWriter.PushAttribute("data-slide-id", parent.Name);
			_htmlWriter.PushAttribute("data-step-numerical-id", _stepCounter.ToString());
			_htmlWriter.StartTag("div", id: step.Name, classes: "step");
			foreach (var element in step.VisualChildren)
			{
				WriteElement(parent.Name, element);
			}
			foreach (var animation in step.AnimationCalls)
			{
				AnimationWriter.Write(_jsWriter, animation, _stepCounter, $"{parent.Name}-{animation.Element.name}");
			}
			_htmlWriter.EndTag();
			_stepCounter++;
		}

		private static void WriteElement(string parentName, Element element, Element parent = null)
		{
			StyleWriter.WriteElement(_cssWriter, parentName, element, parent);
			var needsOuterDiv = element.orientation == Orientation.Center ||
				element.orientation == Orientation.LeftCenter ||
				element.orientation == Orientation.RightCenter ||
				element.orientation == Orientation.StretchCenter;
			if (needsOuterDiv)
			{
				_htmlWriter.StartTag("div", classes: "vertical-center-parent");
			}
			switch (element.type)
			{
				case ElementType.Image:
					WriteImage(parentName, (Image)element);
					break;
				case ElementType.BoxElement:
					WriteBoxElement(parentName, (BoxElement)element);
					break;
				case ElementType.Label:
					WriteLabel(parentName, (Label)element);
					break;
				case ElementType.LineChart:
					WriteLineChart(parentName, (LineChart)element);
					break;
				case ElementType.Rectangle:
					WriteRectangle(parentName, (Rectangle)element);
					break;
				case ElementType.Stack:
					WriteStack(parentName, (Stack)element);
					break;
				case ElementType.Container:
					WriteContainer(parentName, (Container)element);
					break;
				case ElementType.List:
					WriteList(parentName, (List)element);
					break;
				case ElementType.CodeBlock:
					WriteCodeBlock(parentName, (CodeBlock)element);
					break;
				default:
					throw new Exception($"ElementType unknown: {element.type}");
			}

			if (needsOuterDiv)
				_htmlWriter.EndTag();
		}

		private static void WriteList(string parentName, List element)
		{
			var startTag = "ul";
			if (element.isOrdered)
				startTag = "ol";
			_htmlWriter.StartTag(startTag, id: parentName + "-" + element.name, classes: "list " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			foreach (var child in element.children)
			{
				if (child is List subList)
					WriteList(parentName, subList);
				else if (child is Label label)
					WriteListItem(parentName, label);
			}
			_htmlWriter.EndTag();
		}

		private static void WriteListItem(string parentName, Label element)
		{
			_htmlWriter.StartTag("li", useNewLine: false);
			WriteFormattedText(element.text);
			_htmlWriter.EndTag();
		}

		private static void WriteCodeBlock(string parentName, CodeBlock element)
		{
			_htmlWriter.StartTag("div");
			_htmlWriter.PushAttribute("data-start", element.lineStart.ToString());
			_htmlWriter.StartTag("pre", id: parentName + "-" + element.name, classes: "codeblock line-numbers" + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)), useNewLine: false);
			_htmlWriter.StartTag("code", classes: $"language-clike", useNewLine: false);
			_htmlWriter.Write(element.code);
			_htmlWriter.EndTag(false);
			_htmlWriter.EndTag(false);
			_htmlWriter.StartTag("div", classes: "codeblock-caption");
			_htmlWriter.Write(element.caption);
			_htmlWriter.EndTag();
			_htmlWriter.EndTag();
		}

		private static void WriteContainer(string parentName, Container element)
		{
			_htmlWriter.StartTag("div", id: parentName + "-" + element.name, classes: "container " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			WriteElement(parentName, element.child, element);
			_htmlWriter.EndTag();
		}

		private static void WriteStack(string parentName, Stack stack)
		{
			_htmlWriter.StartTag("div", id: parentName + "-" + stack.name, classes: "stack " + string.Join(" ", stack.get_AppliedStyles().Select(s => s.Name)));
			foreach (var element in stack.children)
			{
				//TODO(Debate): They don't have an id.
				//So we need to set a name
				//e.g. stackChild0, stackChild1, stackChild2...
				WriteElement(parentName, element, stack);
			}
			_htmlWriter.EndTag();
		}

		private static void WriteRectangle(string parentName, Rectangle element)
		{
			_htmlWriter.StartTag("div", id: parentName + "-" + element.name, classes: "rect " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.EndTag();
		}

		private static void WriteLineChart(string parentName, LineChart element)
		{
			_htmlWriter.PushAttribute("height", element.height.ToString());
			_htmlWriter.StartTag("div");
			_htmlWriter.StartTag("canvas", id: parentName + "-" + element.name, classes: "lineChart " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			ChartWriter.WriteChart(_jsWriter, parentName, element);
			_htmlWriter.EndTag();
			_htmlWriter.EndTag();
		}

		private static void WriteLabel(string parentName, Label element)
		{
			_htmlWriter.StartTag("p", id: parentName + "-" + element.name, classes: "label " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)), useNewLine: false);
			WriteFormattedText(element.text);
			_htmlWriter.EndTag();
		}

		private static void WriteFormattedText(string text)
		{
			var span = new StringBuilder();
			var useItalics = false;
			var useBolds = false;
			for (int i = 0; i < text.Length; i++)
			{
				var character = text[i];
				var next = (char)0;
				if (i + 1 < text.Length)
					next = text[i + 1];
				switch (character)
				{
					case '_':
						if (next == '_')
						{
							_htmlWriter.Write(span.ToString());
							if (useItalics)
								_htmlWriter.EndTag(false);
							else  //Smartass move. Either you need to end the old tag or start a new one.
								_htmlWriter.StartTag("span", classes: "italic", useNewLine: false);
							useItalics = !useItalics;
							span.Clear();
							i++;
						}
						break;
					case '*':
						if (next == '*')
						{
							_htmlWriter.Write(span.ToString());
							if (useBolds)
								_htmlWriter.EndTag(false);
							else  //Smartass move. Either you need to end the old tag or start a new one.
								_htmlWriter.StartTag("span", classes: "bold", useNewLine: false);
							useBolds = !useBolds;
							span.Clear();
							i++;
						}
						break;
					case '(':
						var veryNext = (char)0;
						if (i + 2 < text.Length)
							veryNext = text[i + 2];
						var veryVeryNext = (char)0;
						if (i + 3 < text.Length)
							veryVeryNext = text[i + 3];
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
							case 'n':
								span.Append("<br>");
								break;
							case '\\':
								span.Append('\\');
								break;
							default:
								throw new Exception();
						}
						break;
					default:
						span.Append(character);
						break;
				}
			}

			_htmlWriter.Write(span.ToString());
			if (useItalics)
				_htmlWriter.EndTag(false);
		}

		private static void WriteBoxElement(string parentName, BoxElement element)
		{
			_htmlWriter.StartTag("div", id: parentName + "-" + element.name, classes: element.TypeName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			foreach (var child in element.Children)
			{
				//TODO(Debate): They don't have an id.
				//So we need to set a name
				//e.g. groupChild0, groupChild1, groupChild2...
				//Or maybe try to get the name from the group-body..
				WriteElement(parentName, child, element);
			}
			_htmlWriter.EndTag();
		}

		private static void WriteImage(string parentName, Image element)
		{
			//<img src="smiley.gif" alt="Smiley face" height="42" width="42">
			_htmlWriter.PushAttribute("src", element.source.Path);
			_htmlWriter.WriteTag("img", id: parentName + "-" + element.name, needsEnding: false, classes: "image " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
		}
	}
}
