using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Debug;
using Slides.Filters;

namespace Minsk.CodeAnalysis
{
	public struct PresentationFlags
	{
		public bool IsLibrarySymbol { get; set; }
		public bool StyleAllowed { get; set; }
		public bool AnimationsAllowed { get; set; }
		public bool GroupsAllowed { get; set; }
		public bool DatasAllowed { get; set; }
		public bool TemplatesAllowed { get; internal set; }
	}

	internal sealed class Evaluator
	{

		private LibrarySymbol[] _referenced;

		LibrarySymbol library = null;
		Presentation presentation = null;

		private SlideAttributes _currentSlide = null;
		private LibrarySymbol _currentReferenced = null;

		private readonly Dictionary<VariableSymbol, BoundStatement> _declarations;

		private List<CaseSymbol> _animationCases = null;
		private List<string> _referencedFiles = new List<string>();
		private List<Transition> _transitions = new List<Transition>();
		private List<CustomFilter> _filters = new List<CustomFilter>();
		private List<Step> _steps = new List<Step>();
		private readonly BoundBlockStatement _root;
		private VariableValueCollection _variables;
		private readonly Dictionary<VariableSymbol, Slide> _slides = new Dictionary<VariableSymbol, Slide>();
		private readonly Dictionary<TypeSymbol, BodySymbol> _customTypes = new Dictionary<TypeSymbol, BodySymbol>();
		private readonly Dictionary<VariableSymbol, Style> _styles = new Dictionary<VariableSymbol, Style>();
		private readonly Dictionary<Element, AnimationCall> _animations = new Dictionary<Element, AnimationCall>();
		private readonly Dictionary<VariableSymbol, BoundTemplateStatement> _templates = new Dictionary<VariableSymbol, BoundTemplateStatement>();
		private readonly List<string> _imports = new List<string>();
		private int slideCount = 0;

		private PresentationFlags _flags = new PresentationFlags();

		private object _lastValue;
		private Stack<List<Element>> _groupChildren = new Stack<List<Element>>();
		private Stack<List<Style>> _groupAppliedStyles = new Stack<List<Style>>();
		private readonly TypeSymbolTypeConverter _typeConverter = TypeSymbolTypeConverter.Instance;

		public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables, LibrarySymbol[] referenced, Dictionary<VariableSymbol, BoundStatement> declarations)
		{
			_root = root;
			_referenced = referenced;
			_declarations = declarations;
			foreach (var declaration in _declarations)
			{
				if (declaration.Key.Type == _typeConverter.LookSymbolUp(typeof(SlideAttributes)))
					slideCount++;
			}
			_variables = new VariableValueCollection(variables);
			_variables.Add(new VariableSymbol("slideCount", true, PrimitiveTypeSymbol.Integer, false), slideCount);
			if (!variables.Any())
			{
				_variables.Add(new VariableSymbol("seperator", true, _typeConverter.LookSymbolUp(typeof(LibrarySymbol)), false), Library.Seperator);
				_variables.Add(new VariableSymbol("code", true, _typeConverter.LookSymbolUp(typeof(LibrarySymbol)), false), Library.Code);
				_variables.Add(new VariableSymbol("auto", true, _typeConverter.LookSymbolUp(typeof(Unit)), false), new Unit(0, Unit.UnitKind.Auto));
				foreach (var color in Color.GetStaticColors())
				{
					_variables.Add(new VariableSymbol(color.Key, true, _typeConverter.LookSymbolUp(typeof(Color)), false), color.Value);
				}
			}
		}

		public object Evaluate()
		{

			var index = 0;

			_flags.AnimationsAllowed = true;
			_flags.DatasAllowed = true;
			_flags.GroupsAllowed = true;
			_flags.StyleAllowed = true;
			_flags.TemplatesAllowed = true;
			while (index < _root.Statements.Length)
			{
				var s = _root.Statements[index];
				Evaluate(s);
				index++;

			}

			if (_flags.IsLibrarySymbol)
			{
				library = new LibrarySymbol(library.Name, _referenced, _customTypes.Values.ToArray(), _styles.Values.ToArray(), _variables, _imports.ToArray());
				foreach (var customType in library.CustomTypes)
				{
					customType.Source = library;
				}
				_lastValue = library;
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

				presentation = new Presentation(_slides.Values.ToArray(), _styles.Values.ToArray(), _filters.ToArray(), _transitions.ToArray(), libraries, _imports.ToArray(), _referencedFiles.ToArray());
				_lastValue = presentation;
			}

			return _lastValue;
		}

