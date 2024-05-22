using Crafting_Interpreters.AST;
using Crafting_Interpreters.Errors;
using Crafting_Interpreters.Interpreters;
using Crafting_Interpreters.Parse;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CraftingInterpreters.Lox
{
    public class Lox
    {
        // Instance of the interpreter
        private static readonly Interpreter interpreter = new Interpreter();
        // Flags for error handling
        private static bool HadError = false;
        private static bool HadRuntimeError = false;

        // Entry point of the application
        static public void Main(String[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        /// <summary>
        /// Runs the Lox script from a file.
        /// </summary>
        /// <param name="path">The path to the Lox script file.</param>
        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            Run(Encoding.Default.GetString(bytes));

            // Indicate an error in the exit code
            if (HadError) System.Environment.Exit(65);
            if (HadRuntimeError) System.Environment.Exit(70);
        }

        /// <summary>
        /// Runs an interactive prompt for the Lox interpreter.
        /// </summary>
        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null) break;
                Run(line);
            }
        }

        /// <summary>
        /// Runs the Lox interpreter on the given source code.
        /// </summary>
        /// <param name="source">The source code to run.</param>
        private static void Run(string source)
        {
            // Goes through step by step to execute the code, from scanning the tokens to executing interpret the statements
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();

            Resolver resolver = new Resolver(interpreter);
            resolver.Resolve(statements);

            // Stop if there was a syntax error.
            if (HadError) return;

            interpreter.Interpret(statements);
        }

        /// <summary>
        /// Reports a syntax error with a line number and message.
        /// </summary>
        /// <param name="line">The line number where the error occurred.</param>
        /// <param name="message">The error message.</param>
        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        /// <summary>
        /// Reports a syntax error with a token and message.
        /// </summary>
        /// <param name="token">The token where the error occurred.</param>
        /// <param name="message">The error message.</param>
        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        /// <summary>
        /// Reports a runtime error.
        /// </summary>
        /// <param name="error">The runtime error that occurred.</param>
        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
            HadRuntimeError = true;
        }

        /// <summary>
        /// Helper method to report an error.
        /// </summary>
        /// <param name="line">The line number where the error occurred.</param>
        /// <param name="where">The location in the code where the error occurred.</param>
        /// <param name="message">The error message.</param>
        private static void Report(int line, string where, string message)
        {
            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            HadError = true;
        }
    }
}
