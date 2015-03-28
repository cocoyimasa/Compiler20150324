using Compiler.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public abstract class Value
    {
        public Value()
        {

        }
        public abstract string Type();
        public static readonly Value VOID = new VoidType();
        public static readonly Value TRUE = new BoolType(true);
        public static readonly Value FALSE = new BoolType(false);
        public static readonly Value ANY = new AnyType();

        public static readonly Value BOOL = new BoolType();
        public static readonly Value INT = new IntType();
        public static readonly Value FLOAT = new FloatType();
        public static readonly Value STRING = new StringType();
        public static bool Subtype(Value type1, Value type2, bool ret)
        {
            if (!ret && type2 is AnyType)
            {
                return true;
            }

            if (type1 is UnionType)
            {
                foreach (Value t in ((UnionType)type1).values)
                {
                    if (!Subtype(t, type2, false))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (type2 is UnionType)
            {
                return ((UnionType)type2).values.Contains(type1);
            }
            else
            {
                return type1.Equals(type2);
            }
        }
    }
    public class AnyType : Value
    {
        public AnyType() { }
        public override string ToString()
        {
            return base.ToString();
        }
        public override string Type()
        {
            return "Any";
        }
    }
    public class BoolType : Value
    {
        public bool value;
        public BoolType() { }
        public BoolType(bool val)
        {
            this.value = val;
        }
        public override string ToString()
        {
            return value.ToString();
        }
        public override string Type()
        {
            return "Bool";
        }

    }
    public class Closure : Value
    {
        public FunctionStatement fun;
        public Scope properties;
        public Scope env;

        public Closure() { }
        public Closure(FunctionStatement fun, Scope properties, Scope env)
        {
            this.fun = fun;
            this.properties = properties;
            this.env = env;
        }
        public override string ToString()
        {
            return fun.ToString();
        }
        public override string Type()
        {
            return "Function";//

        }
    }
    public class FloatType : Value
    {
        public double value;
        public FloatType() { }
        public FloatType(double val) 
        {
            this.value = val;
        }
        public override string ToString()
        {
            return value.ToString();
        }
        public override string Type()
        {
            return "Float";

        }
    }
    public class FunctionType : Value
    {
        public FunctionStatement fun;
        public Scope properties;
        public Scope env;

        public FunctionType() { }
        public FunctionType(FunctionStatement fun, Scope properties, Scope env)
        {
            this.fun = fun;
            this.properties = properties;
            this.env = env;
        }
        public override string ToString()
        {
            if(properties==null)
            {
                return "Void";
            }
            return properties.ToString();
        }
        public override string Type()
        {
            if (properties == null)
            {
                return "Void";
            }
            return properties.ToString();
        }
    }
    public class IntType : Value
    {
        public int value;
        public IntType() { }
        public IntType(int val)
        {
            this.value = val;
        }
        public override string ToString()
        {
            return value.ToString();
        }
        public override string Type()
        {
            return "Int";
        }
    }
    public class StringType : Value
    {
        public string value;
        public StringType() { }
        public StringType(string val)
        {
            this.value = val;
        }
        public override string ToString()
        {
            return "'" + value.ToString() + "'";
        }
        public override string Type()
        {
            return "String";

        }
    }
    public class RecordType : Value
    {
        public RecordType() { }
        public override string ToString()
        {
            return base.ToString();
        }
        public override string Type()
        {
            return "Record";//

        }
    }
    public class UnionType : Value
    {
        public HashSet<Value> values = new HashSet<Value>();
        public UnionType() { }
        public static Value Union(IEnumerable<Value> values)
        {
            UnionType u = new UnionType();
            foreach (Value v in values)
            {
                u.Add(v);
            }
            if (u.Size == 1)
            {
                return u.values.ElementAt(0);
            }
            else
            {
                return u;
            }
        }
        public static Value Union(params Value[] values)
        {
            UnionType u = new UnionType();
            foreach (Value v in values)
            {
                u.Add(v);
            }
            if (u.Size == 1)
            {
                return u.values.ElementAt(0);
            }
            else
            {
                return u;
            }
        }
        public void Add(Value value)
        {
            if (value is UnionType)
            {
                values.Union(((UnionType)value).values);
            }
            else
            {
                values.Add(value);
            }
        }
        public int Size
        {
            get
            {
                return values.Count;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(").Append("U ");

            bool first = true;
            foreach (Value v in values) {
                if (!first) {
                    sb.Append(" ");
                }
                sb.Append(v);
                first = false;
            }

            sb.Append(")");
            return sb.ToString();
        }
        public override string Type()
        {
            return "Union";//
        }
    }
    public class VoidType : Value
    {
        public VoidType() { }
        public override string ToString()
        {
            return "Void";
        }
        public override string Type()
        {
            return "Void";

        }
    }
    public class OutputType:Value
    {
        public List<Value> values;
        public OutputType()
        {
        }
        public OutputType(List<Value> values)
        {
            this.values = values;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var val in values)
            {
                sb.Append(val.ToString() + ":");
                sb.Append(val.Type() + " ");
            }
            return sb.ToString();
        }
        public override string Type()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var val in values)
            {
                sb.Append(val.Type() + " ");
            }
            return sb.ToString();
        }
    }
    public class ArrayType:Value
    {
        public List<Value> values;
        public ArrayType()
        {
        }
        public ArrayType(List<Value> values)
        {
            this.values = values;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            foreach(var val in values)
            {
                sb.Append(val.ToString() + " ");
            }
            sb.Append("]");
            return sb.ToString();
        }
        public override string Type()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            foreach (var val in values)
            {
                sb.Append(val.Type() + " ");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
