using Crafting_Interpreters._interface;
using Crafting_Interpreters.Interpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crafting_Interpreters.Classes
{
    public class LoxClass : ICallable
    {
        public readonly string name;
        private readonly Dictionary<string, LoxFunction> methods;


        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.methods = methods;
        }

        public int Arity()
        {
            LoxFunction initializer = FindMethod("init");
            if (initializer == null) return 0;
            return initializer.Arity();


        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {


            LoxInstance instance = new LoxInstance(this);
            LoxFunction initializer = FindMethod("init");
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }
            return instance;

        }

        public override string ToString()
        {
            return name;
        }

        public LoxFunction FindMethod(string _name)
        {
            if (methods.ContainsKey(_name))
            {
                return methods[_name];
            }

            return null;
        }
    }
}
