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
using Slides.MathExpressions;
using SVGGroup = SVGLib.ContainerElements.Group;
using Color = Slides.Color;
using SVGLib.GraphicsElements;
using SVGLib;
using SVGLib.ContainerElements;
using SVGLib.Datatypes;
using Slides.Elements;
using Slides.Helpers;
using System.IO;
using SVGLib.Filters;

namespace Minsk.CodeAnalysis
{
	internal sealed class Evaluator : StatementEvaluator
	{
		//TODO: Clean up presentation flags
		public static PresentationFlags Flags = new PresentationFlags();

		private readonly PresentationBuilder _presentationBuilder;

		//Fields which save a current state
		private string _currentIdBase;
		private LibrarySymbol _currentReferenced = null;
		private SlideAttributes _currentSlide = null;
		private List<Step> _steps = new List<Step>();
		private List<CaseSymbol> _animationCases = null;
		private readonly Stack<GroupBuilder> _groupBuilders = new Stack<GroupBuilder>();

		//Helper fields
		private readonly Dictionary<VariableSymbol, BoundStatement> _declarations;
		private readonly int _slideCount = 0;
		private readonly JSEmitter _jsEmitter = new JSEmitter();
		
		public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables, LibrarySymbol[] referenced, Dictionary<VariableSymbol, BoundStatement> declarations, PresentationFlags flags)
			: base(root, new VariableValueCollection(variables))
		{
			//You could actually put flags into Evaluate. But you must find a way to change flags during Evaluation (CodeHighlighter Style)
			Flags = flags;
			_presentationBuilder = new PresentationBuilder(referenced);
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
			var root = (BoundBlockStatement)_root;
			while (index < root.Statements.Length)
			{
				var s = root.Statements[index];
				EvaluateStatement(s);
				index++;

			}

			var referenced = _presentationBuilder.GetReferenced();
			if (Flags.IsLibrarySymbol)
			{
				var library = new LibrarySymbol(Flags.LibraryName, referenced, _presentationBuilder.GetCustomTypes(), _presentationBuilder.GetStyles(), _variables, _presentationBuilder.GetImports());
				foreach (var customType in library.CustomTypes)
				{
					customType.Source = library;
				}
				return library;
			}
			else
			{
				var libraries = new Library[referenced.Length];
				for (int i = 0; i < libraries.Length; i++)
				{
					//TODO(Major): Recursion
					libraries[i] = new Library(referenced[i].Name, referenced[i].Libraries?.Select(l => new Library(l.Name, null, l.Styles)).ToArray(), referenced[i].Styles);
					_presentationBuilder.AddImportRange(referenced[i].Imports);
				}
				return new Presentation(_presentationBuilder.GetSlides(), _presentationBuilder.GetStyles(), _presentationBuilder.GetFilters(), _presentationBuilder.GetTransitions(), libraries, _presentationBuilder.GetJSInsertions(), _presentationBuilder.GetImports(), _presentationBuilder.GetReferencedFiles(), Flags.CodeHighlighter);
			}
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
			_presentationBuilder.AddFilter(result);
		}

		private Slides.Attribute[] CollectAnimationFields(BoundBlockStatement statement)
		{
			var result = new List<Slides.Attribute>();
			foreach (var s in statement.Statements)
			{
				result.AddRange(CollectAnimationFields(s));
			}
			return result.ToArray();
		}

		private Slides.Attribute[] CollectAnimationFields(BoundExpressionStatement statement)
		{
			return CollectAnimationFields(statement.Expression);
		}

		private Slides.Attribute[] CollectAnimationFields(BoundExpression expression)
		{
			switch (expression.Kind)
			{
				case BoundNodeKind.AssignmentExpression:
					return CollectAnimationFields((BoundAssignmentExpression)expression);
				default:
					return new Slides.Attribute[0];
			}
		}

