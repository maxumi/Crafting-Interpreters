using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crafting_Interpreters
{
    internal class Scanner
    {
        private string source { get; set; } = "";
        private List<Token> tokens = new List<Token>();


        public Scanner(string source)
        {
            this.source = source;
        }
        // WIP
        public List<Token> ScanTokens()
        {
            return new List<Token>();
        }

    }
}
