using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Code;
using Slides.Debug;
using Slides.Filters;
using Slides.MathExpressions;
using SVGGroup = SVGLib.ContainerElements.Group;
using Color = Slides.Color;
using SVGLib.GraphicsElements;
using SVGLib;
using SVGLib.ContainerElements;
using SVGLib.Datatypes;
using Slides.Elements;
using Slides.Helpers;

namespace Minsk.CodeAnalysis
{


	internal sealed class Evaluator : StatementEvaluator
	{

		private LibrarySymbol[] _referenced;

		LibrarySymbol library = null;
		Presentation presentation = null;

		private string _currentIdBase;
		private SlideAttributes _currentSlide = null;
		private LibrarySymbol _currentReferenced = null;
		public static PresentationFlags Flags = new PresentationFlags();

		private readonly Dictionary<VariableSymbol, BoundStatement> _declarations;

		private readonly Dictionary<string, bool> _libraryUsed = new Dictionary<string, bool>();

		private List<CaseSymbol> _animationCases = null;
		private HashSet<string> _referencedFiles = new HashSet<string>();
		private List<Transition> _transitions = new List<Transition>();
		private List<CustomFilter> _filters = new List<CustomFilter>();
		private List<Step> _steps = new List<Step>();
		private readonly Dictionary<VariableSymbol, Slide> _slides = new Dictionary<VariableSymbol, Slide>();
		private readonly Dictionary<TypeSymbol, BodySymbol> _customTypes = new Dictionary<TypeSymbol, BodySymbol>();
		private readonly Dictionary<VariableSymbol, Style> _styles = new Dictionary<VariableSymbol, Style>();
		private readonly Dictionary<Element, AnimationCall> _animations = new Dictionary<Element, AnimationCall>();
		private readonly Dictionary<VariableSymbol, BoundTemplateStatement> _templates = new Dictionary<VariableSymbol, BoundTemplateStatement>();
		private readonly List<string> _imports = new List<string>();
		private int _invisibleSlideCount = 0;
		private int _slideCount = 0;

		private readonly JSEmitter _jsEmitter = new JSEmitter();
		private readonly List<JSInsertionBlock> _jsInsertions = new List<JSInsertionBlock>();
		private Stack<Dictionary<VariableSymbol, Element>> _groupChildren = new Stack<Dictionary<VariableSymbol, Element>>();
		private bool _isInSVG;
		private Stack<Dictionary<VariableSymbol, SVGGraphicsElement>> _svgChildren = new Stack<Dictionary<VariableSymbol, SVGGraphicsElement>>();
		private Stack<List<CustomStyle>> _groupAppliedStyles = new Stack<List<CustomStyle>>();

		public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables, LibrarySymbol[] referenced, Dictionary<VariableSymbol, BoundStatement> declarations, PresentationFlags flags)
			: base(root, new VariableValueCollection(variables))
		{
			Flags = flags;
			_referenced = referenced;
			_declarations = declarations;
			foreach (var declaration in _declarations)
			{
				if (declaration.Key.Type == _builtInTypes.LookSymbolUp(typeof(SlideAttributes))
					&& declaration.Key.IsVisible)
					_slideCount++;
			}
			_constants.Add(new VariableSymbol("slideCount", true, PrimitiveTypeSymbol.Integer, false), _slideCount);
			_constants.Add(new VariableSymbol("totalTime", true, PrimitiveTypeSymbol.Integer, false), 0);
			//TODO: This needs to be calculated in js!
			//_variables.Add(new VariableSymbol("elapsedTime", true, _builtInTypes.LookSymbolUp(typeof(float)), false), 5);
		}

		public object Evaluate()
		{

			var index = 0;

			Flags.AnimationsAllowed = true;
			Flags.StructsAllowed = true;
			Flags.GroupsAllowed = true;
			Flags.StyleAllowed = true;
			Flags.TemplatesAllowed = true;
			Flags.CodeHighlighter = CodeHighlighter.None;
			var root = (BoundBlockStatement)_root;
			while (index < root.Statements.Length)
			{
				var s = root.Statements[index];
				EvaluateStatement(s);
				index++;

			}

			object result = null;
			if (Flags.IsLibrarySymbol)
			{
				library = new LibrarySymbol(Flags.LibraryName, _referenced, _customTypes.Values.ToArray(), _styles.Values.ToArray(), _variables, _imports.ToArray());
				foreach (var customType in library.CustomTypes)
				{
					customType.Source = library;
				}
				result = library;
			}
			else
			{
				var libraries = new Library[_referenced.Length];
				for (int i = 0; i < libraries.Length; i++)
				{
					//TODO(Major): Recursion
					libraries[i] = new Library(_referenced[i].Name, _referenced[i].Libraries?.Select(l => new Library(l.Name, null, l.Styles)).ToArray(), _referenced[i].Styles);
					_imports.AddRange(_referenced[i].Imports);
				}
				presentation = new Presentation(_slides.Values.ToArray(), _styles.Values.ToArray(), _filters.ToArray(), _transitions.ToArray(), libraries, _jsInsertions.ToArray(), _imports.ToArray(), _referencedFiles.ToArray(), Flags.CodeHighlighter);
				result = presentation;
			}

			return result;
		}

