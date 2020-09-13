using Minsk.CodeAnalysis;
using Slides;
using Slides.Code;
using Slides.Elements;
using Slides.Elements.SVG;
using Slides.Helpers;
using Slides.MathExpressions;
using Slides.SVG;
using SVGLib;
using SVGLib.GraphicsElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Slides.Helpers.JavaScriptEmitter;
using Path = System.IO.Path;
using SVGTag = SVGLib.ContainerElements.SVGTag;

namespace HTMLWriter
{
	public static class PresentationWriter
	{
		static HTMLWriter _htmlWriter;
		static JavaScriptWriter _jsWriter;
		static CSSWriter _cssWriter;
		static string _stdTransition = null;

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
			if (presentation.Flags.CodeHighlighter != CodeHighlighter.None)
			{
				CopyFile("prism.js", targetDirectory, alwaysCopyEverything);
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

						if(presentation.Flags.UsesYouTube)
						{
							_htmlWriter.PushAttribute("async");
							_htmlWriter.UseJS("https://www.youtube.com/iframe_api");
							_jsWriter.StartFunctionCollector("youtubeAPIReady");
							_jsWriter.SwitchInto("youtubeAPIReady");
							_jsWriter.WriteAssignment("ytPlayers", "[]");
							_jsWriter.ResetWriter();
						}

						_htmlWriter.UseCSS("index.css");
						_htmlWriter.UseJS("index.js");
						_htmlWriter.UseCSS("core.css");
						_htmlWriter.UseJS("core.js");
						_htmlWriter.UseJS("datatypes.js");

						_htmlWriter.PushAttribute("async");
						_htmlWriter.UseJS("https://cdnjs.cloudflare.com/ajax/libs/mathjs/6.2.5/math.min.js");
						_htmlWriter.PushAttribute("async");
						_htmlWriter.UseJS("https://cdn.jsdelivr.net/npm/apexcharts");
						if (presentation.Flags.CodeHighlighter != CodeHighlighter.None)
						{
							switch (presentation.Flags.CodeHighlighter)
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

						if (presentation.Flags.CodeHighlighter != CodeHighlighter.None)
							_htmlWriter.UseJS("prism.js");

						FilterWriter.Write(_htmlWriter, presentation.CustomFilter);

						WriteJSInsertions(presentation.JSInsertions);


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

		private static void WriteJSInsertions(JSInsertionBlock[] insertions)
		{
			var timeFunctions = new List<string>();
			var stepFunctions = new List<Step>();
			var sortedInsertions = new List<Stack<JSInsertionBlock>>();
			foreach (var insertion in insertions)
			{
				var foundIndex = sortedInsertions.FindIndex(e => e.First().FunctionName == insertion.FunctionName && e.First().Kind == insertion.Kind);
				if (foundIndex < 0)
				{
					foundIndex = sortedInsertions.Count;
					sortedInsertions.Add(new Stack<JSInsertionBlock>());
				}
				sortedInsertions[foundIndex].Push(insertion);
			}
			foreach (var insertionStack in sortedInsertions)
			{
				var kind = insertionStack.Peek().Kind;
				var functionName = insertionStack.Peek().FunctionName + "_" + kind;
				if (kind == JSInsertionKind.Time)
					timeFunctions.Add(functionName);
				else if(kind == JSInsertionKind.Step)
				{
					stepFunctions.Add((Step)insertionStack.Peek().Value);
				}
				_jsWriter.StartFunction(functionName);
				foreach (var insertion in insertionStack)
				{
					foreach (var variables in insertion.Variables)
					{
						_jsWriter.WriteVariableDeclarationInline(variables.Key, variables.Value);
					}
				}
				_jsWriter.WriteLine("");
				foreach (var insertion in insertionStack)
				{
					_jsWriter.WriteLine(insertion.Body);
				}
				_jsWriter.EndFunction();
			}
			_jsWriter.StartFunction($"update_totalTime");
			foreach (var f in timeFunctions)
			{
				_jsWriter.WriteFunctionCall(f);
			}
			_jsWriter.EndFunction();
			_jsWriter.StartFunction("update_step", "step");
			_jsWriter.StartSwitch("step.dataset.stepNumericalId");
			foreach (var s in stepFunctions)
			{
				_jsWriter.StartCase(s.ID.ToString());
				_jsWriter.WriteFunctionCall($"step_{s.ID}_Step");
				_jsWriter.EndCase();
			}
			_jsWriter.EndSwitch();
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
			foreach (var child in transition.get_Children())
			{
				WriteElement(transition.name, child);
			}
			_htmlWriter.EndTag();
		}

		private static void Write(Slide slide)
		{
			if (!slide.Attributes.isVisible)
				return;
			StyleWriter.WriteSlide(_cssWriter, slide);
			_htmlWriter.PushAttribute("data-transition-id", _stdTransition);
			_htmlWriter.StartTag("section", id: slide.Name, classes: "slide");
			_htmlWriter.StartTag("div", classes: "slide-content");
			foreach (var step in slide.Steps)
			{
				WriteStep(slide, step);
			}
			_htmlWriter.EndTag();
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
			_htmlWriter.PushAttribute("data-step-numerical-id", step.ID.ToString());
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
				AnimationWriter.Write(_jsWriter, animation, step.ID, $"{parent.Name}-{animation.Element.name}");
			}
			_htmlWriter.EndTag();
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

		private static void WriteElement(string parentName, Element element, Element parent = null, string optionalFieldName = null)
		{
			StyleWriter.WriteElement(_cssWriter, parentName, element, parent);
			switch (element.kind)
			{
				case ElementKind.Image:
					WriteImage(parentName, (Image)element, optionalFieldName);
					break;
				case ElementKind.BoxElement:
					WriteBoxElement(parentName, (BoxElement)element, optionalFieldName);
					break;
				case ElementKind.Label:
					WriteLabel(parentName, (Label)element, optionalFieldName);
					break;
				case ElementKind.Chart:
					WriteLineChart(parentName, (LineChart)element, optionalFieldName);
					break;
				case ElementKind.MathPlot:
					WriteMathPlot(parentName, (MathPlot)element, optionalFieldName);
					break;
				case ElementKind.Stack:
					WriteStack(parentName, (Stack)element, optionalFieldName);
					break;
				case ElementKind.Container:
					WriteContainer(parentName, (Container)element, optionalFieldName);
					break;
				case ElementKind.SplittedContainer:
					WriteSplittedContainer(parentName, (SplittedContainer)element, optionalFieldName);
					break;
				case ElementKind.List:
					WriteList(parentName, (List)element, optionalFieldName);
					break;
				case ElementKind.CodeBlock:
					WriteCodeBlock(parentName, (CodeBlock)element, optionalFieldName);
					break;
				case ElementKind.IFrame:
					WriteIFrame(parentName, (IFrame)element, optionalFieldName);
					break;
				case ElementKind.Slider:
					WriteSlider(parentName, (Slider)element, optionalFieldName);
					break;
				case ElementKind.SVGContainer:
					WriteSVGContainer(parentName, (SVGContainer)element, optionalFieldName);
					break;
				case ElementKind.UnitRect:
					WriteUnitRect(parentName, (UnitRect)element, optionalFieldName);
					break;
				case ElementKind.UnitSVGShape:
					WriteUnitSVGShape(parentName, (UnitLine)element, optionalFieldName);
					break;
				case ElementKind.Table:
					WriteTable(parentName, (Table)element, optionalFieldName);
					break;
				case ElementKind.TableChild:
					WriteTableChild(parentName, (TableChild)element, optionalFieldName);
					break;
				case ElementKind.Captioned:
					WriteCaptioned(parentName, (Captioned)element, optionalFieldName);
					break;
				case ElementKind.YouTubePlayer:
					WriteYouTubePlayer(parentName, (YouTubePlayer)element, optionalFieldName);
					break;
				default:
					throw new Exception($"ElementType unknown: {element.kind}");
			}
		}


		private static void WriteList(string parentName, List element, string optionalFieldName)
		{
			var startTag = "ul";
			if (element.isOrdered)
				startTag = "ol";
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag(startTag, id: id, classes: "list " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			foreach (var child in element.children)
			{
				
				if(child is Label) //TODO: Hacky. Replace <p> with <li> instead!
					_htmlWriter.StartTag("li", useNewLine: false);
				WriteElement(id, child, element);
				if (child is Label)
					_htmlWriter.EndTag();

			}
			_htmlWriter.EndTag();
		}

		private static void WriteCodeBlock(string parentName, CodeBlock element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			_htmlWriter.StartTag("div", id: id, classes: "codeblock " + optionalFieldName);
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
			_htmlWriter.EndTag();
		}

		private static void WriteIFrame(string parentName, IFrame element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.PushAttribute("src", element.src);
			if (element.allow != null)
				_htmlWriter.PushAttribute("allow", element.allow);
			//TODO: Use applied styles!
			_htmlWriter.StartTag("iframe", id: id, classes: "iframe " + optionalFieldName);
			_htmlWriter.EndTag();
		}

		//Element.feld = <Formel->value>

		private static void WriteSlider(string parentName, Slider element, string optionalFieldName)
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
			_htmlWriter.StartTag("input", id: id, classes: "slider " + optionalFieldName + " " + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.EndTag();
		}


		private static void WriteSliderFunction(string parentName, Slider element)
		{
			var id = $"{parentName}-{element.name}";
			var jsId = id.Replace('-', '_');
			_jsWriter.StartFunction($"oninput_{jsId}");
			foreach (var insertion in element.get_JSInsertions())
			{
				_jsWriter.WriteFunctionCall(insertion.FunctionName + "_" + insertion.Kind);
			}
			_jsWriter.EndFunction();
		}

		private static void WriteSVGContainer(string parentName, SVGContainer element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;

			var child = element.Element;
			var viewBox = SVGWriter.GetViewBox(child);
			_htmlWriter.PushAttribute("viewBox", $"{viewBox}");
			//var svg = child as SVGTag;
			//if (svg != null)
			//{
			//	_htmlWriter.PushAttribute("preserveAspectRatio", $"{svg.PreserveAspectRatioAlign} {svg.PreserveAspectRatioMeetOrSlice}");
			//}
			_htmlWriter.StartTag("svg", id: id, classes: "svgcontainer " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			//if (svg != null) //TODO: FIXME: Put fill etc on this css. Right now it just gets discarded..
			//{
			//	foreach (var svgChild in svg.Children)
			//	{
			//		SVGWriter.Write(_htmlWriter, svgChild);
			//	}
			//}
			//else 
				SVGWriter.Write(_htmlWriter, child);
			_htmlWriter.EndTag();
		}

		private static void WriteUnitRect(string parentName, UnitRect element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "rect " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.EndTag();
		}
		
		private static void WriteUnitSVGShape(string parentName, UnitLine element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("svg", id: $"{id}-container", classes: "unitSVGShapeContainer " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			SVGWriter.Write(_htmlWriter, element);
			_htmlWriter.EndTag();
		}

		private static void WriteTable(string parentName, Table element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("table", id: id, classes: "table " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			//var cellStyle = element.get_TableChildStyle(parentName);
			//StyleWriter.Write(_cssWriter, cellStyle, out _);
			for (int r = 0; r < element.rows; r++)
			{
				_htmlWriter.StartTag("tr");
				for (int c = 0; c < element.columns; c++)
				{
					//element[r, c].applyStyle(cellStyle);
					WriteElement(id, element[c, r], element);
				}
				_htmlWriter.EndTag();
			}
			_htmlWriter.EndTag();
		}

		private static void WriteTableChild(string parentName, TableChild element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("td", id: id, classes: "tablecell " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.Write(element.content);
			_htmlWriter.EndTag();
		}

		//TODO: Styling for captioned.isOutside and caption.direction;
		private static void WriteCaptioned(string parentName, Captioned element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "captioned " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			WriteElement(id, element.child, element, "child");
			WriteElement(id, element.caption, element, "caption");
			_htmlWriter.EndTag();
		}

		private static void WriteYouTubePlayer(string parentName, YouTubePlayer element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "youtubeplayer " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			_htmlWriter.EndTag();


			_jsWriter.SwitchInto("youtubeAPIReady");
			var size = YouTubePlayer.GetDefaultPlayerSize(element.quality);
			var shouldMuteString = "";
			if(element.isMuted)
				shouldMuteString = "e.target.mute();";

			var playerInit = $@"new YT.Player('{id}', {{
			 height: '{(int)size.Y}',
          width: '{(int)size.X}',
          videoId: '{element.videoId}',
          playerVars: {ObjectToString(element.parameters)},
			 events: {{
				 'onReady': function(e) {{
					 {shouldMuteString}
				 }}
			 }}
		}})";
			_jsWriter.WriteAssignment("curPlayer", playerInit);
			_jsWriter.WriteAssignment("curPlayer.stepNumericalId", element.get_Step().ID.ToString());
			_jsWriter.WriteAssignment("curPlayer.slideId", $"'{element.get_Step().ParentName}'");
			_jsWriter.WriteAssignment("curPlayer.keepPlaying", element.keepPlaying.ToString().ToLower());
			if (element.soundOnly)
				_jsWriter.WriteAssignment("curPlayer.getIframe().style.display", "'none'");
			_jsWriter.WriteAssignment("curPlayer.autoPaused", "false");

			_jsWriter.WriteFunctionCall("ytPlayers.push", new JavaScriptObject("curPlayer"));
			_jsWriter.ResetWriter();
		}

		private static void WriteContainer(string parentName, Container element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "container " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			WriteElement(id, element.child, element);
			_htmlWriter.EndTag();
		}

		private static void WriteSplittedContainer(string parentName, SplittedContainer element, string optionalFieldName)
		{

			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "splittedContainer " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			WriteElement(id, element.childA, element);
			WriteElement(id, element.childB, element);
			_htmlWriter.EndTag();
		}
		private static void WriteStack(string parentName, Stack stack, string optionalFieldName)
		{
			var id = $"{parentName}-{stack.name}";
			if (string.IsNullOrEmpty(stack.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: "stack " + optionalFieldName + " " + string.Join(" ", stack.get_AppliedStyles().Select(s => s.Name)));
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

		private static void WriteLineChart(string parentName, LineChart element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			_htmlWriter.StartTag("div", id: id, classes: "lineChart " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			ChartWriter.WriteChart(_jsWriter, id, element);
			_htmlWriter.EndTag();
		}

		private static void WriteMathPlot(string parentName, MathPlot element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			_htmlWriter.StartTag("div", id: id, classes: "mathPlot " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			ChartWriter.WritePlot(_jsWriter, parentName, element);
			_htmlWriter.EndTag();

		}

		private static void WriteLabel(string parentName, Label element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", classes: "label-container", useNewLine: false);
			_htmlWriter.StartTag("p", id: id, classes: "label " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)), useNewLine: false);
			WriteFormattedText(element.text);
			_htmlWriter.EndTag(useNewLine: false);
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
			text = FormattedString.Convert(text).ToHTML();
			var span = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				var prev = '\0';
				if (i > 0) prev = text[i - 1];
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
								span.Append("<br>");
								break;
							default:
								span.Append(character);
								break;
						}
						break;
					case ' ':
						if (next != ' ' && prev != ' ')
						{
							span.Append(character);
							break;
						}
						int spaces = 1;
						while (text[i + spaces] == ' ' && spaces < 4)
						{
							spaces++;
							if (i + spaces >= text.Length)
								break;
						}
						switch (spaces)
						{
							case 4:
								span.Append("&emsp;");
								break;
							case 3:
								span.Append("&ensp;&nbsp;");
								break;
							case 2:
								span.Append("&ensp;");
								break;
							case 1:
								span.Append("&nbsp;");
								break;
							default:
								throw new Exception();
						}
						i += spaces - 1;
						break;
					default:
						span.Append(character);
						break;
				}
			}

			_htmlWriter.Write(span.ToString());
		}

		private static void WriteBoxElement(string parentName, BoxElement element, string optionalFieldName)
		{
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.StartTag("div", id: id, classes: element.TypeName + " " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));
			int i = 0;
			foreach (var child in element.BoxChildren)
			{
				//TODO(Debate): They don't have an id.
				//Or maybe try to get the name from the group-body..
				// We are trying to do that right now. But you don't always have a

				//TODO(Temp)
				if (child.name == null)
					child.name = i.ToString();
				i++;
				WriteElement(id, child, element);
			}
			_htmlWriter.EndTag();
		}

		private static void WriteImage(string parentName, Image element, string optionalFieldName)
		{
			_htmlWriter.PushAttribute("src", element.source.Path);
			if (element.alt != string.Empty)
				_htmlWriter.PushAttribute("alt", element.alt);
			var id = $"{parentName}-{element.name}";
			if (string.IsNullOrEmpty(element.name))
				id = null;
			_htmlWriter.WriteTag("img", id: id, needsEnding: false, classes: "image " + optionalFieldName + " " + string.Join(" ", element.get_AppliedStyles().Select(s => s.Name)));

		}
	}
}
