using Minsk.CodeAnalysis.Symbols;
using System;

namespace Minsk.CodeAnalysis.Binding
{
	internal abstract class BoundExpression : BoundNode
	{
		public abstract TypeSymbol Type { get; }
		public virtual object ConstantValue => null;

		public bool Contains(BoundExpression expression)
		{
			if (Equals(expression))
				return true;
			foreach (var child in GetChildren())
			{
				if(child is BoundExpression e)
				{
					if (e.Contains(expression))
						return true;
				}
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if(obj is BoundExpression expression)
			{
				if (Kind != expression.Kind)
					return false;
				if (Type != expression.Type)
					return false;
				return EqualsBoundExpression(expression);
			}
			return false;
		}

		public abstract bool EqualsBoundExpression(BoundExpression expression);
	}
}