		protected override void EvaluateFilterStatement(BoundFilterStatement node)
		{
			var name = node.Variable.Name;
			var filters = new List<SVGFilter>();
			var filterNames = new List<string>();
			_variables = _variables.Push();
			_variables.Add(node.Parameter.Statements[0].Variable, new SourceGraphicFilterInput());
			EvaluateStatement(node.Body);
			_variables = _variables.Pop(out var children);
			foreach (var child in children)
			{
				if (child.Value is SVGFilter f)
				{
					filters.Add(f);
					filterNames.Add(child.Key.Name);
				}
			}
			var result = new CustomFilter(name, filters.ToArray(), filterNames.ToArray());
			_variables[node.Variable] = result;
			_filters.Add(result);
		}

		private Slides.Attribute[] CollectAnimationFields(BoundBlockStatement statement, Type animatedObject)
		{
			var result = new List<Slides.Attribute>();
			foreach (var s in statement.Statements)
			{
				result.AddRange(CollectAnimationFields(s, animatedObject));
			}
			return result.ToArray();
		}

		private Slides.Attribute[] CollectAnimationFields(BoundExpressionStatement statement, Type animatedObject)
		{
			return CollectAnimationFields(statement.Expression, animatedObject);
		}

		private Slides.Attribute[] CollectAnimationFields(BoundExpression expression, Type animatedObject)
		{
			switch (expression.Kind)
			{
				case BoundNodeKind.AssignmentExpression:
					return CollectAnimationFields((BoundAssignmentExpression)expression, animatedObject);
					throw new NotImplementedException();
				case BoundNodeKind.FunctionAccessExpression:
					return CollectAnimationFields((BoundFunctionAccessExpression)expression, animatedObject);
				default:
					return new Slides.Attribute[0];
			}
		}

		private Slides.Attribute[] CollectAnimationFields(BoundAssignmentExpression expression, Type animatedObject)
		{
			var value = EvaluateExpression(expression.Expression);
			switch (expression.LValue.Kind)
			{
				case BoundNodeKind.VariableExpression:
					var variableExpression = (BoundVariableExpression)expression.LValue;
					switch (variableExpression.Variable.Name)
					{
						case "interpolation":
							return new Slides.Attribute[0];
					}
					break;
				case BoundNodeKind.FieldAccessExpression:
					//TODO: Fixme!
					var fieldAccessExpression = (BoundFieldAccessExpression)expression.LValue;
					return new Slides.Attribute[]
					{
						new Slides.Attribute(fieldAccessExpression.Field.Variable.Name, value),
					};
				default:
					throw new Exception();
			}
			return new Slides.Attribute[0];
		}

		private Slides.Attribute[] CollectAnimationFields(BoundFunctionAccessExpression expression, Type animatedObject)
		{
			throw new NotSupportedException();
			//TODO(Time): hacky
			//without setPadding() Idk when we use this actually...
			var field = expression.FunctionCall.Function.Name;
			var value = EvaluateExpression(expression.FunctionCall.Arguments[0]);
			var parent = expression.Parent;

			var parentObject = EvaluateExpression(parent);
			if (parentObject.GetType() != animatedObject)
				return new Slides.Attribute[0];

			if (value == null)
				throw new Exception();

			return new Slides.Attribute[]
			{
				new Slides.Attribute(field, value)
			};
		}

		private Slides.Attribute[] CollectAnimationFields(BoundStatement statement, Type animatedObject)
		{
			switch (statement.Kind)
			{
				case BoundNodeKind.ExpressionStatement:
					return CollectAnimationFields((BoundExpressionStatement)statement, animatedObject);
				case BoundNodeKind.BlockStatement:
					return CollectAnimationFields((BoundBlockStatement)statement, animatedObject);
				default:
					return new Slides.Attribute[0];
			}
		}


		private List<TypedModifications> CollectStyleFields(BoundBlockStatement statement, string styleName)
		{
			var result = new List<TypedModifications>();
			foreach (var s in statement.Statements)
			{
				foreach (var typedStyle in CollectStyleFields(s, styleName))
				{
					var foundStyle = false;
					foreach (var foundStyles in result)
					{
						if (foundStyles.Type == typedStyle.Type)
						{
							foreach (var field in typedStyle.ModifiedFields)
							{
								foundStyles.ModifiedFields.Add(field.Key, field.Value);
							}
							foundStyle = true;
							break;
						}
					}
					if (!foundStyle)
						result.Add(typedStyle);

				}
			}
			return result;
		}

		private List<TypedModifications> CollectStyleFields(BoundStatement statement, string styleName)
		{
			switch (statement.Kind)
			{
				case BoundNodeKind.ExpressionStatement:
					return CollectStyleFields((BoundExpressionStatement)statement, styleName);
				case BoundNodeKind.IfStatement:
					return CollectStyleFields((BoundIfStatement)statement, styleName);
				case BoundNodeKind.BlockStatement:
					return CollectStyleFields((BoundBlockStatement)statement, styleName);
				case BoundNodeKind.VariableDeclaration:
					EvaluateStatement(statement);
					return new List<TypedModifications>();
				default:
					Logger.LogUnexpectedSyntaxKind(statement.Kind, "CollectStyleFields");
					return new List<TypedModifications>();
			}
		}

		private List<TypedModifications> CollectStyleFields(BoundExpressionStatement statement, string styleName)
		{
			return CollectStyleFields(statement.Expression, styleName);
		}

