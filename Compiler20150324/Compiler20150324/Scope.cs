using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Scope
    {
        public Dictionary<string, Dictionary<string, object>> table =
            new Dictionary<string, Dictionary<string, object>>();
        public Scope parent;
        public Scope()
        {
        }
        public Scope(Scope p)
        {
            this.parent = p;
        }
        public Scope Copy()
        {
            Scope ret = new Scope();
            foreach (string name in table.Keys)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                foreach (var p in table[name])
                {
                    props.Add(p.Key, p.Value);
                }
                ret.table.Add(name, props);
            }
            return ret;
        }


        public void putAll(Scope other)
        {
            foreach (string name in other.table.Keys)
            {
                Dictionary<string, object> props = new Dictionary<string, object>();
                foreach (var p in other.table[name])
                {
                    props.Add(p.Key, p.Value);
                }
                table.Add(name, props);
            }
        }


        public Value lookup(string name)
        {
            object v = lookupProperty(name, "value");
            if (v == null)
            {
                return null;
            }
            else if (v is Value)
            {
                return (Value)v;
            }
            else
            {
                throw new CodeException(Token.NewToken(""), 
                    "value is not a Value, shouldn't happen: " + v.ToString());
            }
        }


        public Value lookupLocal(string name)
        {
            object v = lookupPropertyLocal(name, "value");
            if (v == null)
            {
                return null;
            }
            else if (v is Value)
            {
                return (Value)v;
            }
            else
            {
                throw new CodeException(Token.NewToken(""), 
                    "value is not a Value, shouldn't happen: " + v.ToString());
            }
        }


        public Value lookupType(string name)
        {
            object v = lookupProperty(name, "type");
            if (v == null)
            {
                return null;
            }
            else if (v is Value)
            {
                return (Value)v;
            }
            else
            {
                throw new CodeException(Token.NewToken(""), 
                    "value is not a Value, shouldn't happen: " + v.ToString());
            }
        }


        public Value lookupLocalType(string name)
        {
            object v = lookupPropertyLocal(name, "type");
            if (v == null)
            {
                return null;
            }
            else if (v is Value)
            {
                return (Value)v;
            }
            else
            {
                throw new CodeException(Token.NewToken(""), 
                    "value is not a Value, shouldn't happen: " + v.ToString());
            }
        }


        public object lookupPropertyLocal(string name, string key)
        {
            if (table.ContainsKey(name))
            {
                Dictionary<string, object> item = table[name];
                if(item.ContainsKey(key))
                {
                    return item[key];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        public object lookupProperty(string name, string key)
        {
            object v = lookupPropertyLocal(name, key);
            if (v != null)
            {
                return v;
            }
            else if (parent != null)
            {
                return parent.lookupProperty(name, key);
            }
            else
            {
                return null;
            }
        }


        public Dictionary<string, object> lookupAllProps(string name)
        {
            if (table.ContainsKey(name))
            {
                return table[name];
            }
            else
            {
                return null;
            }
        }


        public Scope findDefiningScope(string name)
        {
            if (table.ContainsKey(name))
            {
                return this;
            }
            else if (parent != null)
            {
                return parent.findDefiningScope(name);
            }
            else
            {
                return null;
            }
        }
        public void Put(string name, string key, object value)
        {
            Dictionary<string, object> item = null;
            if (table.ContainsKey(name))
            {
                item = table[name];
            }
            else
            {
                item=new Dictionary<string, object>();
            }
            item.Add(key, value);
            table.Add(name, item);
        }


        public void PutProperties(string name, Dictionary<string, object> props)
        {
            Dictionary<string, object> item = null;
            if (table.ContainsKey(name))
            {
                item = table[name];
            }
            else
            {
                item = new Dictionary<string, object>();
            }
            if(props !=null)
            {
                foreach(var p in props)
                {
                    item.Add(p.Key, p.Value);
                }
            }
            table.Add(name, item);
        }
        public void putValue(string name, Value value)
        {
            Put(name, "value", value);
        }
        public void putType(String name, Value value)
        {
            Put(name, "type", value);
        }
        public static Scope initScope()
        {
            Scope init = new Scope();

            init.putValue("+", new AddPrim());
            init.putValue("-", new SubPrim());
            init.putValue("*", new MulPrim());
            init.putValue("/", new DivPrim());

            init.putValue("<", new LtPrim());
            init.putValue("<=", new LePrim());
            init.putValue(">", new GtPrim());
            init.putValue(">=", new GePrim());
            init.putValue("=", new EqPrim());
            init.putValue("and", new AndPrim());
            init.putValue("or", new OrPrim());
            init.putValue("not", new NotPrim());
            init.putValue("<>", new NotEqPrim());

            init.putValue("true", Value.BOOL);
            init.putValue("false", Value.BOOL);

            init.putValue("Int", Value.INT);
            init.putValue("Bool", Value.BOOL);
            init.putValue("String", Value.STRING);
            init.putValue("Any", Value.ANY);

            return init;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < table.Keys.Count;i++)
            {
                var item = table.ElementAt(i).Value;

                for (int j = 0; j < item.Count;j++ )
                {
                    var val = item.ElementAt(j);
                    if(j==item.Count-1)
                    {
                        if(val.Value is Value)
                        {
                            sb.Append(((Value)val.Value).Type());
                        }
                    }
                    else
                    {
                        if (val.Value is Value)
                        {
                            sb.Append(((Value)val.Value).Type()+"->");
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}
