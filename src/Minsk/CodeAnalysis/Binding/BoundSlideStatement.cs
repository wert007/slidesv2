using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundSlideStatement : BoundStatement
	{
		public BoundSlideStatement(VariableSymbol variable, VariableSymbol template, BoundStepStatement[] statements)
		{
			Variable = variable;
			Template = template;
			Statements = statements;
		}

		public override BoundNodeKind Kind => BoundNodeKind.SlideStatement;
		public VariableSymbol Variable { get; }
		public VariableSymbol Template { get; }
		public BoundStepStatement[] Statements{ get; }
	}
}