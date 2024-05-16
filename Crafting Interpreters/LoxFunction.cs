using Crafting_Interpreters._interface;
using Crafting_Interpreters.Classes;
using Crafting_Interpreters.Errors;
using Crafting_Interpreters.Interpreters;
using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Crafting_Interpreters
{
    public class LoxFunction : ICallable
    {
        readonly private Stmt.Function _declaration;
        readonly private Environment _closure;
        private bool _isInitializer;

        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            this._closure = closure;
            this._declaration = declaration;
            this._isInitializer = isInitializer;
        }

        public int Arity()
        {
            return _declaration.Params.Count();
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
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
                if (_isInitializer) return _closure.GetAt(0, "this");
                return returnValue.Value;
            }
            if (_isInitializer) return _closure.GetAt(0, "this");
            return null;
        }
        public LoxFunction Bind(LoxInstance instance)
        {
            Environment environment = new Environment(_closure);
            environment.Define("this", instance);
            return new LoxFunction(_declaration, environment, _isInitializer);
        }
        public override string ToString() { return "<fn " + _declaration.name.Lexeme + ">"; }
    }
}
