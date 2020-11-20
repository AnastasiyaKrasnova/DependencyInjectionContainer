using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace DependencyInjectionLib
{
    public class DependencyConfiguration
    {
        public Dictionary<Type, List<ConfigType>> configuration= new Dictionary<Type, List<ConfigType>>();

       /* public void Register<TImplementation>()
            where TImplementation : class
        {
            RegisterType(typeof(TImplementation), typeof(TImplementation));
        }*/

        public void Register<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation :TInterface
        {
            RegisterType(typeof(TInterface), typeof(TImplementation));
        }

        public void Register(Type TInterface, Type TImplementation)
        {
            RegisterType(TInterface, TImplementation);
        }

        /*public void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            RegisterType(typeof(TImplementation), typeof(TImplementation), true);
        }*/

        public void RegisterSingleton<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : TInterface
        {
            RegisterType(typeof(TInterface), typeof(TImplementation), true);
        }

        public void RegisterSingleton(Type TInterface, Type TImplementation)
        {
            RegisterType(TInterface, TImplementation, true);
        }

        public void RegisterType(Type TInterface, Type TImplementation, bool isSingleton = false)
        {
            if (isValid(TInterface,TImplementation))
            {
                ConfigType configuratedType = new ConfigType(TInterface, TImplementation, isSingleton);

                if (configuration.ContainsKey(TInterface))
                {
                    configuration[TInterface].Add(configuratedType);
                }
                else
                {
                    configuration.Add(TInterface, new List<ConfigType> { configuratedType });
                }

            }
        }

        public ConfigType GetConfiguratedType(Type TInterface)
        {
            return configuration.TryGetValue(TInterface, out var configuratedTypes) ? configuratedTypes.Last() : null;
        }

        public IEnumerable<ConfigType> GetConfiguratedTypes(Type TInterface)
        {
            return configuration.TryGetValue(TInterface, out var configuratedTypes) ? configuratedTypes : null;
        }

        private bool isValid(Type DependencyType, Type TImplementation)
        {
            return !TImplementation.IsAbstract && !TImplementation.IsInterface&&
                    (DependencyType.IsAssignableFrom(TImplementation) || DependencyType.IsGenericTypeDefinition);  
        }
    }
}
