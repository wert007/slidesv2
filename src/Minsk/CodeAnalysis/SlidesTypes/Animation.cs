using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Symbols;

namespace Minsk.CodeAnalysis.SlidesTypes
{
	class AnimationSymbol
	{
		public AnimationSymbol(VariableSymbol variable, VariableSymbol elementParameter, VariableSymbol timeParameter, CaseSymbol[] cases)
		{
			Variable = variable;
			ElementParameter = elementParameter;
			TimeParameter = timeParameter;
			Cases = cases;
		}
		public static VariableSymbol DoneSymbol { get; } = new VariableSymbol("done", true, PrimitiveTypeSymbol.Float, false)
		{
			NeedsDataFlag = false
		};
		public static VariableSymbol InitSymbol { get; } = new VariableSymbol("init", true, PrimitiveTypeSymbol.Float, false)
		{
			NeedsDataFlag = false
		};
		public VariableSymbol Variable { get; }
		public VariableSymbol ElementParameter { get; }
		public VariableSymbol TimeParameter { get; }
		public CaseSymbol[] Cases { get; }
	}

}
