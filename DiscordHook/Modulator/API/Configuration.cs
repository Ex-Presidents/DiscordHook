using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulator.Interfaces;
using Modulator.Data;

namespace Modulator.API
{
    public class Configuration<T> where T : IConfiguration
    {
        #region Variables
        private T _Instance;
        #endregion

        #region Properties
        public static T Instance
        {
            get
            {
                Configuration<T> configuration = ModulatorMemory.GetConfiguration<T>();

                if(configuration == null)
                {
                    configuration = new Configuration<T>();

                    ModulatorMemory.AddConfiguration<T>(configuration);
                }

                return configuration.Config;
            }
        }

        public T Config => _Instance;
        #endregion

        private Configuration()
        {
            if (!Directory.Exists(ModulatorContants.ConfigurationPath))
                Directory.CreateDirectory(ModulatorContants.ConfigurationPath);
            string path = ModulatorContants.ConfigurationPath + "/" + ModulatorContants.AssemblyName;

            if (!File.Exists(path))
            {
                _Instance = Activator.CreateInstance<T>();

                _Instance.LoadDefaults();
            }
            else
            {
                _Instance = XMLData.Deserialize<T>(path);
            }
        }

        #region Functions
        public static void Save()
        {
            Configuration<T> configuration = ModulatorMemory.GetConfiguration<T>();
            string path = ModulatorContants.ConfigurationPath + "/" + ModulatorContants.AssemblyName;

            if (configuration == null)
                return;

            File.WriteAllText(path, XMLData.Serialize(configuration));
        }
        #endregion
    }
}
