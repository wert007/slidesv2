using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundFunctionExpression : BoundExpression
	{
		public BoundFunctionExpression(FunctionSymbol function, BoundExpression[] arguments, LibrarySymbol source)
		{
			Function = function;
			Arguments = arguments;
			Source = source;
		}

		public override TypeSymbol Type => Function.Type;
		public override BoundNodeKind Kind => BoundNodeKind.FunctionExpression;

		public FunctionSymbol Function { get; }
		public BoundExpression[] Arguments { get; }
		public LibrarySymbol Source { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundFunctionExpression)expression;
			if (!Function.Equals(e.Function))
				return false;
			if (Source != null && !Source.Equals(e.Source))
				return false;
			if (Arguments.Length != e.Arguments.Length)
				return false;
			for (int i = 0; i < Arguments.Length; i++)
			{
				if (!Arguments[i].EqualsBoundExpression(e.Arguments[i]))
					return false;
			}
			return true;
		}

	}
}