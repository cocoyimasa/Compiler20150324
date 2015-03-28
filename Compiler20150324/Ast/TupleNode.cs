using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{elements}")]
    public class TupleNode : Node
    {
        public List<Node> elements = new List<Node>();
        public TupleNode()
        {

        }
        public TupleNode(Token tok, List<Node> elements)
            : base(tok)
        {
            this.elements = elements;
        }

        public override Value Interpret(Scope s)
        {
            List<Value> values = InterpretList(elements, s);
            List<Value> results = new List<Value>();
            foreach(var val in values)
            {
                if(!(val is Closure))
                {
                    results.Add(val);
                }
            }
            return new OutputType(results);
        }
        public override Value Typecheck(Scope s)
        {
            return new ArrayType(TypecheckList(elements, s));
        }
        public override string ToString()
        {
            return elements.Aggregate("", (i, a) => i + a.ToString());
        }
    }
}