		private Slides.Attribute[] CollectAnimationFields(BoundAssignmentExpression expression)
		{
			var value = EvaluateExpression(expression.Expression);
			switch (expression.LValue.Kind)
			{
				case BoundNodeKind.VariableExpression:
					var variableExpression = (BoundVariableExpression)expression.LValue;
					if (variableExpression.Variable.Name == "interpolation")
						return new Slides.Attribute[0];
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

		private Slides.Attribute[] CollectAnimationFields(BoundStatement statement)
		{
			switch (statement.Kind)
			{
				case BoundNodeKind.ExpressionStatement:
					return CollectAnimationFields((BoundExpressionStatement)statement);
				case BoundNodeKind.BlockStatement:
					return CollectAnimationFields((BoundBlockStatement)statement);
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
			var fields = new Dictionary<string, object>
			{
				{ field, value }
			};
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
				_presentationBuilder.AddStyle(node.Variable, style);
			}
			else
			{
				var modifiedFields = CollectStyleFields(node.BoundBody, "std");

				var style = new StdStyle(modifiedFields.ToArray());
				_presentationBuilder.AddStyle(new VariableSymbol("std", true, _builtInTypes.LookSymbolUp(typeof(StdStyle)), false), style);
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

		protected override void EvaluateTransitionStatement(BoundTransitionStatement node)
		{
			if (!_declarations.ContainsKey(node.Variable))
				return;
			var name = node.Variable.Name;
			TransitionSlide fromSlide = new TransitionSlide();
			TransitionSlide toSlide = new TransitionSlide();
			_variables = _variables.Push();
			_variables[node.FromParameter.Variable] = fromSlide;
			_variables[node.ToParameter.Variable] = toSlide;
			_groupBuilders.Push(GroupBuilder.CreateGroupBuilder());
			EvaluateStatement(node.Body);
			var from = fromSlide.CreateTransitionCall();
			var to = toSlide.CreateTransitionCall();
			var children = _groupBuilders.Pop().GetGroupValues();
			var result = new Transition(name, from, to, children);
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
						if (!(value.Value is Element) && !(value.Value is TransitionSlide))
							Logger.LogUnusedVariableInBlock(value.Key.Name, value.Value.GetType(), "transition " + name);
						break;
				}
			}
			_presentationBuilder.AddTransition(result);
			_variables.Add(node.Variable, result);
			_declarations.Remove(node.Variable);
		}

		protected override void EvaluateDataStatement(BoundDataStatement node)
		{
			var data = new BodySymbol(node.Type, null);
			_presentationBuilder.AddCustomType(data);
			//TODO(Minor): Maybe we don't need to collect data statements because they declare a type
			//and not a variable.
			_declarations.Remove(new VariableSymbol(node.Type.Name, true, node.Type, false));
		}

		protected override void EvaluateGroupStatement(BoundGroupStatement node)
		{
			var parameters = new List<VariableSymbol>();
			foreach (var parameter in node.Parameters.Statements)
			{
				parameters.Add(parameter.Variable);
			}

			var group = new BodySymbol(node.Type, node.Body);
			_presentationBuilder.AddCustomType(group);
			_declarations.Remove(new VariableSymbol(node.Type.Name, true, node.Type, false));
		}

		protected override void EvaluateSVGStatement(BoundSVGStatement node)
		{
			var parameters = new List<VariableSymbol>();
			foreach (var parameter in node.Parameters.Statements)
			{
				parameters.Add(parameter.Variable);
			}

			var group = new BodySymbol(node.Type, node.Body);
			_presentationBuilder.AddCustomType(group);
			_declarations.Remove(new VariableSymbol(node.Type.Name, true, node.Type, false));
		}

		protected override void EvaluateTemplateStatement(BoundTemplateStatement node)
		{
			if (!_declarations.ContainsKey(node.Variable))
				return;
			if (!Flags.TemplatesAllowed)
				throw new Exception();

			var template = new BodySymbol(node.Variable, node.Body);
			//why tho? why won't we use the template BodySymbol like everywhere else??
			_presentationBuilder.AddTemplate(node.Variable, node);

			_variables[node.Variable] = template;
			_declarations.Remove(node.Variable);
		}

		protected override void DeclareElement(Element e)
		{
			if (_steps.Any())
				e.set_Step(_steps.Last());
			else if (!_groupBuilders.Any()) throw new Exception();
		}

		protected override void EvaluateSlideStatement(BoundSlideStatement node)
		{
			//When _declarations doesn't contain our variable
			//we already evaluated it.
			if (!_declarations.ContainsKey(node.Variable))
				return;
			_variables = _variables.Push();
			_currentSlide = new SlideAttributes(node.Variable.Name, _presentationBuilder.GetSlideIndex(), node.Variable.IsVisible);
			
			_steps = new List<Step>();
			foreach (var statement in node.Statements)
			{
				EvaluateStepStatement(statement);
			}
			Template template = null;
			if (node.Template != null)
			{
				_variables = _variables.Push();
				var currentTemplate = _presentationBuilder.GetTemplate(node.Template);
				_variables[currentTemplate.SlideParameter.Variable] = _currentSlide;
				_variables = _variables.Push();
				EvaluateStatement(currentTemplate.Body);
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
			_presentationBuilder.AddSlide(node.Variable, slide);
			_declarations.Remove(node.Variable);
		}


		private void EvaluateStepStatement(BoundStepStatement node)
		{
			_currentIdBase = _currentSlide.name;
			if (node.Variable != null) _currentIdBase += $"-{node.Variable.Name}";
			//_variables = _variables.Push();
			var step = new Step(node.Variable?.Name, _currentSlide);
			var isContinousStep = _steps.Count > 0;
			_steps.Add(step);
			EvaluateStatement(node.Body);

			//_variables.Pop(out var slideValues);
			var animationCalls = _presentationBuilder.TakeAnimations();
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
							_currentSlide.n_filter = (Filter)value.Value;
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
				var functionNameBase = _currentIdBase;

				var jsCode = _jsEmitter.Emit(node.Body, _variables, functionNameBase);
				var insertion = new JSInsertionBlock(functionNameBase, jsCode.Code, jsCode.VariableDefinitions, dependency.Kind);
				_presentationBuilder.AddJSInsertion(insertion);
				if (dependency.Kind == JSInsertionKind.Slider)
				{
					var fieldExpression = (BoundFieldAccessExpression)dependency.Value;
					var parent = (Slider)EvaluateExpression(fieldExpression.Parent);
					parent.add_JSInsertion(insertion);
				}
			}
		}

		protected override object CheckIfIsImport(object value)
		{
			if (value is ImportExpression<LibrarySymbol> importLibrary)
			{
				return importLibrary.Value;
			}
			else if (value is ImportExpression<Font> importFont)
			{
				_presentationBuilder.AddImport(importFont.Href);
				return importFont.Value;
			}
			return value;
		}


		protected override void TryAddChildren(VariableSymbol variable, object value)
		{
			if(_groupBuilders.Any())
				_groupBuilders.Peek().TryAddChildren(variable, value);
		}

		protected override object EvaluateAnimationCall(AnimationSymbol animation, FunctionSymbol function, object[] args)
		{
			//																												 ^
			//TODO: use function parameter	----------------------------------------------------|
			//For what though? Maybe if we let you put parameters into animations. but that is not
			//possible as of now..
			var cases = new CaseCall[animation.Cases.Length];
			var element = (Element)args[0];
			var time = (Time)args[1];
			_variables = _variables.Push();
			_variables.Add(animation.ElementParameter, element);
			_variables.Add(animation.TimeParameter, time);
			var attributes = new HashSet<string>();
			for (int i = 0; i < cases.Length; i++)
			{
				foreach (var field in CollectAnimationFields(animation.Cases[i].Body).Select(a => a.Name))
				{
					if (!attributes.Contains(field))
						attributes.Add(field);
				}
			}
			for (int i = 0; i < cases.Length; i++)
			{
				var attributes2 = new List<Slides.Attribute>();
				attributes2.AddRange(CollectAnimationFields(animation.Cases[i].Body));
				foreach (var attribute in attributes)
				{
					if (!attributes2.Any(a => a.Name == attribute))
						attributes2.Add(new Slides.Attribute(attribute, GetValue(element, attribute)));
				}
				cases[i] = new CaseCall(animation.Cases[i].Condition, attributes2.ToArray());
			}
			_variables = _variables.Pop(out var _);
			var result = new AnimationCall(animation.Variable.Name, attributes.ToArray(), time, cases, element);
			_presentationBuilder.AddAnimation(element, result);
			return result;
		}

		private object GetValue(Element element, string attribute)
		{
			switch (attribute)
			{
				default:
					throw new NotImplementedException();
			}
		}

		protected override object EvaluateGroupConstructorExpression(object[] args, BodySymbol group)
		{
			if (!(group.Symbol.Type is AdvancedTypeSymbol advanced))
				throw new Exception();

			var isSVG = group.Symbol.Type.CanBeConvertedTo(_builtInTypes.LookSymbolUp(typeof(SVGElement)));
			if (isSVG)
				_groupBuilders.Push(GroupBuilder.CreateSVGGroupBuilder());
			else
				_groupBuilders.Push(GroupBuilder.CreateGroupBuilder());
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
			if (group.Body != null)
				EvaluateStatement(group.Body);

			_currentReferenced = oldReferenced;

			//TODO(Minor): 
			//As of now they are included in the _groupChildren. You just need to give them
			//a better name! Numbers are not that good. arbitrarly numbers even..
			SVGGraphicsElement[] svgValues = null;
			Element[] groupValues = null;
			var builder = _groupBuilders.Pop();
			if (isSVG)
				svgValues = builder.GetSVGValues();
			else
				groupValues = builder.GetGroupValues();
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
				foreach (var style in builder.GetAppliedStyles())
					be.applyStyle(style);
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
					case "borderColor":
						element.borderColor = SlidesConverter.ConvertToColor(variable.Value);
						break;
					case "borderStyle":
						element.borderStyle = (BorderStyle)variable.Value;
						break;
					case "borderThickness":
						element.borderThickness = (Thickness)variable.Value;
						break;
					default:
						var type = variable.Value.GetType();
						var typeSymbol = variable.Key.Type;
						if (!typeSymbol.CanBeConvertedTo(_builtInTypes.LookSymbolUp(typeof(Element))) &&
							type != typeof(ImageSource) && type != typeof(CSVFile))
							Logger.LogUnusedVariableInBlock(variable.Key.Name, _builtInTypes.TryLookTypeUp(variable.Key.Type), $"group {element.TypeName}");
						break;
				}
			}
		}

