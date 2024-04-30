using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crafting_Interpreters.Errors
{
    public class RuntimeError : Exception
    {
        public Token Token { get; }
        public RuntimeError(Token token, string message) : base(message) // Calls the base Exception class constructor with the message.
        {
            this.Token = token;
        }

    }
}
