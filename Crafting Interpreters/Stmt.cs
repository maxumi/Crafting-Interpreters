using System.Collections.Generic;

namespace CraftingInterpreters.Lox
{
    public abstract class Stmt
    {
        public interface Visitor<R>
        {
            R? VisitExpressionStmt(Expression stmt);
            R? VisitIfStmt(If _if);
            R? VisitPrintStmt(Print stmt);
            R? VisitVarStmt(Var stmt);
            R? VisitBlockStmt(Block block);
            R? VisitWhileStmt(While stmt);
        }
        public class Block : Stmt
        {
            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitBlockStmt(this);
            }

            public readonly List<Stmt> statements;
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
        public class If : Stmt
        {
            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitIfStmt(this);
            }

            public readonly Expr condition;
            public readonly Stmt thenBranch;
            public readonly Stmt elseBranch;
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
        public class While : Stmt
        {
            public While(Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }

            public override R Accept<R>(Visitor<R> visitor)
            {
                return visitor.VisitWhileStmt(this);
            }

            public readonly Expr condition;
            public readonly Stmt body;
        }
        public abstract R Accept<R>(Visitor<R> visitor);
    }
}
