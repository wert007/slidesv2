namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundCaseStatement : BoundStatement
	{
		public BoundCaseStatement(BoundExpression condition, BoundStatement body)
		{
			Condition = condition;
			Body = body;
		}

		public BoundExpression Condition { get; }
		public BoundStatement Body { get; }

		public override BoundNodeKind Kind => BoundNodeKind.CaseStatement;

	}
}