		protected override object EvaluateConstructorCall(FunctionSymbol function, object[] args, LibrarySymbol source)
		{
			if (_presentationBuilder.TryGetCustomType(function.Type, out var group))
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
					if(_groupBuilders.Count == 0)
						_currentSlide.applyStyle(style);
					else
						_groupBuilders.Peek().ApplyStyle(style);
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

		protected override void AddReferencedFile(string fileName) => _presentationBuilder.AddReferencedFile(fileName);

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
				if (_presentationBuilder.HasStyle(v.Variable)) return _presentationBuilder.GetStyle(v.Variable);
				//TODO(Minor): if your style is in a different library, tell us which one it is!
				//I mean you probably already do, because otherwise the Binder might cry,
				//but the evaluator doesn't use this piece of information..
				foreach (var lib in _presentationBuilder.GetReferenced())
				{
					var style = lib.Styles.FirstOrDefault(s => s.Name == v.Variable.Name);
					if (style == null)
						continue;
					return style;
				}
			}
			return LookVariableUp(v.Variable);
		}

		protected override void AssignVariable(VariableSymbol variable, object value)
		{
			TryAddChildren(variable, value);
			//If variable is slide attribute and we are not the first step
			//we need to add an jsInsertion here!
			//We probably don't set the variable as well, because otherwise
			//it would change the result in the end.
			if (_steps.Count > 1)
			{
				switch (variable.Name)
				{
					case "background":
					case "padding":
					case "filter":
					case "transition":
					case "font":
					case "fontsize":
					case "color":
						HandleStepJSInsertion(variable, value);
						break;
					default:
						_variables[variable] = value;
						break;
				}
			}
			else
				_variables[variable] = value;
		}

		private void HandleStepJSInsertion(VariableSymbol variable, object value)
		{
			//TODO: Maybe beautify it.
			var jsValue = "'none'";
			if (value != null) jsValue = JavaScriptEmitter.ObjectToString(value);
			var body = $"slide.style.{variable.Name} = {jsValue};";
			var variables = new Dictionary<string, string>();
			variables.Add("slide", JavaScriptEmitter.ObjectToString(_currentSlide));
			_presentationBuilder.AddJSInsertion(new JSInsertionBlock($"step_{_steps.Last().ID}", body, variables, JSInsertionKind.Step, _steps.Last()));
			return;
			//TODO: This will come in handy in the future!
			object originValue = null;
			switch (variable.Name)
			{
				case "background":
					originValue = _currentSlide.background;
					break;
				case "padding":
					originValue = _currentSlide.padding;
					break;
				case "filter":
					originValue = _currentSlide.n_filter;
					break;
				case "transition":
					originValue = _currentSlide.transition;
					break;
				case "font":
					originValue = _currentSlide.font;
					break;
				case "fontsize":
					originValue = _currentSlide.fontsize;
					break;
				case "color":
					originValue = _currentSlide.color;
					break;
				default:
					throw new NotImplementedException();
			}
			jsValue = "''";
			if (originValue != null) jsValue = JavaScriptEmitter.ObjectToString(originValue);
			var revertBody = $"slide.style.{variable.Name} = {jsValue};";
			if(revertBody != body)
				_presentationBuilder.AddJSInsertion(new JSInsertionBlock($"undo_step_{_steps.Last().ID}", revertBody, variables, JSInsertionKind.Step, _steps.Last()));
		}

		protected override void AssignArray(object[] array, int index, object value)
		{
			array[index] = value;
		}


		protected override void AssignField(object parent, VariableSymbol field, object value)
		{
			//If parent is from an earlier step add a jsInsertion 
			//to the current step here!
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
	}
}