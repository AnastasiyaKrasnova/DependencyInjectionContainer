using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectionLib;

namespace DependencyInjectorUnitTests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void SimpleDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IService, ServiceFromInterf>();
            dependencies.Register<AService, ServiceFromAbstract>();
            dependencies.Register<Random, Random>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService>();
            var service2 = provider.Resolve<AService>();
            var service3 = provider.Resolve<Random>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.IsNotNull(service3);
        }

        [TestMethod]
        public void SimpleErrorDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<AService, ErrorAbstratcImpl>();
            dependencies.Register<IService, ErrorNoPublicConstructorImpl>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<AService>();
            var service2 = provider.Resolve<IService>();

            Assert.IsNull(service1);
            Assert.IsNull(service2);
        }

        [TestMethod]
        public void SimpleRecursionDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IRepository, RepositoryImpl1>();
            dependencies.Register<IService, ServiceImpl1>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService>();

            Assert.AreEqual((service1 as ServiceImpl1).rep.TestObject(), "RepositoryImpl is created");
        }

        [TestMethod]
        public void DoubleRecursionDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<AService, ServiceFromAbstract>();
            dependencies.Register<IRepository, RepositoryImpl2>();
            dependencies.Register<IService, ServiceImpl1>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService>();

            Assert.AreEqual(((service1 as ServiceImpl1).rep as RepositoryImpl2).serv.TestObject(), "AService obj is created");
        }

        [TestMethod]
        public void MultipleDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IRepository, RepositoryImpl1>();
            dependencies.Register<IService, ServiceImpl1>();
            dependencies.Register<IService, ServiceImpl2>();

            var provider = new DependencyProvider(dependencies);
            var services = provider.Resolve<IEnumerable<IService>>().ToArray();

            Assert.AreEqual((services[0] as ServiceImpl1).rep.TestObject(), "RepositoryImpl is created");
            Assert.AreEqual((services[1] as ServiceImpl2).rep.TestObject(), "RepositoryImpl is created");
        }

        [TestMethod]
        public void MultipleDependencyTestConstructor()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IRepository, RepositoryImpl1>();
            dependencies.Register<IService, ServiceImpl3>();

            var provider = new DependencyProvider(dependencies);
            var service3 = provider.Resolve<IService>();

            Assert.IsNotNull(service3);
        }

        [TestMethod]
        public void StandartGenericDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register<IRepository, RepositoryImpl1>();
            dependencies.Register<IService<IRepository>, ServiceImpl<IRepository>>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService<IRepository>>();

            Assert.AreEqual((service1 as ServiceImpl<IRepository>).TestObject(), "ServiceImpl<TRepository> with generic is created");
        }

        [TestMethod]
        public void OpenGenericDependencyTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));
            dependencies.Register<IRepository, RepositoryImpl1>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService<IRepository>>();

            Assert.AreEqual((service1 as ServiceImpl<IRepository>).TestObject(), "ServiceImpl<TRepository> with generic is created");
        }

        [TestMethod]
        public void LifetimeTest()
        {
            var dependencies = new DependencyConfiguration();
            dependencies.RegisterSingleton<IService, ServiceImpl1>();
            dependencies.Register<IRepository, RepositoryImpl1>();

            var provider = new DependencyProvider(dependencies);
            var service1 = provider.Resolve<IService>();
            var service2 = provider.Resolve<IService>();
            var service3 = Task.Run(() => provider.Resolve<IService>());
            Assert.IsTrue(Equals(service3.Result, service1) && Equals(service3.Result, service2) && Equals(service1, service2));

            dependencies = new DependencyConfiguration();
            dependencies.Register<IService, ServiceImpl1>();
            dependencies.Register<IRepository, RepositoryImpl1>();

            provider = new DependencyProvider(dependencies);
            service1 = provider.Resolve<IService>();
            service2 = provider.Resolve<IService>();
            service3 = Task.Run(() => provider.Resolve<IService>());
            Assert.IsFalse(Equals(service3.Result, service1) && Equals(service3.Result, service2) && Equals(service1, service2));
        }


    }
}
