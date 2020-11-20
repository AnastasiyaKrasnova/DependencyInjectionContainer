using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectorUnitTests
{
    public interface IService { }

    public abstract class AService {
        public abstract string TestObject();
    }

    public class ServiceFromInterf : IService { }


    public class ServiceFromAbstract : AService { 
        public override string TestObject()
        {
            return "AService obj is created";
        }
    }

    public class ServiceImpl1 : IService
    {
        public IRepository rep = null;

        public ServiceImpl1(IRepository _rep)
        {
            this.rep = _rep;
        }
    }

    public class ServiceImpl2 : IService
    {

        public IRepository rep = null;

        public ServiceImpl2(IRepository _rep)
        {
            this.rep = _rep;
        }
    }

    public class ServiceImpl3 : IService
    {

        public IEnumerable<IRepository> rep = null;

        public ServiceImpl3(IEnumerable<IRepository> _rep)
        {
            this.rep = _rep;
        }
    }

    public interface IRepository
    {
        string TestObject();
    }

    public class RepositoryImpl1 : IRepository
    {
        public RepositoryImpl1() { }
       
        public string TestObject()
        {
            return "RepositoryImpl is created";
        }
    }

    public class RepositoryImpl2 : IRepository
    {
        public AService serv = null;
        public RepositoryImpl2(AService _serv) {
            this.serv = _serv;
        }

        public string TestObject()
        {
            return "RepositoryImp2 obj is created";
        }
    }



    public abstract class ErrorAbstratcImpl : AService
    {
        public ErrorAbstratcImpl() { }
    }


    public class ErrorNoPublicConstructorImpl : IService
    {
        private ErrorNoPublicConstructorImpl() { }
    }

    interface IService<TRepository> where TRepository : IRepository
    {
        string TestObject();
    }

    class ServiceImpl<TRepository> : IService<TRepository>
        where TRepository : IRepository
    {
        public IRepository rep = null;
        public ServiceImpl(TRepository _rep)
        {
            this.rep = _rep;
        }

        public string TestObject()
        {
            return "ServiceImpl<TRepository> with generic is created";
        }
    }
}
