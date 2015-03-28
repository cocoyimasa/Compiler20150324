using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.BackUp
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
        virtual public List<Value> InterpretList(List<Node> elements,Scope s)
        {
            List<Value> values=new List<Value>();
            foreach(var elem in elements)
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
            return token.Type.ToString()+" "+token.Value;
        }
    }
    [DebuggerDisplay("{value}")]
    public class Delimeter:Node
    {
        public string value;
        public static HashSet<string> delims=new HashSet<string>();
        public static Dictionary<string, string> delimMap=new Dictionary<string,string>();
        public Delimeter(Token tok)
            :base(tok)
        {
            value = tok.Value;
        }
        public static void AddDelimPair(string open,string close)
        {
            delims.Add(open);
            delims.Add(close);
            delimMap.Add(open,close);
        }
        public static void addDelimiter(string delim)
        {
            delims.Add(delim);
        }
        public static bool isDelimeter(string delim)
        {
            return delims.Contains(delim);
        }
        public static bool isOpen(string delim)
        {
            return delimMap.Keys.Contains(delim);
        }
        public static bool isClose(string delim)
        {
            return delimMap.Values.Contains(delim);
        }
        public static bool match(Node open, Node close) 
        {
            if (!(open is Delimeter) ||
                !(close is Delimeter))
            {
                return false;
            }
            String matched = delimMap[((Delimeter) open).token.Value];
            return matched != null && matched.Equals(((Delimeter) close).token.Value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.ANY;
        }
    }
    [DebuggerDisplay("{value}")]
    public class IntNum : Node
    {
        public int value;
        public int baseType;
        public IntNum()
        {
        }
        public IntNum(Token tok)
            :base(tok)
        {
            value = Convert.ToInt32(tok.Value);
        }
        public override Value Interpret(Scope s)
        {
            return new IntType(value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.INT;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    [DebuggerDisplay("{value}")]
    public class FloatNum : Node
    {
        public double value;
        public FloatNum()
        {

        }
        public FloatNum(Token tok)
            :base(tok)
        {
            value = Convert.ToDouble(tok.Value);

        }
        public override Value Interpret(Scope s)
        {
            return new FloatType(value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.FLOAT;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    [DebuggerDisplay("{token}")]
    public class StringNode : Node
    {
        public StringNode()
        {
        }
        public StringNode(Token tok)
            :base(tok)
        {

        }
        public override Value Interpret(Scope s)
        {
            return new StringType(token.Value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.STRING;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    [DebuggerDisplay("{value}")]
    public class BoolNode : Node
    {
        public bool value;
        public BoolNode()
        {

        }
        public BoolNode(Token tok)
            :base(tok)
        {
            if(tok.Value.Equals("true"))
            {
                value = true;
            }
            else
            {
                value = false;
            }
        }
        public override Value Interpret(Scope s)
        {
            return new BoolType(value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.BOOL;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    [DebuggerDisplay("{array}")]
    public class ArrayNode : Node
    {
        public List<Node> array;
        public ArrayNode()
        {

        }
        public ArrayNode(Token tok,List<Node> array)
            : base(tok)
        {
            this.array = array;
        }
        public override Value Interpret(Scope s)
        {
            return new ArrayType(InterpretList(array, s));
        }
        public override Value Typecheck(Scope s)
        {
            return new ArrayType(TypecheckList(array,s));
        }
        public override string ToString()
        {
            return array.Aggregate("",(i,a)=>i+a.ToString());
        }
    }
    [DebuggerDisplay("{value}")]
    public class Identifier : Node
    {
        public string value;
        public Identifier()
        {

        }
        public Identifier(Token tok,string val)
            :base(tok)
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
                throw new CodeException(this.token,"unbound variable: " + this.value);
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
                return Value.INT;
            }
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    [DebuggerDisplay("{statements}")]
    public class Block : Node
    {
        public List<Node> statements = new List<Node>();
        public Block()
        {

        }
        public Block(Token tok, List<Node> statements)
            :base(tok)
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
            if(statements.Count>0)
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
    [DebuggerDisplay("{dict}")]
    public class Record : Node
    {
        public Dictionary<string, Node> dict = new Dictionary<string, Node>();
        public Record()
        {

        }
        public Record(Token tok)
            :base(tok)
        {

        }
        public override Value Typecheck(Scope s)
        {
            return Value.ANY;
        }
        public override string ToString()
        {
            return dict.Aggregate("", (i, a) => i + a.Key.ToString()+":"+a.Value.ToString());
        }
    }
    [DebuggerDisplay("{elements}")]
    public class TupleNode : Node
    {
        public List<Node> elements=new List<Node>();
        public TupleNode()
        {

        }
        public TupleNode(Token tok,List<Node> elements)
            :base(tok)
        {
            this.elements = elements;
        }
        public override Value Interpret(Scope s)
        {
            return new ArrayType(InterpretList(elements, s));
        }
        public override Value Typecheck(Scope s)
        {
            return new ArrayType(TypecheckList(elements,s));
        }
        public override string ToString()
        {
            return elements.Aggregate("", (i, a) => i + a.ToString());
        }
    }
    [DebuggerDisplay("{varName}{value}")]
    public class LetStatement:Node
    {
        public string varName;
        public Node value;
        public LetStatement()
        {

        }
        public LetStatement(Token tok,string varName,Node value)
            :base(tok)
        {
            this.varName = varName;
            this.value = value;
        }
        public override Value Interpret(Scope s)
        {
            return value.Interpret(s);
        }
        public override Value Typecheck(Scope s)
        {
            return value.Typecheck(s);
        }
        public override string ToString()
        {
            return token.Value+" "+varName+value.ToString();
        }
    }
    [DebuggerDisplay("{test}{thenBody}{elseBody}")]
    public class IfStatement : Node
    {
        public Node test;
        public Node thenBody;
        public Node elseBody;
        public IfStatement()
        {
        }
        public IfStatement(Token tok,Node test,Node thenBody,Node elseBody)
            :base(tok)
        {
            this.test = test;
            this.thenBody = thenBody;
            this.elseBody = elseBody;
        }
        public override Value Interpret(Scope s)
        {
            BoolType boolValue=(BoolType)test.Interpret(s);
            if(boolValue.value)
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
            if (!(tv is BoolType)) {
                Debug.WriteLine(test, "test is not boolean: " + tv);
                return null;
            }
            Value type1 = Typecheck(thenBody, s);
            Value type2 = Typecheck(elseBody, s);
            return UnionType.Union(type1, type2);
        }
        public override string ToString()
        {
            return test.ToString() + thenBody.ToString()+elseBody.ToString();
        }
    }

    [DebuggerDisplay("{test}{body}")]
    public class WhileStatement : Node
    {
        public Node test;
        public Node body;
        public WhileStatement()
        {

        }
        public WhileStatement(Token tok,Node test,Node body)
            :base(tok)
        {
            this.test = test;
            this.body = body;
        }
        public override Value Interpret(Scope s)
        {
            BoolType testVal=(BoolType)test.Interpret(s);
            if(testVal.value)
            {
                return body.Interpret(s);
            }
            return null;
        }
        public override Value Typecheck(Scope s)
        {
            Value tv = Typecheck(test, s);
            Value type = Typecheck(body, s);
            return type;
        }
        public override string ToString()
        {
            return test.ToString()+body.ToString();
        }
    }
    //to be removed
    public class FuncDeclaration : Node
    {
        public string funcName;
        public List<Node> parameters = new List<Node>();
        public FuncDeclaration()
        {

        }
        public FuncDeclaration(Token tok, string funcName, List<Node> parameters)
            : base(tok)
        {
            this.funcName = funcName;
            this.parameters = parameters;
        }
        public override Value Typecheck(Scope s)
        {
            return Value.ANY;
        }
    }
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
            :base(tok)
        {
            this.funcName = funcName;
            this.paramTable = paramTable;
            this.parameters = parameters;
            this.body = body;
        }
        public override Value Interpret(Scope s)
        {
            return base.Interpret(s);
        }
        public static Scope CheckProperties(Scope paramTable, Scope s)
        {
            if(paramTable.table.Keys.Count==0)
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
                            Value vValue = Typecheck((Node)v,s);
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
            Scope evaled=CheckProperties(paramTable, s);

            FunctionType ft = new FunctionType(this, evaled, s);
            TypeChecker.self.uncalled.Add(ft);
            s.putValue(this.funcName, ft);
            return ft;
        }
        public override string ToString()
        {
            if(parameters.Count==0)
            {
                return "( function:" + body.ToString() + ")";
            }
            return 
                "( function:[" + 
                parameters.Aggregate("", (init, p) => init + p.ToString()) + 
                "]("+ 
                body.ToString() + ")";
        }
    }
    [DebuggerDisplay("{elements}")]
    public class Argument : Node 
    {
        public List<Node> elements=new List<Node>();
        public Argument()
        {

        }
        public Argument(Token tok,List<Node> elements)
            : base(tok)
        {
            this.elements = elements;
        }
        public override Value Typecheck(Scope s)
        {
            return new ArrayType(TypecheckList(elements,s));//
        }
        public override string ToString()
        {
            return elements.Aggregate("", (i, a) => i + a.ToString()+",");
        }

    }
    [DebuggerDisplay("{func}{arguments}")]
    public class FunctionCall : Node
    {
        public Identifier func;
        public Argument arguments;
        public FunctionCall()
        {

        }
        public FunctionCall(Token tok,Identifier func,Argument arguments)
            :base(tok)
        {
            this.func = func;
            this.arguments = arguments;
        }
        public static bool CheckParamsType(Scope properties,Scope s)
        {
            foreach (String key in properties.table.Keys) {
                if (key.Equals("->")) {
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
            if(fun is FunctionType)
            {
                FunctionType funcType = (FunctionType)fun;
                Scope funScope=new Scope(funcType.env);
                List<Identifier> parameters=funcType.fun.parameters;
                List<Node> args = arguments.elements;
                if(funcType.properties!=null)
                {
                    CheckParamsType(funcType.properties, s);
                }
                if(arguments.elements.Count!=parameters.Count)
                {
                    Debug.WriteLine("参数个数不一致");
                }
                foreach (var param in parameters)
                {
                    int i = 0;
                    string paramName = param.value;
                    Value valueType = args[i].Typecheck(s);
                    Value paramType=s.lookup(paramName);
                    if(paramType!=null)
                    {
                        if (!Value.Subtype(valueType, paramType, false))
                        {
                            Debug.WriteLine("实参与形参类型不匹配");
                        }
                    }
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
            else if(fun is Primitives)
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
            return func+"("+arguments.ToString()+")";
        }
    }
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
            :base(tok)
        {
            this.paramTable = paramTable;
            this.body = body;
            this.parameters = parameters;
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
    public class ReturnStatement:Node
    {
        public Node returnValue;
        public Value type;
        public ReturnStatement()
        {

        }
        public ReturnStatement(Token tok,Node returnValue)
            :base(tok)
        {
            this.returnValue = returnValue;
        }
        public override Value Interpret(Scope s)
        {
            return returnValue.Interpret(s);
        }
        public override Value Typecheck(Scope s)
        {
            type= returnValue.Typecheck(s);
            return type;
        }
        public override string ToString()
        {
            return type.Type();
        }
    }

}
