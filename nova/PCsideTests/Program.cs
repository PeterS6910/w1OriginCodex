using System;
using System.Collections.Generic;
using System.Text;

using Contal.Cgp.BaseLib;

namespace Contal.Cgp.PCsideTests
{
    class A : ACgpModule
    {
        public A()
            :base(new CgpModuleConfiguration("ola"))        {
        }
    }

    class B : ACgpModule
    {
        static int _counter = 1;

        public B()
            : base(new CgpModuleConfiguration("test"+_counter++))
        {
        }
    }

    class Program
    {
        static B i1 = new B();
        static B i2 = new B();
        static B i3 = new B();

        static void Report()
        {
            Console.WriteLine("ref0");
            ACgpModule[] arr = i1.GetReferrences(0);
            foreach (ACgpModule module in arr)
            {
                Console.WriteLine(module.Name + " " + module.CgpDesignation);
            }

            Console.WriteLine("ref1");
            arr = i1.GetReferrences(1);
            foreach (ACgpModule module in arr)
            {
                Console.WriteLine(module.Name + " " + module.CgpDesignation);

            }

            Console.WriteLine("refBy .. 1");
            arr = i1.GetReferrencedBy();
            foreach (ACgpModule module in arr)
            {
                Console.WriteLine(module.Name + " " + module.CgpDesignation);
            }

            Console.WriteLine("refBy .. 2");
            arr = i2.GetReferrencedBy();
            foreach (ACgpModule module in arr)
            {
                Console.WriteLine(module.Name + " " + module.CgpDesignation);
            }

            Console.WriteLine("refBy .. 3");
            arr = i3.GetReferrencedBy();
            foreach (ACgpModule module in arr)
            {
                Console.WriteLine(module.Name + " " + module.CgpDesignation);
            }
        }


        static void Main(string[] args)
        {

            A iA = new A();
            A iB = new A();

            i1.Referrence(0, i2);
            i1.Referrence(0, i3);
            i2.Referrence(0, i3);

            Report();

            

            i1.StoreReferrences();

            i1.ClearReferrences();

            Console.WriteLine("Press key ...");
            Console.ReadLine();

            Report();

            Console.WriteLine("Press key ...");
            Console.ReadLine();

            i1.LoadReferrences();
            Report();
            

            Console.ReadLine();
        }
    }
}
