using System.Collections.Generic;
using CraftingInterpreters.Lox;

namespace Crafting_Interpreters.AST
{
    /// <summary>
    /// Represents the base class for all statement nodes in the AST (Abstract Syntax Tree).
    /// </summary>
    public abstract class Stmt
    {
        /// <summary>
        /// Interface for visitor pattern to process different types of statements.
        /// </summary>
        /// <typeparam name="R">The return type of the visitor methods.</typeparam>
        public interface Visitor<R>
        {
            R? VisitBlockStmt(Block stmt);
            R? VisitClassStmt(Class stmt);
            R? VisitExpressionStmt(Expression stmt);
            R? VisitFunctionStmt(Function stmt);
            R? VisitIfStmt(If stmt);
            R? VisitPrintStmt(Print stmt);
            R? VisitReturnStmt(Return stmt);
            R? VisitVarStmt(Var stmt);
            R? VisitWhileStmt(While stmt);
        }

        /// <summary>
        /// Represents a block statement, which is a list of statements.
        /// </summary>
        public class Block : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Block"/> class with a list of statements.
            /// </summary>
            /// <param name="statements">The list of statements in the block.</param>
            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            /// <summary>
            /// Accepts a visitor to process this block statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this block statement.</param>
            /// <returns>The result of processing the block statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }

            public readonly List<Stmt> statements;
        }

        /// <summary>
        /// Represents a class declaration statement.
        /// </summary>
        public class Class : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Class"/> class with a name, superclass, and methods.
            /// </summary>
            /// <param name="name">The name of the class.</param>
            /// <param name="superclass">The superclass of the class.</param>
            /// <param name="methods">The methods of the class.</param>
            public Class(Token name, Expr.Variable superclass, List<Function> methods)
            {
                this.name = name;
                this.superclass = superclass;
                this.methods = methods;
            }

            /// <summary>
            /// Accepts a visitor to process this class declaration statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this class declaration statement.</param>
            /// <returns>The result of processing the class declaration statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitClassStmt(this);
            }

            public readonly Token name;
            public readonly Expr.Variable superclass;
            public readonly List<Function> methods;
        }

        /// <summary>
        /// Represents an expression statement.
        /// </summary>
        public class Expression : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Expression"/> class with an expression.
            /// </summary>
            /// <param name="expression">The expression to evaluate.</param>
            public Expression(Expr expression)
            {
                _expression = expression;
            }

            /// <summary>
            /// Accepts a visitor to process this expression statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this expression statement.</param>
            /// <returns>The result of processing the expression statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }

            public readonly Expr _expression;
        }

        /// <summary>
        /// Represents a function declaration statement.
        /// </summary>
        public class Function : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Function"/> class with a name, parameters, and body.
            /// </summary>
            /// <param name="name">The name of the function.</param>
            /// <param name="Params">The parameters of the function.</param>
            /// <param name="body">The body of the function.</param>
            public Function(Token name, List<Token> Params, List<Stmt> body)
            {
                this.name = name;
                this.Params = Params;
                this.body = body;
            }

            /// <summary>
            /// Accepts a visitor to process this function declaration statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this function declaration statement.</param>
            /// <returns>The result of processing the function declaration statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitFunctionStmt(this);
            }

            public readonly Token name;
            public readonly List<Token> Params;
            public readonly List<Stmt> body;
        }

        /// <summary>
        /// Represents an if statement.
        /// </summary>
        public class If : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="If"/> class with a condition, then branch, and else branch.
            /// </summary>
            /// <param name="condition">The condition to evaluate.</param>
            /// <param name="thenBranch">The statement to execute if the condition is true.</param>
            /// <param name="elseBranch">The statement to execute if the condition is false.</param>
            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            /// <summary>
            /// Accepts a visitor to process this if statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this if statement.</param>
            /// <returns>The result of processing the if statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitIfStmt(this);
            }

            public readonly Expr condition;
            public readonly Stmt thenBranch;
            public readonly Stmt elseBranch;
        }

        /// <summary>
        /// Represents a print statement.
        /// </summary>
        public class Print : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Print"/> class with an expression to print.
            /// </summary>
            /// <param name="expression">The expression to print.</param>
            public Print(Expr expression)
            {
                _expression = expression;
            }

            /// <summary>
            /// Accepts a visitor to process this print statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this print statement.</param>
            /// <returns>The result of processing the print statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }

            public readonly Expr _expression;
        }

        /// <summary>
        /// Represents a return statement.
        /// </summary>
        public class Return : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Return"/> class with a keyword and a return value.
            /// </summary>
            /// <param name="keyword">The return keyword.</param>
            /// <param name="value">The return value.</param>
            public Return(Token keyword, Expr value)
            {
                this.keyword = keyword;
                this.value = value;
            }

            /// <summary>
            /// Accepts a visitor to process this return statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this return statement.</param>
            /// <returns>The result of processing the return statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitReturnStmt(this);
            }

            public readonly Token keyword;
            public readonly Expr value;
        }

        /// <summary>
        /// Represents a variable declaration statement.
        /// </summary>
        public class Var : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Var"/> class with a name and an initializer .
            /// </summary>
            /// <param name="_name">The name of the variable.</param>
            /// <param name="_initializer">The initializer expression for the variable.</param>
            public Var(Token _name, Expr _initializer)
            {
                name = _name;
                initializer = _initializer;
            }

            /// <summary>
            /// Accepts a visitor to process this variable declaration statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this variable declaration statement.</param>
            /// <returns>The result of processing the variable declaration statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVarStmt(this);
            }

            public readonly Token name;
            public readonly Expr initializer;
        }

        /// <summary>
        /// Represents a while statement.
        /// </summary>
        public class While : Stmt
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="While"/> class with a condition and a body.
            /// </summary>
            /// <param name="condition">The condition to evaluate for each iteration.</param>
            /// <param name="body">The body to execute for each iteration.</param>
            public While(Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }

            /// <summary>
            /// Accepts a visitor to process this while statement.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this while statement.</param>
            /// <returns>The result of processing the while statement.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }

            public readonly Expr condition;
            public readonly Stmt body;
        }

        /// <summary>
        /// Abstract method to accept a visitor for processing the statement.
        /// </summary>
        /// <typeparam name="R">The return type of the visitor method.</typeparam>
        /// <param name="visitor">The visitor processing this statement.</param>
        /// <returns>The result of processing the statement.</returns>
        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
