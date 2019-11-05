using Minsk.CodeAnalysis.Symbols;
using System.Collections.Immutable;
using System.Linq;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundArrayExpression : BoundExpression
	{

		public BoundArrayExpression(ImmutableArray<BoundExpression> boundExpressions)
		{
			BoundExpressions = boundExpressions;
			_type = new ArrayTypeSymbol(BoundExpressions[0].Type);
		}

		private TypeSymbol _type;
		public override TypeSymbol Type => _type;

		public override BoundNodeKind Kind => BoundNodeKind.ArrayExpression;

		public ImmutableArray<BoundExpression> BoundExpressions { get; }

	}
}