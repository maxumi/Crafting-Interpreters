using Crafting_Interpreters.Errors;
using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;

namespace Crafting_Interpreters
{
    /// <summary>
    /// The Environment class represents a scope for storing variable bindings.
    /// It supports nested scopes by referring to an enclosing environment.
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// The enclosing environment, representing the outer scope.
        /// </summary>
        public readonly Environment Enclosing;

        /// <summary>
        /// A dictionary to store variable names and their values.
        /// </summary>
        private readonly Dictionary<string, object> values = new();

        /// <summary>
        /// Initializes a new instance of the Environment class with no enclosing environment.
        /// </summary>
        public Environment()
        {
            Enclosing = null;
        }

        /// <summary>
        /// Initializes a new instance of the Environment class with a specified enclosing environment.
        /// </summary>
        /// <param name="enclosing">The enclosing environment.</param>
        public Environment(Environment enclosing)
        {
            this.Enclosing = enclosing;
        }

        /// <summary>
        /// Retrieves the value of a variable.
        /// </summary>
        /// <param name="name">The token representing the variable name.</param>
        /// <returns>The value of the variable.</returns>
        /// <exception cref="RuntimeError">Thrown when the variable is not found.</exception>
        public object Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                return values[name.Lexeme];
            }
            if (Enclosing != null) return Enclosing.Get(name);

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }

        /// <summary>
        /// Defines a new variable in the environment.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public void Define(string name, object value)
        {
            values.Add(name, value);
        }

        /// <summary>
        /// Retrieves the value of a variable at a specified distance in the environment chain.
        /// </summary>
        /// <param name="distance">The distance to the target environment.</param>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The value of the variable.</returns>
        public object GetAt(int distance, string name)
        {
            return Ancestor(distance).values[name];
        }

        /// <summary>
        /// Assigns a value to a variable at a specified distance in the environment chain.
        /// </summary>
        /// <param name="distance">The distance to the target environment.</param>
        /// <param name="name">The token representing the variable name.</param>
        /// <param name="value">The value to assign to the variable.</param>
        public void AssignAt(int distance, Token name, object value)
        {
            Ancestor(distance).values[name.Lexeme] = value;
        }

        /// <summary>
        /// Finds the ancestor environment at a specified distance.
        /// </summary>
        /// <param name="distance">The distance to the target environment.</param>
        /// <returns>The ancestor environment.</returns>
        private Environment Ancestor(int distance)
        {
            Environment environment = this;
            for (int i = 0; i < distance; i++)
            {
                environment = environment.Enclosing;
            }
            return environment;
        }

        /// <summary>
        /// Assigns a value to a variable in the current environment or in an enclosing environment.
        /// </summary>
        /// <param name="name">The token representing the variable name.</param>
        /// <param name="value">The value to assign to the variable.</param>
        /// <exception cref="RuntimeError">Thrown when the variable is not found in any scope.</exception>
        internal void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }

            if (Enclosing != null)
            {
                Enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }
    }
}
