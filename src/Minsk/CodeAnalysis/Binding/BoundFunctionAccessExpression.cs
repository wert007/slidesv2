using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundFunctionAccessExpression : BoundExpression
	{
		public BoundFunctionAccessExpression(BoundExpression parent, BoundFunctionExpression function)
		{
			Parent = parent;
			FunctionCall = function;
		}
		public override TypeSymbol Type => FunctionCall.Type;

		public override BoundNodeKind Kind => BoundNodeKind.FunctionAccessExpression;

		public BoundExpression Parent { get; }
		//Maybe we will introduce something like this again. But right now a BoundExpression is well fitted.
		//This extra safety measure SHOULDN'T be that important.
		//public Member Member { get; }
		public BoundFunctionExpression FunctionCall { get; }

		public override bool EqualsBoundExpression(BoundExpression expression)
		{
			var e = (BoundFunctionAccessExpression)expression;
			if (!Parent.EqualsBoundExpression(e.Parent))
				return false;
			return FunctionCall.EqualsBoundExpression(e.FunctionCall);
		}

	}
}
