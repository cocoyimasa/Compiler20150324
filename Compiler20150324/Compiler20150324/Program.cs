using Compiler.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    static class Sapphire
    {
        public static void SapphireConsole(Scope env)
        {
            while (true)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(">>>");
                    StringBuilder sb = new StringBuilder();
                    string input = Console.ReadLine();
                    while (true)
                    {
                        sb.Append(input);
                        ConsoleKeyInfo info = Console.ReadKey();
                        if(info.KeyChar=='\r')
                        {
                            break;
                        }
                        input = info.KeyChar + Console.ReadLine();
                    }
                    input = sb.ToString();
                    if (input == "!q")
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Command exit is excuting!...");
                        break;
                    }
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        LexicalAnalyzer.InitTokenRules();
                        List<Token> tokens = LexicalAnalyzer.Tokenizer(input);
                        SyntacticalAnalyzer.tokens = tokens;
                        Node result = SyntacticalAnalyzer.ParseProgram();
                        //if (result != null)
                        //{
                        //    Console.WriteLine("Compiling...Syntactical Analysis Completed!");
                        //}
                        TypeChecker tc = new TypeChecker();
                        TypeChecker.self = tc;
                        Value value = tc.TypeCheck(result);
                        Console.WriteLine(value.Type());
                        Interpreter interpret = new Interpreter(result);
                        Value val = interpret.Interpret(env);
                        Console.WriteLine(val.ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //if while语法：if while的条件不用括号括起来
            //lambda语法:lambda x:int,y:int->int{let a=(a+b);}
            //let [a,b,c]=[1,2,43];
            //let a=10;
//            string code = @"def a()
//                          {
//                              let a1=10.00001;
//                              let str=""aaaaaaaa"";
//                          }
//                          //eeeyeyeyeyeyeyeyeyeyeyeyeyyeyeyeyeyeyeyeyeyyeyeye
//                          def b(c,d){
//                          let d=90;
//                          //let a2=(-100)+200+d-20 + d*2-d/2+100*0.3*0.01*0.1*10*10*0.1*1*2/2;
//                          let a2=(-100)+d*2+100*0.3;
//                          let e =0;
//                          //let c = (lambda(e,f){let z=1;})(10,20);
//                          while a2>c {let a2=0.9;}
//                          if d<>0 or a2>c {
//                              let c=10;
//                          }
//                          let z=(-10.2);
//                          }
//                          ";
//            string code = @"def a()
//                                        {
//                                             let c = (lambda(e,f){let z=1;})(10,20);
//                                        }";
//            string code = @"def a(d)
//                            {
//                                let c = (-100)+d*2+100*0.3;
//                                return c;
//                            }
//                            a(10)
//                            ";
            Console.Write("Sapphire 1.0---2015----->>>\npress 'i' to interpret your code,'c' to compile>>>\n>>>");
            string mode = Console.ReadLine();
            if(mode=="i")
            {
                Scope env = Scope.initScope();
                Sapphire.SapphireConsole(env);
            }
            Console.ReadKey();
        }
    }
}
