namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundIfStatement : BoundStatement
	{
		public BoundIfStatement(BoundExpression boundCondition, BoundStatement boundBody, BoundStatement boundElse)
		{
			Condition = boundCondition;
			Body = boundBody;
			Else = boundElse;
		}

		public BoundExpression Condition { get; }
		public BoundStatement Body { get; }
		public BoundStatement Else { get; }

		public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
	}
}