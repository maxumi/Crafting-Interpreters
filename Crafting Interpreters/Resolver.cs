using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CraftingInterpreters.Lox.Expr;
using static System.Formats.Asn1.AsnWriter;

namespace Crafting_Interpreters
{

    public class Resolver : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        private enum FunctionType
        {
            NONE,
            FUNCTION
        }
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> _scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.NONE;
        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        object? Expr.Visitor<object>.VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.name);
            return null;
        }

        object? Expr.Visitor<object>.VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        object? Stmt.Visitor<object>.VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.statements);
            EndScope();
            return null;
        }

        private void EndScope()
        {
            _scopes.Pop();
        }

        public void Resolve(List<Stmt> statements)
        {
            foreach (Stmt statement in statements)
            {
                Resolve(statement);
            }
        }
        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }
        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void BeginScope()
        {
            _scopes.Push(new Dictionary<string, bool>());
        }

        object? Expr.Visitor<object>.VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.callee);

            foreach(Expr argument in expr.arguments)
            {
                Resolve(argument);
            }

            return null;
        }

        object? Stmt.Visitor<object>.VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt._expression);
            return null;

        }

        object? Stmt.Visitor<object>.VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.name);
            Define(stmt.name);

            ResolveFunction(stmt, FunctionType.FUNCTION);
            return null;
        }
        private void ResolveFunction(Stmt.Function function, FunctionType type)
        {
            FunctionType enclosingFunction = currentFunction;
            currentFunction = type;
            BeginScope();
            foreach(Token param in function.Params)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.body);
            EndScope();
            currentFunction = enclosingFunction;

        }

        object? Expr.Visitor<object>.VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;

        }

        object? Stmt.Visitor<object>.VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.thenBranch);
            if (stmt.elseBranch != null) Resolve(stmt.elseBranch);
            return null;
        }

        object? Expr.Visitor<object>.VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        object? Expr.Visitor<object>.VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.left);
            Resolve(expr.right);
            return null;

        }

        object? Stmt.Visitor<object>.VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt._expression);
            return null;
        }

        object? Stmt.Visitor<object>.VisitReturnStmt(Stmt.Return stmt)
        {
            if (currentFunction == FunctionType.NONE)
            {
                Lox.Error(stmt.keyword, "Can't return from top-level code.");
            }

            if (stmt.value != null)
            {
                Resolve(stmt.value);
            }
            return null;

        }

        object? Expr.Visitor<object>.VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return null;

        }

        object? Expr.Visitor<object>.VisitVariableExpr(Expr.Variable expr)
        {
            if (_scopes.Count != 0 && _scopes.Peek().TryGetValue(expr.name.Lexeme, out bool value) && value == false)
            {
                Lox.Error(expr.name, "Can't read local variable in its own initializer.");
            }

            ResolveLocal(expr, expr.name);
            return null;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = _scopes.Count - 1; i >= 0; i--)
            {
                if (_scopes.ToArray()[i].ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, _scopes.Count - 1 - i);
                    return;
                }
            }
        }

        object? Stmt.Visitor<object>.VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.name);
            if (stmt.initializer != null)
            {
                Resolve(stmt.initializer);
            }
            Define(stmt.name);
            return null;
        }
        private void Declare(Token name)
        {
            if (_scopes.Count == 0) return;

            Dictionary<string, bool> scope = _scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, "Already a variable with this name in this scope.");
            }

            scope[name.Lexeme] = false;
        }
        private void Define(Token name)
        {
            if (_scopes.Count == 0) return;

            Dictionary<string, bool> scope = _scopes.Peek();
            scope[name.Lexeme] = true;

        }

        object? Stmt.Visitor<object>.VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.condition);
            Resolve(stmt.body);
            return null;
        }
    }
}
