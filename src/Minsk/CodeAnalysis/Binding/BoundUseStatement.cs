namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundUseStatement : BoundStatement
	{
		
		public BoundUseStatement(BoundExpression[] dependencies, BoundStatement body)
		{
			Dependencies = dependencies;
			Body = body;
		}

		public override BoundNodeKind Kind => BoundNodeKind.UseStatement;

		public BoundExpression[] Dependencies { get; }
		public BoundStatement Body { get; }
	}
}