		private List<TypedModifications> CollectStyleFields(BoundIfStatement statement, string styleName)
		{
			var condition = (bool)EvaluateExpression(statement.Condition);
			if (condition)
			{
				return CollectStyleFields(statement.Body, styleName);
			}
			if (statement.Else != null)
			{
				return CollectStyleFields(statement.Else, styleName);
			}
			return new List<TypedModifications>();
		}

		private List<TypedModifications> CollectStyleFields(BoundExpression expression, string styleName)
		{
			switch (expression.Kind)
			{
				case BoundNodeKind.AssignmentExpression:
					return CollectStyleFields((BoundAssignmentExpression)expression);
				case BoundNodeKind.FunctionAccessExpression:
					return CollectStyleFields((BoundFunctionAccessExpression)expression, styleName);
				default:
					Logger.LogUnexpectedSyntaxKind(expression.Kind, "CollectStyleFields");
					return new List<TypedModifications>();
			}
		}

		private List<TypedModifications> CollectStyleFields(BoundAssignmentExpression expression)
		{
			//TODO: Clean Up
			var field = "";
			var type = "*";
			switch (expression.LValue.Kind)
			{
				case BoundNodeKind.VariableExpression:
					field = ((BoundVariableExpression)expression.LValue).Variable.Name;
					break;
				case BoundNodeKind.FieldAccessExpression:
					var fieldAccessExpression = (BoundFieldAccessExpression)expression.LValue;
					field = fieldAccessExpression.Field.Variable.Name;
					if (fieldAccessExpression.Parent.Kind == BoundNodeKind.VariableExpression)
						type = ((BoundVariableExpression)fieldAccessExpression.Parent).Variable.Name;
					break;
				default:
					throw new Exception();
			}
			var value = EvaluateExpression(expression.Expression);
			var fields = new Dictionary<string, object>();
			fields.Add(field, value);
			return new List<TypedModifications>
			{
				new TypedModifications(type, fields)
			};
		}

		private List<TypedModifications> CollectStyleFields(BoundFunctionAccessExpression expression, string styleName)
		{
			switch (expression.FunctionCall.Function.Name)
			{
				case "applyStyle":
					var style = (CustomStyle)EvaluateExpression(expression.FunctionCall.Arguments[0]);
					return new List<TypedModifications>()
					{ new TypedModifications("*", style.ModifiedFields) };
				default:
					Logger.LogUnmatchedStyleFunction(expression.FunctionCall.Function.Name, styleName);

					return new List<TypedModifications>();
			}
		}


		protected override void EvaluateStyleStatement(BoundStyleStatement node)
		{
			//TODO(Time): I dont know. Can we EVER compute the same style twice?????
			//aka do we need to store styles in _declarations?
			if (node.Variable != null)
				_declarations.Remove(node.Variable);
			if (!Flags.StyleAllowed)
				throw new Exception();
			_variables = _variables.Push();
			if (node.Variable != null)
			{
				var modifiedFields = CollectStyleFields(node.BoundBody, node.Variable.Name);
				//TODO!
				var combinedModifiedFields = new Dictionary<string, object>();
				foreach (var subtype in modifiedFields)
					foreach (var field in subtype.ModifiedFields)
						combinedModifiedFields.Add(field.Key, field.Value);
				var style = new CustomStyle(node.Variable.Name, combinedModifiedFields);
				_styles.Add(node.Variable, style);
			}
			else
			{
				var modifiedFields = CollectStyleFields(node.BoundBody, "std");

				var style = new StdStyle(modifiedFields.ToArray());
				_styles.Add(new VariableSymbol("std", true, _builtInTypes.LookSymbolUp(typeof(StdStyle)), false), style);
			}
			_variables = _variables.Pop(out var _);
		}

		protected override void EvaluateAnimationStatement(BoundAnimationStatement node)
		{
			if (!_declarations.ContainsKey(node.Variable))
				return;
			_animationCases = new List<CaseSymbol>();
			_variables = _variables.Push();
			_variables.Add(AnimationSymbol.InitSymbol, 0f);
			_variables.Add(AnimationSymbol.DoneSymbol, 1f);
			foreach (var statement in node.Body)
			{
				EvaluateCaseStatement(statement);
			}
			_variables = _variables.Pop(out var _);
			_variables[node.Variable] = new AnimationSymbol(node.Variable, node.ElementParameter.Variable, node.TimeParameter.Variable, _animationCases.ToArray());
			_declarations.Remove(node.Variable);
		}

		protected override void EvaluateCaseStatement(BoundCaseStatement node)
		{
			if (_animationCases == null)
				throw new Exception();
			var condition = Convert.ToSingle(EvaluateExpression(node.Condition));
			_animationCases.Add(new CaseSymbol(condition, node.Body));
		}

