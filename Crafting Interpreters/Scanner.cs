using CraftingInterpreters.Lox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CraftingInterpreters.Lox
{
    internal class Scanner
    {
        private string Source { get; set; } = "";
        private List<Token> Tokens { get; set; } = new List<Token>();
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




        private int start = 0;
        private int current = 0;
        private int line = 1;
        public Scanner(string source)
        {
            Source = source;
        }
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
        private bool isAtEnd()
        {
            return current >= Source.Length;
        }
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

        private char Advance()
        {
            return Source[current++];
        }
        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }
        private void AddToken(TokenType type, object literal)
        {
            string text = Source.Substring(start, current - start);
            Tokens.Add(new Token(type, text, literal, line));
        }
        private bool Match(char expected)
        {
            if (isAtEnd()) return false;
            if (Source[current] != expected) return false;

            current++;
            return true;
        }
        private char Peek()
        {
            if (isAtEnd()) return '\0';
            return Source[current];
        }
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
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';

        }

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
        private char PeekNext()
        {
            if (current + 1 >= Source.Length) return '\0';
            return Source[current++];
        }


        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_';

        }
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = Source.Substring(start, current);
            TokenType type = Keywords[text];
            if (type == null)
            {
                type = TokenType.IDENTIFIER;
            }

            AddToken(type);
        }

    }
}
