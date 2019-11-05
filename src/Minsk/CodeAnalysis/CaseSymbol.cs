using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.SlidesTypes;

namespace Minsk.CodeAnalysis
{
	internal class CaseSymbol
	{
		public CaseSymbol(float condition, BoundStatement body)
		{
			Condition = condition;
			Body = body;
		}

		public float Condition { get; }
		public BoundStatement Body { get; }
	}
}