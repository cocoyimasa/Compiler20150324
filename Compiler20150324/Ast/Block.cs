using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{statements}")]
    public class Block : Node
    {
        public List<Node> statements = new List<Node>();
        public Block()
        {

        }
        public Block(Token tok, List<Node> statements)
            : base(tok)
        {
            this.statements = statements;
        }
        public override Value Interpret(Scope s)
        {
            s = new Scope(s);
            if (statements.Count > 0)
            {
                for (int i = 0; i < statements.Count - 1; i++)
                {
                    statements[i].Interpret(s);
                }
                return statements[statements.Count - 1].Interpret(s);
            }
            else
            {
                return Value.VOID;
            }
        }
        public override Value Typecheck(Scope s)
        {
            s = new Scope(s);
            if (statements.Count > 0)
            {
                for (int i = 0; i < statements.Count - 1; i++)
                {
                    statements[i].Typecheck(s);
                }
                return statements[statements.Count - 1].Typecheck(s);
            }
            else
            {
                return Value.VOID;
            }
        }
        public override string ToString()
        {
            return statements.Aggregate("", (i, a) => i + a.ToString());
        }
    }
}
