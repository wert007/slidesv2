namespace Minsk.CodeAnalysis.Syntax
{
	internal class LibraryBlockStatementSyntax
	{
		public LibraryBlockStatementSyntax(UseStatement[] useStatements)
		{
			UseStatements = useStatements;
		}

		public UseStatement[] UseStatements { get; }
	}
}