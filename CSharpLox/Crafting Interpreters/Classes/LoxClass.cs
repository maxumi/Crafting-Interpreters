using Crafting_Interpreters.interfaces;
using Crafting_Interpreters.Interpreters;
using Crafting_Interpreters.LoxFunctions;
using System;
using System.Collections.Generic;

namespace Crafting_Interpreters.Classes
{
    /// <summary>
    /// Represents a class in the Lox language. Implements the ICallable interface.
    /// </summary>
    public class LoxClass : ICallable
    {
        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        public readonly string name;

        /// <summary>
        /// Gets the superclass of this class, if any.
        /// </summary>
        private readonly LoxClass superclass;

        /// <summary>
        /// A dictionary of methods defined in the class.
        /// </summary>
        private readonly Dictionary<string, LoxFunction> methods;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoxClass"/> class with the specified name, superclass, and methods.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <param name="superclass">The superclass of the class.</param>
        /// <param name="methods">The methods defined in the class.</param>
        public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.superclass = superclass;
            this.methods = methods;
        }

        /// <summary>
        /// Gets the number of arguments the class's initializer expects.
        /// </summary>
        /// <returns>The number of arguments.</returns>
        public int Arity()
        {
            LoxFunction initializer = FindMethod("init");
            if (initializer == null) return 0;
            return initializer.Arity();
        }

        /// <summary>
        /// Calls the class, which creates a new instance of the class and initializes it.
        /// </summary>
        /// <param name="interpreter">The interpreter executing the call.</param>
        /// <param name="arguments">The arguments passed to the initializer.</param>
        /// <returns>The new instance of the class.</returns>
        public object Call(Interpreter interpreter, List<object> arguments)
        {
            // Create a new instance of the class.
            LoxInstance instance = new LoxInstance(this);

            // Find the initializer method, if any.
            LoxFunction initializer = FindMethod("init");
            if (initializer != null)
            {
                // Bind the initializer to the new instance and call it.
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            // Return the new instance.
            return instance;
        }

        /// <summary>
        /// Returns a string that represents the class.
        /// </summary>
        /// <returns>A string representation of the class.</returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Finds a method defined in the class or its superclass.
        /// </summary>
        /// <param name="_name">The name of the method to find.</param>
        /// <returns>The method if found, otherwise null.</returns>
        public LoxFunction FindMethod(string _name)
        {
            // Check if the method is defined in the current class.
            if (methods.ContainsKey(_name))
            {
                return methods[_name];
            }

            // If not found and the class has a superclass, check the superclass.
            if (superclass != null)
            {
                return superclass.FindMethod(_name);
            }

            // If the method is not found in the class or superclass, return null.
            return null;
        }
    }
}
