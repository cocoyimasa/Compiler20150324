using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{value}")]
    public class FloatNum : Node
    {
        public double value;
        public FloatNum()
        {

        }
        public FloatNum(Token tok)
            : base(tok)
        {
            value = Convert.ToDouble(tok.Value);

        }
        public override Value Interpret(Scope s)
        {
            return new FloatType(value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.FLOAT;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
