namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundJSInsertionStatement : BoundStatement
	{
		
		public BoundJSInsertionStatement(BoundExpression[] dependencies, BoundStatement body)
		{
			Dependencies = dependencies;
			Body = body;
		}

		public override BoundNodeKind Kind => BoundNodeKind.JSInsertionKind;

		public BoundExpression[] Dependencies { get; }
		public BoundStatement Body { get; }
	}
}