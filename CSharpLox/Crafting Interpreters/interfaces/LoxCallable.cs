using System;
using System.Collections.Generic;
using Crafting_Interpreters.Interpreters;

namespace Crafting_Interpreters.interfaces
{
    /// <summary>
    /// Represents a callable entity in the Lox language, such as a function or method.
    /// </summary>
    public interface ICallable
    {
        /// <summary>
        /// Gets the number of arguments the callable entity expects.
        /// </summary>
        /// <returns>The number of arguments.</returns>
        int Arity();

        /// <summary>
        /// Calls the callable entity with the specified arguments using the given interpreter.
        /// </summary>
        /// <param name="interpreter">The interpreter executing the call.</param>
        /// <param name="arguments">The arguments to the callable entity.</param>
        /// <returns>The result of the call.</returns>
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
