using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{test}{body}")]
    public class WhileStatement : Node
    {
        public Node test;
        public Node body;
        public WhileStatement()
        {

        }
        public WhileStatement(Token tok, Node test, Node body)
            : base(tok)
        {
            this.test = test;
            this.body = body;
        }
        public override Value Interpret(Scope s)
        {
            BoolType testVal = (BoolType)test.Interpret(s);
            if (testVal.value)
            {
                return body.Interpret(s);
            }
            return null;
        }
        public override Value Typecheck(Scope s)
        {
            Value tv = Typecheck(test, s);
            Value type = Typecheck(body, s);
            return type;
        }
        public override string ToString()
        {
            return test.ToString() + body.ToString();
        }
    }
}
