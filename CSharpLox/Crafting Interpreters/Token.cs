using System;

namespace CraftingInterpreters.Lox
{
    /// <summary>
    /// Represents a token in Lox. A token is a single element of the syntax.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// Gets or sets the lexeme (the actual string in the source code) for this token.
        /// </summary>
        public string? Lexeme { get; set; }

        /// <summary>
        /// Gets or sets the literal value of the token, if any.
        /// </summary>
        public object? Literal { get; set; }

        /// <summary>
        /// Gets or sets the line number where the token appears in the source code.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class with the specified type, lexeme, literal, and line number.
        /// </summary>
        /// <param name="type">The type of the token.</param>
        /// <param name="lexeme">The lexeme of the token.</param>
        /// <param name="literal">The literal value of the token.</param>
        /// <param name="line">The line number where the token appears.</param>
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            this.Type = type;
            this.Lexeme = lexeme;
            this.Literal = literal;
            this.Line = line;
        }

        /// <summary>
        /// Returns a string that represents the current token.
        /// </summary>
        /// <returns>A string representation of the token.</returns>
        public override string ToString()
        {
            return Type + " " + Lexeme + " " + Literal;
        }
    }

    /// <summary>
    /// Enumerates the different types of tokens that can appear in the Lox language.
    /// </summary>
    public enum TokenType
    {
        // Single-character tokens.
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

        // One or two character tokens.
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // Literals.
        IDENTIFIER, STRING, NUMBER,

        // Keywords.
        AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
        PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

        // End of file token.
        EOF
    }
}
