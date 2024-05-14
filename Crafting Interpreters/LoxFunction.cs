using Crafting_Interpreters._interface;
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
        public LoxFunction(Stmt.Function declaration)
        {
            this._declaration = declaration;
        }

        public int Arity()
        {
            return _declaration.Params.Count();
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(interpreter.globals);
            for (int i = 0; i < _declaration.Params.Count(); i++) {
                environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(_declaration.body, environment);
            return null;
        }
        public override string ToString() { return "<fn " + _declaration.name.Lexeme + ">"; }
    }
}
