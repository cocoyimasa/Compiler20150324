using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class PreParser
    {
        public static List<Token> tokens = new List<Token>();

        //public static Node ParseNode(Token token)
        //{

        //}
    }
    public class SyntacticalAnalyzer
    {

        public static List<Token> tokens;
        //===============================================================
        public static bool ReadToken(ref int nextToken,TokenType type,ref Token token)
        {
            if (tokens.Count == nextToken)
            {
                return false;
            }
            else if(tokens[nextToken].Type==type)
            {
                token = tokens[nextToken];
                nextToken++;
                return true;
            }
            return false;
        }
        public static bool ReadToken(ref int nextToken, string value,ref Token token)
        {
            if (tokens.Count == nextToken)
            {
                return false;
            }
            else if (tokens[nextToken].Value == value)
            {
                token = tokens[nextToken];
                nextToken++;
                return true;
            }
            return false;
        }
        public static bool LookAhead(ref int nextToken,string value)
        {
            if (tokens.Count == nextToken)
            {
                return false;
            }
            else if (tokens[nextToken].Value == value)
            {
                return true;
            }
            return false;
        }
        public static bool LookAhead(ref int nextToken, TokenType type)
        {
            if (tokens.Count == nextToken)
            {
                return false;
            }
            else if (tokens[nextToken].Type == type)
            {
                return true;
            }
            return false;
        }
        public static bool FindToken(ref int nextToken, ref int index,TokenType type)
        {
            while(true)
            {
                if (tokens.Count == nextToken+index)
                {
                    return false;
                }
                else if (tokens[nextToken+index].Type != type)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }
            return true;
        }
        public static bool FindToken(ref int nextToken, ref int index, string value)
        {
            while (true)
            {
                if (tokens.Count == nextToken + index)
                {
                    return false;
                }
                else if (tokens[nextToken + index].Value != value)
                {
                    index++;
                }
                else
                {
                    break;
                }
            }
            return true;
        }
        public static bool LookAgain(ref int nextToken, TokenType type)
        {
            if (tokens.Count == nextToken)
            {
                return false;
            }
            else if (tokens[nextToken+1].Type == type)
            {
                return true;
            }
            return false;
        }
        
        public static bool LookAgain(ref int nextToken, string value)
        {
            if (tokens.Count == nextToken)
            {
                return false;
            }
            else if (tokens[nextToken + 1].Value == value)
            {
                return true;
            }
            return false;
        }
        public static bool IsType(ref int nextToken,ref Token token)
        {
            return ReadToken(ref nextToken, TokenType.INT, ref token) ||
                ReadToken(ref nextToken, TokenType.FLOAT, ref token) ||
                ReadToken(ref nextToken, TokenType.STRING, ref token) ||
                ReadToken(ref nextToken, TokenType.BOOL, ref token) ||
                ReadToken(ref nextToken, TokenType.Identifier, ref token)
                ;
        }
        //====================================================================
        public static Node ParseProgram()
        {
            List<Node> elements = new List<Node>();
            int nextToken=0;
            Token token = new Token();
            while(true)
            {
                if (ReadToken(ref nextToken, TokenType.DEF, ref token))
                {
                    elements.Add(ParseFunction(ref nextToken, ref token));
                }
                else if (ReadToken(ref nextToken, TokenType.Identifier, ref token))
                {
                    elements.Add(ParseFuncCall(ref nextToken, ref token));
                    //函数调用一律不加分号
                }
                else
                {
                    //以上两个条件不匹配代表读到最后
                    break;
                }
            }
            return new TupleNode(Token.NewToken("program"),elements);
        }
        public static Node ParseBlock(ref int nextToken, ref Token token)
        {
            List<Node> stList = ParseStatementList(ref nextToken,ref token);
            return new Block(Token.NewToken("block"),stList);
            
        }
        public static Scope ParseParameter(string name, out List<Identifier> parameters, ref int nextToken, ref Token token)
        {
            parameters = new List<Identifier>();
            Scope scope = new Scope();
            while (ReadToken(ref nextToken, TokenType.Identifier, ref token))
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                Identifier id = new Identifier(token,token.Value);
                parameters.Add(id);
                if(ReadToken(ref nextToken,":",ref token) && 
                    IsType(ref nextToken,ref token))
                {
                    Identifier type=new Identifier(token,token.Value);
                    dict.Add("type", type);
                }
                else if(LookAhead(ref nextToken,","))
                {
                    Identifier type = new Identifier(Token.NewToken(":type"), "unknow");
                    dict.Add("type", type);
                }
                if(!ReadToken(ref nextToken,",",ref token))
                {
                    break;
                }
                scope.PutProperties(id.value, dict);
            }
            return scope;

        }
        public static Node ParseFunction(ref int nextToken, ref Token token)
        {
            Token firstToken=null;
            string funcName = null;
            List<Identifier> parameters = null;
            Scope scope = null;
            Node body = null;
            if(ReadToken(ref nextToken, TokenType.Identifier, ref token))
            {
                funcName = token.Value;
                firstToken=token;
                if(ReadToken(ref nextToken, "(", ref token))
                {
                    scope = ParseParameter(funcName,out parameters, ref nextToken, ref token);
                    if (!ReadToken(ref nextToken, ")", ref token))
                    {
                        throw new CodeException(token,"缺少)");
                    }
                    if(ReadToken(ref nextToken,":",ref token)&&
                        ReadToken(ref nextToken,TokenType.Identifier,ref token))
                    {
                        scope.Put("->", "type",new Identifier(token,token.Value));
                    }
                }
            }
            if (ReadToken(ref nextToken, "{", ref token))
            {
                body = ParseBlock(ref nextToken, ref token);
            }
            if (!ReadToken(ref nextToken, "}", ref token))
            {
                throw new CodeException(token, "缺少}");
            }
            if(body is Block)
            {
                Block block=(Block)body;
                try
                {
                    List<Node> returnArray=
                        block.statements.Where(s => s.token.Value == "return").ToList();
                    if(returnArray!=null && returnArray.Count!=0)
                    {
                        scope.Put("->", "type", (ReturnStatement)returnArray[0]);
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine("函数"+funcName+"无返回值");
                }
            }
            return new FunctionStatement(Token.NewToken("function"), funcName, parameters, scope,body);
        }
        public static Node ParseFuncCall(ref int nextToken, ref Token token)
        {
            Identifier func = new Identifier(token,token.Value);
            Argument arguments = new Argument();
            if(ReadToken(ref nextToken,"(",ref token))
            {
                arguments = (Argument)ParseArgument(ref nextToken, ref token);
            }
            //不必匹配；
            return new FunctionCall(Token.NewToken("funcCall"), func, arguments);

        }
        public static Node ParseArgument(ref int nextToken, ref Token token)
        {
            List<Node> elements = new List<Node>();
            while (!ReadToken(ref nextToken, ")", ref token))
            {
                if(!ReadToken(ref nextToken,",",ref token))
                {
                    int index = 0;
                    if (FindToken(ref nextToken, ref index, ","))
                    {
                        Node elem = ParseExpression(ref nextToken, ref token, index);
                        elements.Add(elem);
                    }
                }
                else
                {
                    throw new CodeException(token, "语法错误,不应该出现的分号;");
                }
                if(!ReadToken(ref nextToken,",",ref token))
                {
                    break;
                }
            }
            return new Argument(Token.NewToken("arguments"),elements);
        }
        public static Node ParseLet(ref int nextToken, ref Token token)
        {
            string varName="";
            Node value = null;
            if(ReadToken(ref nextToken,TokenType.Identifier,ref token))
            {
                varName=token.Value;
            }
            if(ReadToken(ref nextToken,"=",ref token))
            {
                int index = 0;
                if(FindToken(ref nextToken,ref index,";"))
                {
                    value = ParseExpression(ref nextToken, ref token,index);
                }
            }
            if (!ReadToken(ref nextToken, ";", ref token))
            {
                throw new CodeException(token, "缺少;");
            }
            return new LetStatement(Token.NewToken("let"),varName,value);
        }
        public static Node ParseIf(ref int nextToken, ref Token token)
        {
            Node test = null;
            Block thenBody = null;
            Block elseBody = null;

            int index=0;
            if(FindToken(ref nextToken, ref index, "{"))
            {
                test = ParseExpression(ref nextToken, ref token, index);
            }
            if (ReadToken(ref nextToken, "{", ref token))
            {
                thenBody = (Block)ParseBlock(ref nextToken, ref token);
                if (!ReadToken(ref nextToken, "}", ref token))
                {
                    throw new CodeException(token, "缺少}");
                }
            }
            if(ReadToken(ref nextToken,TokenType.ELSE,ref token))
            {
                if (ReadToken(ref nextToken, "{", ref token))
                {
                    elseBody = (Block)ParseBlock(ref nextToken, ref token);
                    if (!ReadToken(ref nextToken, "}", ref token))
                    {
                        throw new CodeException(token, "缺少}");
                    }
                }
            }
            else
            {
                elseBody = new Block(Token.NewToken("nullElse"), new List<Node>());
            }
            return new IfStatement(Token.NewToken("statement"),test,thenBody,elseBody);
        }
        public static Node ParseWhile(ref int nextToken, ref Token token)
        {
            Node test = null;
            Block body = null;
            int index = 0;
            if (FindToken(ref nextToken, ref index, "{"))
            {
                test = ParseExpression(ref nextToken, ref token, index);
            }
            if (ReadToken(ref nextToken, "{", ref token))
            {
                body = (Block)ParseBlock(ref nextToken, ref token);
                if (!ReadToken(ref nextToken, "}", ref token))
                {
                    throw new CodeException(token, "缺少}");
                }
            }
            else
            {
                body = new Block(Token.NewToken("nullBody"), new List<Node>());
            }
            return new WhileStatement(Token.NewToken("statement"),test,body);
        }
        //lambda x:int,y:int->int{let a=(a+b);}
        public static Node ParseLambda(ref int nextToken, ref Token token)
        {
            List<Identifier> parameters = null;
            Node body = null;
            Scope scope = ParseParameter("lambda", out parameters, ref nextToken, ref token);
            if (!ReadToken(ref nextToken, ")", ref token))
            {
                throw new CodeException(token, "缺少)");
            }
            if (ReadToken(ref nextToken, "-", ref token) &&
                ReadToken(ref nextToken, ">", ref token) &&
                ReadToken(ref nextToken, TokenType.Identifier, ref token))
            {
                scope.Put("lambda", token.Value, "lambda");
            }
            if (ReadToken(ref nextToken, "{", ref token))
            {
                body = ParseBlock(ref nextToken, ref token);
            }
            if (!ReadToken(ref nextToken, "}", ref token))
            {
                throw new CodeException(token, "缺少}");
            }
            return new LambdaExpression(Token.NewToken("lambda"), parameters, scope, body);
        }
        public static bool IsValue(TokenType type)
        {
            switch(type)
            {
                case TokenType.Integer:
                case TokenType.Double:
                case TokenType.Boolean:
                case TokenType.String:
                case TokenType.Identifier:
                    return true;
                default:
                    return false;
            }
        }
        public static bool ReadValue(ref int nextToken,ref Token token)
        {
            return
                ReadToken(ref nextToken, TokenType.Identifier, ref token) ||
                ReadToken(ref nextToken, TokenType.Integer, ref token) ||
                ReadToken(ref nextToken, TokenType.Double, ref token) ||
                ReadToken(ref nextToken, TokenType.String, ref token) ||
                ReadToken(ref nextToken, TokenType.Boolean, ref token)||
                ReadToken(ref nextToken, "(", ref token) ||
                ReadToken(ref nextToken, ")", ref token);
        }
        public static bool ReadAny(ref int nextToken, ref Token token)
        {
            return
                ReadToken(ref nextToken, TokenType.Identifier, ref token) ||
                ReadToken(ref nextToken, TokenType.Integer, ref token) ||
                ReadToken(ref nextToken, TokenType.Double, ref token) ||
                ReadToken(ref nextToken, TokenType.String, ref token) ||
                ReadToken(ref nextToken, TokenType.Boolean, ref token) ||
                ReadToken(ref nextToken, TokenType.Operator, ref token);
        }
        public static Node ParseArray(ref int nextToken, ref Token token)
        {
            List<Node> elements = new List<Node>();
            if(ReadToken(ref nextToken,"[",ref token))
            {
                while(true)
                {
                    if (ReadValue(ref nextToken, ref token))
                    {
                        elements.Add(ProcessNode(token));
                    }
                    else if (ReadToken(ref nextToken, "[", ref token))
                    {
                        elements.Add(ParseArray(ref nextToken, ref token));
                    }
                    else
                        break;
                }
            }
            return new ArrayNode(Token.NewToken("array"),elements);
        }
        public static List<Node> ParseStatementList(ref int nextToken, ref Token token)
        {
            List<Node> statementList = new List<Node>();
            while(true)
            {
                if (ReadToken(ref nextToken, TokenType.DEF, ref token))
                {
                    statementList.Add(ParseFunction(ref nextToken, ref token));
                }
                else if (ReadToken(ref nextToken, TokenType.LET, ref token))
                {
                    statementList.Add(ParseLet(ref nextToken, ref token));
                }
                else if (ReadToken(ref nextToken, TokenType.IF, ref token))
                {
                    statementList.Add(ParseIf(ref nextToken, ref token));
                }
                else if (ReadToken(ref nextToken, TokenType.WHILE, ref token))
                {
                    statementList.Add(ParseWhile(ref nextToken, ref token));
                }
                else if (ReadToken(ref nextToken, TokenType.RETURN, ref token))
                {
                    int index = 0;
                    if (FindToken(ref nextToken, ref index, ";"))
                    {
                        var returnValue=ParseExpression(ref nextToken, ref token, index);
                        ReturnStatement rt=new ReturnStatement(Token.NewToken("return"),returnValue);
                        statementList.Add(rt);
                    }
                    if(!ReadToken(ref nextToken,";",ref token))
                    {
                        throw new CodeException(token, "缺少;");
                    }
                }
                else
                {
                    break;
                }
            }
            return statementList;
        }
        /*
         * 1.Value: 22,22.2,"aaa",true 
         * 2.Id: a,b,c
         * 3.Exp:2+2,a and b,a*3
         * 4.lambda(x y)(let a=(a+b))
         * 5.(a+b) 括号表达式
         * 6.[1,3,4,5] 列表 ArrayList
         * 7.funcCall
         */
        public static Node ParseExpression(ref int nextToken, ref Token token,int index)
        {
            List<Node> exps = new List<Node>();
            if (ReadToken(ref nextToken, TokenType.LAMBDA, ref token))
            {
                return ParseLambda(ref nextToken, ref token);
            }
            else if(LookAhead(ref nextToken,"["))
            {
                return ParseArray(ref nextToken, ref token);
            }
            else if(LookAhead(ref nextToken,TokenType.Identifier)&& LookAgain(ref nextToken,"("))
            {
                ReadToken(ref nextToken, TokenType.Identifier, ref token);
                return ParseFuncCall(ref nextToken, ref token);
            }
            else
            {
                return ParsePrim(ref nextToken,ref token,index);
            }
        }
        public static bool ProcessNegative(ref List<Node> exps,TokenType type,ref int nextToken,ref Token token)
        {
            if (token.Value == "-" &&
                   ReadToken(ref nextToken, type, ref token))//开头的负整数处理
            {
                exps.Add(Identifier.NewIdentifier("("));
                token.Value = "-" + token.Value;
                if(type==TokenType.Integer)
                {
                    exps.Add(new IntNum(token));
                }
                else
                {
                    exps.Add(new FloatNum(token));

                }
                exps.Add(Identifier.NewIdentifier("("));
                return true;
            }
            return false;
        }
        public static Node ParsePrim(ref int nextToken,ref Token token,int index)
        {
            List<Node> exps = new List<Node>();
            Token pre = Token.NewToken("#");//保存前一个token
            bool neg = false;//负数处理
            int count=0;
            while (
                count<index &&
                !LookAhead(ref nextToken,";")&&
                !LookAhead(ref nextToken, ","))
            {
                ReadAny(ref nextToken, ref token);
                count++;//计数
                bool flag = false;
                if (exps.Count == 0)//开头的负整数或浮点数处理
                {
                    if (!ProcessNegative(ref exps, TokenType.Integer, ref nextToken, ref token))
                    {
                        flag=ProcessNegative(ref exps, TokenType.Double, ref nextToken, ref token);
                    }
                }
                if(!flag)
                {
                    if(token.Value=="-"&&pre.Type==TokenType.Operator)
                    {
                        neg = true;
                        continue;
                    }
                    if(neg)
                    {
                        token.Value = "-" + token.Value;
                        neg = false;
                    }
                    exps.Add(ProcessNode(token));
                }
                pre = token;
            }
            if (exps.Count == 0)
            {
                throw new CodeException(token, "表达式个数不能为0");
            }
            if (exps.Count == 1)
            {
                return exps[0];
            }
            else if (exps.Count == 2)// not a | -b
            {
                Token tok = Token.NewToken("primitive");
                FunctionCall call = new FunctionCall(tok, 
                    Identifier.NewIdentifier(exps[0].token.Value), 
                    new Argument(tok, exps.Skip(1).ToList()));
                return call;
            }
            else
            {
                return ParseArithList(exps,ref nextToken, ref token);
            }
        }
        public static Node ParseArithList(List<Node> exps,ref int nextToken,ref Token token)
        {
            Dictionary<string, int> table = new Dictionary<string, int>()
            {
                {"(",10},{")",0},{"or",1},{"and",2},{"not",3},
                {">",4},{"<",4},{">=",4},{"<=",4},{"=",4},
                {"<>",4},
                {"+",5},{"-",5},{"*",6},{"/",6}
            };
            List<Node> opList=new List<Node>();
            Stack<Node> opStack=new Stack<Node>();
            //(-100)+d*2+100*0.3 -》 -100 d 2 * 100 0.3 * + +
            for(int i=0;i<exps.Count;i++)
            {
                Token item=exps[i].token;
                if(item.Type==TokenType.Operator)
                {
                    if(item.Value==")")
                    {
                        Node top=opStack.Pop();
                        while(top.token.Value!="(")
                        {
                            opList.Add(top);
                            top = opStack.Pop();
                        }
                    }
                    else if (opStack.Count != 0 && 
                        table[item.Value] <= table[opStack.Peek().token.Value])
                    {
                        Node top = opStack.Pop();
                        opList.Add(top);
                        opStack.Push(exps[i]);
                    }
                    else
                    {
                        opStack.Push(exps[i]);
                    }
                }
                else
                {
                    opList.Add(exps[i]);
                }
            }
            while(opStack.Count!=0)
            {
                opList.Add(opStack.Pop());
            }
            opStack.Clear();
            for (int i = 0; i < opList.Count; i++)
            {
                Token item=opList[i].token;
                if(item.Type==TokenType.Operator)
                {
                    List<Node> args=new List<Node>();
                    if(item.Value=="not")
                    {
                        args.Add(opStack.Pop());
                    }
                    else
                    {
                        args.Add(opStack.Pop());
                        args.Add(opStack.Pop());
                    }
                    FunctionCall call = new FunctionCall(
                        Token.NewToken("primitive"), 
                        Identifier.NewIdentifier(item.Value), 
                        new Argument(Token.NewToken("primitive"), args));
                    opStack.Push(call);
                }
                else
                {
                    opStack.Push(opList[i]);
                }
            }
            return opStack.Pop();
        }

        public static Node ProcessNode(Token token)
        {
            if (token.Type == TokenType.Integer)
            {
                return new IntNum(token);
            }
            else if (token.Type == TokenType.Double)
            {
                return new FloatNum(token);
            }
            else if (token.Type == TokenType.String)
            {
                return new StringNode(token);
            }
            else if (token.Type == TokenType.Boolean)
            {
                return new BoolNode(token);
            }
            else if (token.Type == TokenType.Identifier)
            {
                return new Identifier(token,token.Value);
            }
            else if (token.Type == TokenType.Operator)
            {
                if (token.Value == "{" || token.Value == "}"
                    || token.Value == "(" || token.Value == ")" ||
                    token.Value == "[" || token.Value == "]" ||
                    token.Value == ";" || token.Value == ",")
                {
                    return new Delimeter(token);
                }
                else
                {
                    return new Identifier(token,token.Value);
                }
            }
            return null;
        }
    }
}
