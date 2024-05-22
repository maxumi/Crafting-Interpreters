using Crafting_Interpreters.interfaces;
using Crafting_Interpreters.Interpreters;
using System;
using System.Collections.Generic;

namespace Crafting_Interpreters.LoxFunctions
{
    /// <summary>
    /// Represents a native function that returns the current time in seconds since the Unix epoch.
    /// Implements the ICallable interface.
    /// </summary>
    internal class Clock : ICallable
    {
        /// <summary>
        /// Returns the number of arguments the callable entity expects.
        /// </summary>
        /// <returns>The number of arguments (always 0 for this native function).</returns>
        public int Arity()
        {
            return 0;
        }

        /// <summary>
        /// Calls the native clock function to return the current time in seconds since the Unix epoch.
        /// </summary>
        /// <param name="interpreter">The interpreter executing the call.</param>
        /// <param name="arguments">The list of arguments passed to the call (none expected).</param>
        /// <returns>The current time in seconds since the Unix epoch.</returns>
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
        }

        /// <summary>
        /// Returns a string that represents the native clock function.
        /// </summary>
        /// <returns>A string representation of the native function.</returns>
        public override string ToString()
        {
            return "<native fn>";
        }
    }
}
