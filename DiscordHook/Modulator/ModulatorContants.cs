using System.IO;
using System.Reflection;
using SDG.Unturned;

namespace Modulator
{
    public static class ModulatorContants
    {
        public static string ServerPath => Directory.GetCurrentDirectory() + ServerSavedata.directory + "/" + Provider.serverID;
        public static string AssemblyName => Assembly.GetExecutingAssembly().GetName().Name;

        public static string ModulatorPath => ServerPath + "/Modulator";
        public static string TranslationPath => ModulatorPath + "/Translations";
        public static string ConfigurationPath => ModulatorPath + "/Configurations";
    }
}
