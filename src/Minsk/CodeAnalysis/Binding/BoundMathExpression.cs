using Minsk.CodeAnalysis.Symbols;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Binding
{
	internal class BoundMathExpression : BoundExpression
	{
		public BoundMathExpression(string expression, AdvancedTypeSymbol type, int unknowns)
		{
			Expression = expression;
			Type = type;
			Unknowns = unknowns;
			Fields = new VariableSymbol[type.Fields.Count];
			for (int i = 0; i < Fields.Length; i++)
			{
				Fields[i] = new VariableSymbol(type.Fields[i].Name, type.Fields[i].IsReadOnly, type.Fields[i].Type, type.Fields[i].NeedsDataFlag);
			}
		}

		public string Expression { get; }
		public override TypeSymbol Type { get; }
		public int Unknowns { get; }
		public VariableSymbol[] Fields { get; }

		public override BoundNodeKind Kind => BoundNodeKind.MathExpression;
	}
}