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
                "Assign   : Token name, Expr value",
                "Binary   : Expr Left, Token Operator, Expr Right",
                "Grouping : Expr Expression",
                "Literal  : object Value",
                "Logical  : Expr left, Token operator, Expr right",
                "Unary    : Token Operator, Expr Right",
                "Variable : Token name"
            });
            DefineAst(outputDir, "Stmt", new List<string>
            {
              "Block      : List<Stmt> statements",
              "Expression : Expr expression",
              "If         : Expr condition, Stmt thenBranch, Stmt elseBranch",
              "Print      : Expr expression",
              "Var        : Token name, Expr initializer",
              "While      : Expr condition, Stmt body"
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

                DefineVisitor(writer, baseName, types);



                // The AST classes.
                foreach (string type in types)
                {
                    string[] typeComponents = type.Split(":");
                    string className = typeComponents[0].Trim();
                    string fields = typeComponents[1].Trim();
                    DefineType(writer, baseName, className, fields);
                }

                // The base accept() method.
                writer.WriteLine();
                writer.WriteLine("  public abstract R Accept<R>(Visitor<R> visitor);");

                writer.WriteLine("    }");
                writer.WriteLine("}");
                writer.Close();
            }
        }

        private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine("  interface Visitor<R> {");

            foreach (string type in types)
            {
                string[] typeInfo = type.Split(':');
                string typeName = typeInfo[0].Trim();
                writer.WriteLine($"    R Visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
            }

            writer.WriteLine("  }");
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

            // Visitor pattern.
            writer.WriteLine();
            writer.WriteLine("    public override R Accept<R>(Visitor<R> visitor) {");
            writer.WriteLine("      return visitor.Visit" + className + baseName + "(this);");
            writer.WriteLine("    }");

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
