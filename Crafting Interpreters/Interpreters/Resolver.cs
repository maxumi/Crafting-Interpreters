using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CraftingInterpreters.Lox.Expr;
using static System.Formats.Asn1.AsnWriter;

namespace Crafting_Interpreters.Interpreters
{

    public class Resolver : Visitor<object>, Stmt.Visitor<object>
    {
        private enum FunctionType
        {
            NONE,
            FUNCTION,
            INITIALIZER,
            METHOD
        }
        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }

        private ClassType currentClass = ClassType.NONE;
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> _scopes = new Stack<Dictionary<string, bool>>();
        private FunctionType currentFunction = FunctionType.NONE;
        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        object? Visitor<object>.VisitAssignExpr(Assign expr)
        {
            Resolve(expr.value);
            ResolveLocal(expr, expr.name);
            return null;
        }

        object? Visitor<object>.VisitBinaryExpr(Binary expr)
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

        object? Visitor<object>.VisitCallExpr(Call expr)
        {
            Resolve(expr.callee);

            foreach (Expr argument in expr.arguments)
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
            foreach (Token param in function.Params)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.body);
            EndScope();
            currentFunction = enclosingFunction;

        }

        object? Visitor<object>.VisitGroupingExpr(Grouping expr)
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

        object? Visitor<object>.VisitLiteralExpr(Literal expr)
        {
            return null;
        }

        object? Visitor<object>.VisitLogicalExpr(Logical expr)
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
                if (currentFunction == FunctionType.INITIALIZER)
                {
                    Lox.Error(stmt.keyword,"Can't return a value from an initializer.");
                }
                Resolve(stmt.value);
            }
            return null;

        }

        object? Visitor<object>.VisitUnaryExpr(Unary expr)
        {
            Resolve(expr.Right);
            return null;

        }

        object? Visitor<object>.VisitVariableExpr(Variable expr)
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
                if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, i);
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

        object? Stmt.Visitor<object>.VisitClassStmt(Stmt.Class stmt)
        {
            ClassType enclosingClass = currentClass;
            currentClass = ClassType.CLASS;

            Declare(stmt.name);
            Define(stmt.name);

            if (stmt.superclass != null && stmt.name.Lexeme.Equals(stmt.superclass.name.Lexeme))
            {
                Lox.Error(stmt.superclass.name, "A class can't inherit from itself.");
            }

            if (stmt.superclass != null)
            {
                currentClass = ClassType.SUBCLASS;
                Resolve(stmt.superclass);
            }

            if (stmt.superclass != null)
            {
                BeginScope();
                _scopes.Peek().Add("super", true);
            }

            BeginScope();
            _scopes.Peek().Add("this", true);

            foreach (Stmt.Function method in stmt.methods)
            {
                FunctionType declaration = FunctionType.METHOD;
                if (method.name.Lexeme.Equals("init"))
                {
                    declaration = FunctionType.INITIALIZER;
                }

                ResolveFunction(method, declaration);
            }
            EndScope();

            if (stmt.superclass != null) EndScope();

            currentClass = enclosingClass;
            return null;
        }

        object Visitor<object>.VisitGetExpr(Get expr)
        {
            Resolve(expr.Object);
            return null;
        }

        object Visitor<object>.VisitSetExpr(Set expr)
        {
            Resolve(expr.value);
            Resolve(expr.Object);
            return null;
        }

        object? Visitor<object>.VisitThisExpr(This expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Lox.Error(expr.keyword,"Can't use 'this' outside of a class.");
                return null;
            }

            ResolveLocal(expr, expr.keyword);
            return null;
        }

        object? Visitor<object>.VisitSuperExpr(Super expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Lox.Error(expr.keyword,"Can't use 'super' outside of a class.");
            }
            else if (currentClass != ClassType.SUBCLASS)
            {
                Lox.Error(expr.keyword, "Can't use 'super' in a class with no superclass.");
            }
            ResolveLocal(expr, expr.keyword);
            return null;
        }
    }
}
