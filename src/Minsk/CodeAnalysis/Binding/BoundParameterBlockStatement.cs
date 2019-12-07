namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundParameterBlockStatement : BoundStatement
	{

		public BoundParameterBlockStatement(BoundParameterStatement[] statements)
		{
			Statements = statements;
		}

		public BoundParameterStatement[] Statements { get; }

		public override BoundNodeKind Kind => BoundNodeKind.ParameterBlockStatement;
	}
}