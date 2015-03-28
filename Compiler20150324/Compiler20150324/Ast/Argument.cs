using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{elements}")]
    public class Argument : Node
    {
        public List<Node> elements = new List<Node>();
        public Argument()
        {

        }
        public Argument(Token tok, List<Node> elements)
            : base(tok)
        {
            this.elements = elements;
        }
        public override Value Typecheck(Scope s)
        {
            return new ArrayType(TypecheckList(elements, s));//
        }
        public override string ToString()
        {
            return elements.Aggregate("", (i, a) => i + a.ToString() + ",");
        }

    }
}
