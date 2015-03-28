using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{value}")]
    public class Identifier : Node
    {
        public string value;
        public Identifier()
        {

        }
        public Identifier(Token tok, string val)
            : base(tok)
        {
            this.value = val;
        }
        public static Identifier NewIdentifier(string val)
        {
            return new Identifier(new Token(TokenType.Identifier, val, 0, 0), val);
        }
        public override Value Interpret(Scope s)
        {
            Value v = s.lookup(value);
            if (v != null)
            {
                return v;
            }
            else
            {
                throw new CodeException(this.token, "unbound variable: " + this.value);
            }
        }
        public override Value Typecheck(Scope s)
        {
            Value v = s.lookup(value);
            if (v != null)
            {
                return v;
            }
            else
            {
                //编译期静态类型推断
                Debug.WriteLine("unbound variable: " + this.value);
                return Value.ANY;
            }
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
