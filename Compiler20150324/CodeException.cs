using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class CodeException:Exception
    {
        protected string name;
        protected int position;
        protected int line;
        protected string detailMsg;

        public string Name
        {
            get { return name; }
            set { name = value; }
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
        public string DetailMsg
        {
            get { return detailMsg; }
            set { detailMsg = value; }
        }
        public CodeException()
        {}
        public CodeException(string _name, int _line, int _pos,string errMsg)
            :base(_name)
        {
            name = _name;
            position = _pos;
            line = _line;
            detailMsg = name + " at " + line + ", " + position + errMsg;
        }
        public CodeException(Token token, string errMsg)
            : base(token.Value)
        {
            name = token.Value;
            position = token.Position;
            line = token.Line;
            detailMsg = name + " at " + line + ", " + position + errMsg;
        }
    }
}
