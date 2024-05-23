using Crafting_Interpreters.AST;
using Crafting_Interpreters.Classes;
using Crafting_Interpreters.Errors;
using Crafting_Interpreters.interfaces;
using Crafting_Interpreters.LoxFunctions;
using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Crafting_Interpreters.AST.Stmt;

namespace Crafting_Interpreters.Interpreters
{
    /// <summary>
    /// The Interpreter class that visits and evaluates expressions and executes statements.
    /// </summary>
    public class Interpreter : Expr.Visitor<object>, Visitor<object>
    {
        /// <summary>
        /// A dictionary to track the scope depth of local variables.
        /// </summary>
        private readonly Dictionary<Expr, int> locals = new Dictionary<Expr, int>();

        /// <summary>
        /// The global environment which holds global variables and function definitions.
        /// </summary>
        public readonly Environment globals = new Environment();

        /// <summary>
        /// The current environment for variable scope.
        /// </summary>
        private Environment environment;

        public Interpreter()
        {
            // Initially, set the environment to the global environment.
            environment = globals;

            // A predefined function that gives the current time in seconds since the Unix epoch.
            globals.Define("clock", new Clock());
        }

        /// <summary>
        /// Interprets a list of statements.
        /// </summary>
        /// <param name="statements">The list of statements to interpret.</param>
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        /// <summary>
        /// Converts an object to its string representation.
        /// </summary>
        /// <param name="obj">The object to stringify.</param>
        /// <returns>The string representation of the object.</returns>
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

        /// <summary>
        /// Visits and evaluates a binary expression.
        /// </summary>
        /// <param name="expr">The binary expression to evaluate.</param>
        /// <returns>The result of the binary expression.</returns>
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
                    throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");
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

        /// <summary>
        /// Visits and evaluates a grouping expression.
        /// </summary>
        /// <param name="expr">The grouping expression to evaluate.</param>
        /// <returns>The result of the grouping expression.</returns>
        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        /// <summary>
        /// Visits and evaluates a literal expression.
        /// </summary>
        /// <param name="expr">The literal expression to evaluate.</param>
        /// <returns>The value of the literal expression.</returns>
        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        /// <summary>
        /// Visits and evaluates a unary expression.
        /// </summary>
        /// <param name="expr">The unary expression to evaluate.</param>
        /// <returns>The result of the unary expression.</returns>
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

        /// <summary>
        /// Checks if an operand is a number.
        /// </summary>
        /// <param name="operator">The operator token.</param>
        /// <param name="operand">The operand to check.</param>
        private void CheckNumberOperand(Token @operator, object operand)
        {
            if (operand is double) return;
            throw new RuntimeError(@operator, "Operand must be a number.");
        }

        /// <summary>
        /// Checks if both operands are numbers.
        /// </summary>
        /// <param name="operator">The operator token.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        private void CheckNumberOperands(Token @operator, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(@operator, "Operands must be numbers.");
        }

        /// <summary>
        /// Determines if an object is truthy.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if the object is truthy, false otherwise.</returns>
        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        /// <summary>
        /// Evaluates an expression.
        /// </summary>
        /// <param name="expr">The expression to evaluate.</param>
        /// <returns>The result of the expression.</returns>
        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        /// <summary>
        /// Executes a statement.
        /// </summary>
        /// <param name="stmt">The statement to execute.</param>
        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        /// <summary>
        /// Stores the depth of a local variable.
        /// </summary>
        /// <param name="expr">The expression for the variable.</param>
        /// <param name="depth">The depth of the variable in the environment chain.</param>
        public void Resolve(Expr expr, int depth)
        {
            locals[expr] = depth;
        }

