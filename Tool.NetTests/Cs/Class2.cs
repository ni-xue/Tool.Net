using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils;
using Tool.Utils.ActionDelegate;

namespace Tool.NetTests.Cs
{
    [TestClass()]
    public class Class2
    {
        readonly byte[] ListData;
        readonly Memory<byte> arraySegment;

        public Class2()
        {
            ListData = new byte[] { 40, 2, 3, 4, 3, 41 };

            arraySegment = ListData;
        }

        [TestMethod()]
        public void Cs0()
        {
            for (int i = 0; i < 1000000; i++)
            {
                byte[] headby = new byte[6];

                Array.Copy(ListData, 0, headby, 0, 6);

                if (headby[0] == 40 && headby[5] == 41)
                {
                    //int index;
                    //for (index = 1; index < 9; index++)
                    //{
                    //    if (headby[index] != '\0') break;
                    //}
                    //string head = Encoding.ASCII.GetString(headby, index, 9 - index); //"(20971520)‬"

                    //return head.ToInt();

                    Console.WriteLine(BitConverter.ToUInt32(headby, 1));
                }
            }
        }

        [TestMethod()]
        public void Cs1()
        {
            ReadOnlySpan<byte> headby = ListData;
            for (int i = 0; i < 1000000; i++)
            {
                //byte[] headby = new byte[6];
                //var headby = data.Slice(0, 6);//.CopyTo(headby);

                if (headby[0] == 40 && headby[5] == 41)
                {
                    //int index;
                    //for (index = 1; index < 9; index++)
                    //{
                    //    if (headby[index] != '\0') break;
                    //}
                    //string head = Encoding.ASCII.GetString(headby, index, 9 - index); //"(20971520)‬"

                    //return head.ToInt();

                    Console.WriteLine(BitConverter.ToUInt32(headby[1..5]));

                    //Array.Clear(ListData, 0, 5);
                }

                //byte[] headby = data.Slice(0, 6).ToArray();//.CopyTo(headby);
            }
        }

        [TestMethod()]
        public void Cs2()
        {
            //var c = ObjectExtension.Provider.GetService<Class1>();

            //ObjectExtension.BuildProvider();

            //var c1 = ObjectExtension.Provider.GetService<Class1>();

            //Class1 class2 = new(10, 100);

            //ObjectExtension.Services.TryAddTransient<Class2>();

            //ObjectExtension.Services.TryAddTransient(typeof(Class2), (a) =>
            //{
            //    return new Class2(); 
            //});

            //ObjectExtension.Services.TryAddScoped<Class2>();


            //ObjectExtension.Services.RemoveAll<Class2>();

            //ObjectExtension.Services.AddObject(class1);

            //var c2 = ObjectExtension.Provider.GetService<Class1>();

            IocHelper.IocCore.RemoveAll();

            //Class1 class1 = new(1,10);

            //ObjectExtension.Services.AddObject(class1);

            ObjectExtension.Services.TryAddTransient<CsA0>();

            ObjectExtension.Services.TryAddTransient<CsA1>();

            ObjectExtension.BuildProvider();

            var c3 = ObjectExtension.Provider.GetService<CsA1>();

            using var a1 = ObjectExtension.Provider.CreateScope();
            var c3_1 = a1.ServiceProvider.GetService<CsA1>();

            var c4 = ObjectExtension.Provider.GetService<CsA1>();

            var c5 = ObjectExtension.Provider.GetService<CsA1>();

            Task.Run(() =>
            {
                var c6 = ObjectExtension.Provider.GetService<CsA1>();

            }).Wait();
        }

        public class CsA0
        {

        }

        public class CsA1
        {
            public CsA1(CsA0 class1)
            {

            }
        }
    }
}