		private bool TransitionStatementNeedsEvaluation(BoundStatement statement, BoundParameterStatement[] parameters, out TransitionCall from, out TransitionCall to)
		{
			from = null;
			to = null;
			if (statement.Kind == BoundNodeKind.ExpressionStatement)
			{
				var expression = ((BoundExpressionStatement)statement).Expression;
				if (expression.Kind == BoundNodeKind.FunctionAccessExpression)
				{
					var functionAccess = ((BoundFunctionAccessExpression)expression);
					var parent = (functionAccess.Parent as BoundVariableExpression).Variable;
					Time duration = new Time(1, Time.TimeUnit.Milliseconds);
					var delay = (Time)EvaluateExpression(functionAccess.FunctionCall.Arguments[0]);
					var name = functionAccess.FunctionCall.Function.Name;
					if (name != "hide")
						duration = (Time)EvaluateExpression(functionAccess.FunctionCall.Arguments[1]);
					if (name == "hide")
						name = "fadeOut";
					if (parent == parameters[0].Variable)
					{
						//from
						from = new TransitionCall(name, duration, delay);
						return false;
					}
					else if (parent == parameters[1].Variable)
					{
						//to
						to = new TransitionCall(name, duration, delay);
						return false;
					}
				}
			}
			return true;
		}

		protected override void EvaluateTransitionStatement(BoundTransitionStatement node)
		{
			if (!_declarations.ContainsKey(node.Variable))
				return;
			var name = node.Variable.Name;
			TransitionCall from = null;
			TransitionCall to = null;
			_variables = _variables.Push();
			foreach (var statement in node.BoundBody.Statements)
			{
				if (TransitionStatementNeedsEvaluation(statement, node.BoundParameters.Statements, out var tempFrom, out var tempTo))
					EvaluateStatement(statement);
				else
				{
					if (from == null)
						from = tempFrom;
					if (to == null)
						to = tempTo;
				}
			}
			var result = new Transition(name, from, to);
			_variables = _variables.Pop(out var oldVariables);
			foreach (var value in oldVariables)
			{
				switch (value.Key.Name)
				{
					case "background":
						result.background = SlidesConverter.ConvertToBrush(value.Value);
						break;
					case "duration":
						result.duration = (Time)value.Value;
						break;
					default:
						Logger.LogUnusedVariableInGroup(value.Key.Name, value.Value.GetType(), "transition " + name);
						break;
				}
			}
			_transitions.Add(result);
			//_variables.Add(new VariableSymbol(name, true, _typeConverter.LookSymbolUp(typeof(Transition)), false), result);
			_variables.Add(node.Variable, result);
			_declarations.Remove(node.Variable);
		}

		protected override void EvaluateDataStatement(BoundDataStatement node)
		{
			if (!Flags.StructsAllowed)
				throw new Exception();
			var data = new BodySymbol(node.Type, null);
			_customTypes.Add(node.Type, data);
			//TODO(Minor): Maybe we don't need to collect data statements because they declare a type
			//and not a variable.
			_declarations.Remove(new VariableSymbol(node.Type.Name, true, node.Type, false));
		}

		protected override void EvaluateGroupStatement(BoundGroupStatement node)
		{
			if (!Flags.GroupsAllowed)
				throw new Exception();
			var parameters = new List<VariableSymbol>();
			foreach (var parameter in node.Parameters.Statements)
			{
				parameters.Add(parameter.Variable);
			}

			var group = new BodySymbol(node.Type, node.Body);
			_customTypes.Add(node.Type, group);
			_declarations.Remove(new VariableSymbol(node.Type.Name, true, node.Type, false));
		}

		protected override void EvaluateSVGStatement(BoundSVGStatement node)
		{
			if (!Flags.GroupsAllowed)
				throw new Exception();
			var parameters = new List<VariableSymbol>();
			foreach (var parameter in node.Parameters.Statements)
			{
				parameters.Add(parameter.Variable);
			}

			var group = new BodySymbol(node.Type, node.Body);
			_customTypes.Add(node.Type, group);
			_declarations.Remove(new VariableSymbol(node.Type.Name, true, node.Type, false));
		}

		//TODO(Major): galore! Everythings missing. It's a wonder it's working!
		//Thank you, that is really helpful! I see that this is a rather short function
		//but is that your only problem with it? Is there something, that isn't working??
		protected override void EvaluateTemplateStatement(BoundTemplateStatement node)
		{
			if (!_declarations.ContainsKey(node.Variable))
				return;
			if (!Flags.TemplatesAllowed)
				throw new Exception();

			var template = new BodySymbol(node.Variable, node.Body);
			_templates.Add(node.Variable, node);


			_variables[node.Variable] = template;
			_declarations.Remove(node.Variable);
		}

		protected override void DeclareElement(Element e)
		{
			e.set_Step(_steps.Last());
		}

		protected override void EvaluateSlideStatement(BoundSlideStatement node)
		{
			//When _declarations doesn't contain our variable
			//we already evaluated it.
			if (!_declarations.ContainsKey(node.Variable))
				return;
			if (Flags.IsLibrarySymbol)
				throw new Exception();
			_variables = _variables.Push();
			_currentSlide = new SlideAttributes(node.Variable.Name, _slides.Count - _invisibleSlideCount, node.Variable.IsVisible);
			if (!node.Variable.IsVisible)
				_invisibleSlideCount++;

			_steps = new List<Step>();
			foreach (var statement in node.Statements)
			{
				EvaluateStepStatement(statement);
			}
			Template template = null;
			if (node.Template != null)
			{
				_variables = _variables.Push();
				var currentTemplate = _templates[node.Template];
				_variables[currentTemplate.SlideParameter.Variable] = _currentSlide;
				_variables = _variables.Push();
				EvaluateStatement(_templates[node.Template].Body);
				_variables = _variables.Pop(out var children);
				var dataChildren = new List<object>();
				var visualChildren = new List<Element>();
				foreach (var value in children)
				{
					if (value.Value is Element e && e.isVisible)
						visualChildren.Add(e);
					else
						dataChildren.Add(value);
				}
				template = new Template(node.Template.Name, visualChildren.ToArray(), dataChildren.ToArray());
			}
			var slide = new Slide(_currentSlide, _steps.ToArray(), template);
			_currentSlide = null;
			_slides.Add(node.Variable, slide);
			_declarations.Remove(node.Variable);
		}


