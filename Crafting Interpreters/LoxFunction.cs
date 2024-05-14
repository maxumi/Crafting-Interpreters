using Crafting_Interpreters._interface;
using Crafting_Interpreters.Errors;
using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Crafting_Interpreters
{
    internal class LoxFunction : ICallable
    {
        readonly private Stmt.Function _declaration;
        readonly private Environment _closure;

        public LoxFunction(Stmt.Function declaration, Environment closure)
        {
            this._closure = closure;
            this._declaration = declaration;
        }

        public int Arity()
        {
            return _declaration.Params.Count();
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            //Environment environment = new Environment(interpreter.globals);
            Environment environment = new Environment(_closure);
            for (int i = 0; i < _declaration.Params.Count(); i++) {
                environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.body, environment);
            }
            catch (Return returnValue)
            {
                return returnValue.Value;
            }
            return null;
        }
        public override string ToString() { return "<fn " + _declaration.name.Lexeme + ">"; }
    }
}
