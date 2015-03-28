using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{func}{arguments}")]
    public class FunctionCall : Node
    {
        public Identifier func;
        public Argument arguments;
        public FunctionCall()
        {

        }
        public FunctionCall(Token tok, Identifier func, Argument arguments)
            : base(tok)
        {
            this.func = func;
            this.arguments = arguments;
        }
        public override Value Interpret(Scope s)
        {
            Value fun = func.Interpret(s);
            if(fun is Closure)
            {
                Closure clourse=(Closure)fun;
                List<Identifier> parameters = clourse.fun.parameters;
                Scope funScope = new Scope(clourse.env);
                if (clourse.properties != null)
                {
                    CheckParamsType(clourse.properties, s);
                }
                if (this.arguments.elements.Count != parameters.Count)
                {
                    throw new CodeException(this.token, "参数个数错误");
                }
                if(this.arguments.elements.Count!=0)
                {
                    for (int i = 0; i < arguments.elements.Count; i++) 
                    {
                        Value value = arguments.elements[i].Interpret(s);
                        funScope.putValue(parameters[i].value, value);
                    }
                }
                return clourse.fun.body.Interpret(funScope);
            }
            else if (fun is Primitives) 
            {
                Primitives prim = (Primitives)fun;
                List<Value> args = InterpretList(arguments.elements, s);
                return prim.Apply(args, this);
            } 
            return Value.VOID;
        }
        public static bool CheckParamsType(Scope properties, Scope s)
        {
            foreach (String key in properties.table.Keys)
            {
                if (key.Equals("->"))
                {
                    continue;
                }
                object type = properties.lookupPropertyLocal(key, "type");
                if (type == null)
                {
                    continue;
                }
                else if (type is Value)
                {
                    Value existing = s.lookup(key);
                    if (existing == null)
                    {
                        s.putValue(key, (Value)type);
                    }
                }
                else
                {
                    Debug.WriteLine("illegal type, shouldn't happen" + type);
                }

            }
            return true;
        }
        public override Value Typecheck(Scope s)
        {
            Value fun = this.func.Typecheck(s);
            if (fun is FunctionType)
            {
                FunctionType funcType = (FunctionType)fun;
                Scope funScope = new Scope(funcType.env);
                List<Identifier> parameters = funcType.fun.parameters;
                List<Node> args = arguments.elements;
                if (funcType.properties != null)
                {
                    CheckParamsType(funcType.properties, s);
                }
                if (arguments.elements.Count != parameters.Count)
                {
                    Debug.WriteLine("参数个数不一致");
                }
                foreach (var param in parameters)
                {
                    int i = 0;
                    string paramName = param.value;
                    Value valueType = args[i].Typecheck(s);
                    Value paramType = s.lookup(paramName);
                    if (paramType != null)
                    {
                        if (!Value.Subtype(valueType, paramType, false))
                        {
                            Debug.WriteLine("实参与形参类型不匹配");
                        }
                        funScope.putValue(paramName, valueType);
                    }
                    else
                    {
                        funScope.putValue(paramName, valueType);
                    }
                    i++;
                }
                if (funcType.properties != null)
                {
                    object retType = funcType.properties.lookupPropertyLocal("->", "type");
                    if (retType != null)
                    {
                        if (retType is Node)
                        {
                            return ((Node)retType).Typecheck(funScope);
                        }
                        else
                        {
                            Debug.WriteLine("错误的返回类型");
                            return null;
                        }
                    }
                    else
                    {
                        if (TypeChecker.self.callStack.Contains(fun))
                        {
                            Debug.WriteLine("You must specify return type for recursive functions: " + func.ToString());
                            return null;
                        }
                        TypeChecker.self.callStack.Add((FunctionType)fun);
                        Value actual = funcType.fun.body.Typecheck(funScope);
                        TypeChecker.self.callStack.Remove((FunctionType)fun);
                        return actual;
                    }
                }
                return Value.VOID;
            }
            else if (fun is Primitives)
            {
                List<Node> argsNode = arguments.elements;
                Primitives prim = (Primitives)fun;
                if (prim.arity >= 0 && argsNode.Count != prim.arity)
                {
                    Debug.WriteLine(this, "incorrect number of arguments for primitive " +
                            prim.name + ", expecting " + prim.arity + ", but got " +
                            argsNode.Count);
                    return null;
                }
                else
                {
                    List<Value> args = TypecheckList(argsNode, s);
                    return prim.Typecheck(args, this);
                }
            }
            Debug.WriteLine("not a function call!");
            return Value.VOID;///
        }
        public override string ToString()
        {
            return func + "(" + arguments.ToString() + ")";
        }
    }
}