		private void EvaluateStepStatement(BoundStepStatement node)
		{
			_currentIdBase = _currentSlide.name;
			if (node.Variable != null) _currentIdBase += $"-{node.Variable.Name}";
			//_variables = _variables.Push();
			var step = new Step(node.Variable?.Name, _currentSlide);
			_steps.Add(step);
			EvaluateStatement(node.Body);

			//_variables.Pop(out var slideValues);
			var animationCalls = _animations.Values.ToArray();
			_animations.Clear();
			var dataChildren = new List<object>();
			var dataChildrenNames = new List<string>();
			var visualChildren = new List<Element>();
			foreach (var value in _variables)
			{
				if (_steps.TakeWhile(s => s != step).Any(s => s.DataChildren.Contains(value.Value) || s.VisualChildren.Contains(value.Value)))
					continue;
				if (value.Key.IsVisible && value.Value is Element e)
				{
					e.name = value.Key.Name;
					visualChildren.Add(e);
				}
				else
					switch (value.Key.Name)
					{
						case "background":
							_currentSlide.background = SlidesConverter.ConvertToBrush(value.Value);
							break;
						case "padding":
							_currentSlide.padding = (Thickness)value.Value;
							break;
						case "filter":
							_currentSlide.filter = (Filter)value.Value;
							break;
						case "transition":
							_currentSlide.transition = (Transition)value.Value;
							break;
						case "font":
							_currentSlide.font = (Font)value.Value;
							break;
						case "fontsize":
							_currentSlide.fontsize = (Unit)value.Value;
							break;
						case "color":
							_currentSlide.color = (Color)value.Value;
							break;
						default:
							dataChildren.Add(value.Value);
							dataChildrenNames.Add(value.Key.Name);
							break;
					}
			}
			step.Finalize(animationCalls, visualChildren.ToArray(), dataChildren.ToArray(), dataChildrenNames.ToArray());

		}

		protected override void EvaluateUseStatement(BoundJSInsertionStatement node)
		{
			if (!_currentSlide.isVisible) return;
			foreach (var dependency in node.Dependencies)
			{
				var kind = dependency.GetJSInsertionKind();
				var functionNameBase = _currentIdBase;

				var jsCode = _jsEmitter.Emit(node.Body, _variables, functionNameBase);
				var insertion = new JSInsertionBlock(functionNameBase, jsCode.Code, jsCode.VariableDefinitions, kind);
				_jsInsertions.Add(insertion);
				if (kind == JSInsertionKind.Slider)
				{
					var fieldExpression = (BoundFieldAccessExpression)dependency;
					var parent = (Slider)EvaluateExpression(fieldExpression.Parent);
					parent.add_JSInsertion(insertion);
				}
			}
		}

		protected override object CheckIfIsImport(object value)
		{
			if (value is ImportExpression<LibrarySymbol> importLibrary)
			{
				_libraryUsed[importLibrary.Value.Name] = true;
				return importLibrary.Value;
			}
			else if (value is ImportExpression<Font> importFont)
			{
				_imports.Add(importFont.Href);
				return importFont.Value;
			}
			return value;
		}


		protected override void TryAddChildren(VariableSymbol variable, object value)
		{
			if (_isInSVG)
				TryAddSVGGroupChildren(value, variable);
			else
				TryAddGroupChildren(value, variable);
		}

		private void TryAddGroupChildren(object value, VariableSymbol variable)
		{
			if (!(value is Element || value is object[]))
				return;
			if (_groupChildren.Count > 0 && variable.IsVisible && !variable.Type.IsData)
			{
				if (variable.Type.Type != TypeType.Array)
				{
					if (value is Element element && element.isVisible)
					{
						_groupChildren.Peek().Add(variable, element);
						//if(variable.BoundArrayIndex != null)
						//{
						//	var i = (int)EvaluateExpression(variable.BoundArrayIndex.BoundIndex);
						//	var v = new VariableSymbol($"{variable.Variable.Name}#{i}", variable.Variable.IsReadOnly, variable.Type, variable.Variable.NeedsDataFlag);
						//	_groupChildren.Peek().Add(v, element);
						//}
					}
				}
				else
					for (int i = 0; i < ((object[])value).Length; i++)
					{
						var e = ((object[])value)[i];
						if (e is Element element && element.isVisible)
						{
							var t = ((ArrayTypeSymbol)variable.Type).Child;
							var variableArray = new VariableSymbol($"{variable.Name}#{i}", variable.IsReadOnly, t, variable.NeedsDataFlag);
							_groupChildren.Peek().Add(variableArray, element);
						}
					}
			}
		}

