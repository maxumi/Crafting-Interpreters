using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingInterpreters.Tools
{
    public class GenerateAst
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(64);
            }

            string outputDir = args[0];
            DefineAst(outputDir, "Expr", new List<string>
            {
                "Binary   : Expr Left, Token Operator, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : object Value",
                "Unary    : Token Operator, Expr Right"
            });



        }

        private static void DefineAst(string outputDir, string baseName, List<string> types)
        {
            string path = Path.Combine(outputDir, baseName + ".cs");
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine();
                writer.WriteLine("namespace CraftingInterpreters.Lox");
                writer.WriteLine("{");
                writer.WriteLine($"    public abstract class {baseName}");
                writer.WriteLine("    {");


                // The AST classes.
                foreach (string type in types)
                {
                    string[] typeComponents = type.Split(":");
                    string className = typeComponents[0].Trim();
                    string fields = typeComponents[1].Trim();
                    DefineType(writer, baseName, className, fields);
                }

                writer.WriteLine("    }");
                writer.WriteLine("}");
                writer.Close();
            }
        }

        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine($"    public class {className} : {baseName}");
            writer.WriteLine("    {");

            // Constructor.
            writer.WriteLine($"        public {className}({fieldList})");
            writer.WriteLine("        {");

            // Store parameters in fields.
            string[] fields = fieldList.Split(", ");
            foreach (string field in fields)
            {
                string name = field.Split(" ")[1];
                writer.WriteLine($"            {name} = {name};");
            }

            writer.WriteLine("        }");

            // Fields.
            writer.WriteLine();
            foreach (string field in fields)
            {
                writer.WriteLine($"        public readonly {field};");
            }

            writer.WriteLine("    }");
        }
    }

}
