using Crafting_Interpreters._interface;
using Crafting_Interpreters.Interpreters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crafting_Interpreters
{
    internal class Clock : ICallable
    {
        public int Arity()
        {
            return 0;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
        }

        public override string ToString() { return "<native fn>"; }
    }
}
