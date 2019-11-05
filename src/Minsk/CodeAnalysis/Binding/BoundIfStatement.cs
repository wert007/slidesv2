namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundIfStatement : BoundStatement
	{
		public BoundIfStatement(BoundExpression boundCondition, BoundStatement boundBody, BoundStatement boundElse)
		{
			BoundCondition = boundCondition;
			BoundBody = boundBody;
			BoundElse = boundElse;
		}

		public BoundExpression BoundCondition { get; }
		public BoundStatement BoundBody { get; }
		public BoundStatement BoundElse { get; }

		public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
	}
}