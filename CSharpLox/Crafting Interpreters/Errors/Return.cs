using System;

namespace Crafting_Interpreters.Errors
{
    /// <summary>
    /// Represents a control flow exception used to return a value from a function in the Lox interpreter.
    /// </summary>
    internal class Return : Exception
    {
        /// <summary>
        /// Gets the value being returned from the function.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Return"/> class with the specified return value.
        /// </summary>
        /// <param name="value">The value to return from the function.</param>
        public Return(object value) : base(null, null)
        {
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Return"/> class with the specified return value and inner exception.
        /// </summary>
        /// <param name="value">The value to return from the function.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public Return(object value, Exception innerException) : base(null, innerException)
        {
            Value = value;
        }
    }
}

