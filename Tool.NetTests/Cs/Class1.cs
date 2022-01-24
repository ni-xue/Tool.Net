using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils.ActionDelegate;

namespace Tool.NetTests.Cs
{
    [TestClass()]
    public class Class1
    {
        public readonly int A;

        public readonly int B;

        public Class1() 
        {
        
        }

        public Class1(int a, int b)
        {
            A = a; B = b;
        }

        [TestMethod()]
        public void Cs0()
        {
            for (int i = 0; i < 10000000; i++)
            {
                Class1 handler = Activator.CreateInstance(typeof(Class1), 1, 5) as Class1;
            }

            Console.WriteLine("现用方式");
        }

        [TestMethod()]
        public void Cs1()
        {
            var cons = typeof(Class1).GetConstructors()[1];
            ClassDispatcher<Class1> classDelegater = new(cons);

            for (int i = 0; i < 10000000; i++)
            {
                Class1 handler = classDelegater.Invoke(1, 5);
            }

            Console.WriteLine("最新方式");
        }

        [TestMethod()]
        public void Cs2()
        {
            for (int i = 0; i < 10000000; i++)
            {
                Class1 handler = new(1, 5);
            }
            Console.WriteLine("原始方式");
        }
    }
}
