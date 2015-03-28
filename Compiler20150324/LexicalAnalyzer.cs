using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Compiler
{
    public enum TokenType
    {
        Operator, Boolean, Double, Integer, String, Identifier, Space, Enter, LINE_COMMENT,
        IF, ELSE, WHILE, DEF, LET, LAMBDA,INT,FLOAT,STRING,BOOL,RETURN,
        BLOCK,STATEMENT,ARGUMENT,LIST
    }
    [DebuggerDisplay("{Type}:{Value}")]
    public class Token
    {
        protected string fileName;
        protected TokenType type;
        protected string value;
        protected int position;
        protected int line;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public TokenType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public int Line
        {
            get { return line; }
            set { line = value; }
        }
        public Token()
        {

        }
        public Token(TokenType _type, string _value, int _line, int _pos)
        {
            type = _type;
            value = _value;
            line = _line;
            position = _pos;
        }

        public Token(Token v)
        {
            this.Type = v.Type;
            this.Value = v.Value;
            this.Line = v.Line;
            this.Position = v.Position;
        }
        public static Token NewToken(string val)
        {
            return new Token(TokenType.Identifier, val, 0, 0);
        }
    }
    public class LexicalAnalyzer
    {
        protected static Dictionary<Regex, TokenType> tokenRules = null;
        public static string LoadSource(string path)
        {
            string code = File.ReadAllText(path);
            return code;
        }
        public static bool InitTokenRules()
        {
            tokenRules = new Dictionary<Regex, TokenType>() { 
                {new Regex(@"def"),TokenType.DEF},
                {new Regex(@"if"),TokenType.IF},
                {new Regex(@"else"),TokenType.ELSE},
                {new Regex(@"lambda"),TokenType.LAMBDA},
                {new Regex(@"let"),TokenType.LET},
                {new Regex(@"while"),TokenType.WHILE},
                {new Regex(@"return"),TokenType.RETURN},
                {new Regex(@"int"),TokenType.INT},
                {new Regex(@"float"),TokenType.FLOAT},
                {new Regex(@"string"),TokenType.STRING},
                {new Regex(@"bool"),TokenType.BOOL},
                {new Regex(@"//[^\r\n]+"),TokenType.LINE_COMMENT},
                {new Regex(@"(true|false)"),TokenType.Boolean},
                {new Regex(@"([0-9]+\.[0-9]+)"),TokenType.Double},
                {new Regex(@"(\d+)"),TokenType.Integer},
                {new Regex(@"(""([^""\\]|\\.)*"")"),TokenType.String},
                {new Regex(@"(\r\n|\n)+"),TokenType.Enter},
                {new Regex(@"(\s+)"),TokenType.Space},
                {new Regex(@"(\{|\}|\[|\]|\(|\)|//|/|,|\.\.|\.|\<=|\>=|=|\<\>|\<|\>|and|or|not|\+|\-|\*|\;)"),TokenType.Operator},
                {new Regex(@"([a-zA-Z_][a-z0-9A-Z_]*)"),TokenType.Identifier}
            };
            return true;
        }
        public static List<Token> Tokenizer(string code)
        {
            List<Token> tokens = new List<Token>();
            int position = 0;
            int line = 1;//保证line从1开始
            while (position < code.Length)
            {
                var result = tokenRules.Select(
                    p => Tuple.Create(p.Key.Match(code, position), p.Value))
                .Where(
                t => t.Item1.Index == position && t.Item1.Success)
                .FirstOrDefault();
                if (result == null)
                {
                    throw new CodeException(result.Item1.Value, line, position, "Lexical error");
                }
                if (result.Item2 == TokenType.LINE_COMMENT ||
                    result.Item2 == TokenType.Enter)
                {
                    line++;
                }
                else if (result.Item2 == TokenType.Space)
                {
                    ;//pass
                }
                else
                {
                    tokens.Add(new Token(result.Item2, result.Item1.Value, line, result.Item1.Index + 1));////保证position从1开始
                }
                position += result.Item1.Value.Length;
            }
            return tokens;
        }
    }
}