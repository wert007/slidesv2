using System.Collections.Generic;
using System.Linq;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis.Lowering
{
	internal sealed class Lowerer : BoundTreeRewriter
	{
		private int _labelCount;

		private Lowerer()
		{
		}

		private LabelSymbol GenerateLabel()
		{
			var name = $"Label{++_labelCount}";
			return new LabelSymbol(name);
		}

		public static BoundStatement Lower(BoundStatement statement)
		{
			var lowerer = new Lowerer();
			var result = lowerer.RewriteStatement(statement);
			return result;
			//            return Flatten(result);
		}

		private static BoundBlockStatement Flatten(BoundStatement statement)
		{
			var builder = new List<BoundStatement>();
			var stack = new Stack<BoundStatement>();
			stack.Push(statement);

			while (stack.Count > 0)
			{
				var current = stack.Pop();

				if (current is BoundBlockStatement block)
				{
					foreach (var s in block.Statements.Reverse())
						stack.Push(s);
				}
				else
				{
					builder.Add(current);
				}
			}

			return new BoundBlockStatement(builder.ToArray());
		}

		//protected override BoundStatement RewriteForStatement(BoundForStatement node)
		//{
		//    // for <var> = <lower> to <upper>
		//    //      <body>
		//    //
		//    // ---->
		//    //
		//    // {
		//    //      var <var> = <lower>
		//    //      let upperBound = <upper>
		//    //      while (<var> <= upperBound)
		//    //      {
		//    //          <body>
		//    //          <var> = <var> + 1
		//    //      }   
		//    // }

		//    var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);
		//    var variableExpression = new BoundVariableExpression(node.Variable);
		//    var upperBoundSymbol = new VariableSymbol("upperBound", true, TypeSymbol.Integer);
		//    var upperBoundDeclaration = new BoundVariableDeclaration(upperBoundSymbol, node.UpperBound);
		//    var condition = new BoundBinaryExpression(
		//        variableExpression,
		//        BoundBinaryOperator.Bind(SyntaxKind.LessOrEqualsToken, TypeSymbol.Integer, TypeSymbol.Integer),
		//        new BoundVariableExpression(upperBoundSymbol)
		//    );
		//    var increment = new BoundExpressionStatement(
		//        new BoundAssignmentExpression(
		//            node.Variable,
		//            new BoundBinaryExpression(
		//                variableExpression,
		//                BoundBinaryOperator.Bind(SyntaxKind.PlusToken, TypeSymbol.Integer, TypeSymbol.Integer),
		//                new BoundLiteralExpression(1)
		//            )
		//        )
		//    );
		//    var whileBody = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(node.Body, increment));
		//    var whileStatement = new BoundWhileStatement(condition, whileBody);
		//    var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
		//        variableDeclaration,
		//        upperBoundDeclaration,
		//        whileStatement
		//    ));

		//    return RewriteStatement(result);
		//}
	}
}