		private void TryAddSVGGroupChildren(object value, VariableSymbol variable)
		{
			if (!(value is SVGGraphicsElement || value is object[]))
				return;
			if (_svgChildren.Count > 0 && variable.IsVisible && !variable.Type.IsData)
			{
				if (variable.Type.Type != TypeType.Array)
				{
					if (value is SVGGraphicsElement element && element.IsVisible)
					{
						_svgChildren.Peek()[variable] = element;
						//if (variable.BoundArrayIndex != null)
						//		{
						//	var i = (int)EvaluateExpression(variable.BoundArrayIndex.BoundIndex);
						//	var v = new VariableSymbol($"{variable.Variable.Name}#{i}", variable.Variable.IsReadOnly, variable.Type, variable.Variable.NeedsDataFlag);
						//	_svggroupChildren.Peek().Add(v, element);
						//}
					}
				}
				else
					for (int i = 0; i < ((object[])value).Length; i++)
					{
						var e = ((object[])value)[i];
						if (e is SVGGraphicsElement element && element.IsVisible)
						{
							var t = ((ArrayTypeSymbol)variable.Type).Child;
							var variableArray = new VariableSymbol($"{variable.Name}#{i}", variable.IsReadOnly, t, variable.NeedsDataFlag);
							_svgChildren.Peek().Add(variableArray, element);
						}
					}
			}
		}




		private object GetValue(Element element, string field)
		{
			switch (field)
			{
				case "margin":
					return element.margin;
				case "margin.Top":
					return element.margin?.top;
				case "background":
					return element.background;
				default:
					throw new NotImplementedException();
			}
		}

		private object EvaluateAnimationCall(AnimationSymbol animation, BoundFunctionExpression function, object[] args)
		{
			//																												 ^
			//TODO: use function parameter	----------------------------------------------------|
			var cases = new CaseCall[animation.Cases.Length];
			var element = (Element)args[0];
			var time = (Time)args[1];
			_variables = _variables.Push();
			_variables.Add(animation.ElementParameter, element);
			_variables.Add(animation.TimeParameter, time);
			var attributes = new HashSet<string>();
			for (int i = 0; i < cases.Length; i++)
			{
				foreach (var field in CollectAnimationFields(animation.Cases[i].Body, args[0].GetType()).Select(a => a.Name))
				{
					if (!attributes.Contains(field))
						attributes.Add(field);
				}
			}
			for (int i = 0; i < cases.Length; i++)
			{
				var attributes2 = new List<Slides.Attribute>();
				attributes2.AddRange(CollectAnimationFields(animation.Cases[i].Body, args[0].GetType()));
				foreach (var attribute in attributes)
				{
					if (!attributes2.Any(a => a.Name == attribute))
						attributes2.Add(new Slides.Attribute(attribute, GetValue(element, attribute)));
				}
				cases[i] = new CaseCall(animation.Cases[i].Condition, attributes2.ToArray());
			}
			_variables = _variables.Pop(out var _);
			var result = new AnimationCall(animation.Variable.Name, attributes.ToArray(), time, cases, element);
			_animations.Add(element, result);
			return result;
		}


		protected override object EvaluateGroupConstructorExpression(object[] args, BodySymbol group)
		{
			if (!(group.Symbol.Type is AdvancedTypeSymbol advanced))
				throw new Exception();

			var isSVG = group.Symbol.Type.CanBeConvertedTo(_builtInTypes.LookSymbolUp(typeof(SVGElement)));
			_isInSVG = isSVG;
			var cVariables = _variables.Push();
			var constructor = advanced.Constructor.FirstOrDefault(c => c.Parameter.Count == args.Length);
			for (int i = 0; i < constructor.Parameter.Count; i++)
			{
				cVariables.Add(constructor.Parameter[i], args[i]);
			}

			_variables = cVariables;
			_variables = _variables.Push();
			var oldReferenced = _currentReferenced;
			_currentReferenced = group.Source;
			if (_currentReferenced != null)
				_libraryUsed[_currentReferenced.Name] = true;
			if (isSVG)
			{
				_svgChildren.Push(new Dictionary<VariableSymbol, SVGGraphicsElement>());
			}
			else
			{
				_groupChildren.Push(new Dictionary<VariableSymbol, Element>());
				_groupAppliedStyles.Push(new List<CustomStyle>());
			}
			if (group.Body != null)
				EvaluateStatement(group.Body);

			_currentReferenced = oldReferenced;

			//TODO(Major): Maybe these values are wrong. If you use for example a for loop
			//where you create a new Label in, then you would have a lot of unnamed
			//children. And all these unnamed children don't have any information
			//about there position. In the end we have Stacks and such for these things

			//On the other hand you could recreate your own stack using a local position
			//variable. So it should be allowed I guess.. Sounds wrong to try silently kill all
			//Elements inside of loops.

			//But we still have the naming problem... you could make it connected to 
			//the iterator so you have hopefully unique names...

			//As of now they are included in the _groupChildren. You just need to give them
			//a better name!
			SVGGraphicsElement[] svgValues = null;
			Element[] groupValues = null;
			if (isSVG)
			{
				svgValues = _svgChildren.Pop().Select(c => c.Value).Where(c => c.IsVisible).ToArray();
			}
			else
			{
				groupValues = _groupChildren.Pop().Select(c => c.Value).Where(c => c.isVisible).ToArray();
			}
			_variables = _variables.Pop(out cVariables);
			_variables = _variables.Pop(out var _);
			object result;
			if (group.Body == null)
			{
				result = new DataObject(advanced, args);
			}
			else if (isSVG)
			{
				var viewBox = (ViewBox)cVariables.FirstOrDefault(v => v.Key.Name == "viewBox").Value;
				result = new SVGTag(viewBox, svgValues);
			}
			else
			{
				var be = new BoxElement(group.Symbol.Name, groupValues);
				foreach (var style in _groupAppliedStyles.Pop())
					(be).applyStyle(style);
				SetAttributes(be, cVariables);
				result = be;
			}
			return result;
		}

