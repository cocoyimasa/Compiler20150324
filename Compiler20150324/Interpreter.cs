using Compiler.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Interpreter
    {
        public Node program;
        public Interpreter(Node program)
        {
            this.program = program;
        }
        public Value Interpret()
        {
            Value val=program.Interpret(Scope.initScope());
            return val;
        }
    }
}
