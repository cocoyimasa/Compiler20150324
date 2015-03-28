using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{param}{parameters}{body}")]
    public class LambdaExpression : Node
    {
        public Scope paramTable;
        public List<Identifier> parameters = new List<Identifier>();
        public Node body;
        public LambdaExpression()
        {

        }
        public LambdaExpression(Token tok, List<Identifier> parameters, Scope paramTable, Node body)
            : base(tok)
        {
            this.paramTable = paramTable;
            this.body = body;
            this.parameters = parameters;
        }
        public override Value Interpret(Scope s)
        {
            Scope evaled = FunctionStatement.CheckProperties(paramTable, s);
            FunctionStatement f = new FunctionStatement(token, "lambda_" + parameters.Aggregate("", (init, p) => init + p.ToString()), this.parameters, this.paramTable, this.body);
            Closure closure = new Closure(f, evaled, s);
            s.putValue(f.funcName, closure);
            return new Closure(f, evaled, s);
        }
        public override Value Typecheck(Scope s)
        {
            Scope evaled = FunctionStatement.CheckProperties(paramTable, s);
            FunctionStatement func = new FunctionStatement(token, "", parameters, paramTable, body);
            FunctionType ft = new FunctionType(func, evaled, s);
            //TypeChecker.self.uncalled.add(ft);
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
