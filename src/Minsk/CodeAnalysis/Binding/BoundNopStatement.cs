namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundNopStatement : BoundStatement
	{
		public override BoundNodeKind Kind => BoundNodeKind.NopStatement;
	}
}