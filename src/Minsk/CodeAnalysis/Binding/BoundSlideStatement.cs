using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundSlideStatement : BoundStatement
	{
		public BoundSlideStatement(VariableSymbol variable, BoundStepStatement[] statements)
		{
			Variable = variable;
			Statements = statements;
		}

		public override BoundNodeKind Kind => BoundNodeKind.SlideStatement;
		public VariableSymbol Variable { get; }
		public BoundStepStatement[] Statements{ get; }
	}
}