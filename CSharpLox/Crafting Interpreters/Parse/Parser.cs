using Crafting_Interpreters.Errors;
using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Crafting_Interpreters.AST.Expr;
using static Crafting_Interpreters.AST.Stmt;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Crafting_Interpreters.AST;

namespace Crafting_Interpreters.Parse
{
    /// <summary>
    /// The Parser class that goes through Tokens to retrive statements.
    /// </summary>
    public class Parser
    {

        /// <summary>
        /// Holds the tokens to be parsed.
        /// </summary>
        private readonly List<Token> tokens;


        /// <summary>
        /// Holds the current token position in the list.
        /// </summary>
        private int current = 0;

        // Constructor initializing the parser with a list of tokens
        /// <summary>
        /// Constructor initializing the parser with a list of tokens
        /// </summary>
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        /// <summary>
        /// Attempts to match any of the given token types. If a match is found, the current token is advanced.
        /// </summary>
        /// <param name="types">Token types to match against.</param>
        /// <returns>True if a match is found, otherwise false.</returns>
        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the current token matches the given type.
        /// </summary>
        /// <param name="type">The token type to check.</param>
        /// <returns>True if the current token matches, otherwise false.</returns>
        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        /// <summary>
        /// Advances to the next token and returns the previous token.
        /// </summary>
        /// <returns>The previous token.</returns>
        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        /// <summary>
        /// Checks if the parser has reached the end of the token list.
        /// </summary>
        /// <returns>True if at the end, otherwise false.</returns>
        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EOF;
        }

        /// <summary>
        /// Parses an expression.
        /// </summary>
        /// <returns>The parsed expression.</returns>
        private Expr Expression()
        {
            return Assignment();
        }

        /// <summary>
        /// Parses an assignment expression.
        /// </summary>
        /// <returns>The parsed assignment expression.</returns>
        private Expr Assignment()
        {
            Expr expr = Or();
            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Variable)
                {
                    Token name = ((Variable)expr).name;
                    return new Expr.Assign(name, value);
                }
                else if (expr is Expr.Get)
                {
                    Expr.Get get = (Expr.Get)expr;
                    return new Expr.Set(get.Object, get.name, value);
                }

                error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        /// <summary>
        /// Parses an 'or' expression.
        /// </summary>
        /// <returns>The parsed 'or' expression.</returns>
        private Expr Or()
        {
            Expr expr = And();

            while (Match(TokenType.OR))
            {
                Token _operator = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, _operator, right);
            }

