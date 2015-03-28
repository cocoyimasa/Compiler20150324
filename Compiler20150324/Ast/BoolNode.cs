using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{value}")]
    public class BoolNode : Node
    {
        public bool value;
        public BoolNode()
        {

        }
        public BoolNode(Token tok)
            : base(tok)
        {
            if (tok.Value.Equals("true"))
            {
                value = true;
            }
            else
            {
                value = false;
            }
        }
        public override Value Interpret(Scope s)
        {
            return new BoolType(value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.BOOL;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
