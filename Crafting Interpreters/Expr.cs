using System.Collections.Generic;

namespace CraftingInterpreters.Lox
{
    public abstract class Expr
    {
        public interface Visitor<R>
        {
            R? VisitAssignExpr(Assign expr);
            R? VisitBinaryExpr(Binary expr);
            R? VisitGroupingExpr(Grouping expr);
            R? VisitLiteralExpr(Literal expr);
            R? VisitLogicalExpr(Logical expr);
            R? VisitUnaryExpr(Unary expr);
            R? VisitVariableExpr(Variable expr);
        }
        public class Assign : Expr
        {
            public Assign(Token name, Expr value)
            {
                this.name = name;
                this.value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }

            public readonly Token name;
            public readonly Expr value;
        }
        public class Binary : Expr
        {
            public Binary(Expr left, Token _operator, Expr right)
            {
                Left = left;
                Operator = _operator;
                Right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }

            public readonly Expr? Left;
            public readonly Token? Operator;
            public readonly Expr? Right;
        }
        public class Grouping : Expr
        {
            public Grouping(Expr expression)
            {
                Expression = expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }

            public readonly Expr Expression;
        }
        public class Literal : Expr
        {
            public Literal(object value)
            {
                Value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }

            public readonly object? Value;
        }
        public class Logical : Expr
        {
            public Logical(Expr left, Token __operator, Expr right)
            {
                this.left = left;
                _operator = __operator;
                this.right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }

            public readonly Expr left;
            public readonly Token _operator;
            public readonly Expr right;
        }
        public class Unary : Expr
        {
            public Unary(Token _operator, Expr right)
            {
                Operator = _operator;
                Right = right;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }

            public readonly Token? Operator;
            public readonly Expr? Right;
        }

        public class Variable : Expr
        {
            public Variable(Token _name)
            {
                name = _name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }

            public readonly Token name;
        }

        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
