using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal sealed class BoundSlideStatement : BoundStatement
	{
		public BoundSlideStatement(bool isVisible, VariableSymbol variable, VariableSymbol template, BoundStepStatement[] statements)
		{
			IsVisible = isVisible;
			Variable = variable;
			Template = template;
			Statements = statements;
		}

		public override BoundNodeKind Kind => BoundNodeKind.SlideStatement;
		public bool IsVisible { get; }
		public VariableSymbol Variable { get; }
		public VariableSymbol Template { get; }
		public BoundStepStatement[] Statements{ get; }
	}
}