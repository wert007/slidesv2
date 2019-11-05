using Minsk.CodeAnalysis.Binding;

namespace Minsk.CodeAnalysis.Symbols
{
	public class StyleSymbol
	{
		internal StyleSymbol(BoundStyleStatement node)
		{
			Variable = node.Variable;
			Parameter = node.BoundParameter?.Variable;
			BoundBody = node.BoundBody;
		}

		public VariableSymbol Variable { get; }
		public VariableSymbol Parameter{ get; }
		internal BoundBlockStatement BoundBody { get; }

		public override string ToString() => $"style {Variable.Name}({Parameter})";
	}
}