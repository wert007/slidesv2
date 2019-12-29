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
				if (!File.Exists(targetFile))
				{
					Directory.CreateDirectory(Path.Combine(targetDirectory, Directory.GetParent(referencedFile).Name));
					File.Copy(referencedFile, targetFile);
				}
				else if (alwaysCopyEverything)
				{
					File.Delete(targetFile);
					File.Copy(referencedFile, targetFile);
				}
			}

			CopyFile("core.css", targetDirectory, alwaysCopyEverything);
			CopyFile("core.js", targetDirectory, alwaysCopyEverything);
			CopyFile("datatypes.js", targetDirectory, alwaysCopyEverything);
			if (presentation.CodeHighlighter != CodeHighlighter.None)
			{
				CopyFile("prism.js", targetDirectory, alwaysCopyEverything);
				CopyFile("github.js", targetDirectory, alwaysCopyEverything);
			}
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
						_htmlWriter.UseJS("https://cdn.jsdelivr.net/npm/apexcharts");
						if (presentation.CodeHighlighter != CodeHighlighter.None)
						{
							switch (presentation.CodeHighlighter)
							{
								case CodeHighlighter.Coy:
									_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism-coy.min.css");
									break;
								case CodeHighlighter.Dark:
									_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism-dark.min.css");
									break;
								case CodeHighlighter.Funky:
									_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism-funky.min.css");
									break;
								case CodeHighlighter.Okaidia:
									_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism-okaidia.min.css");
									break;
								case CodeHighlighter.SolarizedLight:
									_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism-solarizedlight.min.css");
									break;
								case CodeHighlighter.Tomorrow:
									_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism-tomorrow.min.css");
									break;
								case CodeHighlighter.Twilight:
									_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism-twilight.min.css");
									break;
								case CodeHighlighter.Default:
									_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/themes/prism.min.css");
									break;
								default:
									throw new Exception();
							}
							_htmlWriter.UseCSS("https://cdnjs.cloudflare.com/ajax/libs/prism/1.17.1/plugins/line-numbers/prism-line-numbers.min.css");
							_htmlWriter.UseJS("github.js");
						}
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
						if (presentation.CodeHighlighter != CodeHighlighter.None)
							_htmlWriter.UseJS("prism.js");

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
			if (!slide.Attributes.isVisible)
				return;
			StyleWriter.WriteSlide(_cssWriter, slide);
			//data-transition-id="stdTransition"
			_htmlWriter.PushAttribute("data-transition-id", _stdTransition);
			_htmlWriter.StartTag("section", id: slide.Name, classes: "slide");
			foreach (var step in slide.Steps)
			{
				WriteStep(slide, step);
			}
			if (slide.Parent != null)
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
			var id = $"{parentName}-{element.name}";
			if (element.name == null)
				id = null;
			StyleWriter.WriteElement(_cssWriter, id, element, parent);
			switch (element.type)
			{
				case ElementType.Image:
					WriteImage(id, (Image)element);
					break;
				case ElementType.BoxElement:
					WriteBoxElement(id, (BoxElement)element);
					break;
				case ElementType.Label:
					WriteLabel(id, (Label)element);
					break;
				case ElementType.LineChart:
					WriteLineChart(id, (LineChart)element);
					break;
				case ElementType.Rectangle:
					WriteRectangle(id, (Rectangle)element);
					break;
				case ElementType.Stack:
					WriteStack(id, (Stack)element);
					break;
				case ElementType.Container:
					WriteContainer(id, (Container)element);
					break;
				case ElementType.List:
					WriteList(id, (List)element);
					break;
				case ElementType.CodeBlock:
					WriteCodeBlock(id, (CodeBlock)element);
					break;
				case ElementType.IFrame:
					WriteIFrame(id, (IFrame)element);
					break;
				case ElementType.Slider:
					WriteSlider(id, (Slider)element);
					break;
				default:
					throw new Exception($"ElementType unknown: {element.type}");
			}
		}

		private static void WriteList(string id, List element)
		{
			string parentName = null;
			if(id != null)
				parentName = id.Split('-')[0];
			var startTag = "ul";
			if (element.isOrdered)
				startTag = "ol";
			_htmlWriter.StartTag(startTag, id: id, classes: "list " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			foreach (var child in element.children)
			{
				var childId = $"{parentName}-{child.name}";
				if (child is List subList)
					WriteList(childId, subList);
				else if (child is Label label)
					WriteListItem(label);
				else
					throw new Exception();
			}
			_htmlWriter.EndTag();
		}

		private static void WriteListItem(Label element)
		{
			_htmlWriter.StartTag("li", useNewLine: false);
			WriteFormattedText(element.text);
			_htmlWriter.EndTag();
		}

		private static void WriteCodeBlock(string id, CodeBlock element)
		{
			_htmlWriter.StartTag("div", id: id, classes: "codeblock");
			_htmlWriter.PushAttribute("data-start", element.lineStart.ToString());
			var lineNumbersClass = "";
			if (element.showLineNumbers)
				lineNumbersClass = "line-numbers ";
			_htmlWriter.StartTag("pre", classes: lineNumbersClass + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)), useNewLine: false);
			_htmlWriter.StartTag("code", classes: $"language-clike", useNewLine: false);
			WriteText( element.code);
			_htmlWriter.EndTag(false);
			_htmlWriter.EndTag(false);
			_htmlWriter.StartTag("div", classes: "codeblock-caption");
			_htmlWriter.Write(element.caption);
			_htmlWriter.EndTag();
			_htmlWriter.EndTag();
		}

		private static void WriteIFrame(string id, IFrame element)
		{
			_htmlWriter.PushAttribute("src", element.src);
			if (element.allow != null)
				_htmlWriter.PushAttribute("allow", element.allow);
			_htmlWriter.StartTag("iframe", id: id, classes: "iframe");
			_htmlWriter.EndTag();
		}

		//Element.feld = <Formel->value>

		private static void WriteSlider(string id, Slider element)
		{
			var jsId = id.Replace('-', '_');
			WriteSliderFunction(id, element);
			_htmlWriter.PushAttribute("type", "range");
			_htmlWriter.PushAttribute("min", element.range.From.ToString());
			_htmlWriter.PushAttribute("max", element.range.To.ToString());
			if (element.range.Step != 1)
				_htmlWriter.PushAttribute("step", element.range.Step.ToString());
			_htmlWriter.PushAttribute("value", element.value.ToString());
			_htmlWriter.PushAttribute("oninput", $"oninput_{jsId}()");
			_htmlWriter.StartTag("input", id: id, classes: "slider " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.EndTag();
		}

		private static void WriteSliderFunction(string id, Slider element)
		{
			var jsId = id.Replace('-', '_');
			var parentName = id.Split('-')[0];
			_jsWriter.StartFunction($"oninput_{jsId}");
			_jsWriter.WriteVariableDeclarationInline("slider", $"document.getElementById('{id}')");
			foreach (var d in element.get_Dependencies())
			{
				_jsWriter.WriteVariableDeclarationInline(d.Element.name, $"document.getElementById('{parentName}-{d.Element.name}')");
				_jsWriter.WriteAssignment($"{d.Element.name}.{StyleWriter.ToCssAttribute(d.Field)}", d.Value.Insert("slider.value"));
			}
			_jsWriter.EndFunction();
		}

		private static void WriteContainer(string id, Container element)
		{
			_htmlWriter.StartTag("div", id: id, classes: "container " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			string parentName = null;
			if(id != null)
				parentName = id.Split('-')[0];
			WriteElement(parentName, element.child, element);
			_htmlWriter.EndTag();
		}

		private static void WriteStack(string id, Stack stack)
		{
			_htmlWriter.StartTag("div", id: id, classes: "stack " + string.Join(" ", stack.get_AppliedStyles().Select(s => s.Name)));
			string parentName = null;
			if(id != null)
				parentName = id.Split('-')[0];
			foreach (var element in stack.children)
			{
				//TODO(Debate): They don't have an id.
				//So we need to set a name
				//e.g. stackChild0, stackChild1, stackChild2...
				WriteElement(parentName, element, stack);
			}
			_htmlWriter.EndTag();
		}

		private static void WriteRectangle(string id, Rectangle element)
		{
			_htmlWriter.StartTag("div", id: id, classes: "rect " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.EndTag();
		}

		private static void WriteLineChart(string id, LineChart element)
		{
			_htmlWriter.StartTag("div", id: id, classes: "lineChart " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			ChartWriter.WriteChart(_jsWriter, id, element);
			_htmlWriter.EndTag();
		}

		private static void WriteLabel(string id, Label element)
		{
			_htmlWriter.StartTag("p", id: id, classes: "label " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)), useNewLine: false);
			WriteFormattedText(element.text);
			_htmlWriter.EndTag();
		}

		private static void WriteText(string text)
		{
			var span = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				var character = text[i];
				var next = (char)0;
				if (i + 1 < text.Length)
					next = text[i + 1];
				switch (character)
				{
					case '\\':
						i++;
						switch (next)
						{
							case 'n':
								span.Append('\n');
								break;
							case 't':
								span.Append('\t');
								break;
							case '\\':
								span.Append('\\');
								break;
							case '\'':
								span.Append('\'');
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
							case '\'':
								span.Append('\'');
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

		private static void WriteBoxElement(string id, BoxElement element)
		{
			_htmlWriter.StartTag("div", id: id, classes: element.TypeName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			string parentName = null;
			if (id != null) parentName = id.Split('-')[0];
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

		private static void WriteImage(string id, Image element)
		{
			_htmlWriter.PushAttribute("src", element.source.Path);
			if (element.alt != string.Empty)
				_htmlWriter.PushAttribute("alt", element.alt);
			_htmlWriter.WriteTag("img", id: id, needsEnding: false, classes: "image " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
		}
	}
}
