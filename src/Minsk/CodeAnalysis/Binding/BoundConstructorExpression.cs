//using Minsk.CodeAnalysis.Symbols;
//using System.Collections.Immutable;

//namespace Minsk.CodeAnalysis.Binding
//{
//	internal class BoundConstructorExpression : BoundExpression
//	{
//		public BoundConstructorExpression(TypeSymbol type, ImmutableArray<BoundExpression> arguments)
//		{
//			Type = type;
//			Arguments = arguments;
//		}

//		public override TypeSymbol Type { get; }
//		public ImmutableArray<BoundExpression> Arguments { get; }
//		public override BoundNodeKind Kind => BoundNodeKind.ConstructorExpression;
//	}
//}