using Crafting_Interpreters.AST;
using Crafting_Interpreters.Classes;
using Crafting_Interpreters.Errors;
using Crafting_Interpreters.interfaces;
using Crafting_Interpreters.Interpreters;
using System;
using System.Collections.Generic;

namespace Crafting_Interpreters.LoxFunctions
{
    /// <summary>
    /// Represents a function defined in Lox. Implements the ICallable interface.
    /// </summary>
    public class LoxFunction : ICallable
    {
        /// <summary>
        /// The function declaration statement that defines this function.
        /// </summary>
        readonly private Stmt.Function _declaration;

        /// <summary>
        /// The environment/scope in which the function was declared.
        /// </summary>
        readonly private Environment _closure;

        /// <summary>
        /// Indicates if the function is an initializer for a class.
        /// </summary>
        private bool _isInitializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoxFunction"/> class with a function declaration, closure environment, and initializer flag.
        /// </summary>
        /// <param name="declaration">The function declaration.</param>
        /// <param name="closure">The closure environment where the function is defined.</param>
        /// <param name="isInitializer">Indicates if the function is an initializer.</param>
        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            _closure = closure;
            _declaration = declaration;
            _isInitializer = isInitializer;
        }

        /// <summary>
        /// Gets the number of arguments the function expects.
        /// </summary>
        /// <returns>The number of arguments.</returns>
        public int Arity()
        {
            return _declaration.Params.Count();
        }

        /// <summary>
        /// Calls the function with the specified arguments using the given interpreter.
        /// </summary>
        /// <param name="interpreter">The interpreter executing the call.</param>
        /// <param name="arguments">The arguments passed to the function.</param>
        /// <returns>The result of the function call.</returns>
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            Environment environment = new Environment(_closure);

            // Bind the arguments to the function's parameters in the new environment.
            for (int i = 0; i < _declaration.Params.Count(); i++)
            {
                environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
            }

            try
            {
                // Execute the function code.
                interpreter.ExecuteBlock(_declaration.body, environment);
            }
            catch (Return returnValue)
            {
                // If return value is a initializer for a object, return the instance being initialized.
                if (_isInitializer) return _closure.GetAt(0, "this");

                // Return the value.
                return returnValue.Value;
            }
            // if function is a initializer for a object without return statement, return it 
            if (_isInitializer) return _closure.GetAt(0, "this");

            // No return statement or value.
            return null;
        }

        /// <summary>
        /// Binds the function to an instance, creating a new environment with "this" defined.
        /// </summary>
        /// <param name="instance">The instance to bind to.</param>
        /// <returns>A new LoxFunction bound to the instance.</returns>
        public LoxFunction Bind(LoxInstance instance)
        {
            Environment environment = new Environment(_closure);
            environment.Define("this", instance);
            return new LoxFunction(_declaration, environment, _isInitializer);
        }

        /// <summary>
        /// Returns a string that represents the function.
        /// </summary>
        /// <returns>A string representation of the function.</returns>
        public override string ToString()
        {
            return "<fn " + _declaration.name.Lexeme + ">";
        }
    }
}