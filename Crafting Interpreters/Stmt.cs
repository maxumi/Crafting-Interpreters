using System.Collections.Generic;

namespace CraftingInterpreters.Lox
{
    public abstract class Stmt
    {
        public interface Visitor<R>
        {
            R? VisitExpressionStmt(Expression stmt);
            R? VisitPrintStmt(Print stmt);
            R? VisitVarStmt(Var stmt);
        }
        public class Expression : Stmt
        {
            public Expression(Expr expression)
            {
                _expression = expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitExpressionStmt(this);
            }

            public readonly Expr _expression;
        }
        public class Print : Stmt
        {
            public Print(Expr expression)
            {
                _expression = expression;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitPrintStmt(this);
            }

            public readonly Expr _expression;
        }
        public class Var : Stmt
        {
            public Var(Token _name, Expr _initializer)
            {
                name = _name;
                initializer = _initializer;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitVarStmt(this);
            }

            public readonly Token name;
            public readonly Expr initializer;
        }
        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