		private void SetAttributes(BoxElement element, VariableValueCollection variables)
		{
			foreach (var variable in variables)
			{
				switch (variable.Key.Name)
				{
					case "orientation":
						element.orientation = (Orientation)variable.Value;
						break;
					case "background":
						element.background = SlidesConverter.ConvertToBrush(variable.Value);
						break;
					case "width":
						element.width = Unit.Convert(variable.Value);
						break;
					case "height":
						element.height = Unit.Convert(variable.Value);
						break;
					case "fontsize":
						element.fontsize = Unit.Convert(variable.Value);
						break;
					case "padding":
						element.padding = (Thickness)variable.Value;
						break;
					case "initHeight":
						element.initHeight = Unit.Convert(variable.Value);
						break;
					case "initWidth":
						element.initWidth = Unit.Convert(variable.Value);
						break;
					default:
						var type = variable.Value.GetType();
						var typeSymbol = variable.Key.Type;
						if (!typeSymbol.CanBeConvertedTo(_builtInTypes.LookSymbolUp(typeof(Element))) &&
							type != typeof(ImageSource) && type != typeof(CSVFile))
							Logger.LogUnusedVariableInGroup(variable.Key.Name, _builtInTypes.TryLookTypeUp(variable.Key.Type), element.TypeName);
						break;
				}
			}
		}

		protected override object EvaluateConstructorCall(FunctionSymbol function, object[] args, LibrarySymbol source)
		{
			if (_customTypes.TryGetValue(function.Type, out var group))
			{
				return EvaluateGroupConstructorExpression(args, group);
			}
			if (_currentReferenced != null)
			{
				var customType = _currentReferenced.CustomTypes.FirstOrDefault(ct => ct.Symbol.Type == function.Type);
				if (customType != null)
				{
					return EvaluateGroupConstructorExpression(args, customType);
				}
			}
			if (source != null)
			{
				var customType = source.CustomTypes.FirstOrDefault(ct => ct.Symbol.Type == function.Type);
				if (customType != null)
				{
					return EvaluateGroupConstructorExpression(args, customType);
				}
			}
			var constructor = ConstructorSymbolConverter.Instance.LookConstructorInfoUp(function);
			return ConstructorInvoke(constructor, args);

		}

		protected override object EvaluateNativeFunction(string name, object[] args)
		{
			switch (name)
			{
				case "applyStyle":
					var style = (CustomStyle)args[0];
					if (_groupAppliedStyles.Count == 0)
						_currentSlide.applyStyle(style);
					else
						_groupAppliedStyles.Peek().Add(style);
					break;
				case "setData":
					_currentSlide.setData((string)args[0], (string)args[1]);
					break;
				case "lib": break;
				default:
					throw new Exception();
			}
			return null;

		}

		protected override void AddReferencedLibrary(LibrarySymbol source) => _libraryUsed[source.Name] = true;
		protected override void AddReferencedFile(string fileName) => _referencedFiles.Add(fileName);

		public override object LookVariableUp(VariableSymbol variable)
		{
			var value = _constants[variable];
			if (value == null)
				value = _variables[variable];
			if (value == null && _currentReferenced != null)
			{
				value = _currentReferenced.GlobalVariables[variable];
			}
			if (value == null)
			{
				EvaluateStatement(_declarations[variable]);
				value = _variables[variable];
				if (value != null)
					_declarations.Remove(variable);
			}
			if (value == null)
				throw new Exception();
			return value;
		}

		protected override object EvaluateVariableExpression(BoundVariableExpression v)
		{
			if (v.Type == _builtInTypes.LookSymbolUp(typeof(StdStyle)))
			{
				if (_styles.ContainsKey(v.Variable))
					return _styles[v.Variable];
				//TODO(Minor): if your style is in a different library, tell us which one it is!
				//I mean you probably already do, because otherwise the Binder might cry,
				//but the evaluator doesn't use this piece of information..
				foreach (var lib in _referenced)
				{
					var style = lib.Styles.FirstOrDefault(s => s.Name == v.Variable.Name);
					if (style == null)
						continue;
					_libraryUsed[lib.Name] = true;
					return style;
				}
			}
			return LookVariableUp(v.Variable);
		}

		protected override void AssignVariable(VariableSymbol variable, object value)
		{
			TryAddChildren(variable, value);
			_variables[variable] = value;
		}

		protected override void AssignArray(object[] array, int index, object value)
		{
			array[index] = value;
		}


		protected override void AssignField(object parent, VariableSymbol field, object value)
		{
			var parentType = parent.GetType();

			if (parentType == typeof(DataObject))
			{
				var data = (DataObject)parent;
				if (!data.TrySet(field.Name, value))
					throw new Exception();
			}
			else if (parentType == typeof(MathFormula))
			{
				var math = (MathFormula)parent;
				if (!math.TrySet(field.Name, Convert.ToSingle(value)))
					throw new Exception();

			}
			else
			{
				var result = SetField(parent, field.Name, value);
				if (!result)
				{
					result = SetField(parent, field.Name.ToVariableUpper(), value);
					if (!result)
						throw new Exception($"Could not find Field '{field.Name}'");
				}
			}
		}


