namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundStepStatement : BoundStatement
	{
		public BoundStepStatement(string name, BoundStatement body)
		{
			Name = name;
			Body = body;
		}

		public string Name { get; }
		public BoundStatement Body { get; }

		public override BoundNodeKind Kind => BoundNodeKind.StepStatement;
	}
}