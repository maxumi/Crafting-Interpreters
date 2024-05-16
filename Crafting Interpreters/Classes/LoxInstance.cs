using Crafting_Interpreters.Errors;
using CraftingInterpreters.Lox;

namespace Crafting_Interpreters.Classes
{
    public class LoxInstance
    {
        private LoxClass klass;
        private readonly Dictionary<string, object> fields = new();


        public LoxInstance(LoxClass klass) 
        {
            this.klass = klass;
        }

        public override string ToString()
        {
            return klass.name + " instance";
        }

        public object Get(Token name)
        {
            if (fields.ContainsKey(name.Lexeme))
            {
                return fields[name.Lexeme];
            }
            LoxFunction method = klass.FindMethod(name.Lexeme);

            if (method != null) return method.Bind(this);

            throw new RuntimeError(name,"Undefined property '" + name.Lexeme + "'.");
        }
        public void Set(Token name, object value)
        {
            fields[name.Lexeme] = value;
        }

    }
}