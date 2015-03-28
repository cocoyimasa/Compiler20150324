using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{returnValue}{type}")]
    public class ReturnStatement : Node
    {
        public Node returnValue;
        public Value type;
        public ReturnStatement()
        {

        }
        public ReturnStatement(Token tok, Node returnValue)
            : base(tok)
        {
            this.returnValue = returnValue;
        }
        public override Value Interpret(Scope s)
        {
            return returnValue.Interpret(s);
        }
        public override Value Typecheck(Scope s)
        {
            type = returnValue.Typecheck(s);
            return type;
        }
        public override string ToString()
        {
            return type.Type();
        }
    }
}
