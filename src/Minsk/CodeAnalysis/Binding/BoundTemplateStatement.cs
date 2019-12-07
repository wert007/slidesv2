using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundTemplateStatement : BoundStatement
	{
		public BoundTemplateStatement(VariableSymbol variable, BoundParameterStatement slideParameter, BoundBlockStatement body)
		{
			Variable = variable;
			SlideParameter = slideParameter;
			Body = body;
		}

		public override BoundNodeKind Kind => BoundNodeKind.TemplateStatement;

		public VariableSymbol Variable { get; }
		public BoundParameterStatement SlideParameter { get; }
		public BoundBlockStatement Body { get; }
	}
}