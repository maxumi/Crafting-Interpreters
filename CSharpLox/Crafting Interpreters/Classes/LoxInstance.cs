using Crafting_Interpreters.Errors;
using Crafting_Interpreters.LoxFunctions;
using CraftingInterpreters.Lox;
using System.Collections.Generic;

namespace Crafting_Interpreters.Classes
{
    /// <summary>
    /// Represents an instance of a Lox class.
    /// </summary>
    public class LoxInstance
    {
        // The class to which this instance belongs.
        private LoxClass klass;

        // A dictionary to store the instance's fields.
        private readonly Dictionary<string, object> fields = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LoxInstance"/> class with the specified class.
        /// </summary>
        /// <param name="klass">The class to which this instance belongs.</param>
        public LoxInstance(LoxClass klass)
        {
            this.klass = klass;
        }

        /// <summary>
        /// Returns a string that represents the instance.
        /// </summary>
        /// <returns>A string representation of the instance.</returns>
        public override string ToString()
        {
            return klass.name + " instance";
        }

        /// <summary>
        /// Retrieves the value of a property or method defined on the instance or its class.
        /// </summary>
        /// <param name="name">The token representing the name of the property or method.</param>
        /// <returns>The value of the property or method.</returns>
        /// <exception cref="RuntimeError">Thrown if the property or method is not found.</exception>
        public object Get(Token name)
        {
            // Check if the property is defined in the instance's fields.
            if (fields.ContainsKey(name.Lexeme))
            {
                return fields[name.Lexeme];
            }

            // Check if the method is defined in the class.
            LoxFunction method = klass.FindMethod(name.Lexeme);
            if (method != null)
            {
                return method.Bind(this); // Bind the method to this instance.
            }

            // Throw an error if the property or method is not found.
            throw new RuntimeError(name, "Undefined property '" + name.Lexeme + "'.");
        }

        /// <summary>
        /// Sets the value of a property on the instance.
        /// </summary>
        /// <param name="name">The token representing the name of the property.</param>
        /// <param name="value">The value to set.</param>
        public void Set(Token name, object value)
        {
            fields[name.Lexeme] = value;
        }
    }
}
