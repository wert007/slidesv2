using Minsk.CodeAnalysis;
using Slides;
using Slides.Code;
using Slides.MathExpressions;
using Slides.SVG;
using System;
using System.Collections.Generic;
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
					File.Copy(Path.Combine(CompilationFlags.Directory, referencedFile), targetFile);
				}
				else if (alwaysCopyEverything)
				{
					File.Delete(targetFile);
					File.Copy(Path.Combine(CompilationFlags.Directory, referencedFile), targetFile);
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
						_htmlWriter.UseJS("https://cdnjs.cloudflare.com/ajax/libs/mathjs/6.2.5/math.min.js");
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

						WriteStdOverlay();

						if (presentation.CodeHighlighter != CodeHighlighter.None)
							_htmlWriter.UseJS("prism.js");

						FilterWriter.Write(_htmlWriter, presentation.CustomFilter);

						WriteDependency(presentation.Dependencies);


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

		private static void WriteDependency(FieldDependency[] dependencies)
		{
			_jsWriter.StartFunction($"update_totalTime");
			var declaredVariables = new HashSet<string>();
			foreach (var d in dependencies)
			{
				if (d.Element != null)
				{
						var name = d.Element.get_Id().Replace('-', '_');
					if (!declaredVariables.Contains(name))
					{
						_jsWriter.WriteVariableDeclarationInline(name, $"document.getElementById('{d.Element.get_Id()}')");
						declaredVariables.Add(name);
					}
					_jsWriter.WriteAssignment($"{name}.{JavaScriptWriter.ToJSAttribute(CSSWriter.ToCssAttribute(d.Field))}", d.Value.Insert("totalTime"));
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			_jsWriter.EndFunction();

		}

		private static void WriteStdOverlay()
		{
			_htmlWriter.StartTag("div", "search-slide", "invisible");
			_htmlWriter.PushAttribute("type", "text");
			_htmlWriter.WriteTag("input", "search-slide-input");
			_htmlWriter.StartTag("ul", "search-slide-suggestions");
			_htmlWriter.EndTag();
			_htmlWriter.EndTag();
		}

		private static void Write(Transition transition)
		{
			StyleWriter.WriteTransition(_cssWriter, transition);
			//data-duration="1000"
			_htmlWriter.PushAttribute("data-duration", transition.duration.toMilliseconds().ToString());
			_htmlWriter.StartTag("section", id: transition.name, classes: "transition");
			_htmlWriter.EndTag();
		}

		private static void Write(Slide slide)
		{
			if (!slide.Attributes.isVisible)
				return;
			StyleWriter.WriteSlide(_cssWriter, slide);
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
			for (int i = 0; i < step.DataChildren.Length; i++)
			{
				if (step.DataChildren[i] is MathFormula f)
					WriteMathFormula(parent.Name, step.DataChildrenNames[i], f);
			}
			foreach (var animation in step.AnimationCalls)
			{
				AnimationWriter.Write(_jsWriter, animation, _stepCounter, $"{parent.Name}-{animation.Element.name}");
			}
			_htmlWriter.EndTag();
			_stepCounter++;
		}

		private static void WriteMathFormula(string parentName, string name, MathFormula formula)
		{
			var variableName = $"{parentName}_{name}_scope";
			_jsWriter.StartVariableDeclaration(variableName);
			_jsWriter.StartObject();
			for (int i = 0; i < formula.Variables.Length; i++)
			{
				_jsWriter.WriteField(formula.Variables[i], formula.Values[i]);
			}
			_jsWriter.EndObject();
			_jsWriter.EndVariableDeclaration();
		}

		private static void WriteElement(string parentName, Element element, Element parent = null)
		{
			//This is totally wrong!
			foreach (var style in element.get_AppliedStyles())
			{
				if (style.ModifiedFields.ContainsKey("orientation"))
					element.orientation = (Orientation)style.ModifiedFields["orientation"];
			}
			StyleWriter.WriteElement(_cssWriter, parentName, element, parent);
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
				case ElementType.MathPlot:
					WriteMathPlot(parentName, (MathPlot)element);
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
				case ElementType.SplittedContainer:
					WriteSplittedContainer(parentName, (SplittedContainer)element);
					break;
				case ElementType.List:
					WriteList(parentName, (List)element);
					break;
				case ElementType.CodeBlock:
					WriteCodeBlock(parentName, (CodeBlock)element);
					break;
				case ElementType.IFrame:
					WriteIFrame(parentName, (IFrame)element);
					break;
				case ElementType.Slider:
					WriteSlider(parentName, (Slider)element);
					break;
				case ElementType.SVGContainer:
					WriteSVGContainer(parentName, (SVGContainer)element);
					break;
				case ElementType.Table:
					WriteTable(parentName, (Table)element);
					break;
				case ElementType.TableChild:
					WriteTableChild(parentName, (TableChild)element);
					break;
				default:
					throw new Exception($"ElementType unknown: {element.type}");
			}
		}

		private static void WriteList(string parentName, List element)
		{
			var startTag = "ul";
			if (element.isOrdered)
				startTag = "ol";
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag(startTag, id: id, classes: "list " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			var i = 0;
			foreach (var child in element.children)
			{
				//TODO(Temp)
				child.name = i.ToString();
				i++;
				if (child is List subList)
					WriteList(id, subList);
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

		private static void WriteCodeBlock(string parentName, CodeBlock element)
		{
			var id = $"{parentName}-{element.name}";
			_htmlWriter.StartTag("div", id: id, classes: "codeblock");
			_htmlWriter.PushAttribute("data-start", element.lineStart.ToString());
			var lineNumbersClass = "";
			if (element.showLineNumbers)
				lineNumbersClass = "line-numbers ";
			_htmlWriter.StartTag("pre", classes: lineNumbersClass + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)), useNewLine: false);
			//TODO: Use language!
			_htmlWriter.StartTag("code", classes: $"language-clike", useNewLine: false);
			WriteText(element.code);
			_htmlWriter.EndTag(false);
			_htmlWriter.EndTag(false);
			_htmlWriter.StartTag("div", classes: "codeblock-caption");
			_htmlWriter.Write(element.caption);
			_htmlWriter.EndTag();
			_htmlWriter.EndTag();
		}

		private static void WriteIFrame(string parentName, IFrame element)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.PushAttribute("src", element.src);
			if (element.allow != null)
				_htmlWriter.PushAttribute("allow", element.allow);
			_htmlWriter.StartTag("iframe", id: id, classes: "iframe");
			_htmlWriter.EndTag();
		}

		//Element.feld = <Formel->value>

		private static void WriteSlider(string parentName, Slider element)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			if (id == null)
				throw new Exception();
			var jsId = id.Replace('-', '_');
			WriteSliderFunction(parentName, element);
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


		private static void WriteSliderFunction(string parentName, Slider element)
		{
			var id = $"{parentName}-{element.name}";
			var jsId = id.Replace('-', '_');
			_jsWriter.StartFunction($"oninput_{jsId}");
			_jsWriter.WriteVariableDeclarationInline("slider", $"document.getElementById('{id}')");
			foreach (var d in element.get_Dependencies())
			{
				if (d.Element != null)
				{
					_jsWriter.WriteVariableDeclarationInline(d.Element.name, $"document.getElementById('{parentName}-{d.Element.name}')");
					_jsWriter.WriteAssignment($"{d.Element.name}.{JavaScriptWriter.ToJSAttribute(CSSWriter.ToCssAttribute(d.Field))}", d.Value.Insert("slider.value"));
				}
				else if (d.MathFormula != null)
				{
					//TODO: Use of parentName is hacky. But! 
					//The slider should be on the same slide as the math expression
					_jsWriter.WriteAssignment($"{parentName}_{d.MathFormula.Name}_scope.{d.Field}", d.Value.Insert("slider.value"));
					_jsWriter.WriteFunctionCall($"recalculate_{parentName}_{d.MathFormula.Name}_scope");
				}
			}
			_jsWriter.EndFunction();
		}

		private static void WriteSVGContainer(string parentName, SVGContainer element)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;

			var child = element.Element;

			_htmlWriter.PushAttribute("viewBox", $"{child.x} {child.y} {child.width} {child.height}");
			_htmlWriter.StartTag("svg", id: id, classes: "svgcontainer " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			SVGWriter.Write(_htmlWriter, child);
			_htmlWriter.EndTag();
		}

		private static void WriteTable(string parentName, Table element)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("table", id: id, classes: "table " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			var cellStyle = element.get_TableChildStyle(parentName);
			StyleWriter.Write(_cssWriter, cellStyle, out _);
			for (int r = 0; r < element.rows; r++)
			{
				_htmlWriter.StartTag("tr");
				for (int c = 0; c < element.columns; c++)
				{
					element[r, c].applyStyle(cellStyle);
					WriteElement(id, element[r, c], element);
				}
				_htmlWriter.EndTag();
			}
			_htmlWriter.EndTag();
		}

		private static void WriteTableChild(string parentName, TableChild element)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("td", id: id, classes: "tablecell " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.Write(element.content);
			_htmlWriter.EndTag();
		}

		private static void WriteContainer(string parentName, Container element)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "container " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			WriteElement(id, element.child, element);
			_htmlWriter.EndTag();
		}

		private static void WriteSplittedContainer(string parentName, SplittedContainer element)
		{

			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "splittedContainer " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			WriteElement(id, element.childA, element);
			WriteElement(id, element.childB, element);
			_htmlWriter.EndTag();
		}
		private static void WriteStack(string parentName, Stack stack)
		{
			var id = $"{parentName}-{stack.name}";
			if (string.IsNullOrEmpty(stack.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "stack " + string.Join(" ", stack.get_AppliedStyles().Select(s => s.Name)));
			var i = 0;
			foreach (var element in stack.children)
			{
				//TODO(Debate): They don't have an id.
				//So we need to set a name
				//e.g. stackChild0, stackChild1, stackChild2...

				//TODO(Temp)
				element.name = i.ToString();
				i++;
				WriteElement(id, element, stack);
			}
			_htmlWriter.EndTag();
		}

		private static void WriteRectangle(string parentName, Rectangle element)
		{
			var id = $"{parentName}-{element.name}";
			_htmlWriter.StartTag("div", id: id, classes: "rect " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.EndTag();
		}

		private static void WriteLineChart(string parentName, LineChart element)
		{
			var id = $"{parentName}-{element.name}";
			_htmlWriter.StartTag("div", id: id, classes: "lineChart " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			ChartWriter.WriteChart(_jsWriter, id, element);
			_htmlWriter.EndTag();
		}

		private static void WriteMathPlot(string parentName, MathPlot element)
		{
			var id = $"{parentName}-{element.name}";
			_htmlWriter.StartTag("div", id: id, classes: "mathPlot " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			ChartWriter.WritePlot(_jsWriter, parentName, element);
			_htmlWriter.EndTag();

		}

		private static void WriteLabel(string parentName, Label element)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
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

		private static void WriteBoxElement(string parentName, BoxElement element)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: element.TypeName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			int i = 0;
			foreach (var child in element.Children)
			{
				//TODO(Debate): They don't have an id.
				//So we need to set a name
				//e.g. groupChild0, groupChild1, groupChild2...
				//Or maybe try to get the name from the group-body..

				//TODO(Temp)
				if (child.name == null)
					child.name = i.ToString();
				i++;
				WriteElement(id, child, element);
			}
			_htmlWriter.EndTag();
		}

		private static void WriteImage(string parentName, Image element)
		{
			_htmlWriter.PushAttribute("src", element.source.Path);
			if (element.alt != string.Empty)
				_htmlWriter.PushAttribute("alt", element.alt);
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.WriteTag("img", id: id, needsEnding: false, classes: "image " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));

		}
	}
}
