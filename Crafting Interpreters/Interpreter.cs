using Crafting_Interpreters.Errors;
using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crafting_Interpreters
{
    internal class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        private Environment environment = new Environment();
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach(Stmt stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nil";
            if (obj is double)
            {
                string text = obj.ToString();
                if (text.EndsWith(".0"))
                {
                    text = text.Substring(0, text.Length - 2);  // Remove the ".0" from the end
                }
                return text;
            }
            return obj.ToString();
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            switch (expr.Operator.Type)
            {
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
                case TokenType.GREATER:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    // "+" Can mean either the values combined or concatenate the strings
                    if (left is double && right is double) return (double)left + (double)right;
                    if (left is string && right is string) return (string)left + (string)right;
                    throw new RuntimeError(expr.Operator,"Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;
            }

            // Unreachable.
            return null;
        }


        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }


        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evaluate(expr.Right);
            switch (expr.Operator.Type)
            {
                case TokenType.BANG:
                    return !IsTruthy(right);

                case TokenType.MINUS:
                    CheckNumberOperand(expr.Operator, right);

                    return -(double)right;

            }
            // Unreachable.
            return null;

        }

        private void CheckNumberOperand(Token @operator, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(@operator, "Operand must be a number.");

        }
        private void CheckNumberOperands(Token @operator, object left, object right)
        {
            if (left is Double && right is double) return;

            throw new RuntimeError(@operator, "Operands must be numbers.");

        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }


        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);

        }
        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }
        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }

        object? Stmt.Visitor<object>.VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt._expression);
            return null;
        }

        object? Stmt.Visitor<object>.VisitPrintStmt(Stmt.Print stmt)
        {
            object value = Evaluate(stmt._expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        object? Stmt.Visitor<object>.VisitVarStmt(Stmt.Var stmt)
        {
            object value = null;
            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.Lexeme, value);
            return null;
        }

        object? Expr.Visitor<object>.VisitVariableExpr(Expr.Variable expr)
        {
            return environment.Get(expr.name);
        }

        object? Expr.Visitor<object>.VisitAssignExpr(Expr.Assign expr)
        {
            object value = Evaluate(expr.value);
            environment.Assign(expr.name, value);
            return value;
        }

        object? Stmt.Visitor<object>.VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

        private void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            Environment previous = this.environment;
            try
            {
                this.environment = environment;

                foreach (Stmt stmt in statements)
                {
                    Execute(stmt);
                }
            }
            finally
            {
                this.environment = previous;
            }

        }
    }
}
