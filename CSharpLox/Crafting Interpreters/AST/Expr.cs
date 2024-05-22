using System.Collections.Generic;
using CraftingInterpreters.Lox;

namespace Crafting_Interpreters.AST
{
    /// <summary>
    /// Represents the base class for all expression nodes in the AST (Abstract Syntax Tree).
    /// </summary>
    public abstract class Expr
    {
        /// <summary>
        /// Interface for visitor pattern to process different types of expressions.
        /// </summary>
        /// <typeparam name="R">The return type of the visitor methods.</typeparam>
        public interface Visitor<R>
        {
            R? VisitAssignExpr(Assign expr);
            R? VisitBinaryExpr(Binary expr);
            R? VisitCallExpr(Call expr);
            R VisitGetExpr(Get expr);
            R? VisitGroupingExpr(Grouping expr);
            R? VisitLiteralExpr(Literal expr);
            R? VisitLogicalExpr(Logical expr);
            R? VisitSetExpr(Set expr);
            R? VisitSuperExpr(Super expr);
            R? VisitThisExpr(This expr);
            R? VisitUnaryExpr(Unary expr);
            R? VisitVariableExpr(Variable expr);
        }

        /// <summary>
        /// Represents an assignment expression.
        /// </summary>
        public class Assign : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Assign"/> class with a name and a value.
            /// </summary>
            /// <param name="name">The name of the variable being assigned.</param>
            /// <param name="value">The value to assign to the variable.</param>
            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            /// <summary>
            /// Accepts a visitor to process this assignment expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this assignment expression.</param>
            /// <returns>The result of processing the assignment expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }

            public readonly Token name;
            public readonly Expr value;
        }

        /// <summary>
        /// Represents a binary expression.
        /// </summary>
        public class Binary : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Binary"/> class with left and right operands and an operator.
            /// </summary>
            /// <param name="left">The left operand.</param>
            /// <param name="operator">The operator.</param>
            /// <param name="right">The right operand.</param>
            public Binary(Expr left, Token _operator, Expr right)
            {
                Left = left;
                Operator = _operator;
                Right = right;
            }

            /// <summary>
            /// Accepts a visitor to process this binary expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this binary expression.</param>
            /// <returns>The result of processing the binary expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }

