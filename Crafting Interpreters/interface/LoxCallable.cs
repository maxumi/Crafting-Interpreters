using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crafting_Interpreters.Interpreters;

namespace Crafting_Interpreters._interface
{
    public interface ICallable
    {
        int Arity();
        object Call(Interpreter interpreter, List<object> arguments);
    }
}
