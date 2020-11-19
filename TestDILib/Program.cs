using System;
using System.Collections.Generic;
using DependencyInjectionLib;

namespace TestDILib
{
    public interface IFoo { }

    public interface IA { }

    public class A : IA {
        public A()
        {
            Console.WriteLine("This is A");
        }
    }

    public interface IBar { }

    public abstract class ABar : IBar { }

    public class BarFromIBar : IBar
    {
        IFoo foo;
        public BarFromIBar(IFoo foo)
        {
            this.foo = foo;
        }
    }

    public class BarFromABar : ABar
    {
        public BarFromABar()
        {
            Console.WriteLine("BarFromABar");
        }
    }

    public class Foo : IFoo
    {
        public ABar Bar { get; }
        public Foo()
        {
            Console.WriteLine("I am Foo!");
        }
        public Foo(bool k)
        {
            Console.WriteLine("I am Foo with bool!");
        }
        public Foo(ABar bar)
        {
            Bar = bar;
            Console.WriteLine("I am Foo with ABar!");
        }
    }

    interface IService {
        public void Test();
    }
    class ServiceImpl1 : IService
    {
        IRepository impl;
        public ServiceImpl1(IRepository repository)
        {
            impl = repository;
            Console.WriteLine("ServiceImpl1");
        }
        public void Test()
        {
            Console.WriteLine("ServiceImpl1 Test");
            impl.Test();
        }
    }

    class ServiceImpl2 : IService
    {
        IRepository impl;
        public ServiceImpl2(IRepository repository)
        {
            impl = repository;
            Console.WriteLine("ServiceImpl2");
        }
        public void Test()
        {
            Console.WriteLine("ServiceImpl2 Test");
            impl.Test();
        }
    }

    interface IRepository {
        public void Test();
    }
    class RepositoryImpl : IRepository
    {
        public RepositoryImpl() {
            Console.WriteLine("RepositoryImpl");
        }
        public void Test()
        {
            Console.WriteLine("Test Successfull");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DependencyConfiguration dc = new DependencyConfiguration();

            /*dc.Register<IBar, BarFromIBar>();
            dc.Register<ABar, BarFromABar>();
            dc.Register<IFoo, Foo>();

            DependencyProvider dp = new DependencyProvider(dc);

            var foo = dp.Resolve<IFoo>();

            DependencyConfiguration newDC = new DependencyConfiguration();

            newDC.Register<IBar, BarFromIBar>();
            newDC.Register<IBar, BarFromABar>();

            DependencyProvider newDP = new DependencyProvider(newDC);
            var bars = newDP.Resolve<IBar>();*/

            dc.Register<IService, ServiceImpl1>();
            dc.Register<IService, ServiceImpl2>();
            dc.Register<IRepository, RepositoryImpl>();
            DependencyProvider dp = new DependencyProvider(dc);
            var a = dp.Resolve<IEnumerable<IService>>();
            foreach (var ia in a)
            {
                ia.Test();
            }
            Console.ReadLine();
        }
    }
}
