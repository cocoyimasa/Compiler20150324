using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{token}")]
    public class StringNode : Node
    {
        public StringNode()
        {
        }
        public StringNode(Token tok)
            : base(tok)
        {

        }
        public override Value Interpret(Scope s)
        {
            return new StringType(token.Value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.STRING;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
