using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public abstract class Primitives : Value
    {
        public string name;
        public int arity;
        public Primitives()
        {

        }
        public Primitives(string name, int arity)
        {
            this.name = name;
            this.arity = arity;
        }
        public abstract Value Apply(List<Value> args,Node location);
        public abstract Value Typecheck(List<Value> args, Node location);//
        public override string Type()
        {
            return "Primitive";
        }
        public override string ToString()
        {
            return "Primitive";
        }
    }
    public class AddPrim     :Primitives
    {
        public AddPrim() : base("+", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if(args.Count<2)
            {
                throw new CodeException(location.token, "Add：错误的参数个数");
            }
            if(args[0] is FloatType || args[1] is FloatType)
            {
                return Value.FLOAT;
            }
            else if(args[0] is IntType && args[1] is IntType)
            {
                return Value.INT;
            }
            else
                throw new CodeException(location.token, "Add：错误的参数类型");                
        }
        public override string ToString()
        {
            return "Add";
        }
    }
    public class SubPrim     :Primitives
    {

        public SubPrim() : base("-", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Sub：错误的参数个数");
            }
            if (args[0] is FloatType || args[1] is FloatType)
            {
                return Value.FLOAT;
            }
            else if (args[0] is IntType && args[1] is IntType)
            {
                return Value.INT;
            }
            else
                throw new CodeException(location.token, "Sub：错误的参数类型");     
        }
        public override string ToString()
        {
            return "Sub";
        }
    }
    public class MulPrim     :Primitives
    {

        public MulPrim() : base("*", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Mul：错误的参数个数");
            }
            if (args[0] is FloatType || args[1] is FloatType)
            {
                return Value.FLOAT;
            }
            else if (args[0] is IntType && args[1] is IntType)
            {
                return Value.INT;
            }
            else
                throw new CodeException(location.token, "Mul：错误的参数类型");     
        }
        public override string ToString()
        {
            return "Mul";
        }
    }
    public class DivPrim     :Primitives
    {

        public DivPrim() : base("/", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Div：错误的参数个数");
            }
            if (args[0] is FloatType || args[1] is FloatType)
            {
                return Value.FLOAT;
            }
            else if (args[0] is IntType && args[1] is IntType)
            {
                return Value.INT;
            }
            else
                throw new CodeException(location.token, "Div：错误的参数类型"); 
        }
        public override string ToString()
        {
            return "Div";
        }
    }
    public class AndPrim     :Primitives
    {

        public AndPrim() : base("and", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "And：错误的参数个数");
            }
            if (args[0] is BoolType && args[1] is BoolType)
            {
                return Value.BOOL;
            }
            else
                throw new CodeException(location.token, "And：错误的参数类型"); 
        }
        public override string ToString()
        {
            return "And";
        }
    }
    public class OrPrim      :Primitives
    {

        public OrPrim() : base("or", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Or：错误的参数个数");
            }
            if (args[0] is BoolType && args[1] is BoolType)
            {
                return Value.BOOL;
            }
            else
                throw new CodeException(location.token, "Or：错误的参数类型"); 
        }
        public override string ToString()
        {
            return "Or";
        }
    }
    public class NotPrim     :Primitives
    {

        public NotPrim() : base("not", 1) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 1)
            {
                throw new CodeException(location.token, "Not：错误的参数个数");
            }
            if (args[0] is BoolType)
            {
                return Value.BOOL;
            }
            else
                throw new CodeException(location.token, "Not：错误的参数类型"); 
        }
        public override string ToString()
        {
            return "Not";
        }
    }
    public class LtPrim      :Primitives
    {

        public LtPrim() : base("<", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Not：错误的参数个数");
            }
            if (!(args[0] is IntType || args[0] is FloatType) ||
                !(args[1] is FloatType || args[1] is IntType))
            {
                throw new CodeException(location.token, "Not：错误的参数类型");
            }
            return Value.BOOL;
        }

        public override string ToString()
        {
            return "LessThan";
        }
    }
    public class LePrim      :Primitives
    {

        public LePrim() : base("<=", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Not：错误的参数个数");
            }
            if (!(args[0] is IntType || args[0] is FloatType) ||
                !(args[1] is FloatType || args[1] is IntType))
            {
                throw new CodeException(location.token, "Not：错误的参数类型");
            }
            return Value.BOOL;
        }
        public override string ToString()
        {
            return "LessEqual";
        }
    }
    public class GtPrim      :Primitives
    {

        public GtPrim() : base(">", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Not：错误的参数个数");
            }
            if (!(args[0] is IntType || args[0] is FloatType) ||
                !(args[1] is FloatType || args[1] is IntType))
            {
                throw new CodeException(location.token, "Not：错误的参数类型");
            }
            return Value.BOOL;
        }

        public override string ToString()
        {
            return "GreatThan";
        }
    }
    public class GePrim      :Primitives
    {

        public GePrim() : base(">=", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Not：错误的参数个数");
            }
            if (!(args[0] is IntType || args[0] is FloatType) ||
                !(args[1] is FloatType || args[1] is IntType))
            {
                throw new CodeException(location.token, "Not：错误的参数类型");
            }
            return Value.BOOL;
        }

        public override string ToString()
        {
            return "GreatEqual";
        }
    }
    public class NotEqPrim   :Primitives
    {

        public NotEqPrim() : base("<>", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Not：错误的参数个数");
            }
            if (!(args[0] is IntType || args[0] is FloatType) ||
                !(args[1] is FloatType || args[1] is IntType))
            {
                throw new CodeException(location.token, "Not：错误的参数类型");
            }
            return Value.BOOL;
        }

        public override string ToString()
        {
            return "NotEqual";
        }
    }
    public class EqPrim      :Primitives
    {

        public EqPrim() : base("=", 2) { }
        public override Value Apply(List<Value> args, Node location)
        {
            throw new NotImplementedException();
        }
        public override Value Typecheck(List<Value> args, Node location)
        {
            if (args.Count < 2)
            {
                throw new CodeException(location.token, "Not：错误的参数个数");
            }
            if (!(args[0] is IntType || args[0] is FloatType) ||
                !(args[1] is FloatType || args[1] is IntType))
            {
                throw new CodeException(location.token, "Not：错误的参数类型");
            }
            return Value.BOOL;
        }

        public override string ToString()
        {
            return "Equal";
        }
    }
}