		private void Evaluate(BoundStatement node)
		{
			switch (node.Kind)
			{
				case BoundNodeKind.VariableDeclaration:
					EvaluateVariableDeclaration((BoundVariableDeclaration)node);
					break;
				case BoundNodeKind.ExpressionStatement:
					EvaluateExpressionStatement((BoundExpressionStatement)node);
					break;
				case BoundNodeKind.IfStatement:
					EvaluateIfStatement((BoundIfStatement)node);
					break;
				case BoundNodeKind.ForStatement:
					EvaluateForStatement((BoundForStatement)node);
					break;
				case BoundNodeKind.BlockStatement:
					EvaluateBlockStatement((BoundBlockStatement)node);
					break;
				case BoundNodeKind.TemplateStatement:
					EvaluateTemplateStatement((BoundTemplateStatement)node);
					break;
				case BoundNodeKind.SlideStatement:
					EvaluateSlideStatement((BoundSlideStatement)node);
					break;
				case BoundNodeKind.DataStatement:
					EvaluateDataStatement((BoundDataStatement)node);
					break;
				case BoundNodeKind.GroupStatement:
					EvaluateGroupStatement((BoundGroupStatement)node);
					break;
				case BoundNodeKind.LibraryStatement:
					EvaluateLibrarySymbolStatement((BoundLibraryStatement)node);
					break;
				case BoundNodeKind.StyleStatement:
					EvaluateStyleStatement((BoundStyleStatement)node);
					break;
				case BoundNodeKind.AnimationStatement:
					EvaluateAnimationStatement((BoundAnimationStatement)node);
					break;
				case BoundNodeKind.CaseStatement:
					EvaluateCaseStatement((BoundCaseStatement)node);
					break;
				case BoundNodeKind.TransitionStatement:
					EvaluateTransitionStatement((BoundTransitionStatement)node);
					break;
				case BoundNodeKind.FilterStatement:
					EvaluateFilterStatement((BoundFilterStatement)node);
					break;
				default:
					throw new Exception($"Unexpected node {node.Kind}");
			}
		}

		private void EvaluateFilterStatement(BoundFilterStatement node)
		{
			var name = node.Variable.Name;
			var filters = new List<SVGFilter>();
			var filterNames = new List<string>();
			_variables = _variables.Push();
			_variables.Add(node.Parameter.Statements[0].Variable, new SourceGraphicFilterInput());
			Evaluate(node.Body);
			_variables = _variables.Pop(out var children);
			foreach (var child in children)
			{
				if(child.Value is SVGFilter f)
				{
					filters.Add(f);
					filterNames.Add(child.Key.Name);
				}
			}
			var result = new CustomFilter(name, filters.ToArray(), filterNames.ToArray());
			_variables[node.Variable] = result;
			_filters.Add(result);
		}

