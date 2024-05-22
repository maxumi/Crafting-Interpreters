using CraftingInterpreters.Lox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Crafting_Interpreters.Parse
{
    /// <summary>
    /// The Scanner class that goes through text to retrive tokens.
    /// </summary>
    internal class Scanner
    {
        /// <summary>
        /// Holds the source code that is being scanned.
        /// </summary>
        private string Source { get; set; } = "";

        /// <summary>
        /// Stores the list of tokens that have been scanned from the source code.
        /// </summary>
        private List<Token> Tokens { get; set; } = new List<Token>();

        /// <summary>
        /// Defines a dictionary of reserved keywords in the language and their corresponding token types.
        /// </summary>
        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"and", TokenType.AND},
            {"class", TokenType.CLASS},
            {"else", TokenType.ELSE},
            {"false", TokenType.FALSE},
            {"for", TokenType.FOR},
            {"fun", TokenType.FUN},
            {"if", TokenType.IF},
            {"nil", TokenType.NIL},
            {"or", TokenType.OR},
            {"print", TokenType.PRINT},
            {"return", TokenType.RETURN},
            {"super", TokenType.SUPER},
            {"this", TokenType.THIS},
            {"true", TokenType.TRUE},
            {"var", TokenType.VAR},
            {"while", TokenType.WHILE}
        };

        /// <summary>
        /// Marks the beginning of the current lexeme being scanned.
        /// </summary>
        private int start = 0;

        /// <summary>
        /// Tracks the current position in the source code being scanned.
        /// </summary>
        private int current = 0;

        /// <summary>
        /// Keeps track of the current line number in the source code, for error reporting.
        /// </summary>
        private int line = 1;

        /// <summary>
        /// Initializes a new instance of the `Scanner` class with the provided source code.
        /// </summary>
        /// <param name="source">The source code to scan.</param>
        public Scanner(string source)
        {
            Source = source;
        }

        /// <summary>
        /// Scans through the entire source code, tokenizing it into a list of tokens and adding an EOF (end of file) token at the end.
        /// </summary>
        /// <returns>A list of tokens scanned from the source code.</returns>
        public List<Token> ScanTokens()
        {
            while (!isAtEnd())
            {
                // We are at the beginning of the next lexeme.
                start = current;
                ScanToken();
            }

            Tokens.Add(new Token(TokenType.EOF, "", null, line));
            return Tokens;
        }

        /// <summary>
        /// Checks if the scanner has reached the end of the source code.
        /// </summary>
        /// <returns>True if at the end of the source code, false otherwise.</returns>
        private bool isAtEnd()
        {
            return current >= Source.Length;
        }

        /// <summary>
        /// Scans the next token from the source code, handling different characters and recognizing tokens accordingly.
        /// </summary>
        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !isAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace.
                    break;
                case '\n':
                    line++;
                    break;
                case '"': String(); break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(line, "Unexpected character.");
                    }
                    break;
            }
        }

        /// <summary>
        /// Advances the scanner to the next character in the source code and returns the current character.
        /// </summary>
        /// <returns>The current character in the source code.</returns>
        private char Advance()
        {
            return Source[current++];
        }

        /// <summary>
        /// Creates a new token with the specified type and adds it to the list of tokens.
        /// </summary>
        /// <param name="type">The type of token to add.</param>
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        /// <summary>
        /// Creates a new token with the specified type and literal value, then adds it to the list of tokens.
        /// </summary>
        /// <param name="type">The type of token to add.</param>
        /// <param name="literal">The literal value of the token.</param>
        private void AddToken(TokenType type, object literal)
        {
            string text = Source.Substring(start, current - start);
            Tokens.Add(new Token(type, text, literal, line));
        }

        /// <summary>
        /// Checks if the next character matches the expected character and advances if it does.
        /// </summary>
        /// <param name="expected">The expected character to match.</param>
        /// <returns>True if the characters match, false otherwise.</returns>
        private bool Match(char expected)
        {
            if (isAtEnd()) return false;
            if (Source[current] != expected) return false;

            current++;
            return true;
        }

        /// <summary>
        /// Returns the current character without advancing the scanner.
        /// </summary>
        /// <returns>The current character in the source code.</returns>
        private char Peek()
        {
            if (isAtEnd()) return '\0';
            return Source[current];
        }

        /// <summary>
        /// Handles string literals by advancing until the closing quote is found, then adds the string token.
        /// </summary>
        private void String()
        {
            while (Peek() != '"' && !isAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (isAtEnd())
            {
                Lox.Error(line, "Unterminated string.");
                return;
            }

            // The closing ".
            Advance();
            string value = Source.Substring(start + 1, current - start - 2);
            AddToken(TokenType.STRING, value);
        }

        /// <summary>
        /// Checks if a character is a digit (0-9).
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is a digit, false otherwise.</returns>
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        /// <summary>
        /// Handles numeric literals, including integers and decimals, and adds the number token.
        /// </summary>
        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER, double.Parse(Source.Substring(start, current - start)));
        }

        /// <summary>
        /// Returns the character after the current one without advancing, or `\0` if at the end.
        /// </summary>
        /// <returns>The next character in the source code.</returns>
        private char PeekNext()
        {
            if (current + 1 >= Source.Length) return '\0';
            return Source[current + 1];
        }

        /// <summary>
        /// Checks if a character is an alphabetical character (a-z, A-Z) or an underscore (_).
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is alphabetical or an underscore, false otherwise.</returns>
        private bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c == '_';
        }

        /// <summary>
        /// Checks if a character is either alphabetical or a digit.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is alphanumeric, false otherwise.</returns>
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        /// <summary>
        /// Handles identifiers and keywords by advancing through alphanumeric characters and adding the appropriate token.
        /// </summary>
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = Source.Substring(start, current - start);

            if (!Keywords.TryGetValue(text, out TokenType type))
            {
                type = TokenType.IDENTIFIER;
            }

            AddToken(type);
        }
    }
}
