namespace Minsk.CodeAnalysis.Syntax
{
    public abstract class ExpressionSyntax : SyntaxNode
    {
      public virtual bool IsLValue { get; } = false;
    }
}