            return expr;
        }

        /// <summary>
        /// Parses an 'and' expression.
        /// </summary>
        /// <returns>The parsed 'and' expression.</returns>
        private Expr And()
        {
            Expr expr = Equality();

            while (Match(TokenType.AND))
            {
                Token _operator = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, _operator, right);
            }

            return expr;
        }

        /// <summary>
        /// Parses the list of statements.
        /// </summary>
        /// <returns>A list of parsed statements.</returns>
        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }

            return statements;
        }

        /// <summary>
        /// Parses a declaration statement.
        /// </summary>
        /// <returns>The parsed declaration statement.</returns>
        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.CLASS)) return ClassDeclaration();
                if (Match(TokenType.FUN)) { return Function("function"); }
                if (Match(TokenType.VAR))
                {
                    return VarDeclaration();
                }

                return Statement();
            }
            catch (ParseError error)
            {
                Synchronize();
                return null;
            }
        }

        /// <summary>
        /// Parses a class declaration.
        /// </summary>
        /// <returns>The parsed class declaration.</returns>
        private Stmt ClassDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect class name.");
            Expr.Variable superclass = null;
            if (Match(TokenType.LESS))
            {
                Consume(TokenType.IDENTIFIER, "Expect superclass name.");
                superclass = new Expr.Variable(Previous());
            }

            Consume(TokenType.LEFT_BRACE, "Expect '{' before class body.");

            List<Stmt.Function> methods = new();
            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                methods.Add(Function("method"));
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after class body.");

            return new Stmt.Class(name, superclass, methods);
        }

        /// <summary>
        /// Parses a function.
        /// </summary>
        /// <param name="kind">The kind of function (e.g., "function" or "method").</param>
        /// <returns>The parsed function.</returns>
        private Stmt.Function Function(string kind)
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect " + kind + " name.");
            Consume(TokenType.LEFT_PAREN, "Expect '(' after " + kind + " name.");
            List<Token> parameters = new();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count() >= 255)
                    {
                        error(Peek(), "Can't have more than 255 parameters.");
                    }

                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (Match(TokenType.COMMA));
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after parameters.");

            Consume(TokenType.LEFT_BRACE, "Expect '{' before " + kind + " body.");
            List<Stmt> body = Block();
            return new Stmt.Function(name, parameters, body);
        }

        /// <summary>
        /// Parses a variable declaration.
        /// </summary>
        /// <returns>The parsed variable declaration.</returns>
        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Var(name, initializer);
        }

        /// <summary>
        /// Parses a statement.
        /// </summary>
        /// <returns>The parsed statement.</returns>
        private Stmt Statement()
        {
            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.RETURN)) return ReturnStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.LEFT_BRACE)) return new Block(Block());

            return ExpressionStatement();
        }

        /// <summary>
        /// Parses a return statement.
        /// </summary>
        /// <returns>The parsed return statement.</returns>
        private Stmt ReturnStatement()
        {
            Token keyword = Previous();
            Expr value = null;
            if (!Check(TokenType.SEMICOLON))
            {
                value = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new Stmt.Return(keyword, value);
        }

        /// <summary>
        /// Parses a for statement.
        /// </summary>
        /// <returns>The parsed for statement.</returns>
        private Stmt ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");
            Stmt initializer;
            if (Match(TokenType.SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(TokenType.VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }
            Expr condition = null;
            if (!Check(TokenType.SEMICOLON))
            {
                condition = Expression();
            }
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition.");
            Expr increment = null;
            if (!Check(TokenType.RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

            Stmt body = Statement();

            if (increment != null)
            {
                body = new Stmt.Block(new List<Stmt> { body, new Stmt.Expression(increment) });
            }
            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer != null)
            {
                body = new Stmt.Block(new List<Stmt> { initializer, body });
            }

            return body;
        }

        /// <summary>
        /// Parses a while statement.
        /// </summary>
        /// <returns>The parsed while statement.</returns>
        private Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");
            Stmt body = Statement();

            return new Stmt.While(condition, body);
        }

        /// <summary>
        /// Parses an if statement.
        /// </summary>
        /// <returns>The parsed if statement.</returns>
        private Stmt IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(TokenType.ELSE))
            {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        /// <summary>
        /// Parses a block of statements.
        /// </summary>
        /// <returns>A list of statements in the block.</returns>
        private List<Stmt> Block()
        {
            List<Stmt> statements = new();

            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        /// <summary>
        /// Parses a print statement.
        /// </summary>
        /// <returns>The parsed print statement.</returns>
        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Print(value);
        }

        /// <summary>
        /// Parses an expression statement.
        /// </summary>
        /// <returns>The parsed expression statement.</returns>
        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Expression(expr);
        }

        /// <summary>
        /// Parses an equality expression.
        /// </summary>
        /// <returns>The parsed equality expression.</returns>
        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token _operator = Previous();
                Expr right = Comparison();
                expr = new Binary(expr, _operator, right);
            }

            return expr;
        }

        /// <summary>
        /// Parses a comparison expression.
        /// </summary>
        /// <returns>The parsed comparison expression.</returns>
        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token _operator = Previous();
                Expr right = Term();
                expr = new Binary(expr, _operator, right);
            }

            return expr;
        }

        /// <summary>
        /// Parses a term expression.
        /// </summary>
        /// <returns>The parsed term expression.</returns>
        private Expr Term()
        {
            Expr expr = Factor();
            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token _operator = Previous();
                Expr right = Factor();
                expr = new Binary(expr, _operator, right);
            }

            return expr;
        }

        /// <summary>
        /// Parses a factor expression.

        /// </summary>
        /// <returns>The parsed factor expression.</returns>
        private Expr Factor()
        {
            Expr expr = Unary();

            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token _operator = Previous();
                Expr right = Unary();
                expr = new Binary(expr, _operator, right);
            }

            return expr;
        }

        /// <summary>
        /// Parses a unary expression.
        /// </summary>
        /// <returns>The parsed unary expression.</returns>
        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token _operator = Previous();
                Expr right = Unary();
                return new Unary(_operator, right);
            }

            return Call();
        }

        /// <summary>
        /// Parses a call expression.
        /// </summary>
        /// <returns>The parsed call expression.</returns>
        private Expr Call()
        {
            Expr expr = Primary();

            while (true)
            {
                if (Match(TokenType.LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else if (Match(TokenType.DOT))
                {
                    Token name = Consume(TokenType.IDENTIFIER, "Expect property name after '.'.");
                    expr = new Expr.Get(expr, name);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        /// <summary>
        /// Finishes parsing a call expression.
        /// </summary>
        /// <param name="callee">The callee expression.</param>
        /// <returns>The parsed call expression.</returns>
        private Expr FinishCall(Expr callee)
        {
            List<Expr> arguments = new();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    arguments.Add(Expression());
                } while (Match(TokenType.COMMA));
            }
            Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");
            return new Expr.Call(callee, paren, arguments);
        }

        /// <summary>
        /// Parses a primary expression.
        /// </summary>
        /// <returns>The parsed primary expression.</returns>
        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new Literal(false);
            if (Match(TokenType.TRUE)) return new Literal(true);
            if (Match(TokenType.NIL)) return new Literal(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new Literal(Previous().Literal);
            }

            if (Match(TokenType.SUPER))
            {
                Token keyword = Previous();
                Consume(TokenType.DOT, "Expect '.' after 'super'.");
                Token method = Consume(TokenType.IDENTIFIER, "Expect superclass method name.");
                return new Expr.Super(keyword, method);
            }

            if (Match(TokenType.THIS)) return new Expr.This(Previous());

            if (Match(TokenType.IDENTIFIER))
            {
                return new Variable(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Grouping(expr);
            }

            throw error(Peek(), "Expect expression.");
        }

        /// <summary>
        /// Reports a parse error for the given token.
        /// </summary>
        /// <param name="token">The token where the error occurred.</param>
        /// <param name="message">The error message.</param>
        /// <returns>A new ParseError exception.</returns>
        private ParseError error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        /// <summary>
        /// Consumes the current token if it matches the expected type.
        /// </summary>
        /// <param name="type">The expected token type.</param>
        /// <param name="message">The error message if the token does not match.</param>
        /// <returns>The consumed token.</returns>
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();

            throw error(Peek(), message);
        }

        /// <summary>
        /// Returns the current token without advancing.
        /// </summary>
        /// <returns>The current token.</returns>
        private Token Peek()
        {
            return tokens[current];
        }

        /// <summary>
        /// Returns the previous token.
        /// </summary>
        /// <returns>The previous token.</returns>
        private Token Previous()
        {
            return tokens[current - 1];
        }

        /// <summary>
        /// Synchronizes the parser by discarding tokens until it reaches a known point where parsing can continue.
        /// </summary>
        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
    }
}
