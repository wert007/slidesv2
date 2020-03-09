using Minsk.CodeAnalysis.Symbols;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundArrayExpression : BoundExpression
	{

		public BoundArrayExpression(BoundExpression[] expressions)
		{
			Expressions = expressions;
			_type = new ArrayTypeSymbol(Expressions[0].Type);
		}

		private TypeSymbol _type;
		public override TypeSymbol Type => _type;

		public override BoundNodeKind Kind => BoundNodeKind.ArrayExpression;

		public BoundExpression[] Expressions { get; }
		public int Length => Expressions.Length;

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundArrayExpression)expression;
			if (Length != e.Length)
				return false;
			for (int i = 0; i < Length; i++)
			{
				if (!Expressions[i].EqualsBoundExpression(e.Expressions[i]))
					return false;
			}
			return true;
		}
	}
}