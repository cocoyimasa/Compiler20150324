using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    public class AssignChecker
    {
        public static void CheckDuplicate(Node pattern)
        {
            CheckDuplicate1(pattern, new HashSet<string>());
        }
        public static void CheckDuplicate1(Node pattern,HashSet<string> set)
        {
            if(pattern is Identifier)
            {
                string name = ((Identifier)pattern).value;
                if(set.Contains(name))
                {
                    throw new CodeException(pattern.token, "重复的变量名");
                }
                set.Add(name);
            }
            else if(pattern is ArrayNode)
            {
                foreach(var arr in ((ArrayNode)pattern).array)
                {
                    CheckDuplicate1(arr, set);
                }
            }
            //
        }
        public static void Assign(Node pattern,Value value,Scope env)
        {
            if(pattern is Identifier)
            {
                string name = ((Identifier)pattern).value;
                env.putValue(name, value);
            }
            else if(pattern is ArrayNode)
            {
                List<Node> names=((ArrayNode)pattern).array;
                if(value is ArrayType)
                {
                    List<Value> values = ((ArrayType)value).values;
                    if (names.Count != values.Count)
                    {
                        throw new CodeException(pattern.token, "变量与值个数不一致");
                    }
                    for (int i = 0; i < names.Count; i++)
                    {
                        Assign(names[i], values[i], env);
                    }
                }
                else
                {
                    throw new CodeException(pattern.token, "值的类型错误，应该是ArrayType");
                }
            }
        }
    }
    [DebuggerDisplay("{varName}{value}")]
    public class LetStatement : Node
    {
        public Node pattern;
        public Node value;
        public LetStatement()
        {

        }
        public LetStatement(Token tok, Node pattern, Node value)
            : base(tok)
        {
            this.pattern = pattern;
            this.value = value;
        }
        public override Value Interpret(Scope s)
        {
            Value element=value.Interpret(s);
            AssignChecker.CheckDuplicate(pattern);
            AssignChecker.Assign(pattern, element, s);
            return element;
        }
        public override Value Typecheck(Scope s)
        {
            Value element = value.Typecheck(s);
            AssignChecker.CheckDuplicate(pattern);
            AssignChecker.Assign(pattern, element, s);
            return element;
        }
        public override string ToString()
        {
            return token.Value + " " + pattern.ToString() + value.ToString();
        }
    }
}
