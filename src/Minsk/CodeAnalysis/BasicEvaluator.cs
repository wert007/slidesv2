using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using Slides;
using Slides.Data;
using Slides.Helpers;
using Slides.MathExpressions;
using Color = Slides.Data.Color;

namespace Minsk.CodeAnalysis
{
	internal abstract class BasicEvaluator
	{
		protected readonly BoundStatement _root;
		protected VariableValueCollection _constants = new VariableValueCollection(null);
		protected readonly BuiltInTypes _builtInTypes = BuiltInTypes.Instance;
		protected readonly _GlobalFunctionsConverter _builtInFunctions = _GlobalFunctionsConverter.Instance;
		public BasicEvaluator(BoundStatement root)
		{
			_root = root;

			_constants.Add(new VariableSymbol("seperator", true, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol))), Library.Seperator);
			_constants.Add(new VariableSymbol("coding", true, _builtInTypes.LookSymbolUp(typeof(LibrarySymbol))), Library.Code);
			_constants.Add(new VariableSymbol("auto", true, _builtInTypes.LookSymbolUp(typeof(Unit))), new Unit(0, Unit.UnitKind.Auto));
			_constants.Add(new VariableSymbol("thin", true, _builtInTypes.LookSymbolUp(typeof(Unit))), Unit.Thin);
			_constants.Add(new VariableSymbol("medium", true, _builtInTypes.LookSymbolUp(typeof(Unit))), Unit.Medium);
			_constants.Add(new VariableSymbol("thick", true, _builtInTypes.LookSymbolUp(typeof(Unit))), Unit.Thick);
			foreach (var color in Color.GetStaticColors())
			{
				_constants.Add(new VariableSymbol(color.Key, true, _builtInTypes.LookSymbolUp(typeof(Color))), color.Value);
			}
		}

		public virtual object LookVariableUp(VariableSymbol variable)
		{
			return _constants[variable];
		}

		public object EvaluateExpression(BoundExpression node)
		{
			object result = null;
			switch (node.Kind)
			{
				case BoundNodeKind.StringExpression:
					result = EvaluateStringExpression((BoundStringExpression)node);
					break;
				case BoundNodeKind.LiteralExpression:
					result = EvaluateLiteralExpression((BoundLiteralExpression)node);
					break;
				case BoundNodeKind.VariableExpression:
					result = EvaluateVariableExpression((BoundVariableExpression)node);
					break;
				case BoundNodeKind.AssignmentExpression:
					result = EvaluateAssignmentExpression((BoundAssignmentExpression)node);
					break;
				case BoundNodeKind.UnaryExpression:
					result = EvaluateUnaryExpression((BoundUnaryExpression)node);
					break;
				case BoundNodeKind.BinaryExpression:
					result = EvaluateBinaryExpression((BoundBinaryExpression)node);
					break;
				case BoundNodeKind.FunctionExpression:
					result = EvaluateFunctionExpression((BoundFunctionExpression)node);
					break;
				case BoundNodeKind.EmptyArrayConstructorExpression:
					result = EvaluateEmptyArrayConstructorExpression((BoundEmptyArrayConstructorExpression)node);
					break;
				case BoundNodeKind.ArrayExpression:
					result = EvaluateArrayExpression((BoundArrayExpression)node);
					break;
				case BoundNodeKind.EnumExpression:
					result = EvaluateEnumExpression((BoundEnumExpression)node);
					break;
				case BoundNodeKind.FieldAccessExpression:
					result = EvaluateFieldAccessExpression((BoundFieldAccessExpression)node);
					break;
				case BoundNodeKind.FunctionAccessExpression:
					result = EvaluateFunctionAccessExpression((BoundFunctionAccessExpression)node);
					break;
				case BoundNodeKind.ConversionExpression:
					result = EvaluateConversion((BoundConversion)node);
					break;
				case BoundNodeKind.MathExpression:
					result = EvaluateMathExpression((BoundMathExpression)node);
					break;
				case BoundNodeKind.ArrayAccessExpression:
					result = EvaluateArrayAccessExpression((BoundArrayAccessExpression)node);
					break;
				default:
					throw new Exception($"Unexpected node {node.Kind}");
			}
			if (result?.GetType().IsArray == true)
				return ConvertToArray(result);
			return result;
		}

		private object[] ConvertToArray(object value)
		{
			if (value is ICollection<int> intArray)
			{
				var result = new object[intArray.Count];
				for (int i = 0; i < result.Length; i++)
					result[i] = intArray.ElementAt(i);
				return result;
			}
			if (value is ICollection<float> floatArray)
			{
				var result = new object[floatArray.Count];
				for (int i = 0; i < result.Length; i++)
					result[i] = floatArray.ElementAt(i);
				return result;
			}
			if (value is ICollection<string> stringArray)
			{
				var result = new object[stringArray.Count];
				for (int i = 0; i < result.Length; i++)
					result[i] = stringArray.ElementAt(i);
				return result;
			}
			if (value is ICollection<bool> boolArray)
			{
				var result = new object[boolArray.Count];
				for (int i = 0; i < result.Length; i++)
					result[i] = boolArray.ElementAt(i);
				return result;
			}
			if (value is ICollection<object> objectArray)
			{
				//This assures, that we can change the array later and it will be actually saved.
				return (object[])objectArray;
			}
			throw new NotImplementedException();
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
			return node.ConstantValue;
		}

		protected virtual object EvaluateVariableExpression(BoundVariableExpression node)
		{
			return LookVariableUp(node.Variable);
		}

		protected virtual void AssignVariable(VariableSymbol variable, object value) { }
		protected virtual void AssignField(object parent, VariableSymbol field, object value) { }
		protected virtual void AssignArray(object[] array, int index, object value, VariableSymbol optionalVariable) { }

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
					VariableSymbol variable = null;
					if (arrayAccessExpression.Child is BoundVariableExpression variableExpression)
						variable = variableExpression.Variable;
					AssignArray(array, index, value, variable);
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
				case BoundUnaryOperatorKind.NoneableNegation:
					return operand;
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
				case BoundBinaryOperatorKind.RangeMultiplication:
					var range = left as Range ?? (Range)right;
					var multiplicator = left as int? ?? (int)right;
					if (multiplicator > 0)
						return new Range(range.From * multiplicator, range.To * multiplicator, range.Step * multiplicator);
					else if (multiplicator < 0)
						return new Range(range.To * multiplicator, range.From * multiplicator, range.Step * multiplicator);
					else
						return new Range(0, 0, 0);
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
			return SlidesHelper.AddOrientations(horizontal, vertical);
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
			if (source != null)
			{
				var result = source.Call(function, args);
				AddReferencedLibrary(source);
				return result;
			}
			else
			{
				var tracker = new ReferenceTracker();
				GlobalFunctions.Set_BackDump(tracker);
				var result = _builtInFunctions.Call(function, args);
				GlobalFunctions.Set_BackDump(null);
				if (tracker.Reference != null)
					AddReferencedFile(tracker.Reference);
				return result;
			}
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
			return EvaluateFieldAccessExpressionValue(parent, node.Field.Variable.Name, _builtInTypes.LookTypeUp(node.Field.Variable.Type));
		}

		protected static object EvaluateFieldAccessExpressionValue(object parent, string fieldName, Type fieldType)
		{
			var type = parent.GetType();
			if (type == typeof(DataObject))
			{
				var data = (DataObject)parent;
				if (!data.TryLookUp(fieldName, out var value))
					throw new Exception();
				return value;
			}
			var prop = type.GetProperty(fieldName, fieldType) ?? type.GetProperty(fieldName.ToVariableUpper(), fieldType);
			var v = prop.GetValue(parent);
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

		protected bool EvaluateCustomFunctionAccessCall(object parent, FunctionSymbol function, object[] args, out object result)
		{
			int mod(int x, int m)
			{
				return (x % m + m) % m;
			}

			if (parent.GetType().IsArray)
			{
				switch (function.Name)
				{
					case "len":
						result = ((ICollection<object>)parent).Count;
						return true;
					case "getSafe":
						//TODO: Test getSafe with integers!!
						result = ((ICollection<object>)parent).ElementAtOrDefault((int)args[0]);
						return true;
					case "getLoop":
						var array = (ICollection<object>)parent;
						result = array.ElementAt(mod((int)args[0],array.Count));
						return true;
					default:
						result = null;
						return false;
				}
			}
			else if (parent is string str)
			{
				switch (function.Name)
				{
					case "split":
						result = str.Split(new[] { (string)args[0] }, StringSplitOptions.None);
						return true;
					default:
						result = null;
						return false;
				}
			}
			else
			{
				result = null;
				return false;
			}
		}

		protected object EvaluateFunctionAccessCall(object parent, FunctionSymbol function, object[] args)
		{
			if (EvaluateCustomFunctionAccessCall(parent, function, args, out var result)) return result;
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
					case Unit u:
						return u;
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
			if(targetType.Type == TypeType.Noneable)
			{
				var targetSymbol = ((NoneableTypeSymbol)targetType).BaseType;
				var target = _builtInTypes.LookTypeUp(targetSymbol);
				if (target.IsAssignableFrom(value.GetType()))
					return value;
				return null;
			}
			//Do we need checks here?
			return value;
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