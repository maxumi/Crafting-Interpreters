using Crafting_Interpreters;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CraftingInterpreters.Lox
{
    class Lox
    {
        private static bool HadError = false;

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
        }
        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null) break;
            }
        }
        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            foreach(Token token in tokens)
            {
                Console.WriteLine($"{token}");
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where,string message)
        {

            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            HadError = true;
        }
    }
}