		private static bool SetField(object parent, string fieldName, object value)
		{
			var parentType = parent.GetType();
			var field = parentType.GetField(fieldName);
			var property = parentType.GetProperty(fieldName);
			if (field != null)
			{
				field.SetValue(parent, value);
			}
			else if (property != null)
			{
				property.SetValue(parent, SlidesConverter.Convert(value, property.PropertyType));
			}
			else return false;
			return true;
		}


		/*
		private void AssignFieldAccess(BoundFieldAccessExpression lValue, object value, JSInsertion? formulaDependency, Stack<int> indices = null)
		{
			var parent = EvaluateExpression(lValue.Parent);
			var parentType = parent.GetType();
			var fieldName = lValue.Field.Variable.Name;

			if (parentType == typeof(DataObject))
			{
				var data = (DataObject)parent;
				if (!data.TrySet(fieldName, value))
					throw new Exception();
			}
			else if (parentType == typeof(MathFormula))
			{
				var math = (MathFormula)parent;
				if (!math.TrySet(fieldName, Convert.ToSingle(value)))
					throw new Exception();

			}
			else
			{
				var result = SetField(parent, fieldName, indices, value);
				if (!result)
				{
					result = SetField(parent, fieldName.ToVariableUpper(), indices, value);
					if (!result)
						throw new Exception($"Could not find Field '{fieldName}'");
				}
			}
			if (formulaDependency != null)
			{
				var fd = formulaDependency.Value;
				FieldDependency d = new FieldDependency(parent, fieldName, new Formula(fd.JSCode));
				switch (fd.Kind)
				{
					case JSInsertionKind.Slider:
						var fieldAccess = (BoundFieldAccessExpression)fd.Dependent;
						var slider = (Slider)EvaluateExpression(fieldAccess.Parent);
						slider.add_Dependency(d);
						break;
					case JSInsertionKind.Time:
						_dependencies.Add(d);
						break;
					default:
						throw new Exception();

				}
			}
		}
		*/

		private static bool _SetField(object parent, string fieldName, Stack<int> indices, object value)
		{
			var parentType = parent.GetType();
			var field = parentType.GetField(fieldName);
			var property = parentType.GetProperty(fieldName);
			if (field != null)
			{
				if (indices == null)
					field.SetValue(parent, value);
				else
				{
					var obj = (object[])field.GetValue(parent);
					while (indices.Count > 1)
					{
						obj = (object[])obj[indices.Pop()];
					}
					obj[indices.Pop()] = value;
				}
			}
			else if (property != null)
			{
				if (indices == null)
					property.SetValue(parent, SlidesConverter.Convert(value, property.PropertyType));
				else
				{
					var obj = (object[])property.GetValue(parent);
					while (indices.Count > 1)
					{
						obj = (object[])obj[indices.Pop()];
					}
					var index = indices.Pop();
					obj[index] = SlidesConverter.Convert(value, obj[index].GetType());
				}
			}
			else return false;
			return true;
		}

		//private object EvaluateFieldAssignmentExpression(BoundFieldAssignmentExpression node)
		//{
		//	var value = EvaluateExpression(node.Initializer);
		//	var parent = EvaluateExpression(node.Field.Parent);
		//	var parentType = parent.GetType();
		//	var fieldName = node.Field.Field.Variable.Name;

		//	if (FormulaCreator.NeedsDependency(node.Initializer, out BoundExpression dependent))
		//	{
		//		FieldDependency d = null;
		//		if (parent is Element e)
		//		{
		//			d = new FieldDependency(e, fieldName, this.CreateFunction(node.Initializer, dependent));

		//		}
		//		else if (parent is MathFormula m)
		//		{
		//			d = new FieldDependency(m, fieldName, this.CreateFunction(node.Initializer, dependent));
		//		}
		//		if (d != null)
		//		{
		//			if (dependent is BoundFieldAccessExpression fieldAccess)
		//			{
		//				var slider = (Slider)EvaluateExpression(fieldAccess.Parent);
		//				slider.add_Dependency(d);
		//			}
		//			else if (dependent is BoundVariableExpression variable)
		//			{
		//				_dependencies.Add(d);
		//			}
		//			else
		//				throw new Exception();
		//		}
		//	}

		//	if (parentType == typeof(DataObject))
		//	{
		//		var data = (DataObject)parent;
		//		if (!data.TrySet(fieldName, value))
		//			throw new Exception();
		//		return value;
		//	}
		//	if (parentType == typeof(MathFormula))
		//	{
		//		var math = (MathFormula)parent;
		//		if (!math.TrySet(fieldName, Convert.ToSingle(value)))
		//			throw new Exception();
		//		return value;
		//	}

		//	var field = parentType.GetField(fieldName);
		//	if (field != null)
		//	{
		//		field.SetValue(parent, value);
		//		return value;
		//	}
		//	var property = parentType.GetProperty(fieldName);
		//	if (property != null)
		//	{
		//		if (property.PropertyType == typeof(Brush))
		//			property.SetValue(parent, Brush.FromObject(value));
		//		else
		//			property.SetValue(parent, value);
		//		return value;
		//	}
		//	throw new Exception();
		//}

	}
}