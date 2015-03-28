using Compiler.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
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
            //                            {
            //                                 let c = (lambda(e,f){let z=1;})(10,20);
            //                            }";
            string code = @"def a(d)
                            {
                                let c = (-100)+d*2+100*0.3;
                                return c;
                            }
                            a(10)
                            ";
            LexicalAnalyzer.InitTokenRules();
            List<Token> tokens = LexicalAnalyzer.Tokenizer(code);
            //foreach (var item in tokens)
            //{
            //    //固定长度输出 string.PadLeft string.PadRight
            //    Console.WriteLine(item.Type.ToString().PadRight(12, ' ') + ":   " + item.Value);
            //}
            SyntacticalAnalyzer.tokens = tokens;
            Node result = SyntacticalAnalyzer.ParseProgram();
            if (result != null)
            {
                Console.WriteLine("Compiling...Syntactical Analysis Completed!");
                //Console.WriteLine(((FunctionStatement)result.children[0]).decl.funcName);
                //Console.WriteLine(((FunctionStatement)result.children[0]).body[0].value.Value);
            }
            TypeChecker tc = new TypeChecker();
            TypeChecker.self = tc;
            Value value=tc.TypeCheck(result);
            Console.WriteLine(value.ToString());
            Interpreter interpret = new Interpreter(result);
            Value val = interpret.Interpret();
            Console.WriteLine(val.ToString());
            Console.ReadKey();
        }
    }
}