        /// <summary>
        /// Determines if two objects are equal.
        /// </summary>
        /// <param name="a">The first object.</param>
        /// <param name="b">The second object.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        private bool IsEqual(object a, object b)
        {
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        /// <summary>
        /// Visits and executes an expression statement.
        /// </summary>
        /// <param name="stmt">The expression statement to execute.</param>
        /// <returns>Null.</returns>
        object? Visitor<object>.VisitExpressionStmt(Expression stmt)
        {
            Evaluate(stmt._expression);
            return null;
        }

        /// <summary>
        /// Visits and executes a print statement.
        /// </summary>
        /// <param name="stmt">The print statement to execute.</param>
        /// <returns>Null.</returns>
        object? Visitor<object>.VisitPrintStmt(Print stmt)
        {
            object value = Evaluate(stmt._expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        /// <summary>
        /// Visits and executes a variable declaration statement.
        /// </summary>
        /// <param name="stmt">The variable declaration statement to execute.</param>
        /// <returns>Null.</returns>
        object? Visitor<object>.VisitVarStmt(Var stmt)
        {
            object value = null;
            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }

            environment.Define(stmt.name.Lexeme, value);
            return null;
        }

        /// <summary>
        /// Visits and evaluates a variable expression.
        /// </summary>
        /// <param name="expr">The variable expression to evaluate.</param>
        /// <returns>The value of the variable.</returns>
        object? Expr.Visitor<object>.VisitVariableExpr(Expr.Variable expr)
        {
            return LookUpVariable(expr.name, expr);
        }

        /// <summary>
        /// Looks up a variable in the appropriate environment.
        /// </summary>
        /// <param name="name">The name token of the variable.</param>
        /// <param name="expr">The expression for the variable.</param>
        /// <returns>The value of the variable.</returns>
        private object? LookUpVariable(Token name, Expr expr)
        {
            if (locals.TryGetValue(expr, out int distance))
            {
                return environment.GetAt(distance, name.Lexeme);
            }
            else
            {
                return globals.Get(name);
            }
        }

        /// <summary>
        /// Visits and evaluates an assignment expression.
        /// </summary>
        /// <param name="expr">The assignment expression to evaluate.</param>
        /// <returns>The assigned value.</returns>
        object? Expr.Visitor<object>.VisitAssignExpr(Expr.Assign expr)
        {
            object value = Evaluate(expr.value);

            if (locals.TryGetValue(expr, out int distance))
            {
                environment.AssignAt(distance, expr.name, value);
            }
            else
            {
                globals.Assign(expr.name, value);
            }
            return value;
        }

        /// <summary>
        /// Visits and executes a block statement.
        /// </summary>
        /// <param name="stmt">The block statement to execute.</param>
        /// <returns>Null.</returns>
        object? Visitor<object>.VisitBlockStmt(Block stmt)
        {
            ExecuteBlock(stmt.statements, new Environment(environment));
            return null;
        }

        /// <summary>
        /// Executes a block of statements with a new environment.
        /// </summary>
        /// <param name="statements">The list of statements to execute.</param>
        /// <param name="environment">The new environment for the block.</param>
        public void ExecuteBlock(List<Stmt> statements, Environment environment)
        {
            // this contains the previous environment reference and holds it for now.
            Environment previous = this.environment;
            try
            {
                // Set the current environment to the new environment for executing the block.
                this.environment = environment;

                // Execute each statement in the block using the new environment.
                foreach (Stmt stmt in statements)
                {
                    Execute(stmt);
                }
            }
            finally
            {
                // this will then return to the previous environment so it goes back to the scope of before.
                this.environment = previous;
            }
        }

        /// <summary>
        /// Visits and executes an if statement.
        /// </summary>
        /// <param name="stmt">The if statement to execute.</param>
        /// <returns>Null.</returns>
        object? Visitor<object>.VisitIfStmt(If stmt)
        {
            if (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.thenBranch);
            }
            else if (stmt.elseBranch != null)
            {
                Execute(stmt.elseBranch);
            }
            return null;
        }

        /// <summary>
        /// Visits and evaluates a logical expression.
        /// </summary>
        /// <param name="expr">The logical expression to evaluate.</param>
        /// <returns>The result of the logical expression.</returns>
        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            object left = Evaluate(expr.left);

            if (expr._operator.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(expr.right);
        }

        /// <summary>
        /// Visits and executes a while statement.
        /// </summary>
        /// <param name="stmt">The while statement to execute.</param>
        /// <returns>Null.</returns>
        object? Visitor<object>.VisitWhileStmt(While stmt)
        {
            while (IsTruthy(Evaluate(stmt.condition)))
            {
                Execute(stmt.body);
            }
            return null;
        }

        /// <summary>
        /// Visits and evaluates a call expression.
        /// </summary>
        /// <param name="expr">The call expression to evaluate.</param>
        /// <returns>The result of the call.</returns>
        object? Expr.Visitor<object>.VisitCallExpr(Expr.Call expr)
        {
            object callee = Evaluate(expr.callee);

            List<object> arguments = new();
            foreach (Expr argument in expr.arguments)
            {
                arguments.Add(Evaluate(argument));
            }

            if (!(callee is ICallable))
            {
                throw new RuntimeError(expr.paren, "Can only call functions and classes.");
            }

            ICallable function = (ICallable)callee;
            if (arguments.Count() != function.Arity())
            {
                throw new RuntimeError(expr.paren, "Expected " +
                    function.Arity() + " arguments but got " +
                    arguments.Count() + ".");
            }
            return function.Call(this, arguments);
        }

        /// <summary>
        /// Visits and executes a function declaration statement.
        /// </summary>
        /// <param name="stmt">The function statement to execute.</param>
        /// <returns>Null.</returns>
        object? Visitor<object>.VisitFunctionStmt(Function stmt)
        {
            LoxFunction function = new LoxFunction(stmt, environment, false);
            environment.Define(stmt.name.Lexeme, function);
            return null;
        }

        /// <summary>
        /// Visits and executes a return statement.
        /// </summary>
        /// <param name="stmt">The return statement to execute.</param>
        /// <returns>Throws a return exception with the return value.</returns>
        object? Visitor<object>.VisitReturnStmt(Stmt.Return stmt)
        {
            object value = null;
            if (stmt.value != null) value = Evaluate(stmt.value);

            throw new Errors.Return(value);
        }

        /// <summary>
        /// Visits and executes a class declaration statement.
        /// </summary>
        /// <param name="stmt">The class statement to execute.</param>
        /// <returns>Null.</returns>
        object? Visitor<object>.VisitClassStmt(Class stmt)
        {
            object superclass = null;
            if (stmt.superclass != null)
            {
                superclass = Evaluate(stmt.superclass);
                if (!(superclass is LoxClass))
                {
                    throw new RuntimeError(stmt.superclass.name, "Superclass must be a class.");
                }
            }
            // Define the class name in the current environment
            environment.Define(stmt.name.Lexeme, null);
            if (stmt.superclass != null)
            {
                environment = new Environment(environment);
                environment.Define("super", superclass);
            }

            Dictionary<string, LoxFunction> methods = new();

            foreach (Stmt.Function method in stmt.methods)
            {
                LoxFunction function = new LoxFunction(method, environment, method.name.Lexeme.Equals("init"));
                methods[method.name.Lexeme] = function;
            }
            LoxClass klass = new LoxClass(stmt.name.Lexeme, (LoxClass)superclass, methods);

            if (superclass != null)
            {
                environment = environment.Enclosing;
            }

            // Assign the LoxClass instance to the class name in the environment
            environment.Assign(stmt.name, klass);
            return null;
        }

        /// <summary>
        /// Visits and evaluates a get expression (property access).
        /// </summary>
        /// <param name="expr">The get expression to evaluate.</param>
        /// <returns>The value of the property.</returns>
        object Expr.Visitor<object>.VisitGetExpr(Expr.Get expr)
        {
            object _object = Evaluate(expr.Object);
            if (_object is LoxInstance)
            {
                return ((LoxInstance)_object).Get(expr.name);
            }

            throw new RuntimeError(expr.name, "Only instances have properties.");
        }

        /// <summary>
        /// Visits and evaluates a set expression (property assignment).
        /// </summary>
        /// <param name="expr">The set expression to evaluate.</param>
        /// <returns>The assigned value.</returns>
        object Expr.Visitor<object>.VisitSetExpr(Expr.Set expr)
        {
            object _object = Evaluate(expr.Object);

            if (!(_object is LoxInstance))
            {
                throw new RuntimeError(expr.name, "Only instances have fields.");
            }

            object value = Evaluate(expr.value);
            ((LoxInstance)_object).Set(expr.name, value);
            return value;
        }

        /// <summary>
        /// Visits and evaluates a 'this' expression.
        /// </summary>
        /// <param name="expr">The 'this' expression to evaluate.</param>
        /// <returns>The value of 'this'.</returns>
        object? Expr.Visitor<object>.VisitThisExpr(Expr.This expr)
        {
            return LookUpVariable(expr.keyword, expr);
        }

        /// <summary>
        /// Visits and evaluates a 'super' expression.
        /// </summary>
        /// <param name="expr">The 'super' expression to evaluate.</param>
        /// <returns>The method from the superclass bound to the current instance.</returns>
        object? Expr.Visitor<object>.VisitSuperExpr(Expr.Super expr)
        {
            int distance = locals[expr];
            LoxClass superclass = (LoxClass)environment.GetAt(distance, "super");

            // "this" is always one level nearer than "super"'s environment.
            LoxInstance _object = (LoxInstance)environment.GetAt(distance - 1, "this");

            LoxFunction method = superclass.FindMethod(expr.method.Lexeme);

            if (method == null)
            {
                throw new RuntimeError(expr.method, "Undefined property '" + expr.method.Lexeme + "'.");
            }

            return method.Bind(_object);
        }
    }
}
