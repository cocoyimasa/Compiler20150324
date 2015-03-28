using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{value}")]
    public class IntNum : Node
    {
        public int value;
        public int baseType;
        public IntNum()
        {
        }
        public IntNum(Token tok)
            : base(tok)
        {
            value = Convert.ToInt32(tok.Value);
        }
        public override Value Interpret(Scope s)
        {
            return new IntType(value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.INT;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