            public readonly Expr? Left;
            public readonly Token? Operator;
            public readonly Expr? Right;
        }

        /// <summary>
        /// Represents a function or method call expression.
        /// </summary>
        public class Call : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Call"/> class with a callee, parenthesis token, and arguments.
            /// </summary>
            /// <param name="callee">The expression for the function or method being called.</param>
            /// <param name="paren">The parenthesis token.</param>
            /// <param name="arguments">The list of arguments passed to the call.</param>
            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }

            /// <summary>
            /// Accepts a visitor to process this call expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this call expression.</param>
            /// <returns>The result of processing the call expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitCallExpr(this);
            }

            public readonly Expr callee;
            public readonly Token paren;
            public readonly List<Expr> arguments;
        }

        /// <summary>
        /// Represents a property access expression.
        /// </summary>
        public class Get : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Get"/> class with an object and a property name.
            /// </summary>
            /// <param name="Object">The object whose property is being accessed.</param>
            /// <param name="name">The name of the property being accessed.</param>
            public Get(Expr Object, Token name)
            {
                this.Object = Object;
                this.name = name;
            }

            /// <summary>
            /// Accepts a visitor to process this property access expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this property access expression.</param>
            /// <returns>The result of processing the property access expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGetExpr(this);
            }

            public readonly Expr Object;
            public readonly Token name;
        }

        /// <summary>
        /// Represents a grouping expression.
        /// </summary>
        public class Grouping : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Grouping"/> class with an expression.
            /// </summary>
            /// <param name="expression">The expression within the grouping.</param>
            public Grouping(Expr expression)
            {
                Expression = expression;
            }

            /// <summary>
            /// Accepts a visitor to process this grouping expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this grouping expression.</param>
            /// <returns>The result of processing the grouping expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }

            public readonly Expr Expression;
        }

        /// <summary>
        /// Represents a literal value expression.
        /// </summary>
        public class Literal : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Literal"/> class with a value.
            /// </summary>
            /// <param name="value">The literal value.</param>
            public Literal(object value)
            {
                Value = value;
            }

            /// <summary>
            /// Accepts a visitor to process this literal expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this literal expression.</param>
            /// <returns>The result of processing the literal expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }

            public readonly object? Value;
        }

        /// <summary>
        /// Represents a logical expression (e.g., AND, OR).
        /// </summary>
        public class Logical : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Logical"/> class with left and right operands and an operator.
            /// </summary>
            /// <param name="left">The left operand.</param>
            /// <param name="operator">The operator.</param>
            /// <param name="right">The right operand.</param>
            public Logical(Expr left, Token __operator, Expr right)
            {
                this.left = left;
                _operator = __operator;
                this.right = right;
            }

            /// <summary>
            /// Accepts a visitor to process this logical expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this logical expression.</param>
            /// <returns>The result of processing the logical expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }

            public readonly Expr left;
            public readonly Token _operator;
            public readonly Expr right;
        }

        /// <summary>
        /// Represents a property assignment expression.
        /// </summary>
        public class Set : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Set"/> class with an object, property name, and value.
            /// </summary>
            /// <param name="object">The object whose property is being assigned.</param>
            /// <param name="name">The name of the property being assigned.</param>
            /// <param name="value">The value to assign to the property.</param>
            public Set(Expr _object, Token name, Expr value)
            {
                Object = _object;
                this.name = name;
                this.value = value;
            }

            /// <summary>
            /// Accepts a visitor to process this property assignment expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this property assignment expression.</param>
            /// <returns>The result of processing the property assignment expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSetExpr(this);
            }

            public readonly Expr Object;
            public readonly Token name;
            public readonly Expr value;
        }

        /// <summary>
        /// Represents a 'super' expression to call a superclass method.
        /// </summary>
        public class Super : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Super"/> class with a keyword and method name.
            /// </summary>
            /// <param name="keyword">The 'super' keyword.</param>
            /// <param name="method">The name of the method to call on the superclass.</param>
            public Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.method = method;
            }

            /// <summary>
            /// Accepts a visitor to process this 'super' expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this 'super' expression.</param>
            /// <returns>The result of processing the 'super' expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSuperExpr(this);
            }

            public readonly Token keyword;
            public readonly Token method;
        }

        /// <summary>
        /// Represents a 'this' expression.
        /// </summary>
        public class This : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="This"/> class with a keyword.
            /// </summary>
            /// <param name="keyword">The 'this' keyword.</param>
            public This(Token keyword)
            {
                this.keyword = keyword;
            }

            /// <summary>
            /// Accepts a visitor to process this 'this' expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this 'this' expression.</param>
            /// <returns>The result of processing the 'this' expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitThisExpr(this);
            }

            public readonly Token keyword;
        }

        /// <summary>
        /// Represents a unary expression.
        /// </summary>
        public class Unary : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Unary"/> class with an operator and operand.
            /// </summary>
            /// <param name="operator">The unary operator.</param>
            /// <param name="right">The operand.</param>
            public Unary(Token _operator, Expr right)
            {
                Operator = _operator;
                Right = right;
            }

            /// <summary>
            /// Accepts a visitor to process this unary expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this unary expression.</typeparam>
            /// <returns>The result of processing the unary expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }

            public readonly Token? Operator;
            public readonly Expr? Right;
        }

        /// <summary>
        /// Represents a variable expression.
        /// </summary>
        public class Variable : Expr
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Variable"/> class with a variable name.
            /// </summary>
            /// <param name="name">The name of the variable.</param>
            public Variable(Token _name)
            {
                name = _name;
            }

            /// <summary>
            /// Accepts a visitor to process this variable expression.
            /// </summary>
            /// <typeparam name="R">The return type of the visitor method.</typeparam>
            /// <param name="visitor">The visitor processing this variable expression.</param>
            /// <returns>The result of processing the variable expression.</returns>
            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }

            public readonly Token name;
        }

        /// <summary>
        /// Abstract method to accept a visitor for processing the expression.
        /// </summary>
        /// <typeparam name="R">The return type of the visitor method.</typeparam>
        /// <param name="visitor">The visitor processing this expression.</param>
        /// <returns>The result of processing the expression.</returns>
        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
