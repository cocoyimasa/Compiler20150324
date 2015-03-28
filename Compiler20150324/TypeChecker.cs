using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class TypeChecker
    {
        public static TypeChecker self;
        public HashSet<FunctionType> uncalled = new HashSet<FunctionType>();
        public HashSet<FunctionType> callStack = new HashSet<FunctionType>();
        public Value TypeCheck(Node program)
        {
            Scope s = Scope.initScope();
            Value ret = program.Typecheck(s);
            while (uncalled.Count!=0) {
            List<FunctionType> toRemove = new List<FunctionType>(uncalled);
            foreach (FunctionType ft in toRemove) {
                invokeUncalled(ft, s);
            }
            uncalled.RemoveWhere(u=>toRemove.Contains(u));
            }
            return ret;
        }
        public void invokeUncalled(FunctionType fun, Scope s)
        {
            Scope funScope = new Scope(fun.env);
            if (fun.properties != null)
            {
                FunctionStatement.CheckProperties(fun.properties, funScope);
            }

            TypeChecker.self.callStack.Add(fun);
            Value actual = fun.fun.body.Typecheck(funScope);
            TypeChecker.self.callStack.Remove(fun);
            if (fun.properties != null)
            {
                object retNode = fun.properties.lookupPropertyLocal("->", "type");
            
                if(retNode != null)
                {
                    if (!(retNode is Node))
                    {
                        throw new CodeException(Token.NewToken(""), "illegal return type: " + retNode);
                    }

                    Value expected = ((Node)retNode).Typecheck(funScope);
                    if (!Value.Subtype(actual, expected, true))
                    {
                        throw new CodeException(fun.fun.token,
                            "type error in return value, expected: " + expected + ", actual: " + actual);
                    }
                }
            }
        }
    }
}
