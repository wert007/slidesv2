using System.Collections.Immutable;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis
{
	internal sealed class ReferenceCollector
	{
		public SyntaxTree Imports { get; }
		BlockStatementSyntax _statement;
		private ImmutableArray<StatementSyntax>.Builder _importStatements;



		public ReferenceCollector(SyntaxTree tree)
		{
			_importStatements = ImmutableArray.CreateBuilder<StatementSyntax>();
			CollectReference(tree.Root);
			if (_importStatements.Count > 0)
			{
				_statement = new BlockStatementSyntax(_importStatements.ToImmutable());
				Imports = new SyntaxTree(new CompilationUnitSyntax(_statement, tree.Root.EndOfFileToken));
			}
			else
			Imports = null;
		}

		private void CollectReference(CompilationUnitSyntax root)
		{
			CollectReference(root.Statement);
		}

		private void CollectReference(StatementSyntax statement)
		{
			switch (statement.Kind)
			{
				case SyntaxKind.ImportStatement:
					CollectReference((ImportStatementSyntax)statement);
					break;
				case SyntaxKind.FileBlockStatement:
					CollectReference((FileBlockStatementSyntax)statement);
					break;
				case SyntaxKind.BlockStatement:
					CollectReference((BlockStatementSyntax)statement);
					break;

				default:
					break;
			}
		}

		private void CollectReference(ImportStatementSyntax syntax)
		{
			_importStatements.Add(syntax);
		}

		private void CollectReference(FileBlockStatementSyntax syntax)
		{
			foreach (var statement in syntax.Statements)
			{
				CollectReference(statement);
			}
		}

		private void CollectReference(BlockStatementSyntax syntax)
		{
			foreach (var statement in syntax.Statements)
			{
				CollectReference(statement);
			}
		}
	}
}