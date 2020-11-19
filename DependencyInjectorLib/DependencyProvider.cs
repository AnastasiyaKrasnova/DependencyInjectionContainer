using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjectionLib
{
    public class DependencyProvider
    {
        private DependencyConfiguration config;
        private ConcurrentDictionary<Type, object> ImplementationInstances = new ConcurrentDictionary<Type, object>();

        public DependencyProvider(DependencyConfiguration _config)
        {
            config = _config;
        }

        public TDependency Resolve<TDependency>()
        {
            return (TDependency)GetInstance(typeof(TDependency));
        }

        private object GetInstance(Type DependencyType)
        {
            if (typeof(IEnumerable).IsAssignableFrom(DependencyType))
            {
                var collection = CreateIEnumerable(DependencyType);
                return collection;
            }
            else if (DependencyType.IsGenericType)
            {
                Type Implementation = ImplementaionForOpenGeneric(DependencyType);
                return Create(config.GetConfiguratedType(DependencyType), Implementation);
            }
            else if (config.GetConfiguratedType(DependencyType)!=null)
            {
                return Create(config.GetConfiguratedType(DependencyType));
            }
            return null;

        }

        private object CreateIEnumerable(Type type)
        {
            var argType = type.GetGenericArguments()[0];
            var configuratedType = config.GetConfiguratedType(argType);
            if (configuratedType != null)
            {
                var collection = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(argType));

                var configuratedTypes = config.GetConfiguratedTypes(argType);
                foreach (var confType in configuratedTypes)
                {
                    collection.Add(Create(confType));
                }

                return collection;
            }
            return null;
        }

        private object Create(ConfigType config_type, Type extraImpl=null)
        {
            Type Implementation = null;
            if (extraImpl == null)
                Implementation = config_type.Implementation;
            else
                Implementation = extraImpl;

            if (config_type.IsSingleton && ImplementationInstances.ContainsKey(Implementation))
                return ImplementationInstances[Implementation];

            ConstructorInfo[] constructors = Implementation.GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();
            object ResultObject = null;
            bool isCreated = false;
            int ctorNum = 1;
            while (!isCreated && ctorNum <= constructors.Count())
            {
                try
                {
                    ConstructorInfo useConstructor = constructors[ctorNum - 1];
                    object[] parameters = GetConstructorParams(useConstructor);
                    ResultObject = Activator.CreateInstance(Implementation, parameters);
                    isCreated = true;
                }
                catch
                {
                    isCreated = false;
                    ctorNum++;
                }
            }

            if (config_type.IsSingleton && !ImplementationInstances.ContainsKey(Implementation))
                if (!ImplementationInstances.TryAdd(Implementation, ResultObject))
                    return ImplementationInstances[Implementation];

            return ResultObject;
        }

        private Type ImplementaionForOpenGeneric(Type DependencyType)
        {
            var arg = DependencyType.GetGenericArguments().FirstOrDefault();
            var Implementation = config.GetConfiguratedType(DependencyType.GetGenericTypeDefinition()).Implementation;
            if (Implementation != null)
            {
                if (config.GetConfiguratedType(arg) != null)
                {
                    return Implementation.MakeGenericType(config.GetConfiguratedType(arg).Implementation);
                }
                return Implementation.MakeGenericType(arg);
            }
            else return null;
            
        }


        private object[] GetConstructorParams(ConstructorInfo constructor)
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            object[] parametersValues = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                parametersValues[i] = GetInstance(config.GetConfiguratedType(parameters[i].ParameterType).Interface);
            }

            return parametersValues;
        }

    }
}
