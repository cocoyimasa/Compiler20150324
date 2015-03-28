using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.WriteLine("DEBUG　TEST IN OUTPUT WINDOWS");
            List<int> intlist = new List<int>() { 1, 2, 3, 4, 5, 5 };
            Console.WriteLine(intlist.Aggregate("",(i,j)=>i+j.ToString()));
            Console.ReadKey();
        }
    }
}
