using Crafting_Interpreters.Errors;
using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crafting_Interpreters
{
    public class Environment
    {
        private readonly Dictionary<string, object> values = new ();

        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }

            throw new RuntimeError(name,"Undefined variable '" + name.Lexeme + "'.");
        }


       public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        internal void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }
            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");

        }
    }
}
