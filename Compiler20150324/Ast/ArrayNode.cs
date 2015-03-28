using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{array}")]
    public class ArrayNode : Node
    {
        public List<Node> array;
        public ArrayNode()
        {

        }
        public ArrayNode(Token tok, List<Node> array)
            : base(tok)
        {
            this.array = array;
        }
        public override Value Interpret(Scope s)
        {
            return new ArrayType(InterpretList(array, s));
        }
        public override Value Typecheck(Scope s)
        {
            return new ArrayType(TypecheckList(array, s));
        }
        public override string ToString()
        {
            return array.Aggregate("", (i, a) => i + a.ToString());
        }
    }
}
