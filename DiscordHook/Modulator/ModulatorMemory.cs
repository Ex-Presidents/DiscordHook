using System;
using System.IO;
using System.Collections.Generic;
using Modulator.API;
using Modulator.Interfaces;

namespace Modulator
{
    public static class ModulatorMemory
    {
        #region Variables
        private static Dictionary<Type, Configuration<IConfiguration>> _ConfigList = new Dictionary<Type, Configuration<IConfiguration>>();
        #endregion

        static ModulatorMemory()
        {
            if (!Directory.Exists(ModulatorContants.ModulatorPath))
                Directory.CreateDirectory(ModulatorContants.ModulatorPath);
        }

        #region Configuration
        public static Configuration<T> GetConfiguration<T>() where T : IConfiguration
        {
            if (!_ConfigList.ContainsKey(typeof(T)))
                return null;

            return _ConfigList[typeof(T)] as Configuration<T>;
        }

        public static bool ConfigurationExists<T>() where T : IConfiguration
            => _ConfigList.ContainsKey(typeof(T));

        public static void AddConfiguration<T>(Configuration<T> configuration) where T : IConfiguration
        {
            if (ConfigurationExists<T>())
                return;

            _ConfigList.Add(typeof(T), configuration as Configuration<IConfiguration>);
        }
        #endregion
    }
}
