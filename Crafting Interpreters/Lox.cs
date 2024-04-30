using Crafting_Interpreters;
using Crafting_Interpreters.Errors;
using CraftingInterpreters.Lox;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CraftingInterpreters.Lox
{
    public class Lox
    {
        private static readonly Interpreter interpreter = new Interpreter();
        private static bool HadError = false;
        private static bool HadRuntimeError = false;

        static public void Main(String[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
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
        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            Run(Encoding.Default.GetString(bytes));

            // Indicate an error in the exit code
            if (HadError) Environment.Exit(65);
            if (HadRuntimeError) Environment.Exit(70);
        }
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
        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            Parser parser = new Parser(tokens);
            Expr expression = parser.Parse();

            // Stop if there was a syntax error.
            if (HadError) return;
            interpreter.Interpret(expression);


        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }
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
        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
            HadRuntimeError = true;
        }
        private static void Report(int line, string where,string message)
        {

            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            HadError = true;
        }
    }
}