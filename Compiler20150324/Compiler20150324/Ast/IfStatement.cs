using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{test}{thenBody}{elseBody}")]
    public class IfStatement : Node
    {
        public Node test;
        public Node thenBody;
        public Node elseBody;
        public IfStatement()
        {
        }
        public IfStatement(Token tok, Node test, Node thenBody, Node elseBody)
            : base(tok)
        {
            this.test = test;
            this.thenBody = thenBody;
            this.elseBody = elseBody;
        }
        public override Value Interpret(Scope s)
        {
            BoolType boolValue = (BoolType)test.Interpret(s);
            if (boolValue.value)
            {
                return thenBody.Interpret(s);
            }
            else
            {
                return elseBody.Interpret(s);
            }
        }
        public override Value Typecheck(Scope s)
        {
            Value tv = Typecheck(test, s);
            if (!(tv is BoolType))
            {
                Debug.WriteLine(test, "test is not boolean: " + tv);
                return null;
            }
            Value type1 = Typecheck(thenBody, s);
            Value type2 = Typecheck(elseBody, s);
            return UnionType.Union(type1, type2);
        }
        public override string ToString()
        {
            return test.ToString() + thenBody.ToString() + elseBody.ToString();
        }
    }
}
