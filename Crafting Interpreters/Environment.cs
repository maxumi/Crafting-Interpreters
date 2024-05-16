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
        public readonly Environment Enclosing;
        private readonly Dictionary<string, object> values = new ();
        public Environment()
        {
            Enclosing = null;
        }
        public Environment(Environment enclosing)
        {
            this.Enclosing = enclosing;
        }
        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }
            if (Enclosing != null) return Enclosing.Get(name);

            throw new RuntimeError(name,"Undefined variable '" + name.Lexeme + "'.");
        }


       public void Define(string name, object value)
        {
            values.Add(name, value);
        }
        public object GetAt(int distance, string name)
        {
            return Ancestor(distance).values[name];

        }

        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance).values[name.Lexeme] = value;
        }
        private Environment Ancestor(int distance)
        {
            Environment environment = this;
            for (int i = 0; i < distance; i++)
            {
                environment = environment.Enclosing;
            }

            return environment;
        }
        internal void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");

        }
    }
}
