using System;

namespace DependencyInjectionLib
{
    public class ConfigType
    {
        public bool IsSingleton { get; set; }

        public Type Interface { get; }

        public Type Implementation { get; }

        public ConfigType(Type interf, Type implementation, bool isSingleton = false)
        {
            Interface = interf;
            Implementation = implementation;
            IsSingleton = isSingleton;
        }
    }
}