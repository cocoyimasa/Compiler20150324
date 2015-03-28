using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{token}")]
    public class Node
    {
        public Token token;
        public Node()
        { }
        public Node(Token tok)
        {
            this.token = tok;
        }
        virtual public Value Interpret(Scope s)
        {
            return Value.ANY;
        }
        virtual public List<Value> InterpretList(List<Node> elements, Scope s)
        {
            List<Value> values = new List<Value>();
            foreach (var elem in elements)
            {
                values.Add(elem.Interpret(s));
            }
            return values;
        }
        virtual public Value Typecheck(Scope s)
        {
            return Value.ANY;
        }
        public static Value Typecheck(Node node, Scope s)
        {
            return node.Typecheck(s);
        }
        public static List<Value> TypecheckList(List<Node> nodes, Scope s)
        {
            List<Value> types = new List<Value>();
            foreach (Node n in nodes)
            {
                types.Add(n.Typecheck(s));
            }
            return types;
        }
        public override string ToString()
        {
            return token.Type.ToString() + " " + token.Value;
        }
    }
}
