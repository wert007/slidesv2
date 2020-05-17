using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Filters;
using Slides.MathExpressions;
using Color = Slides.Color;

namespace Minsk.CodeAnalysis
{
	internal abstract class BasicEvaluator
	{
		protected readonly BoundStatement _root;
		protected VariableValueCollection _constants = new VariableValueCollection(null);
		protected readonly BuiltInTypes _builtInTypes = BuiltInTypes.Instance;

		public BasicEvaluator(BoundStatement root)
		{
			_root = root;

			_constants.Add(new VariableSymbol("seperator", true, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol)), false), Library.Seperator);
			_constants.Add(new VariableSymbol("code", true, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol)), false), Library.Code);
			_constants.Add(new VariableSymbol("auto", true, _builtInTypes.LookSymbolUp(typeof(Unit)), false), new Unit(0, Unit.UnitKind.Auto));
			foreach (var color in Color.GetStaticColors())
			{
				_constants.Add(new VariableSymbol(color.Key, true, _builtInTypes.LookSymbolUp(typeof(Color)), false), color.Value);
			}
		}

		public virtual object LookVariableUp(VariableSymbol variable)
		{
			return _constants[variable];
		}

		public object EvaluateExpression(BoundExpression node)
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
				case BoundNodeKind.UnaryExpression:
					return EvaluateUnaryExpression((BoundUnaryExpression)node);
				case BoundNodeKind.BinaryExpression:
					return EvaluateBinaryExpression((BoundBinaryExpression)node);
				case BoundNodeKind.FunctionExpression:
					return EvaluateFunctionExpression((BoundFunctionExpression)node);
				case BoundNodeKind.EmptyArrayConstructorExpression:
					return EvaluateEmptyArrayConstructorExpression((BoundEmptyArrayConstructorExpression)node);
				case BoundNodeKind.ArrayExpression:
					return EvaluateArrayExpression((BoundArrayExpression)node);
				case BoundNodeKind.EnumExpression:
					return EvaluateEnumExpression((BoundEnumExpression)node);
				case BoundNodeKind.FieldAccessExpression:
					return EvaluateFieldAccessExpression((BoundFieldAccessExpression)node);
				case BoundNodeKind.FunctionAccessExpression:
					return EvaluateFunctionAccessExpression((BoundFunctionAccessExpression)node);
				case BoundNodeKind.ConversionExpression:
					return EvaluateConversion((BoundConversion)node);
				case BoundNodeKind.MathExpression:
					return EvaluateMathExpression((BoundMathExpression)node);
				case BoundNodeKind.ArrayAccessExpression:
					return EvaluateArrayAccessExpression((BoundArrayAccessExpression)node);
				default:
					throw new Exception($"Unexpected node {node.Kind}");
			}
		}

		protected virtual object EvaluateStringExpression(BoundStringExpression node)
		{
			var values = new object[node.Expressions.Length];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = EvaluateExpression(node.Expressions[i]);
			}
			return EvaluateStringExpressionValue(values);
		}

		protected object EvaluateStringExpressionValue(object[] values) => string.Join("", values);

		protected virtual object EvaluateLiteralExpression(BoundLiteralExpression node)
		{
			return node.Value;
		}

		protected virtual object EvaluateVariableExpression(BoundVariableExpression node)
		{
			return LookVariableUp(node.Variable);
		}

		protected virtual void AssignVariable(VariableSymbol variable, object value) { }
		protected virtual void AssignField(object parent, VariableSymbol field, object value) { }
		protected virtual void AssignArray(object[] array, int index, object value) { }

		protected virtual object EvaluateAssignmentExpression(BoundAssignmentExpression node)
		{
			var value = EvaluateExpression(node.Expression);
			switch (node.LValue.Kind)
			{
				case BoundNodeKind.VariableExpression:
					AssignVariable(((BoundVariableExpression)node.LValue).Variable, value);
					break;
				case BoundNodeKind.FieldAccessExpression:
					var fieldAccessExpression = (BoundFieldAccessExpression)node.LValue;
					var parent = EvaluateExpression(fieldAccessExpression.Parent);
					var field = fieldAccessExpression.Field.Variable;
					AssignField(parent, field, value);
					break;
				case BoundNodeKind.ArrayAccessExpression:
					var arrayAccessExpression = (BoundArrayAccessExpression)node.LValue;
					var array = (object[])EvaluateExpression(arrayAccessExpression.Child);
					var index = (int)EvaluateExpression(arrayAccessExpression.Index);
					AssignArray(array, index, value);
					break;
				default:
					throw new NotImplementedException();
			}
			return value;
		}

		protected virtual object EvaluateUnaryExpression(BoundUnaryExpression node)
		{
			var operand = EvaluateExpression(node.Operand);
			return EvaluateUnaryExpressionValue(node.Op, operand);
		}

		protected object EvaluateUnaryExpressionValue(BoundUnaryOperator op, object operand)
		{
			switch (op.Kind)
			{
				case BoundUnaryOperatorKind.Identity:
					return (int)operand;
				case BoundUnaryOperatorKind.Negation:
					if (operand.GetType() == typeof(int))
						return -(int)operand;
					if (operand.GetType() == typeof(float))
						return -(float)operand;
					var oldUnit = (Unit)operand;
					return new Unit(-oldUnit.Value, oldUnit.Kind);
				case BoundUnaryOperatorKind.LogicalNegation:
					return !(bool)operand;
				default:
					throw new Exception($"Unexpected unary operator {op}");
			}
		}

		protected virtual object EvaluateBinaryExpression(BoundBinaryExpression node)
		{
			var left = EvaluateExpression(node.Left);
			var right = EvaluateExpression(node.Right);

			return EvaluateBinaryExpressionValue(left, node.Op, right);
		}

		protected object EvaluateBinaryExpressionValue(object left, BoundBinaryOperator op, object right)
		{
			switch (op.Kind)
			{
				case BoundBinaryOperatorKind.Addition:
					return Add(left, right);
				case BoundBinaryOperatorKind.Concatination:
					return left.ToString() + right.ToString();
				case BoundBinaryOperatorKind.Subtraction:
					return Subtract(left, right);
				case BoundBinaryOperatorKind.Multiplication:
					return Multiply(left, right);
				case BoundBinaryOperatorKind.Division:
					return Divide(left, right);
				case BoundBinaryOperatorKind.Exponentiation:
					return Exponentiate(left, right);

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
					return AddEnums(left, right);
				case BoundBinaryOperatorKind.FilterAddition:
					return new FilterAddition((Filter)left, (Filter)right);
				case BoundBinaryOperatorKind.Range:
					int il = (int)left;
					int ir = (int)right;
					if (il <= ir)
						return new Range(il, ir, 1);
					return new Range(il, ir, -1);
				case BoundBinaryOperatorKind.NotNoneValue:
					if (left == null) return right;
					return left;
				default:
					throw new Exception($"Unexpected binary operator {op}");
			}
		}
		private static object Add(object left, object right)
		{
			if (left is Unit && right is Unit)
				return (Unit)left + (Unit)right;
			if (left.GetType() == typeof(float) || right.GetType() == typeof(float))
				return Convert.ToSingle(left) + Convert.ToSingle(right);
			return (int)left + (int)right;
		}

		private static object Subtract(object left, object right)
		{
			if (left is Unit && right is Unit)
				return (Unit)left - (Unit)right;
			if (left.GetType() == typeof(float) || right.GetType() == typeof(float))
				return Convert.ToSingle(left) - Convert.ToSingle(right);
			return (int)left - (int)right;
		}

		private static object Multiply(object left, object right)
		{
			if (left.GetType() == typeof(Unit) && right.GetType() == typeof(float))
				return new Unit(((Unit)left).Value * (float)right, ((Unit)left).Kind);
			if (right.GetType() == typeof(Unit) && left.GetType() == typeof(float))
				return new Unit(((Unit)right).Value * (float)left, ((Unit)right).Kind);
			if (left.GetType() == typeof(float) || right.GetType() == typeof(float))
				return Convert.ToSingle(left) * Convert.ToSingle(right);
			return (int)left * (int)right;
		}

		private static object Divide(object left, object right)
		{
			if (left.GetType() == typeof(Unit) && right.GetType() == typeof(float))
				return new Unit(((Unit)left).Value / (float)right, ((Unit)left).Kind);
			if (left.GetType() == typeof(float) || right.GetType() == typeof(float))
				return Convert.ToSingle(left) / Convert.ToSingle(right);
			if ((int)right == 0) return 0;
			return (int)left / (int)right;
		}

		private static object Exponentiate(object left, object right)
		{
			if (left.GetType() == typeof(int) && right.GetType() == typeof(int))
				return (int)Math.Pow((int)left, (int)right);
			return (float)Math.Pow((float)left, (float)right);
		}

		private object AddEnums(object left, object right)
		{
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
			return AddOrientations(horizontal, vertical);
		}

		protected Orientation AddOrientations(Horizontal h, Vertical v)
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


		protected virtual object EvaluateFunctionExpression(BoundFunctionExpression node)
		{
			var args = new object[node.Arguments.Length];
			for (int i = 0; i < node.Arguments.Length; i++)
			{
				args[i] = EvaluateExpression(node.Arguments[i]);
			}
			return EvaluateFunctionExpressionValue(node.Function, args, node.Source);
		}

		protected object EvaluateFunctionExpressionValue(FunctionSymbol function, object[] args, LibrarySymbol source)
		{
			if (function.Name == "constructor")
			{
				return EvaluateConstructorCall(function, args, source);
			}

			switch (function.Name)
			{
				case "useStyle":
				case "useGroup":
				case "useData":
				case "useAnimation":
				case "applyStyle":
				case "setData":
				case "lib": return EvaluateNativeFunction(function.Name, args);
			}
			MethodInfo method = null;
			if (source != null)
			{
				method = source.LookMethodInfoUp(function);
				AddReferencedLibrary(source);
			}
			else
			{
				method = GlobalFunctionsConverter.Instance.LookMethodInfoUp(function);
			}
			if (function.Name == "image")
			{
				var fileName = args[0].ToString();
				AddReferencedFile(fileName);
			}
			return MethodInvoke(method, null, args);
		}

		protected virtual object EvaluateNativeFunction(string name, object[] args) => null;
		protected virtual void AddReferencedFile(string fileName) { }

		protected virtual void AddReferencedLibrary(LibrarySymbol source) { }

		protected virtual object EvaluateConstructorCall(FunctionSymbol function, object[] args, LibrarySymbol source)
		{
			if (source != null)
			{
				var customType = source.CustomTypes.FirstOrDefault(ct => ct.Symbol.Type == function.Type);
				if (customType != null)
				{
					return EvaluateGroupConstructorExpression(args, customType);
				}
			}
			return EvaluateConstructorCall(function, args);
		}

		protected virtual object EvaluateConstructorCall(FunctionSymbol function, object[] args)
		{
			if(ConstructorSymbolConverter.Instance.TryConstructorInfoLookUp(function, out var constructor))
				return ConstructorInvoke(constructor, args);
			return null;
		}

		protected virtual object EvaluateGroupConstructorExpression(object[] args, BodySymbol body) => null;

		protected virtual object EvaluateEmptyArrayConstructorExpression(BoundEmptyArrayConstructorExpression node)
		{
			var length = (int)EvaluateExpression(node.Length);
			return EvaluateEmptyArrayConstructorExpressionValue((ArrayTypeSymbol)node.Type, length);
		}

		protected object EvaluateEmptyArrayConstructorExpressionValue(ArrayTypeSymbol type, int length)
		{
			var result = new object[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = type.Child.DefaultValue;
			}
			return result;
		}

		protected virtual object EvaluateArrayExpression(BoundArrayExpression node)
		{
			var result = new object[node.Expressions.Length];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = EvaluateExpression(node.Expressions[i]);
			}
			return result;
		}

		protected virtual object EvaluateEnumExpression(BoundEnumExpression node)
		{
			var type = _builtInTypes.LookTypeUp(node.Type);
			foreach (var value in type.GetEnumValues())
			{
				if (value.ToString() == node.Value)
					return value;
			}
			throw new Exception();
		}

		protected virtual object EvaluateFieldAccessExpression(BoundFieldAccessExpression node)
		{
			var parent = EvaluateExpression(node.Parent);
			return EvaluateFieldAccessExpressionValue(parent, node.Field.Variable.Name);
		}

		protected static object EvaluateFieldAccessExpressionValue(object parent, string fieldName)
		{
			var type = parent.GetType();
			if (type == typeof(DataObject))
			{
				var data = (DataObject)parent;
				if (!data.TryLookUp(fieldName, out var value))
					throw new Exception();
				return value;
			}
			var v = type.GetProperty(fieldName).GetValue(parent);
			return v;
		}

		protected virtual object EvaluateFunctionAccessExpression(BoundFunctionAccessExpression node)
		{
			var parent = EvaluateExpression(node.Parent);
			var function = node.FunctionCall;
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
				return EvaluateAnimationCall((AnimationSymbol)parent, function.Function, args);

			return EvaluateFunctionAccessCall(parent, function.Function, args);
		}


		protected virtual object EvaluateAnimationCall(AnimationSymbol parent, FunctionSymbol function, object[] args) => null;

		protected object EvaluateFunctionAccessCall(object parent, FunctionSymbol function, object[] args)
		{
			if (function.Name == "len")
				return ((ICollection<object>)parent).Count;
			var type = parent.GetType();
			var parameters = function.Parameter.Select(p => _builtInTypes.LookTypeUp(p.Type)).ToArray();
			var method = type.GetMethod(function.Name, parameters);
			return MethodInvoke(method, parent, args);
		}

		protected virtual object EvaluateConversion(BoundConversion node)
		{
			var value = EvaluateExpression(node.Expression);
			return EvaluateConversionValue(node.Type, value, node.Expression.Type);
		}

		protected object EvaluateConversionValue(TypeSymbol targetType, object value, TypeSymbol sourceType)
		{
			if (targetType == PrimitiveTypeSymbol.Unit)
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
			if (targetType == PrimitiveTypeSymbol.Float)
				return Convert.ToSingle(value);
			if (sourceType.Type == TypeType.Noneable &&
				((NoneableTypeSymbol)sourceType).BaseType == targetType)
			{
				if (value == null)
					throw new Exception();
				return value;
			}
			throw new Exception();
		}

		protected virtual object EvaluateMathExpression(BoundMathExpression node)
		{
			return new MathFormula(node.Expression);
		}

		protected virtual object EvaluateArrayAccessExpression(BoundArrayAccessExpression node)
		{
			var index = (int)EvaluateExpression(node.Index);
			var value = (object[])EvaluateExpression(node.Child);
			return EvaluateArrayAccessExpressionValue(value, index);
		}

		protected object EvaluateArrayAccessExpressionValue(object[] value, int index) => value[index];


		protected object MethodInvoke(MethodInfo method, object obj, object[] args)
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
				if (type == typeof(float))
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

		protected object ConstructorInvoke(ConstructorInfo constructor, object[] args)
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


	}
}