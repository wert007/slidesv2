using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slides
{
	public class OptionalValue<T>
	{
		public OptionalValue(T value)
		{
			Value = value;
			HasValue = false;
		}

		public OptionalValue()
		{
			Value = default;
			HasValue = false;
		}

		public T Value { get; }
		public bool HasValue { get; }

		public T GetValueOrDefault()
		{
			if (typeof(T) == typeof(int))
			{

			}
			SimpleLogger.Logger.Log($"No specific Value for Type T '{typeof(T)}' in OptionalValue.GetValueOrDefault().");
			return Value;
		}

		internal static OptionalValue<T1> CreateFromNullable<T1>(T1 value)
		{
			if (value == null)
				return new OptionalValue<T1>();
			return new OptionalValue<T1>(value);
		}

		internal static OptionalValue<T1> CreateWithDefault<T1>(T1 value, T1 defaultValue)
		{
			if (value.Equals(defaultValue))
				return new OptionalValue<T1>();
			return new OptionalValue<T1>(value);
		}
	}
}
