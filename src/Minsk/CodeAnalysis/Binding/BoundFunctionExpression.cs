using Minsk.CodeAnalysis.SlidesTypes;
using Minsk.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundFunctionExpression : BoundExpression
	{
		public BoundFunctionExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments, LibrarySymbol source)
		{
			Function = function;
			Arguments = arguments;
			Source = source;
		}

		public override TypeSymbol Type => Function.Type;
		public override BoundNodeKind Kind => BoundNodeKind.FunctionExpression;

		public FunctionSymbol Function { get; }
		public ImmutableArray<BoundExpression> Arguments { get; }
		public LibrarySymbol Source { get; }

	}
}