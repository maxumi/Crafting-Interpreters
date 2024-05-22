using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crafting_Interpreters.AST;




// Pay no mind to this class, this was used to test the AST early.

namespace Crafting_Interpreters.Misc
{
    public class AstPrinter : Expr.Visitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string? VisitAssignExpr(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }

        public string? VisitCallExpr(Expr.Call expr)
        {
            throw new NotImplementedException();
        }

        public string? VisitLogicalExpr(Expr.Logical expr)
        {
            throw new NotImplementedException();
        }

        public string? VisitVariableExpr(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder? builder = new StringBuilder();
            builder.Append("(").Append(name);
            foreach (var expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();

        }

        string Expr.Visitor<string>.VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

        }

        string Expr.Visitor<string>.VisitGetExpr(Expr.Get expr)
        {
            throw new NotImplementedException();
        }

        string Expr.Visitor<string>.VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);

        }

        string Expr.Visitor<string>.VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();

        }

        string Expr.Visitor<string>.VisitSetExpr(Expr.Set expr)
        {
            throw new NotImplementedException();
        }

        string? Expr.Visitor<string>.VisitSuperExpr(Expr.Super expr)
        {
            throw new NotImplementedException();
        }

        string? Expr.Visitor<string>.VisitThisExpr(Expr.This expr)
        {
            throw new NotImplementedException();
        }

        string Expr.Visitor<string>.VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, expr.Right);

        }
    }
}
