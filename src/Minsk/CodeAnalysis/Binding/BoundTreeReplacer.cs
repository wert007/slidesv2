using Minsk.CodeAnalysis.Symbols;
using System;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Binding
{
	internal static class BoundTreeReplacer
   {
      public static BoundStatement RewriteStatement(BoundStatement statement, BoundNode source, BoundNode replacement)
      {
         if (statement == source)
            return (BoundStatement)replacement;
         switch (statement.Kind)
         {
            case BoundNodeKind.BlockStatement:
               return RewriteBlockStatement((BoundBlockStatement)statement, source, replacement);
            case BoundNodeKind.VariableDeclaration:
               return RewriteVariableDeclaration((BoundVariableDeclaration)statement, source, replacement);   
            case BoundNodeKind.ForStatement:
               return RewriteForStatement((BoundForStatement)statement, source, replacement);
            case BoundNodeKind.ExpressionStatement:
               return RewriteExpressionStatement((BoundExpressionStatement)statement, source, replacement);
            case BoundNodeKind.IfStatement:
               return RewriteIfStatement((BoundIfStatement)statement, source, replacement);
            default:
               throw new NotImplementedException();
         }
      }


      private static BoundExpression RewriteExpression(BoundExpression expression, BoundNode source, BoundNode replacement)
      {
         if (expression == source)
            return (BoundExpression)replacement;
         switch (expression.Kind)
         {
            case BoundNodeKind.EnumExpression:
            case BoundNodeKind.MathExpression:
            case BoundNodeKind.LiteralExpression:
               return expression;
            case BoundNodeKind.VariableExpression:
               return RewriteVariableExpression((BoundVariableExpression)expression, source, replacement);
            case BoundNodeKind.AssignmentExpression:
               return RewriteAssignmentExpression((BoundAssignmentExpression)expression, source, replacement);
            case BoundNodeKind.UnaryExpression:
               return RewriteUnaryExpression((BoundUnaryExpression)expression, source, replacement);
            case BoundNodeKind.BinaryExpression:
               return RewriteBinaryExpression((BoundBinaryExpression)expression, source, replacement);
            case BoundNodeKind.FunctionExpression:
               return RewriteFunctionExpression((BoundFunctionExpression)expression, source, replacement);
            case BoundNodeKind.ArrayExpression:
               return RewriteArrayExpression((BoundArrayExpression)expression, source, replacement);
            case BoundNodeKind.FunctionAccessExpression:
               return RewriteFunctionAccessExpression((BoundFunctionAccessExpression)expression, source, replacement);
            case BoundNodeKind.FieldAccessExpression:
               return RewriteFieldAccessExpression((BoundFieldAccessExpression)expression, source, replacement);
            case BoundNodeKind.StringExpression:
               return RewriteStringExpression((BoundStringExpression)expression, source, replacement);
            case BoundNodeKind.ConversionExpression:
               return RewriteConversion((BoundConversion)expression, source, replacement);
            case BoundNodeKind.EmptyArrayConstructorExpression:
               return RewriteEmptyArrayConstructorExpression((BoundEmptyArrayConstructorExpression)expression, source, replacement);
            default:
               throw new NotImplementedException();
         }
      }

      private static BoundBlockStatement RewriteBlockStatement(BoundBlockStatement statement, BoundNode source, BoundNode replacement)
      {
         if (statement == source)
            return (BoundBlockStatement)replacement;
         var statements = new List<BoundStatement>();
         foreach (var s in statement.Statements)
            statements.Add(RewriteStatement(s, source, replacement));
         return new BoundBlockStatement(statements.ToArray());
      }

      private static BoundVariableDeclaration RewriteVariableDeclaration(BoundVariableDeclaration statement, BoundNode source, BoundNode replacement)
      {
         if (statement == source)
            return (BoundVariableDeclaration)replacement;
         var initializer = RewriteExpression(statement.Initializer, source, replacement);
         return new BoundVariableDeclaration(statement.Variable, initializer);
      }

      private static BoundForStatement RewriteForStatement(BoundForStatement statement, BoundNode source, BoundNode replacement)
      {
         if (statement == source)
            return (BoundForStatement)replacement;
         var collection = RewriteExpression(statement.Collection, source, replacement);
         var body = RewriteBlockStatement(statement.Body, source, replacement);
         return new BoundForStatement(statement.Variable, statement.OptionalIndexer, collection, body);
      }

      private static BoundExpressionStatement RewriteExpressionStatement(BoundExpressionStatement statement, BoundNode source, BoundNode replacement)
      {
         if (statement == source)
            return (BoundExpressionStatement)replacement;
         var expression = RewriteExpression(statement.Expression, source, replacement);
         return new BoundExpressionStatement(expression);
      }

      private static BoundStatement RewriteIfStatement(BoundIfStatement statement, BoundNode source, BoundNode replacement)
      {
         if (statement == source)
            return (BoundStatement)replacement;
         var condition = RewriteExpression(statement.Condition, source, replacement);
         var body = RewriteStatement(statement.Body, source, replacement);
         var elseStatement = RewriteStatement(statement.Else, source, replacement);
         return new BoundIfStatement(condition, body, elseStatement);
      }

      private static BoundExpression RewriteVariableExpression(BoundVariableExpression expression, BoundNode source, BoundNode replacement)
      {
         if (expression == source)
            return (BoundExpression)replacement;
         return new BoundVariableExpression(expression.Variable);
      }

      private static BoundArrayAccessExpression RewriteArrayIndex(BoundArrayAccessExpression arrayIndex, BoundNode source, BoundNode replacement)
      {
         if (arrayIndex == null)
            return null;
         var index = RewriteExpression(arrayIndex.Index, source, replacement);
         var child = RewriteExpression(arrayIndex.Child, source, replacement);
         return new BoundArrayAccessExpression(index, child);
      }

      private static BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression expression, BoundNode source, BoundNode replacement)
      {
         if (expression == source)
            return (BoundExpression)replacement;
         var lvalue = RewriteExpression(expression.LValue, source, replacement);
         var e = RewriteExpression(expression.Expression, source, replacement);
         return new BoundAssignmentExpression(lvalue, e);
      }

      private static BoundExpression RewriteUnaryExpression(BoundUnaryExpression expression, BoundNode source, BoundNode replacement)
      {
         if (expression == source)
            return (BoundExpression)replacement;
         var operand = RewriteExpression(expression.Operand, source, replacement);
         //TODO
         //var op = BoundUnaryOperator.Bind(expression.Operand.S)
         return new BoundUnaryExpression(expression.Op, operand);
      }

      private static BoundExpression RewriteBinaryExpression(BoundBinaryExpression expression, BoundNode source, BoundNode replacement)
      {
         if (expression == source)
            return (BoundExpression)replacement;

         var left = RewriteExpression(expression.Left, source, replacement);
         var right = RewriteExpression(expression.Right, source, replacement);
         return new BoundBinaryExpression(left, expression.Op, right);
      }

      private static BoundExpression RewriteFunctionExpression(BoundFunctionExpression expression, BoundNode source, BoundNode replacement)
      {

         if (expression == source)
            return (BoundExpression)replacement;

         var arguments = new List<BoundExpression>();
         foreach (var a in expression.Arguments)
         {
            arguments.Add(RewriteExpression(a, source, replacement));
         }

         return new BoundFunctionExpression(expression.Function, arguments.ToArray(), expression.Source);
      }

      private static BoundExpression RewriteArrayExpression(BoundArrayExpression expression, BoundNode source, BoundNode replacement)
      {

         if (expression == source)
            return (BoundExpression)replacement;

         var expressions = new List<BoundExpression>();
         foreach (var e in expression.Expressions)
         {
            expressions.Add(RewriteExpression(e, source, replacement));
         }
         return new BoundArrayExpression(expressions.ToArray(), expression.BaseType);
      }

      private static BoundExpression RewriteFunctionAccessExpression(BoundFunctionAccessExpression expression, BoundNode source, BoundNode replacement)
      {
         if (expression == source)
            return (BoundExpression)replacement;

         var parent = RewriteExpression(expression.Parent, source, replacement);
         //TODO: Error prone
         var function = (BoundFunctionExpression)RewriteFunctionExpression(expression.FunctionCall, source, replacement);
         return new BoundFunctionAccessExpression(parent, function);
      }

      private static BoundExpression RewriteFieldAccessExpression(BoundFieldAccessExpression expression, BoundNode source, BoundNode replacement)
      {

         if (expression == source)
            return (BoundExpression)replacement;

         var parent = RewriteExpression(expression.Parent, source, replacement);
         //TODO: Errorprone
         var field = (BoundVariableExpression)RewriteVariableExpression(expression.Field, source, replacement);
         return new BoundFieldAccessExpression(parent, field);
      }

      private static BoundExpression RewriteStringExpression(BoundStringExpression expression, BoundNode source, BoundNode replacement)
      {

         if (expression == source)
            return (BoundExpression)replacement;

         var expressions = new List<BoundExpression>();
         foreach (var e in expression.Expressions)
         {
            expressions.Add(RewriteExpression(e, source, replacement));
         }

         return new BoundStringExpression(expressions.ToArray());
      }

      private static BoundExpression RewriteConversion(BoundConversion expression, BoundNode source, BoundNode replacement)
      {

         if (expression == source)
            return (BoundExpression)replacement;

         var e = RewriteExpression(expression.Expression, source, replacement);
         return new BoundConversion(e, expression.Type);
      }

      private static BoundExpression RewriteEmptyArrayConstructorExpression(BoundEmptyArrayConstructorExpression expression, BoundNode source, BoundNode replacement)
      {
         if (expression == source)
            return (BoundExpression)replacement;

         var length = RewriteExpression(expression.Length, source, replacement);
         return new BoundEmptyArrayConstructorExpression(length, expression.Type);
      }
   }
}
