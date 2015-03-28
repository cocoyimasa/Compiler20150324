using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{funcName}{body}")]
    public class FunctionStatement : Node
    {
        public string funcName;
        public Scope paramTable;
        public List<Identifier> parameters = new List<Identifier>();
        public Node body;
        public FunctionStatement()
        {

        }
        public FunctionStatement(Token tok, string funcName, List<Identifier> parameters, Scope paramTable, Node body)
            : base(tok)
        {
            this.funcName = funcName;
            this.paramTable = paramTable;
            this.parameters = parameters;
            this.body = body;
        }
        public override Value Interpret(Scope s)
        {
            Scope evaled = CheckProperties(paramTable, s);
            Closure closure = new Closure(this, evaled, s);
            s.putValue(this.funcName,closure);
            return closure;
        }
        public static Scope CheckProperties(Scope paramTable, Scope s)
        {
            if (paramTable==null)
            {
                return null;
            }
            Scope evaled = new Scope();
            foreach (var field in paramTable.table.Keys)
            {
                if (field.Equals("->"))
                {
                    evaled.PutProperties(field, paramTable.lookupAllProps(field));
                }
                else
                {
                    Dictionary<string, object> props = paramTable.lookupAllProps(field);
                    foreach (var e in props)
                    {
                        object v = e.Value;
                        if (v is Node)
                        {
                            Value vValue = Typecheck((Node)v, paramTable);
                            evaled.Put(field, e.Key, vValue);
                        }
                        else
                        {
                            Debug.WriteLine("property is not a node, parser bug: " + v);
                        }
                    }
                }
            }
            return evaled;
        }
        public override Value Typecheck(Scope s)
        {
            Scope evaled = CheckProperties(paramTable, s);

            FunctionType ft = new FunctionType(this, evaled, s);
            TypeChecker.self.uncalled.Add(ft);
            s.putValue(this.funcName, ft);
            return ft;
        }
        public override string ToString()
        {
            if (parameters.Count == 0)
            {
                return "( function:" + body.ToString() + ")";
            }
            return
                "( function:[" +
                parameters.Aggregate("", (init, p) => init + p.ToString()) +
                "](" +
                body.ToString() + ")";
        }
    }
}
