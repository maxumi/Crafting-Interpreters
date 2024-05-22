using CraftingInterpreters.Lox;
using System;

namespace Crafting_Interpreters.Errors
{
    /// <summary>
    /// Represents a runtime error encountered during the execution of a Lox program.
    /// </summary>
    public class RuntimeError : Exception
    {
        /// <summary>
        /// Gets the token where the runtime error occurred.
        /// </summary>
        public Token Token { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeError"/> class with a specified token and error message.
        /// </summary>
        /// <param name="token">The token where the runtime error occurred.</param>
        /// <param name="message">The error message describing the runtime error.</param>
        public RuntimeError(Token token, string message) : base(message) // Calls the base Exception class constructor with the message.
        {
            this.Token = token;
        }
    }
}