		private void EvaluateLibrarySymbolStatement(BoundLibraryStatement node)
		{
			_flags.IsLibrarySymbol = true;
			_flags.AnimationsAllowed = false;
			_flags.DatasAllowed = false;
			_flags.GroupsAllowed = false;
			_flags.StyleAllowed = false;
			Evaluate(node.BoundBody);
			library = new LibrarySymbol(node.Variable.Name);
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
				case BoundNodeKind.FieldAssignmentExpression:
					return CollectAnimationFields((BoundFieldAssignmentExpression)expression, animatedObject);
				case BoundNodeKind.FunctionAccessExpression:
					return CollectAnimationFields((BoundFunctionAccessExpression)expression, animatedObject);
				default:
					return new Slides.Attribute[0];
			}
		}

		private Slides.Attribute[] CollectAnimationFields(BoundFieldAssignmentExpression expression, Type animatedObject)
		{
			var field = expression.Field.Field.Variable.Name;
			var parent = expression.Field.Parent;
			while(parent.Kind == BoundNodeKind.FieldAccessExpression)
			{
				var fieldExpression = (BoundFieldAccesExpression)parent;
				field = fieldExpression.Field.Variable.Name + "." + field;
				parent = fieldExpression.Parent;
			}
			var parentObject = EvaluateExpression(parent);
			if (parentObject.GetType() != animatedObject)
				return new Slides.Attribute[0];
			var value = EvaluateExpression(expression.Initializer);
			if (value == null)
				throw new Exception();
			return new Slides.Attribute[]
			{
				new Slides.Attribute(field, value)
			};
		}

		private Slides.Attribute[] CollectAnimationFields(BoundFunctionAccessExpression expression, Type animatedObject)
		{
			//TODO(Time): hacky
			//without setPadding() Idk when we use this actually...
			var field = expression.Function.Function.Name;
			var value = EvaluateExpression(expression.Function.Arguments[0]);
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

		private Dictionary<string, object> CollectStyleFields(BoundBlockStatement statement, string styleName)
		{
			var result = new Dictionary<string, object>();
			foreach (var s in statement.Statements)
			{
				foreach (var field in CollectStyleFields(s, styleName))
				{
					if (!result.ContainsKey(field.Key))
						result.Add(field.Key, field.Value);
					else
						result[field.Key] = field.Value;
				}
			}
			return result;
		}

		private Dictionary<string, object> CollectStyleFields(BoundStatement statement, string styleName)
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
					Evaluate(statement);
					return new Dictionary<string, object>();
				default:
					Logger.LogUnexpectedSyntaxKind(statement.Kind, "CollectStyleFields");
					return new Dictionary<string, object>();
			}
		}

		private Dictionary<string, object> CollectStyleFields(BoundExpressionStatement statement, string styleName)
		{
			return CollectStyleFields(statement.Expression, styleName);
		}

		private Dictionary<string, object> CollectStyleFields(BoundIfStatement statement, string styleName)
		{
			var condition = (bool)EvaluateExpression(statement.BoundCondition);
			if(condition)
			{
				return CollectStyleFields(statement.BoundBody, styleName);
			}
			if(statement.BoundElse != null)
			{
				return CollectStyleFields(statement.BoundElse, styleName);
			}
			return new Dictionary<string, object>();
		}

		private Dictionary<string, object> CollectStyleFields(BoundExpression expression, string styleName)
		{
			switch (expression.Kind)
			{
				case BoundNodeKind.FieldAssignmentExpression:
					return CollectStyleFields((BoundFieldAssignmentExpression)expression);
				case BoundNodeKind.AssignmentExpression:
					return CollectStyleFields((BoundAssignmentExpression)expression);
				case BoundNodeKind.FunctionAccessExpression:
					return CollectStyleFields((BoundFunctionAccessExpression)expression, styleName);
				default:
					Logger.LogUnexpectedSyntaxKind(expression.Kind, "CollectStyleFields");
					return new Dictionary<string, object>();
			}
		}

		private Dictionary<string, object> CollectStyleFields(BoundFieldAssignmentExpression expression)
		{
			//TODO(Major): Std-Style needs a very own class for ModifiedFields. There could be subtypes and everything!
			var field = expression.Field.Field.Variable.Name;
			var value = EvaluateExpression(expression.Initializer);
			var result = new Dictionary<string, object>();
			result.Add(field, value);
			return result;
		}

		private Dictionary<string, object> CollectStyleFields(BoundAssignmentExpression expression)
		{
			//TODO(Time): Maybe hacky
			var field = expression.Variables[0].Name;
			var value = EvaluateExpression(expression.Expression);
			var result = new Dictionary<string, object>();
			result.Add(field, value);
			return result;
		}

		private Dictionary<string, object> CollectStyleFields(BoundFunctionAccessExpression expression, string styleName)
		{
			switch (expression.Function.Function.Name)
			{
				case "applyStyle":
					var style = (Style)EvaluateExpression(expression.Function.Arguments[0]);
					return style.ModifiedFields;
				//case "setPadding":
				//case "setMargin":
				//	var value = EvaluateExpression(expression.Function.Arguments[0]);
				//	var result = new Dictionary<string, object>();
				//	result.Add(expression.Function.Function.Name, value);
				//	return result;
				default:
					Logger.LogUnmatchedStyleFunction(expression.Function.Function.Name, styleName);

					return new Dictionary<string, object>();
			}
		}


		private void EvaluateStyleStatement(BoundStyleStatement node)
		{
			//TODO(Time): I dont know. Can we EVER compute the same style twice?????
			//aka do we need to store styles in _declarations?
			if(node.Variable != null)
				_declarations.Remove(node.Variable);
			if (!_flags.StyleAllowed)
				throw new Exception();
			_variables = _variables.Push();
			if (node.Variable != null)
			{
				var modifiedFields = CollectStyleFields(node.BoundBody, node.Variable.Name);

				var style = new Style(node.Variable.Name, modifiedFields);
				_styles.Add(node.Variable, style);
			}
			else
			{
				var modifiedFields = CollectStyleFields(node.BoundBody, "std");

				var style = new Style("std", modifiedFields);
				_styles.Add(new VariableSymbol("std", true, _typeConverter.LookSymbolUp(typeof(Style)), false), style);
			}
			_variables = _variables.Pop(out var _);
		}

		private void EvaluateAnimationStatement(BoundAnimationStatement node)
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

		private void EvaluateCaseStatement(BoundCaseStatement node)
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
					var delay = (Time)EvaluateExpression(functionAccess.Function.Arguments[0]);
					var name = functionAccess.Function.Function.Name;
					if (name != "hide")
						duration = (Time)EvaluateExpression(functionAccess.Function.Arguments[1]);
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

		private void EvaluateTransitionStatement(BoundTransitionStatement node)
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
					Evaluate(statement);
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
						result.background = Brush.FromObject(value.Value);
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

		private void EvaluateDataStatement(BoundDataStatement node)
		{
			if (!_flags.DatasAllowed)
				throw new Exception();
			var data = new BodySymbol(node.Type, null);
			_customTypes.Add(node.Type, data);
			//TODO(Minor): Maybe we don't need to collect data statements because they declare a type
			//and not a variable.
			_declarations.Remove(new VariableSymbol(node.Type.Name, true, node.Type, false));
		}

		private void EvaluateGroupStatement(BoundGroupStatement node)
		{
			if (!_flags.GroupsAllowed)
				throw new Exception();
			var parameters = new List<VariableSymbol>();
			foreach (var parameter in node.BoundParameters.Statements)
			{
				parameters.Add(parameter.Variable);
			}

			var group = new BodySymbol(node.Type, node.BoundBody);
			_customTypes.Add(node.Type, group);
			_declarations.Remove(new VariableSymbol(node.Type.Name, true, node.Type, false));
		}

		//TODO(Major): galore! Everythings missing. It's a wonder it's working!
		private void EvaluateTemplateStatement(BoundTemplateStatement node)
		{
			if (!_declarations.ContainsKey(node.Variable))
				return;
			if (!_flags.TemplatesAllowed)
				throw new Exception();

			var template = new BodySymbol(node.Variable, node.Body);
			_templates.Add(node.Variable, node);


			_variables[node.Variable] = template;
			_declarations.Remove(node.Variable);
		}

		private void EvaluateSlideStatement(BoundSlideStatement node)
		{
			//When _declarations doesn't contain our variable
			//we already evaluated it.
			if (!_declarations.ContainsKey(node.Variable))
				return;
			if (_flags.IsLibrarySymbol)
				throw new Exception();
			_variables = _variables.Push();
			_currentSlide = new SlideAttributes(node.Variable.Name, _slides.Count);

			_steps = new List<Step>();
			foreach (var statement in node.Statements)
			{
				_steps.Add(EvaluateStepStatement(statement));
			}
			Template template = null;
			if(node.Template != null)
			{
				_variables = _variables.Push();
				var currentTemplate = _templates[node.Template];
				_variables[currentTemplate.SlideParameter.Variable] = _currentSlide;
				_variables = _variables.Push();
				Evaluate(_templates[node.Template].Body);
				_variables = _variables.Pop(out var children);
				var dataChildren = new List<object>();
				var visualChildren = new List<Element>();
				foreach (var value in children)
				{
					if (value.Key.IsVisible && value.Value is Element e)
					{
						e.name = value.Key.Name;
						visualChildren.Add(e);
					}
					else
						switch (value.Key.Name)
						{
							default:
								dataChildren.Add(value);
								break;
						}
				}
				template = new Template(node.Template.Name, visualChildren.ToArray(), dataChildren.ToArray());
			}
			var slide = new Slide(_currentSlide, _steps.ToArray(), template);
			_currentSlide = null;
			_slides.Add(node.Variable, slide);
			_declarations.Remove(node.Variable);
		}

		private Step EvaluateStepStatement(BoundStepStatement node)
		{
			//_variables = _variables.Push();
			Evaluate(node.Body);

			//_variables.Pop(out var slideValues);
			var animationCalls = _animations.Values.ToArray();
			_animations.Clear();
			var dataChildren = new List<object>();
			var visualChildren = new List<Element>();
			foreach (var value in _variables)
			{
				if (_steps.Any(s => s.DataChildren.Contains(value.Value) || s.VisualChildren.Contains(value.Value)))
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
							_currentSlide.background = Brush.FromObject(value.Value);
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
							dataChildren.Add(value);
							break;
					}
			}
			var step = new Step(node.Variable?.Name, _currentSlide, animationCalls, visualChildren.ToArray(), dataChildren.ToArray());
			return step;
		}

		private void EvaluateBlockStatement(BoundBlockStatement node)
		{
			foreach (var statement in node.Statements)
			{
				Evaluate(statement);
			}
		}

		private void EvaluateIfStatement(BoundIfStatement node)
		{
			var condition = EvaluateExpression(node.BoundCondition);
			if (node.BoundCondition.Type.Type == TypeType.Nullable)
			{
				if (condition != null)
					Evaluate(node.BoundBody);
				else if (node.BoundElse != null)
					Evaluate(node.BoundElse);
			}
			else if (condition.Equals(true))
				Evaluate(node.BoundBody);
			else if (node.BoundElse != null)
				Evaluate(node.BoundElse);
		}

		private void EvaluateForStatement(BoundForStatement node)
		{
			_variables = _variables.Push();
			var collection = EvaluateExpression(node.Collection);
			if (collection is Range r)
			{
				if(r.Step >= 0)
					for (int i = r.From; i < r.To; i += r.Step)
					{
						_variables[node.Variable] = i;
						Evaluate(node.Body);
					}
				else
					for(int i = r.From; i > r.To; i += r.Step)
					{
						_variables[node.Variable] = i;
						Evaluate(node.Body);
					}
			}
			else
			{
				if (!(collection is IEnumerable<object> e))
					throw new Exception();

				foreach (var item in e)
				{
					_variables[node.Variable] = item;
					Evaluate(node.Body);
				}
			}
			_variables = _variables.Pop(out var _);
		}

		private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
		{
			var value = EvaluateExpression(node.Initializer);

			if (value is ImportExpression<LibrarySymbol> importLibrary)
			{
				value = importLibrary.Value;
			}
			else if (value is ImportExpression<Font> importFont)
			{
				value = importFont.Value;
				_imports.Add(importFont.Href);
			}


			if (value != null && value.GetType() == typeof(LibrarySymbol) && _referenced != null)
				return;
			if (node.Variables.Length == 1)
			{
				var variable = node.Variables[0];
				if (_groupChildren.Count > 0 && variable.IsVisible && !variable.Type.IsData)
					_groupChildren.Peek().Add((Element)value);
				_variables[variable] = value;
				_lastValue = value;
				if (value is Element e)
				{
					e.name = variable.Name;
				}
			}
			else
			{
				var tuple = (TupleType)value;
				for(int i = 0; i < node.Variables.Length; i++)
				{
					var variable = node.Variables[i];
					if (_groupChildren.Count > 0 && variable.IsVisible && !variable.Type.IsData)
						_groupChildren.Peek().Add((Element)tuple[i]);
					_variables[variable] = tuple[i];
					_lastValue = value;
					if (tuple[i] is Element e)
					{
						e.name = variable.Name;
					}
				}
			}
		}

		private void EvaluateExpressionStatement(BoundExpressionStatement node)
		{
			_lastValue = EvaluateExpression(node.Expression);
		}

		private object EvaluateExpression(BoundExpression node)
		{
			switch (node.Kind)
			{
				case BoundNodeKind.StringExpression:
					return EvaluateStringExpression((BoundStringExpression)node);
				case BoundNodeKind.LiteralExpression:
					return EvaluateLiteralExpression((BoundLiteralExpression)node);
				case BoundNodeKind.VariableExpression:
					return EvaluateVariableExpression((BoundVariableExpression)node);
				case BoundNodeKind.AssignmentExpression:
					return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
				case BoundNodeKind.FieldAssignmentExpression:
					return EvaluateFieldAssignmentExpression((BoundFieldAssignmentExpression)node);
				case BoundNodeKind.UnaryExpression:
					return EvaluateUnaryExpression((BoundUnaryExpression)node);
				case BoundNodeKind.BinaryExpression:
					return EvaluateBinaryExpression((BoundBinaryExpression)node);
				case BoundNodeKind.FunctionExpression:
					return EvaluateFunctionExpression((BoundFunctionExpression)node);
				case BoundNodeKind.ArrayExpression:
					return EvaluateArrayExpression((BoundArrayExpression)node);
				case BoundNodeKind.EnumExpression:
					return EvaluateEnumExpression((BoundEnumExpression)node);
				case BoundNodeKind.FieldAccessExpression:
					return EvaluateFieldAccessExpression((BoundFieldAccesExpression)node);
				case BoundNodeKind.StaticFieldAccessExpression:
					return EvaluateStaticFieldAccessExpression((BoundStaticFieldAccesExpression)node);
				case BoundNodeKind.FunctionAccessExpression:
					return EvaluateFunctionAccessExpression((BoundFunctionAccessExpression)node);
				case BoundNodeKind.Conversion:
					return EvaluateConversion((BoundConversion)node);
				default:
					throw new Exception($"Unexpected node {node.Kind}");
			}
		}

		private object EvaluateConversion(BoundConversion node)
		{
			var value = EvaluateExpression(node.Expression);
			if (node.Type == PrimitiveTypeSymbol.Unit)
				switch (value)
				{
					//TODO(Improvement): Maybe don't differntiate between int and float. It's confusing
					case float f:
						return new Unit(f * 100, Unit.UnitKind.Percent);
					case int i:
						return new Unit(i, Unit.UnitKind.Pixel);
					default:
						throw new NotSupportedException();
				}
			throw new Exception();
		}

		private object EvaluateStringExpression(BoundStringExpression node)
		{
			object[] values = new object[node.Expressions.Length];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = EvaluateExpression(node.Expressions[i]);
			}
			return string.Join("", values);
		}

		private object EvaluateFunctionAccessExpression(BoundFunctionAccessExpression node)
		{
			var parent = EvaluateExpression(node.Parent);
			var function = node.Function;
			var args = new object[function.Arguments.Length];
			for (int i = 0; i < function.Arguments.Length; i++)
			{
				args[i] = EvaluateExpression(function.Arguments[i]);
				//TODO: I don't think the binder always catches this. But he should. - p
				if (args[i] == null && !function.Arguments[i].Type.AllowsNone)
					throw new Exception();
			}
			var type = parent.GetType();
			if (type == typeof(AnimationSymbol))
				return EvaluateAnimationCall((AnimationSymbol)parent, function, args);
			var parameters = new Type[function.Function.Parameter.Count];
			for (int i = 0; i < parameters.Length; i++)
			{
				parameters[i] = args[i]?.GetType() ?? _typeConverter.LookTypeUp(function.Function.Parameter[i].Type);
			}
			return EvaluateFunctionAccessCall(function.Function, parent, args);
		}

		private object GetValue(Element element, string field)
		{
			switch (field)
			{
				case "margin":
					return element.margin;
				case "margin.Top":
					return element.margin?.Top;
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

		private object EvaluateFunctionAccessCall(FunctionSymbol function, object obj, object[] args)
		{
			switch (function.Name)
			{
				case "len":
					return ((ICollection<object>)obj).Count;
				default:
					var type = obj.GetType();
					var parameters = args.Select(a => a.GetType()).ToArray();
					var method = type.GetMethod(function.Name, parameters);
					return MethodInvoke(method, obj, args);
			}
		}

		private object EvaluateEnumExpression(BoundEnumExpression node)
		{
			var type = _typeConverter.LookTypeUp(node.Type);
			foreach (var value in type.GetEnumValues())
			{
				if (value.ToString() == node.Value)
					return value;
			}
			throw new Exception();
		}

		private object EvaluateFieldAccessExpression(BoundFieldAccesExpression node)
		{
			var parent = EvaluateExpression(node.Parent);
			var type = parent.GetType();
			if (type == typeof(DataObject))
			{
				var data = (DataObject)parent;
				if (!data.TryLookUp(node.Field.Variable.Name, out var value))
					throw new Exception();
				return value;
			}
			return type.GetProperty(node.Field.Variable.Name).GetValue(parent);
		}

		private object EvaluateStaticFieldAccessExpression(BoundStaticFieldAccesExpression node)
		{
			var type = _typeConverter.LookTypeUp(node.BaseType);
			return type.GetProperty(node.Field.Name).GetValue(null);
		}

		private object EvaluateArrayExpression(BoundArrayExpression node)
		{
			var result = new object[node.BoundExpressions.Length];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = EvaluateExpression(node.BoundExpressions[i]);
			}
			return result;
		}

		private object EvaluateGroupConstructorExpression(BoundExpression[] arguments, BodySymbol group)
		{
			if (!(group.Symbol.Type is AdvancedTypeSymbol advanced))
				throw new Exception();

			var cVariables = _variables.Push();
			var constructor = advanced.Constructor.FirstOrDefault(c => c.Parameter.Count == arguments.Length);
			var args = new object[constructor.Parameter.Count];
			for (int i = 0; i < constructor.Parameter.Count; i++)
			{
				args[i] = EvaluateExpression(arguments[i]);
				//TODO(BigDecision): Maybe allow multiple Constructors for custom groups.
				//For example:
				//group a(i : int):
				//	print(i);
				//endgroup
				//
				//group a(f : float):
				//	print(f);
				//endgroup
				cVariables.Add(constructor.Parameter[i], args[i]);
			}

			_variables = cVariables;
			_variables = _variables.Push();
			var oldReferenced = _currentReferenced;
			_currentReferenced = group.Source;

			_groupChildren.Push(new List<Element>());
			_groupAppliedStyles.Push(new List<Style>());
			if (group.Body != null)
				Evaluate(group.Body);

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
			var slideValues = _groupChildren.Pop().ToArray();

			_variables = _variables.Pop(out cVariables);
			_variables = _variables.Pop(out var _);
			object result;
			if (group.Body != null)
			{
				result = new BoxElement(group.Symbol.Name, slideValues);
				foreach (var style in _groupAppliedStyles.Pop())
					((BoxElement)result).applyStyle(style);
				SetAttributes((BoxElement)result, cVariables);
			}
			else
			{
				result = new DataObject(advanced, args);
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
						element.background = Brush.FromObject(variable.Value);
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
						if (!typeSymbol.CanBeConvertedTo(_typeConverter.LookSymbolUp(typeof(Element))) &&
							type != typeof(ImageSource) && type != typeof(CSVFile))
							Logger.LogUnusedVariableInGroup(variable.Key.Name, _typeConverter.TryLookTypeUp(variable.Key.Type), element.TypeName);
						break;
				}
			}
		}

		private object EvaluateFunctionExpression(BoundFunctionExpression expression)
		{
			var args = new object[expression.Arguments.Length];
			for (int i = 0; i < expression.Arguments.Length; i++)
			{
				args[i] = EvaluateExpression(expression.Arguments[i]);
			}
			if (expression.Function.Name == "constructor")
			{
				if (_customTypes.TryGetValue(expression.Function.Type, out var group))
				{
					return EvaluateGroupConstructorExpression(expression.Arguments, group);
				}
				if (_currentReferenced != null)
				{
					var customType = _currentReferenced.CustomTypes.FirstOrDefault(ct => ct.Symbol.Type == expression.Function.Type);
					if (customType != null)
					{
						return EvaluateGroupConstructorExpression(expression.Arguments, customType);
					}
				}
				if (expression.Source != null)
				{
					var customType = expression.Source.CustomTypes.FirstOrDefault(ct => ct.Symbol.Type == expression.Function.Type);
					if (customType != null)
					{
						return EvaluateGroupConstructorExpression(expression.Arguments, customType);
					}
				}
				return EvaluateConstructorCall(expression.Function, args);
			}

			switch (expression.Function.Name)
			{
				case "useStyle":
					_flags.StyleAllowed = true;
					return null;
				case "useGroup":
					_flags.GroupsAllowed = true;
					return null;
				case "useData":
					_flags.DatasAllowed = true;
					return null;
				case "useAnimation":
					_flags.AnimationsAllowed = true;
					return null;
				case "applyStyle":
					var style = (Style)args[0];
					if (_groupAppliedStyles.Count == 0)
						_currentSlide.applyStyle(style);
					else
						_groupAppliedStyles.Peek().Add(style);
					return null;
				case "lib": return null;
			}
			MethodInfo method = null;
			if (expression.Source != null)
				method = expression.Source.LookMethodInfoUp(expression.Function);
			else
				method = GlobalFunctionsConverter.Instance.LookMethodInfoUp(expression.Function);
			if (expression.Function.Name == "image")
			{
				var fileName = args[0].ToString();
				if (!_referencedFiles.Contains(fileName))
					_referencedFiles.Add(fileName);
			}
			return MethodInvoke(method, null, args);
		}

		private object MethodInvoke(MethodInfo method, object obj, object[] args)
		{

			object convertArray(Type type, object value)
			{
				var localValue = value;
				if (type.IsArray)
				{
					var array = (object[])localValue;
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = convertArray(type.GetElementType(), array[i]);
					}
					localValue = array;
				}
				if(type == typeof(float))
				{
					var l = new List<float>();
					foreach (var v in (object[])localValue)
					{
						l.Add(Convert.ToSingle(v));
					}
					return l.ToArray();
				}
				object converted = null;
				var m = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type);
				converted = m.Invoke(null, new object[] { localValue });
				m = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(type);
				return m.Invoke(null, new object[] { converted });

			}
			object[] convertedArgs = new object[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				convertedArgs[i] = args[i];

				//C-Sharp cannot convert a object[] to a string[] or whatever. So
				//we use the helper method convertArray, which takes the target type
				//and a array to convert it. Even though this function makes use of 
				//Enumerable.OfType(), so maybe that will cause problems. Theoretically
				//convertArray should also work for not-arrays. - p
				if (args[i].GetType().IsArray)
				{
					var type = method.GetParameters()[i].ParameterType.GetElementType();
					convertedArgs[i] = convertArray(type, convertedArgs[i]);
				}
			}

			return method.Invoke(obj, convertedArgs);
		}

		private object ConstructorInvoke(ConstructorInfo constructor, object[] args)
		{

			object convertArray(Type type, object value)
			{
				var localValue = value;
				if (type.IsArray)
				{
					var array = (object[])localValue;
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = convertArray(type.GetElementType(), array[i]);
					}
					localValue = array;
				}
				var m = typeof(Enumerable).GetMethod("OfType").MakeGenericMethod(type);
				var converted = m.Invoke(null, new object[] { localValue });
				m = typeof(Enumerable).GetMethod("ToArray").MakeGenericMethod(type);
				return m.Invoke(null, new object[] { converted });

			}
			object[] convertedArgs = new object[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				convertedArgs[i] = args[i];

				//C-Sharp cannot convert a object[] to a string[] or whatever. So
				//we use the helper method convertArray, which takes the target type
				//and a array to convert it. Even though this function makes use of 
				//Enumerable.OfType(), so maybe that will cause problems. Theoretically
				//convertArray should also work for not-arrays. - p
				if (args[i].GetType().IsArray)
				{
					var type = constructor.GetParameters()[i].ParameterType.GetElementType();
					convertedArgs[i] = convertArray(type, convertedArgs[i]);
				}
			}

			return constructor.Invoke(convertedArgs);
		}

		private object EvaluateConstructorCall(FunctionSymbol function, object[] args)
		{
			var constructor = ConstructorSymbolConverter.Instance.LookConstructorInfoUp(function);
			return ConstructorInvoke(constructor, args);
		}



		private object EvaluateLiteralExpression(BoundLiteralExpression n)
		{
			return n.Value;
		}

		private object EvaluateVariableExpression(BoundVariableExpression v)
		{
			if (v.Type == _typeConverter.LookSymbolUp(typeof(Style)))
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
					return style;
				}
			}
			var value = _variables[v.Variable];
			if (value == null && _currentReferenced != null)
			{
				value = _currentReferenced.GlobalVariables[v.Variable];
			}
			if(value == null)
			{
				Evaluate(_declarations[v.Variable]);
				value = _variables[v.Variable];
				if(value != null)
					_declarations.Remove(v.Variable);
			}
			if (value == null)
				throw new Exception();
			if (v.BoundArrayIndex == null)
			{
				return value;
			}
			return EvaluateArrayIndexAccess((object[])value, v.BoundArrayIndex);
		}

		private object EvaluateArrayIndexAccess(object[] value, BoundArrayIndex arrayIndex)
		{
			var index = (int)EvaluateExpression(arrayIndex.BoundIndex);
			if (arrayIndex.BoundChild == null)
				return value[index];
			return EvaluateArrayIndexAccess((object[])value[index], arrayIndex.BoundChild);
		}

		private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
		{
			var value = EvaluateExpression(a.Expression);
			object[] array = null;
			if (a.Variables.Length > 1)
				array = (object[])value;
			for (int i = 0; i < a.Variables.Length; i++)
			{
				if (a.Variables.Length == 1)
				{
					_variables[a.Variables[i]] = value;
					return value;
				}
				_variables[a.Variables[i]] = array[i];
			}
			return value;
		}

		private object EvaluateFieldAssignmentExpression(BoundFieldAssignmentExpression node)
		{
			var value = EvaluateExpression(node.Initializer);
			var parent = EvaluateExpression(node.Field.Parent);
			var parentType = parent.GetType();
			var fieldName = node.Field.Field.Variable.Name;
			if (parentType == typeof(DataObject))
			{
				var data = (DataObject)parent;
				if (!data.TrySet(fieldName, value))
					throw new Exception();
				return value;
			}
			var field = parentType.GetField(fieldName);
			if (field != null)
			{
				field.SetValue(parent, value);
				return value;
			}
			var property = parentType.GetProperty(fieldName);
			if (property != null)
			{
				if(property.PropertyType == typeof(Brush))
					property.SetValue(parent, Brush.FromObject(value));
				else
					property.SetValue(parent, value);
				return value;
			}
			throw new Exception();
		}

		private object EvaluateUnaryExpression(BoundUnaryExpression u)
		{
			var operand = EvaluateExpression(u.Operand);

			switch (u.Op.Kind)
			{
				case BoundUnaryOperatorKind.Identity:
					return (int)operand;
				case BoundUnaryOperatorKind.Negation:
					if (operand.GetType() == typeof(int))
						return -(int)operand;
					var oldUnit = (Unit)operand;
					return new Unit(-oldUnit.Value, oldUnit.Kind);
				case BoundUnaryOperatorKind.LogicalNegation:
					return !(bool)operand;
				case BoundUnaryOperatorKind.OnesComplement:
					return ~(int)operand;
				default:
					throw new Exception($"Unexpected unary operator {u.Op}");
			}
		}

		private object EvaluateBinaryExpression(BoundBinaryExpression b)
		{
			var left = EvaluateExpression(b.Left);
			var right = EvaluateExpression(b.Right);

			switch (b.Op.Kind)
			{
				case BoundBinaryOperatorKind.Addition:
					if (left.GetType() == typeof(Unit) && right.GetType() == typeof(Unit))
						return (Unit)left + (Unit)right;
					if (left.GetType() == typeof(float) && right.GetType() == typeof(float))
						return (float)left + (float)right;
					return (int)left + (int)right;
				case BoundBinaryOperatorKind.Concatination:
					return left.ToString() + right.ToString();
				case BoundBinaryOperatorKind.Subtraction:
					if (left.GetType() == typeof(Unit) && right.GetType() == typeof(Unit))
						return (Unit)left - (Unit)right;
					if (left.GetType() == typeof(float) && right.GetType() == typeof(float))
						return (float)left - (float)right;
					return (int)left - (int)right;
				case BoundBinaryOperatorKind.Multiplication:
					if (left.GetType() == typeof(Unit) && right.GetType() == typeof(float))
						return new Unit(((Unit)left).Value * (float)right, ((Unit)left).Kind);
					if (right.GetType() == typeof(Unit) && left.GetType() == typeof(float))
						return new Unit(((Unit)right).Value * (float)left, ((Unit)right).Kind);
					if (left.GetType() == typeof(float) && right.GetType() == typeof(float))
						return (float)left * (float)right;
					return (int)left * (int)right;
				case BoundBinaryOperatorKind.Division:
					if (left.GetType() == typeof(Unit) && right.GetType() == typeof(float))
						return new Unit(((Unit)left).Value / (float)right, ((Unit)left).Kind);
					if (left.GetType() == typeof(float) && right.GetType() == typeof(float))
						return (float)left / (float)right;
					return (int)left / (int)right;
				case BoundBinaryOperatorKind.BitwiseAnd:
					if (b.Type == PrimitiveTypeSymbol.Integer)
						return (int)left & (int)right;
					else
						return (bool)left & (bool)right;
				case BoundBinaryOperatorKind.BitwiseOr:
					if (b.Type == PrimitiveTypeSymbol.Integer)
						return (int)left | (int)right;
					else
						return (bool)left | (bool)right;
				case BoundBinaryOperatorKind.BitwiseXor:
					if (b.Type == PrimitiveTypeSymbol.Integer)
						return (int)left ^ (int)right;
					else
						return (bool)left ^ (bool)right;
				case BoundBinaryOperatorKind.LogicalAnd:
					return (bool)left && (bool)right;
				case BoundBinaryOperatorKind.LogicalOr:
					return (bool)left || (bool)right;
				case BoundBinaryOperatorKind.Equals:
					return Equals(left, right);
				case BoundBinaryOperatorKind.NotEquals:
					return !Equals(left, right);
				case BoundBinaryOperatorKind.Less:
					return (int)left < (int)right;
				case BoundBinaryOperatorKind.LessOrEquals:
					return (int)left <= (int)right;
				case BoundBinaryOperatorKind.Greater:
					return (int)left > (int)right;
				case BoundBinaryOperatorKind.GreaterOrEquals:
					return (int)left >= (int)right;
				case BoundBinaryOperatorKind.EnumAddition:
					var horizontal = Horizontal.Left;
					if (left is Horizontal hl)
						horizontal = hl;
					else if (right is Horizontal hr)
						horizontal = hr;
					var vertical = Vertical.Top;
					if (left is Vertical vl)
						vertical = vl;
					else if (right is Vertical vr)
						vertical = vr;
					return AddEnum(horizontal, vertical);
				case BoundBinaryOperatorKind.FilterAddition:
					return new FilterAddition((Filter)left, (Filter)right);
				case BoundBinaryOperatorKind.Range:
					int il = (int)left;
					int ir = (int)right;
					if (il <= ir)
						return new Range(il, ir, 1);
					return new Range(il, ir, -1);
				default:
					throw new Exception($"Unexpected binary operator {b.Op}");
			}
		}

		private Orientation AddEnum(Horizontal h, Vertical v)
		{
			switch (h)
			{
				case Horizontal.Left:
					switch (v)
					{
						case Vertical.Top: return Orientation.LeftTop;
						case Vertical.Stretch: return Orientation.LeftStretch;
						case Vertical.Center: return Orientation.LeftCenter;
						case Vertical.Bottom: return Orientation.LeftBottom;
					}
					break;
				case Horizontal.Stretch:
					switch (v)
					{
						case Vertical.Top: return Orientation.StretchTop;
						case Vertical.Stretch: return Orientation.Stretch;
						case Vertical.Center: return Orientation.StretchCenter;
						case Vertical.Bottom: return Orientation.StretchBottom;
					}
					break;
				case Horizontal.Center:
					switch (v)
					{
						case Vertical.Top: return Orientation.CenterTop;
						case Vertical.Stretch: return Orientation.CenterStretch;
						case Vertical.Center: return Orientation.Center;
						case Vertical.Bottom: return Orientation.CenterBottom;
					}
					break;
				case Horizontal.Right:
					switch (v)
					{
						case Vertical.Top: return Orientation.RightTop;
						case Vertical.Stretch: return Orientation.RightStretch;
						case Vertical.Center: return Orientation.RightCenter;
						case Vertical.Bottom: return Orientation.RightBottom;
					}
					break;
			}
			throw new Exception();
		}
	}
}