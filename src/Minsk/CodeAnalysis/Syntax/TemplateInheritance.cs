namespace Minsk.CodeAnalysis.Syntax
{
	public class TemplateInheritance
	{
		public TemplateInheritance(SyntaxToken lessToken, SyntaxToken identifier)
		{
			LessToken = lessToken;
			Identifier = identifier;
		}

		public SyntaxToken LessToken { get; }
		public SyntaxToken Identifier { get; }

		public override string ToString()
		{
			return $"< {Identifier.Text}";
		}
	}
}