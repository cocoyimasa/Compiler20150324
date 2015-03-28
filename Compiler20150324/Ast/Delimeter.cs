using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Ast
{
    [DebuggerDisplay("{value}")]
    public class Delimeter : Node
    {
        public string value;
        public static HashSet<string> delims = new HashSet<string>();
        public static Dictionary<string, string> delimMap = new Dictionary<string, string>();
        public Delimeter(Token tok)
            : base(tok)
        {
            value = tok.Value;
        }
        public static void AddDelimPair(string open, string close)
        {
            delims.Add(open);
            delims.Add(close);
            delimMap.Add(open, close);
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
            String matched = delimMap[((Delimeter)open).token.Value];
            return matched != null && matched.Equals(((Delimeter)close).token.Value);
        }
        public override Value Typecheck(Scope s)
        {
            return Value.ANY;
        }
    }
}
