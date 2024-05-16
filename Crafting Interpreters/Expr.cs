using System.Collections.Generic;
using static CraftingInterpreters.Lox.Expr;

namespace CraftingInterpreters.Lox
{
    public abstract class Expr
    {
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
        public class Call : Expr
        {
            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                this.callee = callee;
                this.paren = paren;
                this.arguments = arguments;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitCallExpr(this);
            }

            public readonly Expr callee;
            public readonly Token paren;
            public readonly List<Expr> arguments;
        }
        public class Get : Expr
        {
            public Get(Expr Object, Token name)
            {
                this.Object = Object;
                this.name = name;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitGetExpr(this);
            }

            public readonly Expr Object;
        public readonly Token name;
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
        public class Set : Expr
        {
            public Set(Expr _object, Token name, Expr value)
            {
                this.Object = _object;
                this.name = name;
                this.value = value;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSetExpr(this);
            }

            public readonly Expr Object;
            public readonly Token name;
            public readonly Expr value;
        }
        public class Super : Expr
        {
            public Super(Token keyword, Token method)
            {
                this.keyword = keyword;
                this.method = method;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitSuperExpr(this);
            }

            public readonly Token keyword;
            public readonly Token method;
        }
        public class This : Expr
        {
            public This(Token keyword)
            {
                this.keyword = keyword;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitThisExpr(this);
            }

            public readonly Token keyword;
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
