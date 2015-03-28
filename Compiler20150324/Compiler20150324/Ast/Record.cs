using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{dict}")]
    public class Record : Node
    {
        public Dictionary<string, Node> dict = new Dictionary<string, Node>();
        public Record()
        {

        }
        public Record(Token tok)
            : base(tok)
        {

        }
        public override Value Typecheck(Scope s)
        {
            return Value.ANY;
        }
        public override string ToString()
        {
            return dict.Aggregate("", (i, a) => i + a.Key.ToString() + ":" + a.Value.ToString());
        }
    }
}
