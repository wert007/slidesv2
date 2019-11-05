namespace Minsk.CodeAnalysis.Syntax
{
	internal class SyntaxTokenPair : SyntaxNode
	{
		public SyntaxTokenPair(SyntaxToken first, SyntaxToken second)
		{
			First = first;
			Second = second;
		}

		public override SyntaxKind Kind => SyntaxKind.SyntaxTokenPair;

		public SyntaxToken First { get; }
		public SyntaxToken Second { get; }
	}
}