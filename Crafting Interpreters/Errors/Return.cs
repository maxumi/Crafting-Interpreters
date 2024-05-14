using CraftingInterpreters.Lox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crafting_Interpreters.Errors
{
    internal class Return : Exception
    {
        public readonly object Value;
        public Return(object value) : base(null, null)
        {
            Value = value;
        }
        public Return(object value, Exception innerException) : base(null, innerException)
        {
            Value = value;
        }
    }
}
